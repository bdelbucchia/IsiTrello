using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;
using Xamarin.Forms;
using IsiTrello.Infrastructures;
using IsiTrello.iOS.CustomRenderer;
using Xamarin.Forms.Platform.iOS;
using CoreGraphics;

[assembly: ExportRenderer(typeof(LinksLabel), typeof(LinksLabelRenderer))]
namespace IsiTrello.iOS.CustomRenderer
{
    public class LinksLabelRenderer : ViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            var view = (LinksLabel)Element;
            if (view == null) return;

            UITextView uilabelleftside = new UITextView(new CGRect(0, 0, view.Width, view.Height));
            uilabelleftside.Text = view.Text;
            uilabelleftside.Font = UIFont.SystemFontOfSize((float)view.FontSize);
            uilabelleftside.Editable = false;
            uilabelleftside.ScrollEnabled = false;

            // Setting the data detector types mask to capture all types of link-able data
            uilabelleftside.DataDetectorTypes = UIDataDetectorType.All;
            uilabelleftside.Selectable = true;
            uilabelleftside.ShouldInteractWithUrl += delegate { return true; };
            uilabelleftside.BackgroundColor = UIColor.Clear;

            // overriding Xamarin Forms Label and replace with our native control
            SetNativeControl(uilabelleftside);
        }
    }
}