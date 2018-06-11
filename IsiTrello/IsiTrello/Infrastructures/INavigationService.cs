using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.Infrastructures
{
    public interface INavigationService
    {
        bool HasPages();
        void RegisterPage(string pageKey, Func<Page> creator);
        void UnregisterPage(string pageKey);
        Task NavigateAsync(string pageKey, object viewModel = null);
        bool CanGoBack { get; }
        Task GoBackAsync();
        Task PushModalAsync(string pageKey, object viewModel = null);
        Task PopModalAsync();
        Task PopToPage(string pageKey, object viewModel = null);
    }
}
