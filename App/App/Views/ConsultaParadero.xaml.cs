using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;

namespace App.Views
{
    public partial class ConsultaParadero : ContentPage
    {
        public ConsultaParadero()
        {
            InitializeComponent();

            Micros.ItemSelected += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };

            Micros.ItemTapped += (sender, e) => {
                ((ListView)sender).SelectedItem = null;
            };

            Micros.IsPullToRefreshEnabled = true;
            Micros.Refreshing += Micros_Refreshing;
        }

        async void Micros_Refreshing(object sender, EventArgs e)
        {
            await RefreshMicros();
            Micros.EndRefresh();
        }

        async void OnEntryCompleted(object sender, EventArgs args)
        {
            await RefreshMicros();
        }

        async Task RefreshMicros()
        {
            try
            {
                var bll = new BLL.WebServiceProxy();

                var model = await bll.Read(CodigoParadero.Text.ToUpper());
                Micros.ItemsSource = null;
                Micros.ItemsSource = model.Micros;

                NombreParadero.Text = model.Nombre;
            }
            catch (Exception ex)
            {
                Micros.ItemsSource = null;
                await DisplayAlert("ERROR", ex.Message, "OK");
            }
        }
    }
}
