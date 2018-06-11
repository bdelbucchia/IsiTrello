using IsiTrello.Infrastructures;
using IsiTrello.Model;
using IsiTrello.ViewModel;
using Newtonsoft.Json;
using Plugin.Connectivity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.Services
{
    public class Client : IClient
    {

        string Url = "https://api.trello.com/1/";
        private string authorisation;
        HttpClient client;

        /// <summary>
        /// Constructeur, genere le httpclient, le configure pour attaquer l'API Trello 
        /// et enregistre les informations d'authentification pour l'API.
        /// </summary>
        public Client()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.BaseAddress = new Uri(Url);
            authorisation = "key=" + App.Keys["TrelloClientKey"] + "&token=" + App.Keys["TrelloUserToken"];
        }

        /// <summary>
        /// Recupere la liste des tableaux auxquels le compte IsiTrello a acces.
        /// </summary>
        /// <returns>IEnumerable {id, name}</returns>
        public async Task<IEnumerable<Blist>> GetBoardlist()
        {
            checkAuth();
            var req = await client.GetAsync($"members/me/boards?filter=open&fields=name,prefs&{authorisation}");
            if (req.IsSuccessStatusCode)
            {
                var result = await req.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Blist>>(result);
            }
            else
            {
                RaiseServerError(req);
                return new List<Blist>();
            }
        }
        /// <summary>
        /// Recupere le nom du tableau pour afficher en Titre de page.
        /// </summary>
        /// <param name="boardID">id/lien du tableau cible</param>
        /// <returns>Chaine de caracteres name</returns>
        public async Task<string> GetBoardname(string boardID)
        {
            checkAuth();
            var req = await client.GetAsync("boards/" + boardID + "/name?" + authorisation);
            if (req.IsSuccessStatusCode)
            {
                var result = await req.Content.ReadAsStringAsync();
                var def = new { _value = "" };
                var name = JsonConvert.DeserializeAnonymousType(result, def);
                return name._value;
            }
            else
            {
                RaiseServerError(req);
                return string.Empty;
            }
        }

        /// <summary>
        /// Recupere le tableau afin de pouvoir afficher les donnees
        /// </summary>
        /// <param name="boardID">id/lien du tableau cible</param>
        /// <returns>Collection de Listes avec leurs cartes</returns>
        public async Task<IEnumerable<List>> GetBoard(string boardID)
        {
            checkAuth();
            var req = await client.GetAsync("boards/" + boardID + "/lists?cards=all&" + authorisation);
            if (req.IsSuccessStatusCode)
            {
                var result = await req.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<List>>(result);
            }
            else
            {
                RaiseServerError(req);
                return new List<List>();
            }
        }
        /// <summary>
        /// Recupere les pieces jointes avec un nom et un lien a presenter sur la carte visualisee.
        /// </summary>
        /// <param name="Shortlink">id/lien de la carte en cours</param>
        /// <returns></returns>
        public async Task<IEnumerable<Attachment>> GetAttachments(string Shortlink)
        {
            checkAuth();
            var req = await client.GetAsync("cards/" + Shortlink + "/attachments?fields=url,name&" + authorisation);
            if (req.IsSuccessStatusCode)
            {
                var result = await req.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Attachment>>(result);
            }
            else
            {
                RaiseServerError(req);
                return new List<Attachment>();
            }
        }
        /// <summary>
        /// Recupere les Checklists a afficher sur la carte visualisee.
        /// </summary>
        /// <param name="idcard">id/lien de la carte en cours</param>
        /// <returns></returns>
        public async Task<IEnumerable<Checklist>> GetChecklist(string idcard)
        {
            checkAuth();
            var req = await client.GetAsync("cards/" + idcard + "/checklists?fields=name&checkItem_fields=name,state&" + authorisation);
            if (req.IsSuccessStatusCode)
            {
                var result = await req.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<Checklist>>(result);
            }
            else
            {
                RaiseServerError(req);
                return new List<Checklist>();
            }
        }

        private void RaiseServerError(HttpResponseMessage resp)
        {
            var statusCode = "Erreur " + (int)resp.StatusCode;
            var errorMessage = string.IsNullOrWhiteSpace(resp.ReasonPhrase) ? resp.StatusCode.ToString() : resp.ReasonPhrase;
            resp.Dispose();
            throw new Exception(statusCode + ":" + errorMessage);
        }

        private void checkAuth()
        {
            if (!CrossConnectivity.Current.IsConnected || !DependencyService.Get<IAzureClient>().KnownToken())
            {
                DependencyService.Get<INavigationService>().PopToPage("SignIn", new SignInViewModel());
            }
        }
    }
}
