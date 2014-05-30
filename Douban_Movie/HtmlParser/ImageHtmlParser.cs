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
        public ProgressBar progressBar { get; set; }
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

        /*
        public void parseImage()
        {
            if (movie.imageLoaded == false)
            {
                movie.nextImageLink = Movie.movieLinkHeader + movie.id + "/photos?type=S";
                client = new WebClient();
                client.DownloadStringCompleted += downloadImageCompleted;
                client.DownloadStringAsync(new Uri(movie.nextImageLink));
            }
            else
            {
                foreach (MovieImage i in movie.imageSet) {
                    imageCollection.Add(i);
                }
                if (progressBar != null)
                {
                    progressBar.Visibility = Visibility.Collapsed;
                }
                if (movie.hasMoreImage == false)
                {
                    button.IsEnabled = false;
                    text.Text = AppResources.Finish;
                }
                else
                {
                    button.IsEnabled = true;
                }
            }
        }
        */

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

        /*
        public void downloadImageCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null && !e.Cancelled)
                {
                    string page = e.Result;
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(page);
                    HtmlNodeCollection nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='cover']");
                    if (nodeCollection == null)
                    {
                        if (progressBar != null)
                        {
                            progressBar.Visibility = Visibility.Collapsed;
                        }
                        movie.hasMoreImage = false;
                        button.IsEnabled = false;
                        text.Text = AppResources.Finish;
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
                            movie.imageSet.Add(image);
                        }
                        if (progressBar != null)
                        {
                            progressBar.Visibility = Visibility.Collapsed;
                        }
                        nodeCollection = doc.DocumentNode.SelectNodes("//div[@class='paginator']");
                        if (nodeCollection == null)
                        {
                            movie.hasMoreImage = false;
                            button.IsEnabled = false;
                            text.Text = AppResources.Finish;
                        }
                        else
                        {
                            HtmlNodeCollection nc = nodeCollection[0].SelectNodes("span[@class='next']");
                            if (nc == null)
                            {
                                movie.hasMoreImage = false;
                                button.IsEnabled = false;
                                text.Text = AppResources.Finish;
                            }
                            else
                            {
                                HtmlNodeCollection aCollection = nc[0].SelectNodes("a");
                                if (aCollection == null)
                                {
                                    movie.hasMoreImage = false;
                                    button.IsEnabled = false;
                                    text.Text = AppResources.Finish;
                                }
                                else
                                {
                                    movie.hasMoreImage = true;
                                    string link = aCollection[0].Attributes["href"].Value;
                                    link = link.Replace("&amp;", "&");
                                    movie.nextImageLink = link;
                                    button.IsEnabled = true;
                                }
                            }
                        }
                    }
                    movie.imageLoaded = true;
                }
                else
                {
                    var wEx = e.Error as WebException;
                    if (wEx.Status == WebExceptionStatus.RequestCanceled)
                    {
                        if (App.isFromDormant)
                        {
                            App.isFromDormant = false;
                            if (isFromLoadMore)
                            {
                                loadMore();
                            }
                            else
                            {
                                parseImage();
                            }
                        }
                    }
                    else
                    {
                        if (progressBar != null)
                        {
                            progressBar.Visibility = Visibility.Collapsed;
                        }
                    }
                }
            }
            catch (WebException)
            {
                button.IsEnabled = true;
                if (progressBar != null)
                {
                    progressBar.Visibility = Visibility.Collapsed;
                }
                MessageBoxResult result = MessageBox.Show(AppResources.ConnectionError, "", MessageBoxButton.OK);
            }
            catch (Exception)
            {
                button.IsEnabled = true;
                if (progressBar != null)
                {
                    progressBar.Visibility = Visibility.Collapsed;
                }
            }
        }
        */

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
