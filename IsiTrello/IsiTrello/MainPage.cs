using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

using Xamarin.Forms;

namespace IsiTrello
{
    public class MainPage : MasterDetailPage
    {
        public MainPage()
        {
            var listPage = new BoardView();
            Master = new NavigationPage(listPage) { Title = listPage.Title, Icon = listPage.Icon };
            Detail = new NavigationPage(new CardView());
        }
    }
}