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
    class ReviewHtmlParser
    {
        public Review review { get; set; }
        public ObservableCollection<Comment> commentCollection { get; set; }
        private Downloader downloader;
        public bool hasMoreComments { get; set; }
        public ReviewHtmlParser(Review r)
        {
            review = r;
            commentCollection = new ObservableCollection<Comment>();
            hasMoreComments = false;
        }

        public async Task getReview()
        {
            downloader = new Downloader(Review.reviewLinkHeader + review.id);
            String reviewHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(reviewHtml);
            try
            {
                getReview(doc);
            }
            catch (Exception)
            {
            }
        }

        public async Task loadMore()
        {
            downloader = new Downloader(review.nextCommentLink);
            String commentHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(commentHtml);
            try
            {
                loadComments(doc);
            }
            catch (Exception)
            {

            }
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

        private Comment getComment(HtmlNode node)
        {
            string author = "";
            string time = "";
            string content = "";

            try
            {
                HtmlNode commentNode = node.SelectNodes("div[@class='content report-comment']")[0];
                HtmlNode authorNode = commentNode.SelectNodes("div[@class='author']")[0];
                HtmlNode aNode = authorNode.SelectNodes("a")[0];
                author = Util.replaceSpecialChar(aNode.InnerText.Trim());
                aNode.Remove();
                time = authorNode.InnerText.Trim().Substring(0, 19);
                content = Util.formatReview(commentNode.SelectNodes("p")[0].InnerText);
            }
            catch (Exception)
            {
                throw;
            }
            Comment c = new Comment();
            c.author = author;
            c.time = time;
            c.content = content;
            return c;
        }

        private void loadComments(HtmlDocument doc)
        {
            try
            {
                HtmlNodeCollection collection = doc.DocumentNode.SelectNodes("//div[@class='comment-item']");
                if (collection == null)
                {
                    hasMoreComments = false;
                }
                else
                {
                    foreach (HtmlNode node in collection)
                    {
                        Comment c;
                        try
                        {
                            c = getComment(node);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        commentCollection.Add(c);
                    }
                    HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='paginator']");
                    if (nodeCollection == null)
                    {
                        hasMoreComments = false;
                    }
                    else
                    {
                        HtmlNodeCollection nc = nodeCollection[0].SelectNodes("span[@class='next']");
                        if (nc == null)
                        {
                            hasMoreComments = false;
                        }
                        else
                        {
                            HtmlNodeCollection aCollection = nc[0].SelectNodes("a");
                            if (aCollection == null)
                            {
                                hasMoreComments = false;
                            }
                            else
                            {
                                hasMoreComments = true;
                                string link = aCollection[0].Attributes["href"].Value;
                                review.nextCommentLink = link;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void getReview(HtmlDocument doc)
        {
            try
            {
                string date = doc.DocumentNode.SelectNodes("//span[@property='v:dtreviewed']")[0].InnerText.Trim();
                if (review.date == null || review.date == "")
                {
                    review.date = date;
                }

                HtmlNode reviewNode = doc.DocumentNode.SelectNodes("//div[@property='v:description']")[0];
                HtmlNodeCollection aNodeCollection = reviewNode.SelectNodes("a");
                if (aNodeCollection != null)
                {
                    foreach (HtmlNode node in aNodeCollection)
                    {
                        HtmlNode newNode = HtmlNode.CreateNode(node.InnerText);
                        reviewNode.ReplaceChild(newNode, node);
                    }
                }
                string r = reviewNode.InnerHtml;
                review.review = Util.formatReview(r);
                loadComments(doc);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
