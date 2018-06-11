using IsiTrello.Model;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace IsiTrello.ViewModel
{
    public class LabelViewModel
    {
        readonly Label label;
        public string id { get { return label.id; } }
        public string idBoard { get { return label.idBoard; } }
        public string name { get { return label.name; } }
        public string color { get { return ColorHex(label.color); } }
        public int uses { get; set; }

        /// <summary>
        /// Convertisseur de couleur pour maintenir les couleurs utilisees en accord avec celle utilisees par le site.
        /// </summary>
        /// <param name="input">Champ couleur fourni par l'api</param>
        /// <returns></returns>
        private string ColorHex(string input)
        {
            switch (input)
            {
                case ("green"):
                    return "#61BD4F";
                case ("yellow"):
                    return "#F2D600";
                case ("orange"):
                    return "#FFAB4A";
                case ("red"):
                    return "#EB5A46";
                case ("purple"):
                    return "#C377E0";
                case ("blue"):
                    return "#0079BF";
                case ("sky"):
                    return "#0098B7";
                case ("lime"):
                    return "#4DC26B";
                case ("pink"):
                    return "#CD5A91";
                case ("black"):
                    return "#272727";
                case (null):
                    return "Transparent";
                default:
                    return input;
                }
        }
        public LabelViewModel(Label label)
        {
            this.label = label;
        }
    }

    public class CheckItemVM
    {
        readonly CheckItem checkitem;
        public bool ischecked { get; private set; }
        public string itemname { get { return checkitem.name; } }
        public CheckItemVM(CheckItem cki)
        {
            this.checkitem = cki;
            if (cki.state == "complete")
                ischecked = true;
            else
                ischecked = false;
        }
    }
    public class CheckListViewModel
    {
        readonly Checklist checklist;
        public string namelist { get { return checklist.name; } }
        public IList<CheckItemVM> listitems{ get; private set; }

        /// <summary>
        /// Constructeur, converti les valeurs des "CheckItems" pour l'affichage.
        /// </summary>
        /// <param name="ckl">Checklist affichee</param>
        public CheckListViewModel(Checklist vChecklist)
        {
            listitems = new ObservableCollection<CheckItemVM>();
            this.checklist = vChecklist;
            foreach (CheckItem checkitem in vChecklist.checkitems)
            {
                listitems.Add(new CheckItemVM(checkitem));       
            }
        }
    }
}
