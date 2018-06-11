using IsiTrello.Infrastructures;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.Services
{
    public class MessageVisualizerService : IMessageVisualizerService
    {
        public async Task<bool> ShowMessage(
            string title, string message, string ok, string cancel = null)
        {
            if (cancel == null)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, ok);
                return true;
            }

            return await Application.Current.MainPage.DisplayAlert(
                title, message, ok, cancel);
        }
    }
}