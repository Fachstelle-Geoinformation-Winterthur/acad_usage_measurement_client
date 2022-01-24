// <copyright company="Vermessungsamt Winterthur">
// Author: Edgar Butwilowski
// Copyright (c) 2021 Vermessungsamt Winterthur. All rights reserved.
// </copyright>

using Autodesk.AutoCAD.Runtime;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(win.acad_usage_measurement.UsageStatistics))]
namespace win.acad_usage_measurement
{
    public class UsageStatistics : IExtensionApplication
    {

        public void Initialize()
        {
            winUsageStatisticsInitialize();
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }

        public void winUsageStatisticsInitialize()
        {
            Thread longRunningTask = new Thread(() => _winUsageStatisticsSendPing());
            longRunningTask.Start();
        }

        private void _winUsageStatisticsSendPing()
        {

            try
            {
                System.Uri serverUrl = null;
                string propsPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "acad_usage_measurement_properties.txt");

                bool serverUrlGood = false;
                try
                {
                    Properties props = new Properties(propsPath);
                    string serverUrlString = props.getValue("server");
                    serverUrlGood = System.Uri.TryCreate(serverUrlString,
                        System.UriKind.Absolute, out serverUrl);
                    if (serverUrlGood)
                    {
                        serverUrlGood = serverUrl.Scheme == System.Uri.UriSchemeHttp
                            || serverUrl.Scheme == System.Uri.UriSchemeHttps;
                    }
                }
                catch (Exception ex)
                {
                    // do nothing
                }
                catch (System.IO.FileNotFoundException ex)
                {
                    // do nothing
                }
                catch (System.Exception ex)
                {
                    // do nothing
                }

                string appCode = "0"; // 0 = unknown
                string version = "unknown";

                try
                {
                    string appTitleText = Autodesk.AutoCAD.ApplicationServices.Application.MainWindow.Text.ToString();
                    if (appTitleText.Contains("AutoCAD Map"))
                    {
                        appCode = "2";
                    }
                    else if (appTitleText.Contains("Civil 3D"))
                    {
                        appCode = "3";
                    }
                    else if (appTitleText.Contains("AutoCAD"))
                    {
                        appCode = "1";
                    }
                }
                catch (Exception ex)
                {
                    // do nothing
                }
                catch (System.Exception ex)
                {
                    // do nothing
                }


                try
                {
                    version = Autodesk.AutoCAD.ApplicationServices.Application.Version.ToString();
                }
                catch (Exception ex)
                {
                    // do nothing
                }
                catch (System.Exception ex)
                {
                    // do nothing
                }

                int timeToSleep = 600000; // = 1000 millis * 60 sec * 10 min => 10 min

                using (HttpClient httpClient = new HttpClient())
                {
                    bool success;
                    string requestString;
                    while (true)
                    {
                        success = false;
                        try
                        {
                            requestString = serverUrl.ToString() + "/ping?username=" + System.Environment.UserName +
                                "&domainname=" + System.Environment.UserDomainName + "&appcode=" +
                                appCode + "&version=" + version + "&machinename=" + System.Environment.MachineName;
                            Task<HttpResponseMessage> httpTask = httpClient.GetAsync(requestString);
                            Thread.Sleep(timeToSleep);
                            httpTask.Wait();
                        }
                        catch (Exception ex)
                        {
                            // do nothing
                        }
                        catch (System.Exception ex)
                        {
                            // do nothing
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                // do nothing
            }
            catch (System.Exception ex)
            {
                // do nothing
            }
        }


    }
}
