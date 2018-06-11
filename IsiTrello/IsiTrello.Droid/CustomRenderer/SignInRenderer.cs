
using Android.App;
using Android.Content;
using IsiTrello;
using IsiTrello.ViewModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(IsiTrello.SignIn), typeof(IsiTrello.Droid.SignInRenderer))]
namespace IsiTrello.Droid
{
    public class SignInRenderer : PageRenderer
    {
        SignIn page;
        SignInViewModel svm;

        public SignInRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as SignIn;
            svm = page.BindingContext as SignInViewModel;
            var activity = this.Context as Activity;
            svm.platformParameters = new PlatformParameters(activity);
        }
    }
}