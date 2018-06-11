using IsiTrello.Infrastructures;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.Services
{
    public class AzureClient : IAzureClient
    {
        public  AuthenticationContext AuthContext;
        public string clientId; //[Enter your client ID as registered in the Azure Management Portal, e.g. 3d8c4803-ffcd-4b2a-baec-05056abdc408]
        public string taskApiResourceId; //notre APIResource
        public  string graphApiResourceId = "https://graph.windows.net";//GraphAPI de Microsoft
        public Uri redirectUri; //Uri de reponse pour l'API

        /// <summary>
        /// Verification de la présence d'un Token et de sa validité pour rafraichissement
        /// </summary>
        /// <returns></returns>
        public bool KnownToken()
        {
            //If we find a Cache, we're going to check if there's already a token
            AuthContext = new AuthenticationContext(taskApiResourceId);
            if (AuthContext.TokenCache.ReadItems().Count() > 0)
            {
                //If there's a cached token, check if refresh token is still valid and refresh session if so
                DateTimeOffset cachedTokenLimit = AuthContext.TokenCache.ReadItems().First().ExpiresOn;
                if (cachedTokenLimit <= DateTimeOffset.Now)
                    RefreshToken();
                return true;
                
            }
            return false;
        }

        /// <summary>
        /// Rafraichissement du Token ? A verifier
        /// </summary>
        public async void RefreshToken()
        {
            try
            {
                AuthenticationResult authResult = await AuthContext.AcquireTokenSilentAsync(graphApiResourceId, clientId);
            }
            catch (Exception ex)
            {   
                AuthContext.TokenCache.Clear();
                await DependencyService.Get<INavigationService>().NavigateAsync("SignIn");
                await DependencyService.Get<IMessageVisualizerService>().ShowMessage("Refreshing Error", ex.Message, "OK");
            }
        }

        /// <summary>
        /// Recuperation d'un token pour l'utilisateur en cours.
        /// </summary>
        /// <param name="platformParameters"></param>
        public async Task<bool> GetToken(IPlatformParameters platformParameters)
        {
            AuthenticationResult authResult = null;
            try
            {
                authResult = await AuthContext.AcquireTokenAsync(graphApiResourceId, clientId, redirectUri, platformParameters);
                return authResult != null;
            }
            catch (Exception ex)
            {
                await DependencyService.Get<IMessageVisualizerService>().ShowMessage("Error signing you in.", ex.Message, "OK");
                return false;
            }
        }
        public AzureClient()
        {
           clientId = App.Keys["AzureClientID"];
           taskApiResourceId = App.Keys["AzureAPIRessource"];
           redirectUri = new Uri(App.Keys["AzureRedirectUri"]);
           AuthContext = new AuthenticationContext(taskApiResourceId);
        }
    }
}
