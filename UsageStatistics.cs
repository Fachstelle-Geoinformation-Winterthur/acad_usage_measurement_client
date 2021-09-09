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
            winUsageStatistics();
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }

        [CommandMethod("win_usage_statistics")]
        public void winUsageStatistics()
        {
            Thread longRunningTask = new Thread(() => _winUsageStatistics());
            longRunningTask.Start();
        }

        private void _winUsageStatistics()
        {

            try
            {
                System.Uri server1Url = null;
                string server2UrlString = null;
                string propsPath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                    "acad_usage_measurement_properties.txt");

                bool server1UrlGood = false;
                try
                {
                    Properties props = new Properties(propsPath);
                    string server1UrlString = props.getValue("server1");
                    server1UrlGood = System.Uri.TryCreate(server1UrlString,
                        System.UriKind.Absolute, out server1Url);
                    if (server1UrlGood)
                    {
                        server1UrlGood = server1Url.Scheme == System.Uri.UriSchemeHttp
                            || server1Url.Scheme == System.Uri.UriSchemeHttps;
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

                try
                {
                    Properties props = new Properties(propsPath);
                    server2UrlString = props.getValue("server2");
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
                            requestString = server1Url.ToString() + "/ping?username=" + System.Environment.UserName +
                                "&domainname=" + System.Environment.UserDomainName + "&appcode=" +
                                appCode + "&version=" + version + "&machinename=" + System.Environment.MachineName;
                            Task<HttpResponseMessage> httpTask = httpClient.GetAsync(requestString);
                            Thread.Sleep(timeToSleep);
                            httpTask.Wait();
                            success = httpTask.Status == TaskStatus.RanToCompletion;
                        }
                        catch (Exception ex)
                        {
                            // do nothing
                        }
                        catch (System.Exception ex)
                        {
                            // do nothing
                        }
                        if (!success)
                        {
                            try
                            {
                                Task<HttpResponseMessage> httpTask = httpClient.GetAsync(
                                    server2UrlString + "/ping?username=" + System.Environment.UserName +
                                    "&domainname=" + System.Environment.UserDomainName + "&appcode=" +
                                    appCode + "&version=" + version);
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
