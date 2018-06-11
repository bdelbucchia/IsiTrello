using IsiTrello.ViewModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(IsiTrello.SignIn), typeof(IsiTrello.UWP.SignInRenderer))]

namespace IsiTrello.UWP
{
    class SignInRenderer : PageRenderer

    {

        SignIn page;
        SignInViewModel svm;
        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as SignIn;
            svm = page.BindingContext as SignInViewModel;
            svm.platformParameters = new PlatformParameters(PromptBehavior.Auto, false);
        }

    }
}