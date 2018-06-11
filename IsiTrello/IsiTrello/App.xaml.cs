using IsiTrello.Infrastructures;
using IsiTrello.Services;
using IsiTrello.ViewModel;
using Plugin.Connectivity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xamarin.Forms;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;

namespace IsiTrello
{
    public partial class App : Application
    {
        internal static Dictionary<string, string> Keys;
        internal static bool hasiOSNotifAuth;

        /// <summary>
        /// Recuperation des clefs utilisees pour les requetes API grace au App.config
        /// </summary>
        /// <returns>Dictionnaire de clefs</returns>
        internal static Dictionary<string, string> GetKeys()
        {
            var type = typeof(App);
            var resource = type.Namespace + ".app.config";
            using (var stream = type.GetTypeInfo().Assembly.GetManifestResourceStream(resource))
            using (var reader = new StreamReader(stream))
            {
                var doc = XDocument.Parse(reader.ReadToEnd());
                var Keys = doc.Descendants("add").ToDictionary(k => k.Attribute("key").Value.ToString(),
                                                            k => k.Attribute("value").Value.ToString());
                return Keys;
            }
        }
            static App()
        {
            Keys = GetKeys();
            // Register dependencies.
            DependencyService.Register<MessageVisualizerService>();
            DependencyService.Register<INavigationService, StackNavigationService>();
            DependencyService.Register<Client>();
            DependencyService.Register<AzureClient>();
        }
        public App()
        {
            
            InitializeComponent();

            MessagingCenter.Subscribe<IMessageNotifAuth, bool>(this, "notification", (sender, arg) => hasiOSNotifAuth = arg);

            //Register Pages for the navigation
            var navService = DependencyService.Get<INavigationService>();
            navService.RegisterPage("Boardlist", () => new BoardList());
            navService.RegisterPage("Board", () => new BoardView());
            //navService.RegisterPage("CardDetail", () => new CardView());
            navService.RegisterPage("SignIn", () => new SignIn());
            navService.RegisterPage("ListsView", () => new ListsView());

            // The root page of your application
            // Selon la presence d'un token d'identification et l'etat de la connection, on propose l'authentification.
            ContentPage content;
            if (CrossConnectivity.Current.IsConnected && DependencyService.Get<IAzureClient>().KnownToken())
            {
                content = new BoardList();
                content.BindingContext = new BoardListViewModel();
            }
            else
            {
                content = new SignIn();
                content.BindingContext = new SignInViewModel();
            }
            NavigationPage.SetHasNavigationBar(content, true);
            MainPage = new NavigationPage(content) { BarTextColor= Color.White, Title= "IsiTrello"};

        }

        protected override void OnStart()
        {
            base.OnStart();

            // Initialisation de l'App Center
            // L'App Secret est diponible dans les settings de chaque App
            AppCenter.Start("android=ae828ac0-8295-4d53-983d-392e03c6ba0d; " +
                  "uwp=2e469e24-36ce-40ad-9d67-b7ff1738335f;" +
                  "ios=429f28f1-5e05-454d-949e-e89b5488981c",
                  typeof(Analytics), typeof(Crashes));
            // Handle when your app starts
            MessagingCenter.Subscribe<IMessageNotifAuth, bool>(this, "notification", (sender, arg) => hasiOSNotifAuth = arg);
        }

        protected override void OnSleep()
        {
            base.OnSleep();
            // Handle when your app sleeps
            MessagingCenter.Unsubscribe<IMessageNotifAuth, bool>(this, "notification");

        }

        protected override void OnResume()
        {
            base.OnResume();
            // Handle when your app resumes.
            // Maintient la verification des authorisations notifications iOS.

            MessagingCenter.Subscribe<IMessageNotifAuth, bool>(this, "notification", (sender, arg) => hasiOSNotifAuth = arg);
        }
    }
}
