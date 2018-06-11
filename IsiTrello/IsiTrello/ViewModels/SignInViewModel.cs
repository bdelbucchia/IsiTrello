using IsiTrello.Infrastructures;
using IsiTrello.Services;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiTrello.ViewModel
{
    public class SignInViewModel : BaseViewModel
    {
        private ICommand signInCmd;
        private IDependencyService _dependencyService;
        private bool didAuthenticate;
        //Platform parameter utiliser par la bibliotheque ADAL, est initiliser par les renderer.
        public IPlatformParameters platformParameters { get; set; }
        public ICommand SignInCmd
        {
            get { return signInCmd; }
            set { SetPropertyValue(ref signInCmd, value); }
        }
        private bool _noConnection;

        public bool noConnection
        {
            get { return _noConnection; }
            set
            {
                SetPropertyValue(ref _noConnection, value);
                ((Command)SignInCmd).ChangeCanExecute();
            }
        }


        public SignInViewModel() : this (new DependencyServiceWrapper())
        {
        }
        public SignInViewModel(IDependencyService depServ)
        {
            _dependencyService = depServ;
            CrossConnectivity.Current.ConnectivityChanged += UpdateNetworkStatus;
            SignInCmd = new Command(async () =>
           {
               IsBusy = true;
               try
               {
                   didAuthenticate = await _dependencyService.Get<IAzureClient>().GetToken(platformParameters);
               }
               catch
               {
                   new Exception("Erreur au login");
               }
               finally
               {
                   if (didAuthenticate)
                       await _dependencyService.Get<INavigationService>().PopToPage("Boardlist", new BoardListViewModel());
                   else
                       await _dependencyService.Get<INavigationService>().GoBackAsync();
               }
               IsBusy = false;
           }, () =>canAuthenticate());
            noConnection = CrossConnectivity.Current.IsConnected ? false : true;
        }

        /// <summary>
        /// Verifie l'etat de la connection afin de permettre l'authentification.
        /// </summary>
        /// <returns></returns>
        private bool canAuthenticate()
        {
            return !noConnection;
        }
        /// <summary>
        /// Signale un changement de l'etat de la connection
        /// </summary>
        private void UpdateNetworkStatus(object sender, ConnectivityChangedEventArgs e)
        {
            if (CrossConnectivity.Current != null && e.IsConnected)
                noConnection = false;
            else
                noConnection = true;
        }
    }
}
