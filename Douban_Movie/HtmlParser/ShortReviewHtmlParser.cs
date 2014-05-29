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
    class ShortReviewHtmlParser
    {
        private ObservableCollection<ShortReview> shortReviewCollection;
        private bool moreReview;
        private Movie movie { get; set; }
        private Downloader downloader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m">Movie</param>
        public ShortReviewHtmlParser(Movie m)
        {
            movie = m;
            shortReviewCollection = new ObservableCollection<ShortReview>();
            moreReview = false;
            movie.nextShortReviewLink = Movie.movieLinkHeader + movie.id + "/comments";
            downloader = new Downloader(movie.nextShortReviewLink);
        }

        /// <summary>
        /// Get short review
        /// </summary>
        /// <returns>short review and if there's more</returns>
        public async Task<Tuple<ObservableCollection<ShortReview>, bool>> getShortReview()
        {
            String shortReviewHtml = await downloader.downloadString();
            parseShortReviewHtml(shortReviewHtml);
            return Tuple.Create(shortReviewCollection, moreReview);
        }

        private void parseShortReviewHtml(String shortReviewHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(shortReviewHtml);
            HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='comment']");
            
            // Short review doesn't exist
            if (nodeCollection == null)
            {
                moreReview = false;
            }
            else
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    ShortReview sr;
                    try
                    {
                        sr = getShortReview(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    shortReviewCollection.Add(sr);
                }
                nodeCollection = doc.DocumentNode.SelectNodes("//div[@id='paginator']");
                if (nodeCollection == null)
                {
                    moreReview = false;
                }
                else
                {
                    HtmlNodeCollection nc = nodeCollection[0].SelectNodes("a[@class='next']");
                    if (nc == null)
                    {
                        moreReview = false;
                    }
                    else
                    {
                        moreReview = true;
                        string link = nc[0].Attributes["href"].Value;
                        link = link.Replace("&amp;", "&");
                        movie.nextShortReviewLink = Movie.movieLinkHeader + movie.id + "/comments" + link;
                    }
                }
            }
        }

        /// <summary>
        /// Load more review
        /// </summary>
        /// <returns></returns>
        public async Task<bool> loadMore()
        {
            downloader = new Downloader(movie.nextShortReviewLink);
            String shortReviewHtml = await downloader.downloadString();
            parseShortReviewHtml(shortReviewHtml);
            return moreReview;
        }

        /// <summary>
        /// Get short review from node
        /// </summary>
        /// <param name="node">HtmlNode</param>
        /// <returns>ShortReview</returns>
        private ShortReview getShortReview(HtmlNode node)
        {
            string name = "";
            string time = "";
            string content = "";
            string star = App.imagePath + "00 star.png";
            try
            {
                HtmlNode spanNode = node.SelectNodes("h3")[0].SelectNodes("span[@class='comment-info']")[0];
                name = spanNode.SelectNodes("a")[0].InnerText;
                HtmlNodeCollection nodes = spanNode.SelectNodes("span");
                if (nodes.Count == 1)
                {
                    time = Util.replaceSpecialChar(nodes[0].InnerText.Trim());
                }
                else
                {
                    time = nodes[1].InnerText.Trim();
                    star = App.imagePath + nodes[0].Attributes["class"].Value.Substring(7, 2) + " star.png";
                }
                content = Util.formatShortReview(node.SelectNodes("p")[0].InnerText.Trim());
            }
            catch (Exception)
            {
                throw;
            }
            ShortReview sr = new ShortReview();
            sr.author = name;
            sr.time = time;
            sr.content = content;
            sr.star = star;
            return sr;
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
