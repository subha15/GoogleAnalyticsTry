using Google.Apis.Analytics.v3;
using Google.Apis.Analytics.v3.Data;
using Google.Apis.AnalyticsReporting.v4;
using Google.Apis.AnalyticsReporting.v4.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Google.Apis.Analytics.v3.DataResource.GaResource;

namespace GoogleAnalyticsTry
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var credential = GetCredential();

                string choice = "";
                Console.WriteLine("Choose One Option with SL No\n1.Reatime\n2.Audience");
                
                var option = Console.ReadLine().ToString();

                if (option == "1")
                    choice = "Realtime";
                else if (option == "2")
                    choice = "Audience";

                switch (choice)
                    {
                        case "Realtime":

                            //DataResource.RealtimeResource.GetRequest request
                            //DataResource.RealtimeResource.GetRequest request = svc.Data.Realtime.Get(String.Format("ga:{0}", profileId), "rt:activeUsers");
                            //RealtimeData feed = request.Execute();

                            using (var svc = new AnalyticsService(
                                        new BaseClientService.Initializer
                                        {
                                            HttpClientInitializer = credential,
                                            ApplicationName = "Google Analytics API Console"
                                        })
                                   )
                            {
                                
                                RealtimeData response;

                                var request = svc.Data.Realtime.Get("ga:255352475", "rt:activeUsers");
                                request.Dimensions = "rt:userType,rt:country,rt:trafficType,rt:deviceCategory,rt:city";                           
                                response = request.Execute();
                                foreach (var row in response.Rows)
                                {
                                    foreach (string col in row)
                                    {
                                        Console.Write(col + " ");  // writes the value of the column
                                    }
                                    Console.Write("\r\n");
                                }

                                break;
                            }

                        case "Audience":

                            using (var svc = new AnalyticsReportingService(
                                            new BaseClientService.Initializer
                                            {
                                                HttpClientInitializer = credential,
                                                //ApplicationName = "Google Analytics API Console"
                                            })
                                )
                            {
                                var dateRange = new DateRange
                                {
                                    StartDate = "2021-11-01",
                                    EndDate = "2021-11-30"
                                };
                                var sessions = new Metric
                                {
                                    Expression = "ga:users",
                                    Alias = "Active Users"
                                };
                                var date = new Dimension { Name = "ga:date" };

                                var reportRequest = new ReportRequest
                                {
                                    DateRanges = new List<DateRange> { dateRange },
                                    //Dimensions = new List<Dimension> { date },
                                    Metrics = new List<Metric> { sessions },
                                    ViewId = "255352475"
                                };
                                var getReportsRequest = new GetReportsRequest
                                {
                                    ReportRequests = new List<ReportRequest> { reportRequest }
                                };
                                var batchRequest = svc.Reports.BatchGet(getReportsRequest);
                                var response = batchRequest.Execute();
                            if (response.Reports.First().Data.Rows != null)

                                Console.WriteLine(JsonConvert.SerializeObject(response.Reports.First().Data.Rows.First()));
                                    foreach (var x in response.Reports.First().Data.Rows)
                                    {
                                        Console.WriteLine(string.Join(", ", x.Dimensions) +
                                        "   " + string.Join(", ", x.Metrics.First().Values));
                                    }

                             

                               
                                
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid Choice");
                            Console.WriteLine("Press Any Key to Exit");
                            Console.ReadKey();
                            Environment.Exit(0);
                            break;

                    }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static GoogleCredential GetCredential()
        {
            using (var stream = new FileStream("C://Users/subho//Downloads//verdant-inquiry-331606-6b363acfa2e7.json",
                 FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream).CreateScoped(new[] { AnalyticsService.Scope.Analytics,AnalyticsService.Scope.AnalyticsReadonly,AnalyticsReportingService.Scope.AnalyticsReadonly, AnalyticsReportingService.Scope.Analytics });

                //const string loginEmailAddress = "sa-ga-reporting@verdant-inquiry-331606.iam.gserviceaccount.com";
                //return await GoogleWebAuthorizationBroker.AuthorizeAsync(
                //    GoogleClientSecrets.FromStream(stream).Secrets,
                //    new[] { AnalyticsReportingService.Scope.Analytics },
                //    loginEmailAddress, CancellationToken.None,
                //    new FileDataStore("GoogleAnalyticsApiConsole"));      
            }
        }
    }
}
