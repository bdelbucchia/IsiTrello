using IsiTrello.Infrastructures;
using IsiTrello.UWP.CustomRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms.Platform.UWP;
using Xamarin.Forms;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Text.RegularExpressions;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using System.ComponentModel;

[assembly:ExportRenderer(typeof(LinksLabel),typeof(LinksLabelRenderer))]
namespace IsiTrello.UWP.CustomRenderer
{
    public class LinksLabelRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            string text = Control.Text;

            ParseMail(text);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            string text = Control.Text;

            ParseMail(text);
        }

        private void ParseMail(string text)
        {
            if (Control != null && !string.IsNullOrWhiteSpace(text))
            {
                Control.Inlines.Clear();
                Regex regx = new Regex(@"(http(s)?://[\S]+|www.[\S]+|[\S]+@[\S]+)", RegexOptions.IgnoreCase);
                Regex isWWW = new Regex(@"(http[s]?://[\S]+|www.[\S]+)");
                Regex isEmail = new Regex(@"[\S]+@[\S]+");
                foreach (var item in regx.Split(text))
                {
                    if (isWWW.IsMatch(item))
                    {
                        Hyperlink link = new Hyperlink { NavigateUri = new Uri(item.ToLower().StartsWith("http") ? item : $"http://{item}"), Foreground = Windows.UI.Xaml.Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush };
                        link.Inlines.Add(new Run { Text = item });
                        Control.Inlines.Add(link);
                    }
                    else if (isEmail.IsMatch(item))
                    {
                        Hyperlink link = new Hyperlink { NavigateUri = new Uri($"mailto:{item}"), Foreground = Windows.UI.Xaml.Application.Current.Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush };
                        link.Inlines.Add(new Run { Text = item });
                        Control.Inlines.Add(link);
                    }
                    else Control.Inlines.Add(new Run { Text = item });
                }

            }
        }
    }
}
