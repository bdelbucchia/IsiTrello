using IsiTrello.Infrastructures;
using IsiTrello.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.ViewModel
{
    public class ListsViewModel : BaseViewModel
    {
        private readonly IDependencyService _dependencyService;

        readonly List liste;

        public string name
        {
            get { return liste.name; }
        }
        public string id
        {
            get { return liste.id; }
        }
        public bool closed
        {
            get { return liste.closed; }
        }
        private int _Position;

        public int Position
        {
            get { return _Position; }
            set
            {
                SetPropertyValue(ref _Position, value);
            }
        }
        private IList<CardViewModel> _cards;

        public IList<CardViewModel> cards
        {
            get { return _cards; }
            set
            {
                SetPropertyValue(ref _cards, value);
            }
        }

        public ListsViewModel(List liste, IDependencyService DS)
        {
            _dependencyService = DS;
            this.liste = liste;
            this.cards = new ObservableCollection<CardViewModel>();
            var position = 0;
            foreach (var card in liste.cards)
            {
                this.cards.Add(new CardViewModel(card, _dependencyService) { pos = position});
                position += 1;
            }
        }

        public async Task LoadDetails()
        {
            foreach (var card in cards)
            {
                if (card.pos != Position)
                    await card.GetDetails();
            }
        }
    }
}
