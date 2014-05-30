using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;
using System.Windows.Controls.Primitives;
using Microsoft.Phone.Controls;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using Microsoft.Phone.Shell;
using System.Windows;
using PanoramaApp2.Resources;
using PanoramaApp2.Utility;

namespace PanoramaApp2.HtmlParser
{
    class Top250HtmlParser
    {
        private static int currentIndex = 0;
        private static int maxIndex = 9;

        public static ObservableCollection<Movie> observableMovieList = new ObservableCollection<Movie>();
        private static Downloader downloader;
        public static bool hasMore = true;

        public static async Task getTop250()
        {
            downloader = new Downloader(Movie.top250 + "?start=" + currentIndex * 25 + "&format=");
            String top250Html = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(top250Html);
            HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='item']");
            if (nodeCollection != null)
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    Movie movie;
                    try
                    {
                        movie = getTopMovie(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    observableMovieList.Add(movie);
                }
            }
            currentIndex++;
            if (currentIndex > maxIndex)
            {
                hasMore = false;
            }
            else
            {
                hasMore = true;
            }
        }

        private static Movie getTopMovie(HtmlNode node)
        {
            string title = "";
            string posterUrl = "";
            string rating = "";
            string quote = "";
            string link = "";

            try
            {
                HtmlNode linkNode = node.SelectNodes("div[@class='pic']")[0].SelectNodes("a")[0];
                link = linkNode.Attributes["href"].Value.Trim();
                HtmlNode imgNode = linkNode.SelectNodes("img")[0];
                posterUrl = imgNode.Attributes["src"].Value.Trim();
                title = imgNode.Attributes["alt"].Value.Trim();
                HtmlNode bdNode = node.SelectNodes("div[@class='info']")[0].SelectNodes("div[@class='bd']")[0];
                HtmlNodeCollection ratingNodes = bdNode.SelectNodes("div[@class='star']")[0].SelectNodes("span[@class='rating5-t']");
                if (ratingNodes == null)
                {
                    ratingNodes = bdNode.SelectNodes("div[@class='star']")[0].SelectNodes("span[@class='rating45-t']");
                }
                rating = ratingNodes[0].SelectNodes("em")[0].InnerText.Trim();
                quote = bdNode.SelectNodes("p[@class='quote']")[0].SelectNodes("span")[0].InnerText.Trim();
            }
            catch (System.NullReferenceException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
            }

            Movie movie = new Movie();
            movie.id = "";
            for (int i = Movie.movieLinkHeader.Length; i < link.Length; i++)
            {
                if (link[i] >= '0' && link[i] <= '9')
                {
                    movie.id += link[i];
                }
                else
                {
                    break;
                }
            }
            movie.title = Util.replaceSpecialChar(title);
            movie.rating = rating;
            movie.star = Util.getStarPath(rating);
            movie.posterUrl = posterUrl;
            movie.quote = Util.replaceSpecialChar(quote);
            return movie;
        }
    }
}
