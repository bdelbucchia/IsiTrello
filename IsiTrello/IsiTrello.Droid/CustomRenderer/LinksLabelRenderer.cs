using Android.Widget;
using Xamarin.Forms;
using IsiTrello.Infrastructures;
using IsiTrello.Droid.CustomRenderer;
using Xamarin.Forms.Platform.Android;
using Android.Text.Util;
using Android.Util;
using System.ComponentModel;
using Android.Text.Method;
using Android.Content;

[assembly: ExportRenderer(typeof(LinksLabel), typeof(LinksLabelRenderer))]
namespace IsiTrello.Droid.CustomRenderer
{
    public class LinksLabelRenderer : LabelRenderer
    {
        public LinksLabelRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.AutoLinkMask = MatchOptions.All;
                Control.LinksClickable = true;
                Control.Clickable = true;
                Control.MovementMethod = LinkMovementMethod.Instance;
            }
        }
    }
}