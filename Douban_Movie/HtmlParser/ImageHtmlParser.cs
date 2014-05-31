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
    class ImageHtmlParser
    {
        private Movie movie;
        public ObservableCollection<MovieImage> imageCollection { get; set; }
        public bool hasMore { get; set; }
        private Downloader downloader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m"></param>
        public ImageHtmlParser(Movie m)
        {
            movie = m;
            imageCollection = new ObservableCollection<MovieImage>();
            movie.nextImageLink = Movie.movieLinkHeader + movie.id + "/photos?type=S";
            downloader = new Downloader(movie.nextImageLink);
            hasMore = false;
        }

        /// <summary>
        /// Get image
        /// </summary>
        /// <returns></returns>
        public async Task getImage()
        {
            String imageHtml = await downloader.downloadString();
            parseImageHtml(imageHtml);
        }

        /// <summary>
        /// Parse image html page
        /// </summary>
        /// <param name="imageHtml"></param>
        public void parseImageHtml(String imageHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(imageHtml);
            HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='cover']");
            if (nodeCollection == null)
            {
                hasMore = false;
            }
            else
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    MovieImage image;
                    try
                    {
                        image = getImage(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    imageCollection.Add(image);
                }
                nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='paginator']");
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
                            movie.nextImageLink = link;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Load more image
        /// </summary>
        /// <returns></returns>
        public async Task loadMore()
        {
            downloader = new Downloader(movie.nextImageLink);
            String imageHtml = await downloader.downloadString();
            parseImageHtml(imageHtml);
        }

        /// <summary>
        /// Get image from html node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private MovieImage getImage(HtmlNode node)
        {
            string smallUrl = "";
            string largeUrl = "";
            string id = "";
            try
            {
                string link = node.SelectNodes("a")[0].Attributes["href"].Value.Trim();
                id = link.Substring(Movie.homePage.Length + 14, link.Length - 15 - Movie.homePage.Length);
                smallUrl = MovieImage.smallUrlHeader + id + ".jpg";
                largeUrl = MovieImage.largeUrlHeader + id + ".jpg";
            }
            catch (Exception)
            {
                throw;
            }
            MovieImage image = new MovieImage();
            image.id = id;
            image.smallUrl = smallUrl;
            image.largeUrl = largeUrl;
            return image;
        }

        /// <summary>
        /// Cancel download
        /// </summary>
        public void cancelDownload()
        {
            downloader.cancelDownload();
        }

        /// <summary>
        /// If download is canceled
        /// </summary>
        /// <returns>If download is canceled</returns>
        public bool isCanceled()
        {
            return downloader.isCanceled();
        }
    }
}
