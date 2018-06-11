using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Xamarin.Forms;
using IsiTrello.Droid;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Xamarin.Forms.ListView), typeof(CustomListViewRenderer))]
namespace IsiTrello.Droid
{
        public class CustomListViewRenderer : ListViewRenderer
        {
        public CustomListViewRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ListView> e)
            {
                base.OnElementChanged(e);

                if (e.NewElement != null)
                {
                    var listView = this.Control as Android.Widget.ListView;
                    listView.NestedScrollingEnabled = true;
                    listView.LongClickable = false;
                }
            }
            public override bool OnNestedFling(Android.Views.View target, float velocityX, float velocityY, bool consumed)
            {
                if (velocityX > velocityY && target is Android.Widget.ListView)
                {
                    return this.DispatchNestedPreFling(velocityX, velocityY);
                }
                else
                    return false;
            }
        }
    }