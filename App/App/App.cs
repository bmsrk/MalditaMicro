using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace App
{
    public class App : Application
    {
        public App()
        {
            var tabsPage = new TabbedPage();

            tabsPage.Title = "Maldita Micro!";

            tabsPage.Children.Add(new NavigationPage(new Views.ConsultaParadero())
            {
                Title = "Consulta Paradero"
            });

            MainPage = tabsPage;
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
