// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

using Plugin.Toasts.UWP;
using Xamarin.Forms;

namespace IsiTrello.UWP
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            //Enregistrement et initilisation Toast
            DependencyService.Register<ToastNotification>();
            ToastNotification.Init();

            LoadApplication(new IsiTrello.App());
        }
    }
}
