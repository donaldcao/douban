using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Threading;
using System.IO;
using System.Net;
using Microsoft.Phone.Tasks;

namespace PanoramaApp2
{
    class TesterTrailer
    {
        public static  async Task testHttpClient()
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            HttpClient client = new HttpClient(handler);
            String bid = "";
            String ll = "";
            System.Diagnostics.Debug.WriteLine("here in test http client");
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://movie.douban.com/trailer/157497/#content");
                System.Diagnostics.Debug.WriteLine(response.Headers.ToString());
                System.Diagnostics.Debug.WriteLine("cookies is " + response.Headers.GetValues("Set-Cookie").FirstOrDefault());
                Uri uri = new Uri("http://movie.douban.com/trailer/157497/#content");
                IEnumerable<Cookie> responseCookies = cookies.GetCookies(uri).Cast<Cookie>();
                foreach (Cookie cookie in responseCookies)
                {
                    if (cookie.Name == "bid")
                    {
                        bid = cookie.Value;
                    }
                    if (cookie.Name == "ll")
                    {
                        ll = cookie.Value;
                    }
                }
                System.Diagnostics.Debug.WriteLine(bid + " " + ll);

                HttpClientHandler requestHandler = new HttpClientHandler();
                requestHandler.AllowAutoRedirect = false;
                HttpClient requestClient = new HttpClient(requestHandler);
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, "http://movie.douban.com/trailer/video_url?tid=157497");
                requestMessage.Headers.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                requestMessage.Headers.Add("Accept-Encoding", "gzip, deflate");
                requestMessage.Headers.Add("Accept-Language", "en-US,en;q=0.5");
                requestMessage.Headers.Add("Connection", "close");
                String cookieString = "bid=" + bid + "; ll=" + ll;
                requestMessage.Headers.Add("Cookie", cookieString);
                requestMessage.Headers.Add("Host", "movie.douban.com");
                requestMessage.Headers.Add("Referer", "	http://movie.douban.com/swf/movie_player_1.4.4.swf");
                requestMessage.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; rv:30.0) Gecko/20100101 Firefox/30.0");
                System.Diagnostics.Debug.WriteLine("request header is " + requestMessage.Headers.ToString());
                HttpResponseMessage newResponse = await requestClient.SendAsync(requestMessage);
                System.Diagnostics.Debug.WriteLine("Header is " + newResponse.Headers.ToString());
                System.Diagnostics.Debug.WriteLine("Status code is " + newResponse.StatusCode);
                System.Diagnostics.Debug.WriteLine(newResponse.Headers.GetValues("Location").FirstOrDefault());

                String videoUri = newResponse.Headers.GetValues("Location").FirstOrDefault();

                MediaPlayerLauncher mediaPlayerLauncher = new MediaPlayerLauncher();

                mediaPlayerLauncher.Media = new Uri(@videoUri, UriKind.Absolute);
                mediaPlayerLauncher.Location = MediaLocationType.Data;
                mediaPlayerLauncher.Controls = MediaPlaybackControls.Pause | MediaPlaybackControls.Stop;
                mediaPlayerLauncher.Orientation = MediaPlayerOrientation.Landscape;

                mediaPlayerLauncher.Show();
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("Exception is " + e.Message);
                System.Diagnostics.Debug.WriteLine("TOstring is " + e.ToString());
            }
        }
    }
}
