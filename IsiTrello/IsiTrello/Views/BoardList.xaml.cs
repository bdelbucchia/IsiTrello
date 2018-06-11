
using IsiTrello.Infrastructures;
using Xamarin.Forms;

namespace IsiTrello
{
    public partial class BoardList : ContentPage
    {
        public BoardList()
        {
            NavigationPage.SetBackButtonTitle(this, "");
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((NavigationPage)App.Current.MainPage).BarBackgroundColor = Color.FromHex("#607D8B");
        }
    }
}
