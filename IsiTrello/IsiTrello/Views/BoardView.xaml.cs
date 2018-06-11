using Xamarin.Forms;

namespace IsiTrello
{
    public partial class BoardView : ContentPage
    {
        public BoardView()
        {
            NavigationPage.SetBackButtonTitle(this, "");
            InitializeComponent();
        }
    }
}
