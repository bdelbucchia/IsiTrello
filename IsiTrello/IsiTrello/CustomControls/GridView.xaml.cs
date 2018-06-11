using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiTrello.Infrastructures
{
    public partial class GridView : Grid
    {
        public GridView()
        {
        }

        public static readonly BindableProperty CommandParameterProperty = 
            BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(GridView), null);

        public static readonly BindableProperty CommandProperty = 
            BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(GridView), null);

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(GridView), default(IEnumerable<object>),
                defaultBindingMode: BindingMode.OneWay, propertyChanged: OnItemsSourceChanged);

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(GridView), default(DataTemplate),
                propertyChanged: (bindable, oldvalue, newvalue) => ((GridView)bindable).OnSizeChanged());

        public static readonly BindableProperty _maxColumns =
            BindableProperty.Create(nameof(MaxColumns), typeof(int), typeof(GridView), 1);

        private void OnSizeChanged()
        {
            ForceLayout();
        }

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var element = newValue as IEnumerable<object>;
            if (element == null)
                return;
            ((GridView)bindable).BuildTiles(element);
        }

        //private int _maxColumns;
        private GridLength _tileHeight = GridLength.Auto;

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public int MaxColumns
        {
            get { return (int)GetValue(_maxColumns); }
            set { SetValue(_maxColumns , value); }
        }

        public GridLength TileHeight
        {
            get { return _tileHeight; }
            set { _tileHeight = value; }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public void BuildTiles(IEnumerable<object> tiles)
        {            
            try
            {
                if (tiles == null || tiles.Count() == 0)
                    Children?.Clear();

                var t = ItemsSource.GetType();
                var isObs = t.GetTypeInfo().IsGenericType && ItemsSource.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>);
                if (isObs)
                {
                    ObservableSource = new ObservableCollection<object>(ItemsSource.Cast<object>());
                }

                // Wipe out the previous definitions if they're there.
                RowDefinitions?.Clear();
                ColumnDefinitions?.Clear();

                var enumerable = ObservableSource as IList ?? tiles.ToList();
                var numberOfRows = Math.Ceiling(enumerable.Count / (float)MaxColumns);

                for (var i = 0; i < MaxColumns; i++)
                    ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star});

                for (var i = 0; i < numberOfRows; i++)
                    RowDefinitions?.Add(new RowDefinition { Height = TileHeight });

                for (var index = 0; index < enumerable.Count; index++)
                {
                    var column = index % MaxColumns;
                    var row = (int)Math.Floor(index / (float)MaxColumns);

                    var tile = BuildTile(enumerable[index]);

                    Children.Add(tile, column, row);
                }
                //ForceLayout();
            }
            catch
            { // can throw exceptions if binding upon disposal
                /*throw new Exception(" Unhandled excepection while building tiles");*/
            }
        }

        private View BuildTile(object item1)
        {
            var content = ItemTemplate.CreateContent();
            var buildTile = content as View;
            if (buildTile == null)
                return null;
            //var buildTile = ItemTemplate.  as Layout;
            buildTile.BindingContext = item1;
            buildTile.InputTransparent = false;

            var tapGestureRecognizer = new TapGestureRecognizer
            {
                Command = Command,
                CommandParameter = item1,
                NumberOfTapsRequired = 1
            };

            return buildTile;
        }
        ObservableCollection<object> _observableSource;
        protected ObservableCollection<object> ObservableSource
        {
            get { return _observableSource; }
            set
            {
                if (_observableSource != null)
                {
                    _observableSource.CollectionChanged -= CollectionChanged;
                }
                _observableSource = value;

                if (_observableSource != null)
                {
                    _observableSource.CollectionChanged += CollectionChanged;
                }
            }
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        int index = e.NewStartingIndex;
                        foreach (var item in e.NewItems)
                        {
                            Children.Insert(index++, BuildTile(item));
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var item = ObservableSource[e.OldStartingIndex];
                        Children.RemoveAt(e.OldStartingIndex);
                        Children.Insert(e.NewStartingIndex, BuildTile(item));
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    {
                        Children.RemoveAt(e.OldStartingIndex);
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    {
                        Children.RemoveAt(e.OldStartingIndex);
                        Children.Insert(e.NewStartingIndex, BuildTile(ObservableSource[e.NewStartingIndex]));
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    foreach (var item in ItemsSource)
                        Children.Add(BuildTile(item));
                    break;
            }
        }
    }
}
