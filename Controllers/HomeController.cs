using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Security;
using System.Net.Http;
using Newtonsoft.Json;
using static Semprini.Mattia._5i.Calendar.JsonMeteo;

namespace Semprini.Mattia._5i.Calendar.Controllers
{
    public class HomeController : Controller
    { 

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult indexmeteo()
        {
            return View("Index_Meteo");
        }

        [HttpPost]
        public async Task<ActionResult> Cerca(Jsonmeteo u)
        {
            try
            {
                Jsonmeteo Meteo;
                HttpClient client = new HttpClient();

                string result = await client.GetStringAsync(
                   new Uri(@"http://api.wunderground.com/api/ff9622a1a7822d3a/conditions/q/IT/" + u.City + ".json"));
                Meteo = JsonConvert.DeserializeObject<Jsonmeteo>(result);
                if (Meteo.current_observation != null)
                {
                    return View("View_Meteo", Meteo);
                }
                else
                {
                    return View("Error");
                }
            }
            catch
            {
                return View("Error");
            }
        }

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Google Calendar API .NET Quickstart";

        public ActionResult Calendario()
        {
            try
            {

                List<string> eventi = new List<string>();
                UserCredential credential;

                using (var stream =
                    new FileStream(Server.MapPath("~/client_secret.json"), FileMode.Open, FileAccess.Read))
                {
                    string credPath = System.Environment.GetFolderPath(
                        System.Environment.SpecialFolder.Personal);


                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.Load(stream).Secrets,
                        Scopes,
                        "user",
                        CancellationToken.None).Result;
                    //eventi.Add("Il file delle credenziali è stato salvato in: " + credPath);
                }


                // Create Google Calendar API service.
                var service = new CalendarService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = ApplicationName,
                });

                // Define parameters of request.
                EventsResource.ListRequest request = service.Events.List("primary");
                request.TimeMin = DateTime.Now;
                request.ShowDeleted = false;
                request.SingleEvents = true;
                request.MaxResults = 10;
                request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

                // List events.
                Events events = request.Execute();

                if (events.Items != null && events.Items.Count > 0)
                {
                    foreach (var eventItem in events.Items)
                    {
                        string when = eventItem.Start.DateTime.ToString();
                        if (String.IsNullOrEmpty(when))
                        {
                            when = eventItem.Start.Date;
                        }
                        eventi.Add(eventItem.Summary + " il " + when);
                    }
                }
                else
                {
                    eventi.Add("Nessun evento imminente.");
                }
                return View("View_eventi", eventi);
            }
            catch
            {
                return View("Error");
            }
            
        }

    }
    
        
    

}

