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
    class PeopleMovieHtmlParser
    {
        private People people;
        public ObservableCollection<Movie> movieCollection { get; set; }
        private Downloader downloader;
        public bool hasMore { get; set; }

        public PeopleMovieHtmlParser(People p)
        {
            people = p;
            people.nextMovieLink = People.peopleLinkHeader + people.id + "/movies";
            hasMore = false;
            movieCollection = new ObservableCollection<Movie>();
            downloader = new Downloader(people.nextMovieLink);
        }

        public async Task getMovie()
        {
            String movieHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(movieHtml);
            HtmlNodeCollection nameNodes = doc.DocumentNode.SelectNodes("//ul");
            if (nameNodes == null || nameNodes.Count < 4)
            {
                hasMore = false;
            }
            else
            {
                foreach (HtmlNode node in nameNodes[3].SelectNodes("li"))
                {
                    Movie m;
                    try
                    {
                        m = getMovie(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    movieCollection.Add(m);
                }
                HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='paginator']");
                if (nodeCollection == null)
                {
                    hasMore = false;
                }
                else
                {
                    HtmlNodeCollection nc = nodeCollection[0].SelectNodes("span[@class='next']");
                    if (nc == null)
                    {
                        hasMore = false;
                    }
                    else
                    {
                        HtmlNodeCollection aCollection = nc[0].SelectNodes("a");
                        if (aCollection == null)
                        {
                            hasMore = false;
                        }
                        else
                        {
                            hasMore = true;
                            string link = aCollection[0].Attributes["href"].Value;
                            link = link.Replace("&amp;", "&");
                            people.nextMovieLink = People.peopleLinkHeader + people.id + "/movies" + link;
                        }
                    }
                }
            }
        }

        public async Task loadMore()
        {
            downloader = new Downloader(people.nextMovieLink);
            await getMovie();
        }

        public void cancelDownload()
        {
            if (downloader != null)
            {
                downloader.cancelDownload();
            }
        }

        public bool isCanceled()
        {
            if (downloader != null)
            {
                return downloader.isCanceled();
            }
            return false;
        }

        private Movie getMovie(HtmlNode node)
        {
            string id = "";
            string name = "";
            string rating = "";
            string star = "";
            string posterUrl = "";

            try
            {
                HtmlNode dlNode = node.SelectNodes("dl")[0];
                posterUrl = dlNode.SelectNodes("dt")[0].SelectNodes("a")[0].SelectNodes("img")[0].Attributes["src"].Value.Trim();
                HtmlNode ddNode = dlNode.SelectNodes("dd")[0];
                HtmlNode aNode = ddNode.SelectNodes("h6")[0].SelectNodes("a")[0];
                string link = aNode.Attributes["href"].Value;
                name = aNode.InnerText.Trim();
                id = link.Substring(Movie.movieLinkHeader.Length, link.Length - 1 - Movie.movieLinkHeader.Length);
                HtmlNode starNode = ddNode.SelectNodes("div[@class='star clearfix']")[0];
                rating = starNode.SelectNodes("span")[1].InnerText.Trim();
                if (rating == string.Empty)
                {
                    rating = "0";
                }
                star = Util.getStarPath(rating);
            }
            catch (Exception)
            {
                throw;
            }
            Movie m = new Movie();
            m.id = id;
            m.title = Util.replaceSpecialChar(name);
            m.rating = rating;
            m.star = star;
            m.posterUrl = posterUrl;
            return m;
        }


    }
}
