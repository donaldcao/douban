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
    class LatestHtmlParser
    {
        private static ObservableCollection<Movie> movieCollection = new ObservableCollection<Movie>();
        private static Downloader downloader = new Downloader(Movie.latest);

        /// <summary>
        /// Get latest movie
        /// </summary>
        /// <returns>Latest movie collection</returns>
        public async static Task<ObservableCollection<Movie>> getLatestMovie()
        {
            string latestHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(latestHtml);
            HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='item mod odd']");
            if (nodeCollection != null)
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    Movie movie;
                    try
                    {
                        movie = getLatestMovie(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    movieCollection.Add(movie);
                }
            }
            nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='item mod ']");
            if (nodeCollection != null)
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    Movie movie;
                    try
                    {
                        movie = getLatestMovie(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    movieCollection.Add(movie);
                }
            }

            return movieCollection;
        }

        /// <summary>
        /// Parse movie from html node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Movie</returns>
        private static Movie getLatestMovie(HtmlNode node)
        {
            string title = "";
            string region = "";
            string genre = "";
            string releaseDate = "";
            string posterUrl = "";
            string link = "";
            try
            {
                link = node.SelectNodes("a[@class='thumb']")[0].Attributes["href"].Value;
                posterUrl = node.SelectNodes("a[@class='thumb']")[0].SelectNodes("img")[0].Attributes["src"].Value;
                HtmlNode introNode = node.SelectNodes("div[@class='intro']")[0];
                title = introNode.SelectNodes("h3")[0].SelectNodes("a")[0].InnerText;
                HtmlNode ulNode = introNode.SelectNodes("ul")[0];
                HtmlNodeCollection liNodes = ulNode.SelectNodes("li[@class='dt']");
                releaseDate = liNodes[0].InnerText;
                genre = liNodes[1].InnerText;
                region = liNodes[2].InnerText;
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
            movie.genre = genre;
            movie.region = region;
            movie.posterUrl = posterUrl;
            movie.releaseDate = releaseDate;
            movie.title = title;
            return movie;
        }
    }
}
