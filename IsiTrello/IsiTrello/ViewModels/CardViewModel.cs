using IsiTrello.Infrastructures;
using IsiTrello.Model;
using IsiTrello.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace IsiTrello.ViewModel
{
    public class CardViewModel : BaseViewModel
    {
        private readonly Card carte;
        private readonly IDependencyService _dependencyService;
        public string id { get { return carte.id; } }
        public string desc { get { return carte.desc; } }
        public string name { get { return carte.name; } }
        public Badges badges { get { return carte.badges; } }
        public bool dueComplete { get { return carte.dueComplete; } }
        public int pos { get; set; }
        public string due
        {
            get
            {
                if (carte.due != null)
                    return DateTime.Parse(carte.due, null, System.Globalization.DateTimeStyles.RoundtripKind).ToString("dd/MM/yy");
                else
                    return "";
            }
        }
        public string dueDate
        {
            get
            {
                if (carte.due != null)
                    return "📅 " + DateTime.Parse(carte.due, null, System.Globalization.DateTimeStyles.RoundtripKind).ToString("dd MMMM yyyy à HH:mm");
                else
                    return "";
            }
        }
        public List<object> idChecklists { get { return carte.idChecklists; } }
        public IList<CheckListViewModel> checklists { get; }
        public IList<LabelViewModel> labels { get; }
        public IList<Attachment> attachments { get; }

        //On utilise ici les bagdges comme indicateur de l'affichage des champs PiecesJointes et Checklist
        public bool HasChecklist { get { return carte.badges.checkItems > 0; } }
        public bool HasAttachment { get { return carte.badges.attachments > 0; } }
        public bool HasDue { get { return (!string.IsNullOrEmpty(carte.badges.due)); } }

        /// <summary>
        /// Constructeur, on cree a partir d'une carte recuperee par le client les elements utilises pour l'affichage.
        /// On recupere egalement a cette occasion les checklist et pieces jointes.
        /// </summary>
        /// <param name="carte">la carte ciblee</param>
        public CardViewModel(Card card, IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            attachments = new ObservableCollection<Attachment>();
            checklists = new ObservableCollection<CheckListViewModel>();
            labels = new ObservableCollection<LabelViewModel>();
            this.carte = card;

            foreach (Model.Label label in carte.labels)
                this.labels.Add(new LabelViewModel(label));
            
            GoToAttachment = new Command((url) => Device.OpenUri(new Uri(url.ToString())));
        }

        /// <summary>
        /// Generation des differentes collection necessaire a la presentation d'une carte.
        /// Conversion des labels, checklists et pieces jointes avec leur ViewModel respectifs.
        /// </summary>
        public async Task GetDetails()
        {
            if (HasAttachment && attachments.Count == 0 || HasChecklist && checklists.Count == 0)
            {
                IsBusy = true;
                try
                {
                    var TaskCheckList = _dependencyService.Get<IClient>().GetChecklist(carte.id);
                    var TaskAttachments = _dependencyService.Get<IClient>().GetAttachments(carte.id);
                    await Task.WhenAll(TaskCheckList, TaskAttachments);

                    var Checklists = TaskCheckList.Result;
                    foreach (Checklist checklist in Checklists)
                    {
                        this.checklists.Add(new CheckListViewModel(checklist));
                        RaisePropertyChanged(nameof(checklists));
                        RaisePropertyChanged(nameof(HasChecklist));
                    }

                    var Attachments = TaskAttachments.Result;
                    foreach (Attachment attachment in Attachments)
                    {
                        this.attachments.Add(attachment);
                        RaisePropertyChanged(nameof(attachments));
                        RaisePropertyChanged(nameof(HasAttachment));
                    }
                }
                catch
                {
                    throw new Exception("Erreur on attachments or checklists");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        public ICommand GoToAttachment { get; set; }

    }
}
