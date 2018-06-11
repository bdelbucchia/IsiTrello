using IsiTrello.ViewModel;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(IsiTrello.SignIn), typeof(IsiTrello.iOS.SignInRenderer))]
namespace IsiTrello.iOS
{
    public partial class SignInRenderer : PageRenderer
    {
        SignIn page;
        SignInViewModel svm;

        protected override void OnElementChanged (VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as SignIn;
            svm = page.BindingContext as SignInViewModel;
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            svm.platformParameters = new PlatformParameters(this);

            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}