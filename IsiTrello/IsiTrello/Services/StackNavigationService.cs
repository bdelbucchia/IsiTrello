using IsiTrello.Infrastructures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace IsiTrello.Services
{
    public class StackNavigationService : INavigationService
    {
        private static readonly Task TaskCompleted = Task.FromResult(0);
        readonly Dictionary<string, Func<Page>> registeredPages = new Dictionary<string, Func<Page>>();

        public void RegisterPage(string pageKey, Func<Page> creator)
        {
            if (string.IsNullOrEmpty(pageKey))
                throw new ArgumentNullException("pageKey");
            if (creator == null)
                throw new ArgumentNullException("creator");
            if (!registeredPages.ContainsKey(pageKey))
                registeredPages.Add(pageKey, creator);
        }

        public bool HasPages()
        {
            return registeredPages.Count > 0;
        }

        public void UnregisterPage(string pageKey)
        {
            if (string.IsNullOrEmpty(pageKey))
                throw new ArgumentNullException("pageKey");
            registeredPages.Remove(pageKey);
        }

        Page GetPageByKey(string pageKey)
        {
            if (string.IsNullOrEmpty(pageKey))
                throw new ArgumentNullException("key");

            Func<Page> creator;
            return registeredPages.TryGetValue(pageKey, out creator) ? creator.Invoke() : null;
        }

        private INavigation navigation;
        INavigation Navigation
        {
            get
            {
                if (navigation == null)
                {
                    // Most of the time this is good.
                    var main = Application.Current.MainPage;
                    if (main is NavigationPage)
                        navigation = main.Navigation;

                    // Special case for Master/Detail page.
                    else if (main is MasterDetailPage)
                    {
                        MasterDetailPage mdPage = main as MasterDetailPage;
                        if (mdPage != null)
                        {
                            if (mdPage.Master is NavigationPage)
                                navigation = mdPage.Master.Navigation;
                            if (mdPage.Detail is NavigationPage)
                                navigation = mdPage.Detail.Navigation;
                        }
                    }
                    else throw new Exception("Failed to locate navigation");
                }
                return navigation;
            }
        }

        public Task NavigateAsync(string pageKey, object viewModel = null)
        {
            if (Device.Idiom == TargetIdiom.Phone)
            {
                var mdPage = Application.Current.MainPage as MasterDetailPage;
                if (mdPage != null)
                {
                    mdPage.IsPresented = false;
                }
            }

            var page = GetPageByKey(pageKey);
            if (page == null)
                return TaskCompleted;

            if (viewModel != null)
                page.BindingContext = viewModel;

            return Navigation.PushAsync(page);
        }

        public bool CanGoBack
        {
            get
            {
                return Navigation.NavigationStack.Count > 1;
            }
        }

        public Task GoBackAsync()
        {
            return !CanGoBack ? TaskCompleted : Navigation.PopAsync();
        }

        public Task PushModalAsync(string pageKey, object viewModel = null)
        {
            var page = GetPageByKey(pageKey);
            if (page == null)
                throw new ArgumentException("Cannot navigate to unregistered page", "pageKey");

            if (viewModel != null)
                page.BindingContext = viewModel;

            return Navigation.PushModalAsync(page);
        }

        public Task PopModalAsync()
        {
            return Navigation.PopModalAsync();
        }

        /// <summary>
        /// Implemente le retour à une page particuliere comme point de depart du stack
        /// </summary>
        /// <param name="pageKey">l'identifiant de la page</param>
        /// <param name="viewModel">le viewmodel cible</param>
        /// <returns></returns>
        public Task PopToPage(string pageKey, object viewModel = null)
        {
            var page = GetPageByKey(pageKey);
            if (page == null)
                throw new ArgumentException("Cannot navigate to unregistered page", "pageKey");

            if (viewModel != null)
                page.BindingContext = viewModel;

            Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);
            return Navigation.PopToRootAsync();
        }
    }
}