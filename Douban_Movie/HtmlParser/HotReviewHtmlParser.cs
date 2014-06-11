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
    class HotReviewHtmlParser
    {
        public static bool hasMore = true;
        public static string nextLink = Review.hotReviewHeader;
        public static ObservableCollection<Review> reviewCollection = new ObservableCollection<Review>();
        private static Downloader downloader;

        /// <summary>
        /// Get hot review
        /// </summary>
        /// <returns></returns>
        public static async Task getHotReview()
        {
            downloader = new Downloader(nextLink);
            String hotReviewHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(hotReviewHtml);
            HtmlNodeCollection nodeCollction = doc.DocumentNode.SelectNodes("//ul[@class='tlst clearfix']");
            if (nodeCollction != null)
            {

                foreach (HtmlNode node in nodeCollction)
                {
                    Review review;
                    try
                    {
                        review = getReview(node);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    reviewCollection.Add(review);
                }
                HtmlNodeCollection nextLinkNode = doc.DocumentNode.SelectNodes("//span[@class='next']")[0].SelectNodes("a");
                if (nextLinkNode == null)
                {
                    hasMore = false;
                }
                else
                {
                    nextLink = nextLinkNode[0].Attributes["href"].Value;
                    hasMore = true;
                }
            }
            else
            {
                hasMore = false;
            }
        }

        /// <summary>
        /// Get review from html node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        private static Review getReview(HtmlNode node)
        {
            string title = "";
            string id = "";
            string review = "";
            string reviewer = "";
            string movieName = "";
            string movieId = "";
            string startPath = "";

            try
            {
                HtmlNode aNode = node.SelectNodes("li[@class='nlst']")[0].SelectNodes("h3")[0].SelectNodes("a")[0];
                title = aNode.InnerText.Trim();
                string link = aNode.Attributes["href"].Value.Trim();
                id = link.Substring(Review.reviewLinkHeader.Length, link.Length - 1 - Review.reviewLinkHeader.Length);
                HtmlNode liNode = node.SelectNodes("li[@class='clst report-link']")[0];
                HtmlNode spanNode = liNode.SelectNodes("span[@class='pl ll obss']")[0];
                reviewer = spanNode.SelectNodes("span[@class='starb']")[0].SelectNodes("a")[0].InnerText.Trim();
                startPath = spanNode.SelectNodes("span")[1].Attributes["class"].Value.Substring(7) + " star.png";
                aNode = node.SelectNodes("li[@class='ilst']")[0].SelectNodes("a")[0];
                movieName = aNode.Attributes["title"].Value.Trim();
                link = aNode.Attributes["href"].Value.Trim();
                movieId = link.Substring(Movie.movieLinkHeader.Length, link.Length - 1 - Movie.movieLinkHeader.Length);
                HtmlNode reviewNode = liNode.SelectNodes("div[@class='review-short']")[0];
                HtmlNodeCollection aNodes = reviewNode.SelectNodes("a");
                if (aNodes != null)
                {
                    aNodes[0].Remove();
                }
                review = reviewNode.InnerText.Trim();

            }
            catch (Exception)
            {
                throw;
            }
            Review r = new Review();
            r.title = Util.formatShortReview(title);
            r.id = id;
            r.reviewShort = Util.formatShortReview(review);
            r.reviewer = Util.replaceSpecialChar(reviewer);
            r.movieId = movieId;
            r.movieName = Util.replaceSpecialChar(movieName);
            r.star = App.imagePath + startPath;
            return r;
        }
    }
}
