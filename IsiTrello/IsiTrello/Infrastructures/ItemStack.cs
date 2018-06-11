using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Reflection;
using Xamarin.Forms;

namespace IsiTrello.Infrastructures
{
    // MIT License
    // Nicholas Ventimiglia
    // 2016-9-19

    /// <summary>
    /// For repeated content without a automatic scroll view. Supports DataTemplates, Horizontal and Vertical layouts !
    /// </summary>
    /// <remarks>
    /// Warning TODO NO Visualization or Paging! Handle this in your view model.
    /// </remarks>
    public class ItemsStack : StackLayout
    {
        #region BindAble
        public static readonly BindableProperty ItemsSourceProperty =
        // BindableProperty.Create<ItemsStack, IEnumerable>(p => p.ItemsSource, default(IEnumerable<object>), BindingMode.TwoWay, null, ItemsSourceChanged);
        BindableProperty.Create(nameof(ItemsSource), typeof(IEnumerable), typeof(ItemsStack), default(IEnumerable<object>), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: (bindable, oldvalue, newvalue) => ItemsSourceChanged(bindable, oldvalue as IEnumerable, newvalue as IEnumerable));

        public static readonly BindableProperty SelectedItemProperty = 
            //BindableProperty.Create<ItemsStack, object>(p => p.SelectedItem, default(object), BindingMode.TwoWay, null, OnSelectedItemChanged);
            BindableProperty.Create(nameof(SelectedItem), typeof(object), typeof(ItemsStack), default(object), defaultBindingMode: BindingMode.OneWay,
            propertyChanged: OnSelectedItemChanged);

        public static readonly BindableProperty ItemTemplateProperty = 
            //BindableProperty.Create<ItemsStack, DataTemplate>(p => p.ItemTemplate, default(DataTemplate));
            BindableProperty.Create(nameof(ItemTemplate), typeof(DataTemplate), typeof(ItemsStack), default(DataTemplate));

        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        private static void ItemsSourceChanged(BindableObject bindable, IEnumerable oldValue, IEnumerable newValue)
        {
            var itemsLayout = (ItemsStack)bindable;
            itemsLayout.SetItems();
        }

        private static void OnSelectedItemChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var itemsView = (ItemsStack)bindable;
            if (newValue == oldValue)
                return;

            itemsView.SetSelectedItem(newValue);
        }
        #endregion

        #region item rendering
        protected readonly ICommand ItemSelectedCommand;

        protected virtual void SetItems()
        {
            Children.Clear();

            if (ItemsSource == null)
            {
                ObservableSource = null;
                return;
            }

            foreach (var item in ItemsSource)
                Children.Add(GetItemView(item));

            var t = ItemsSource.GetType();
            var isObs = t.GetTypeInfo().IsGenericType && ItemsSource.GetType().GetGenericTypeDefinition() == typeof(ObservableCollection<>);
            if (isObs)
            {
                ObservableSource = new ObservableCollection<object>(ItemsSource.Cast<object>());
            }
        }

        protected virtual View GetItemView(object item)
        {

            var content = ItemTemplate.CreateContent();

            var view = content as View;
            if (view == null)
                return null;
            if (item == null)
                return null;
            view.BindingContext = item;

            var gesture = new TapGestureRecognizer
            {
                Command = ItemSelectedCommand,
                CommandParameter = item
            };

            AddGesture(view, gesture);

            return view;
        }

        protected void AddGesture(View view, TapGestureRecognizer gesture)
        {
            view.GestureRecognizers.Add(gesture);

            var layout = view as Layout<View>;

            if (layout == null)
                return;

            foreach (var child in layout.Children)
                AddGesture(child, gesture);
        }

        protected virtual void SetSelectedItem(object selectedItem)
        {
            var handler = SelectedItemChanged;
            if (handler != null)
                handler(this, new SelectedItemChangedEventArgs(selectedItem));
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
                            Children.Insert(index++, GetItemView(item));
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    {
                        var item = ObservableSource[e.OldStartingIndex];
                        Children.RemoveAt(e.OldStartingIndex);
                        Children.Insert(e.NewStartingIndex, GetItemView(item));
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
                        Children.Insert(e.NewStartingIndex, GetItemView(ObservableSource[e.NewStartingIndex]));
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Children.Clear();
                    foreach (var item in ItemsSource)
                        Children.Add(GetItemView(item));
                    break;
            }
        }
        #endregion

        public ItemsStack()
        {

            ItemSelectedCommand = new Command<object>(item =>
            {
                SelectedItem = item;
            });
        }
    }

    public interface IObservableReadOnlyCollection<out T> : IEnumerable<T>, INotifyCollectionChanged
    {
        T this[int i] { get; }
        int Count { get; }
    }

    public class ObservableReadOnlyCollection<T> : IObservableReadOnlyCollection<T>
    {
        public ObservableCollection<T> _inner;
        public ObservableReadOnlyCollection(ObservableCollection<T> inner) { _inner = inner; }

        public T this[int i] => _inner[i];
        public int Count => _inner.Count;

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { _inner.CollectionChanged += value; }
            remove { _inner.CollectionChanged -= value; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }
    }
}