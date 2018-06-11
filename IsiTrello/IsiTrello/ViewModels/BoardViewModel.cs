using IsiTrello.Infrastructures;
using IsiTrello.Services;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiTrello.ViewModel
{
    public class BoardViewModel : BaseViewModel
    {
        public IList<ListsViewModel> Listes { get; private set; }
        CardViewModel selectedCard;
        string _boardid;
        string _boardname;
        private readonly IDependencyService _dependencyService;
        private bool _trelloError;
        public bool TrelloError
        {
            get { return _trelloError; }
            set
            {
                SetPropertyValue(ref _trelloError, value);
            }
        }
        private string _backgroundColor;

        public string backgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                _backgroundColor = value;
                SetPropertyValue(ref _backgroundColor, value);
            }
        }

        private ICommand refreshBoard;
        public ICommand RefreshBoard
        {
            get { return refreshBoard; }
            set { SetPropertyValue(ref refreshBoard, value); }
        }
        public string BoardName
        {
            get { return _boardname; }
            private set { SetPropertyValue (ref _boardname, value); }
        }
        public int Position { get; set; }

        //Champs Carte Selectionnee, permet la selection d'une carte pour naviguer vers la vue detaillee,
        //se voit reinitialise une fois la navigation engagee.
        public CardViewModel SelectedCard
        {
            get { return selectedCard; }
            set
            {
                SetPropertyValue(ref selectedCard, value);
                if (value != null)
                {
                    GoToCard(selectedCard);
                }
                else
                    return;
                SelectedCard = null;
            }
        }

        private async void GoToCard(CardViewModel card)
        {
            this.IsBusy = true;
            try
            {
                await card.GetDetails();
                Listes[Position].Position = card.pos;
                await Listes[Position].LoadDetails();

            }
            catch
            {
                var notificator = _dependencyService.Get<IToastNotificator>();
                var options = new NotificationOptions
                {
                    Title = "Erreur",
                    Description = "Une erreur est survenue lors du chargement",
                    ClearFromHistory = false
                };
                //Pour iOS, permet d'eviter le cas ou en l'absence d'authorisation pour les notifications
                //le process reste bloqué avec la demande de notification.
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (App.hasiOSNotifAuth)
                    {
                        var result = await notificator.Notify(options);
                    }
                }
                else
                {
                    var result = await notificator.Notify(options);
                }
            }
            finally
            { IsBusy = false; }
            await _dependencyService.Get<INavigationService>().NavigateAsync("ListsView", Listes[Position]);
        }

        /// <summary>
        /// Constructeur.
        /// Recupere les listes a afficher et tente d'afficher l'indicateur d'activite.
        /// </summary>
        public BoardViewModel(string boardid, string boardname, string color, IDependencyService dependencyService)
        {
            this._boardid = boardid;
            this.BoardName = boardname;
            this.backgroundColor = color;
            RefreshBoard = new Command(async () => await PopulateList());
                _dependencyService = dependencyService;
                Listes = new ObservableCollection<ListsViewModel>();
        }
        public BoardViewModel(string boardid, string boardname, string color) : this(boardid, boardname, color, new DependencyServiceWrapper())
        {
        }
        /// <summary>
        /// Recuperation et assignation des listes et de leurs cartes grace a l'API.
        /// Demande d'un client  de connection a celle-ci puis assignation de chacune a un ViewModel puis enregistrement pour l'affichage.
        /// En cas d'erreur de l'api, envoie une notification pour avertir l'utilisateur.
        /// </summary>
        public async Task PopulateList()
        {

            this.TrelloError = false;
            this.IsBusy = true;
            try
            {
                //var TBname =_dependencyService.Get<IClient>().GetBoardname();
                var board = await _dependencyService.Get<IClient>().GetBoard(_boardid);
                //await Task.WhenAll(TBname, Tboard);
                //BoardName = TBname.Result;
                //var board = Tboard.Result;
                if (Listes != null)
                    Listes = new ObservableCollection<ListsViewModel>();
                
                foreach (var liste in board)
                    Listes.Add(new ListsViewModel(liste, _dependencyService));
            }
            catch(Exception e)
            {
                var Message = e.Message.Split(':');
                BoardName = "Erreur";
                var notificator = _dependencyService.Get<IToastNotificator>();
                var options = new NotificationOptions
                {
                    Title = Message[0],
                    Description = "Le serveur a retourné une erreur : " + Message[1],
                    ClearFromHistory = false
                };
                //Pour iOS, permet d'eviter le cas ou en l'absence d'authorisation pour les notifications
                //le process reste bloqué avec la demande de notification.
                if (Device.RuntimePlatform == Device.iOS)
                {
                    if (App.hasiOSNotifAuth)
                    {
                        var result = await notificator.Notify(options);
                    }
                }
                else
                {
                    var result = await notificator.Notify(options);
                }
                this.TrelloError = true;
            }
            finally { this.IsBusy = false; }
        }
    }
}
