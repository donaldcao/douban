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
    class ReviewParser
    {
        private Movie movie;
        public ObservableCollection<Review> reviewCollection { get; set; }
        public bool hasMoreReview { get; set; }
        private Downloader downloader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="m">Movie</param>
        public ReviewParser(Movie m)
        {
            movie = m;
            movie.nextReviewLink = Movie.movieLinkHeader + movie.id + "/reviews";
            reviewCollection = new ObservableCollection<Review>();
            downloader = new Downloader(movie.nextReviewLink);
            hasMoreReview = false;
        }

        /// <summary>
        /// Get review
        /// </summary>
        /// <returns></returns>
        public async Task getReview()
        {
            String reviewHtml = await downloader.downloadString();
            parseReviewHtml(reviewHtml);
        }

        /// <summary>
        /// Parse review from html page
        /// </summary>
        /// <param name="reviewHtml"></param>
        private void parseReviewHtml(String reviewHtml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(reviewHtml);
            HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='review']");
            if (nodeCollection == null)
            {
                hasMoreReview = false;
            }
            else
            {
                foreach (HtmlNode node in nodeCollection)
                {
                    Review r;
                    try
                    {
                        r = getReview(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    reviewCollection.Add(r);
                }
                nodeCollection = doc.DocumentNode.SelectNodes("//div[@id='paginator']");
                if (nodeCollection == null)
                {
                    hasMoreReview = false;
                }
                else
                {
                    HtmlNodeCollection nc = nodeCollection[0].SelectNodes("a[@class='next']");
                    if (nc == null)
                    {
                        hasMoreReview = false;
                    }
                    else
                    {
                        hasMoreReview = true;
                        string link = nc[0].Attributes["href"].Value;
                        link = link.Replace("&amp;", "&");
                        movie.nextReviewLink = Movie.movieLinkHeader + movie.id + "/reviews" + link;
                    }
                }
            }
        }

        public async Task loadMore()
        {
            downloader = new Downloader(movie.nextReviewLink);
            String reviewHtml = await downloader.downloadString();
            parseReviewHtml(reviewHtml);
        }

        /// <summary>
        /// Cancel download
        /// </summary>
        public void cancelDownload()
        {
            downloader.cancelDownload();
        }

        /// <summary>
        /// Is download canceled
        /// </summary>
        /// <returns>If download is canceled</returns>
        public bool isCanceled() {
            return downloader.isCanceled();
        }

        /// <summary>
        /// Get review from htmlnode
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private Review getReview(HtmlNode node)
        {
            string author = "";
            string star = "";
            string date = "";
            string shortReview = "";
            string title = "";
            string id = "";

            try
            {
                HtmlNode titleNode = node.SelectNodes("div[@class='review-hd']")[0].SelectNodes("h3")[0].SelectNodes("a")[1];
                title = Util.replaceSpecialChar(titleNode.InnerText.Trim());
                string link = titleNode.Attributes["href"].Value;
                id = link.Substring(Review.reviewLinkHeader.Length, link.Length - 1 - Review.reviewLinkHeader.Length);
                HtmlNode infoNode = node.SelectNodes("div[@class='review-hd']")[0].SelectNodes("div[@class='review-hd-info']")[0];
                HtmlNode aNode = infoNode.SelectNodes("a")[0];
                HtmlNode spanNode = infoNode.SelectNodes("span")[0];
                author = Util.replaceSpecialChar(aNode.InnerText.Trim());
                star = App.imagePath + spanNode.Attributes["class"].Value.Substring(7, 2) + " star.png";
                aNode.Remove();
                spanNode.Remove();
                date = infoNode.InnerText.Trim();
                shortReview = Util.formatShortReview(node.SelectNodes("div[@class='review-bd']")[0].SelectNodes("div[@class='review-short']")[0].SelectNodes("span[@class='']")[0].InnerText.Trim());
            }
            catch (Exception)
            {
                throw;
            }
            Review r = new Review();
            r.date = date;
            r.id = id;
            r.movieName = movie.title;
            r.movieId = movie.id;
            r.reviewer = author;
            r.reviewShort = shortReview;
            r.star = star;
            r.title = title;
            return r;
        }
    }
}
