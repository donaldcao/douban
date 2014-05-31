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
    class PeopleImageHtmlParser
    {
        private People people;
        public ObservableCollection<MovieImage> imageCollection { get; set; }
        private Downloader downloader;
        public bool hasMore;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p">People</param>
        public PeopleImageHtmlParser(People p)
        {
            imageCollection = new ObservableCollection<MovieImage>();
            people = p;
            people.nextImageLink = People.peopleLinkHeader + people.id + "/photos";
            downloader = new Downloader(people.nextImageLink);
            hasMore = false;
        }

        /// <summary>
        /// Get image
        /// </summary>
        /// <returns></returns>
        public async Task getImage()
        {
            String imageHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(imageHtml);
            HtmlNodeCollection uCollection = doc.DocumentNode.SelectNodes("//ul[@class='poster-col4 clearfix']");
            if (uCollection == null)
            {
                hasMore = false;
            }
            else
            {
                HtmlNodeCollection liCollection = uCollection[0].SelectNodes("li");
                if (liCollection == null)
                {
                    hasMore = false;
                }
                else
                {
                    foreach (HtmlNode node in liCollection)
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
                                string link = aCollection[0].Attributes["href"].Value.Trim();
                                people.nextImageLink = link;
                            }
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
            downloader = new Downloader(people.nextImageLink);
            await getImage();
        }

        /// <summary>
        /// Cancel download
        /// </summary>
        public void cancelDownload()
        {
            if (downloader != null)
            {
                downloader.cancelDownload();
            }
        }

        /// <summary>
        /// If download is canceled
        /// </summary>
        /// <returns>If download is canceled</returns>
        public bool isCanceled()
        {
            if (downloader != null)
            {
                return downloader.isCanceled();
            }
            return false;
        }

        /// <summary>
        /// Parse image from html node
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Image</returns>
        private MovieImage getImage(HtmlNode node)
        {
            string smallUrl = "";
            string largeUrl = "";
            string id = "";
            try
            {
                string link = node.SelectNodes("div[@class='cover']")[0].SelectNodes("a")[0].Attributes["href"].Value.Trim();
                int headerLength = people.id.Length + 7;
                id = link.Substring(People.peopleLinkHeader.Length + headerLength, link.Length - headerLength - 1 - People.peopleLinkHeader.Length);
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
    }
}
