using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Content;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Plugin.Toasts;
using CarouselView.FormsPlugin.Android;

namespace IsiTrello.Droid
{
    [Activity(Label = "IsiTrello", Icon = "@drawable/icon", Theme = "@style/SplahScreen", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {

            base.Window.RequestFeature(Android.Views.WindowFeatures.ActionBar);
            base.SetTheme(Resource.Style.MainTheme);

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);
            global::Xamarin.Forms.Forms.Init(this, bundle);
            CarouselViewRenderer.Init();

            //Enregistrement et initalisation Toast.Plugin
            DependencyService.Register<ToastNotification>();
            ToastNotification.Init(this, new PlatformOptions() { SmallIconDrawable = Android.Resource.Drawable.IcDialogInfo });

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }
    }
}

