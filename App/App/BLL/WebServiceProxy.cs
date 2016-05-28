using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using Models;
using System.Threading.Tasks;

namespace BLL
{
    public class WebServiceProxy
    {
        string url = "http://web.smsbus.cl/web/buscarAction.do?d=busquedaParadero&ingresar_paradero={0}";

        private static CookieContainer cookieContainer { get; set; }

        public async Task<Paradero> Read(string CodigoParadero)
        {
            return await readSite(CodigoParadero);
        }

        private Models.Paradero parseMultipleServicios(string data, string codigoParadero)
        {
            var array = data.Split('<');

            var nombreParadero = array.Where(x => x.Contains("nombre_paradero_respuesta")).First();

            nombreParadero = nombreParadero.Substring(nombreParadero.IndexOf(':') + 1, nombreParadero.Length - (nombreParadero.IndexOf(':') + 1)).Trim();

            var errores = array.Where(x => x.Contains("servicio_error_solo_paradero") || x.Contains("respuesta_error_solo_paradero"));

            var exitos = array.Where(x => x.Contains("servicio_respuesta_solo_paradero") || x.Contains("bus_respuesta_solo_paradero") || x.Contains("tiempo_respuesta_solo_paradero") || x.Contains("distancia_respuesta_solo_paradero"));

            var paradero = new Models.Paradero();

            paradero.Nombre = nombreParadero;
            paradero.Codigo = codigoParadero;
            paradero.Micros = new List<Models.Micro>();

            for (var i = 1; i <= (exitos.Count() / 4); ++i)
            {
                var n = i * 4 - 4;

                paradero.Micros.Add(new Models.Micro()
                {
                    PoseeInfo = true,
                    Servicio = CleanString(exitos.ElementAt(n)),
                    Patente = CleanString(exitos.ElementAt(n + 1)),
                    Mensaje = CleanString(exitos.ElementAt(n + 2)),
                    Distancia = int.Parse(CleanString(exitos.ElementAt(n + 3)).Replace(" mts.", ""))
                });
            }

            paradero.Micros = paradero.Micros.OrderBy(x => x.Distancia).ToList();

            for (var i = 1; i <= (errores.Count() / 2); ++i)
            {
                var n = i * 2 - 2;

                paradero.Micros.Add(new Models.Micro()
                {
                    PoseeInfo = false,
                    Servicio = CleanString(errores.ElementAt(n)),
                    Mensaje = CleanString(errores.ElementAt(n + 1))
                });
            }

            

            return paradero;
        }

        private async Task<Models.Paradero> readSite(string paradero)
        {
            var handler = getHttpHandler();

            var client = new HttpClient(handler);

            client.DefaultRequestHeaders.ExpectContinue = false;

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Accept", "*/*")
            });

            var response = await client.PostAsync(string.Format(url, paradero), null);

            var responseText = await response.Content.ReadAsStringAsync();

            if (responseText.Contains("menu_solo_paradero"))
            {
                return parseMultipleServicios(responseText, paradero);
            }
            else if (responseText.Contains("menu_respuesta"))
            {
                return parseSingleServicio(responseText, paradero);
            }
            else
            {
                readCookies(response);
                return await readSite(paradero);
            }
        }

        private Models.Paradero parseSingleServicio(string data, string codigoParadero)
        {
            var array = data.Split('<');

            var nombreParadero = array.Where(x => x.Contains("nombre_paradero_respuesta")).First();

            nombreParadero = nombreParadero.Substring(nombreParadero.IndexOf(':') + 1, nombreParadero.Length - (nombreParadero.IndexOf(':') + 1)).Trim();

            var errores = array.Where(x => x.Contains("respuesta_error"));

            var exitos = array.Where(x => x.Contains("proximo_bus_respuesta") || x.Contains("proximo_tiempo_respuesta") || x.Contains("proximo_distancia_respuesta"));

            var paradero = new Paradero();

            paradero.Nombre = nombreParadero;
            paradero.Codigo = codigoParadero;
            paradero.Micros = new List<Micro>();

            var numServicio = CleanString(array.ElementAt(array.ToList().IndexOf(array.Where(x => x.Contains("numero_parada_respuesta")).First()) + 8));

            if (exitos.Count() > 0)
            {
                for (var i = 1; i <= (exitos.Count() / 3); ++i)
                {
                    var n = i * 3 - 3;

                    paradero.Micros.Add(new Micro()
                    {
                        Servicio = numServicio,
                        Mensaje = CleanString(exitos.ElementAt(n + 1)),
                        PoseeInfo = true,
                        Patente = CleanString(exitos.ElementAt(n)),
                        Distancia = int.Parse(CleanString(exitos.ElementAt(n + 2)).Replace(" mts.", ""))
                    });
                }
            }

            paradero.Micros = paradero.Micros.OrderBy(x => x.Distancia).ToList();

            if (errores.Count() > 0)
            {
                paradero.Micros.Add(new Micro()
                {
                    Servicio = numServicio,
                    Mensaje = CleanString(array.Where(x => x.Contains("respuesta_error")).First()),
                    PoseeInfo = false
                });
            }

            return paradero;
        }

        private HttpClientHandler getHttpHandler()
        {
            var handler = new HttpClientHandler();

            if (cookieContainer != null)
            {
                handler.CookieContainer = cookieContainer;
            }

            return handler;
        }

        private static string CleanString(string str)
        {
            return str.Substring(str.IndexOf('>') + 1, str.Length - (str.IndexOf('>') + 1));
        }

        private void readCookies(HttpResponseMessage response)
        {
            var pageUri = new Uri("http://web.smsbus.cl");

            IEnumerable <string> cookies;
            if (response.Headers.TryGetValues("set-cookie", out cookies))
            {
                cookieContainer = new CookieContainer();

                foreach (var c in cookies)
                {
                    cookieContainer.SetCookies(pageUri, c);
                }
            }
        }
    }
}

