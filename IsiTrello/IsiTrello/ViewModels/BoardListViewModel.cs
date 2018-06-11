using IsiTrello.Infrastructures;
using IsiTrello.Services;
using Plugin.Toasts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiTrello.ViewModel
{
    public class BoardListViewModel : BaseViewModel
    {
        public IList<BoardViewModel> Boardlist { get; private set; }
        BoardViewModel selectedBoard;
        string _pagename;
        string _errorMsg;
        public string ErrorMsg
        {
            get { return _errorMsg; }
            set { SetPropertyValue(ref _errorMsg, value); }
        }
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
        private string _listName;
        public string ListName
        {
            get { return _listName; }
            set
            {
                SetPropertyValue(ref _listName, value);
            }
        }
        //private ICommand refreshBoard;
        //public ICommand RefreshBoard
        //{
        //    get { return refreshBoard; }
        //    set { SetPropertyValue(ref refreshBoard, value); }
        //}
        public string PageName
        {
            get { return _pagename; }
            private set { SetPropertyValue(ref _pagename, value); }
        }

        //Champs Tableau Selectionnee, permet la navigation vers le contenu du tableau,
        //se voit reinitialise une fois la navigation engagee.
        public BoardViewModel SelectedBoard
        {
            get { return selectedBoard; }
            set
            {
                SetPropertyValue(ref selectedBoard, value);
                if (value != null)
                {
                    GoToBoard(selectedBoard);
                }
                else
                    return;
                SelectedBoard = null;
            }
        }

        private async void GoToBoard(BoardViewModel board)
        {
            IsBusy = true;
            try
            {
                await board.PopulateList();
                ((NavigationPage)Application.Current.MainPage).BarBackgroundColor = Color.FromHex(board.backgroundColor);
                await _dependencyService.Get<INavigationService>().NavigateAsync("Board", board);
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
        }

        /// <summary>
        /// Constructeur.
        /// Recupere les listes a afficher et tente d'afficher l'indicateur d'activite.
        /// </summary>
        public BoardListViewModel(IDependencyService dependencyService)
        {
            //RefreshBoard = new Command(() => PopulateList());
            _dependencyService = dependencyService;
            Boardlist = new ObservableCollection<BoardViewModel>();
            PopulateList();
        }
        public BoardListViewModel() : this(new DependencyServiceWrapper())
        {
        }

        /// <summary>
        /// Recuperation des noms et id des tableaux auquels IsiTrello a acces.
        /// En cas d'erreur de l'api, envoie une notification pour avertir l'utilisateur.
        /// </summary>
        private async void PopulateList()
        {

            this.TrelloError = false;
            this.IsBusy = true;
            try
            {
                var Tboardlist = await _dependencyService.Get<IClient>().GetBoardlist();
                if (Boardlist.Count != 0)
                    Boardlist.Clear();
                foreach (var board in Tboardlist)
                {
                    this.Boardlist.Add(new BoardViewModel(board.id, board.name, board.prefs.bacgroundColor, _dependencyService));
                }
                ListName = "Expérimentation partage CoIndus";
                if (Boardlist.Count == 0)
                    this.ErrorMsg = "Aucun Tableau n'a été trouvé.";
            }
            catch (Exception e)
            {
                var Message = e.Message.Split(':');
                PageName = "Erreur";
                var notificator = _dependencyService.Get<IToastNotificator>();
                var options = new NotificationOptions
                {
                    Title = Message[0],
                    Description = "Le serveur a retourné une erreur : " + Message[1],
                    ClearFromHistory = false
                };
                //Pour iOS, permet d'eviter le cas ou en l'absence d'authorisation pour les notifications
                //le process reste bloqué avec la demande de notification.
                if (Device.RuntimePlatform==Device.iOS)
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
                this.ErrorMsg = "Une erreur inattendue est survenue lors de la requête auprès du serveur Trello.";
            }
            finally { this.IsBusy = false; }
        }
    }
}
