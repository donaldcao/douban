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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows;
using PanoramaApp2.JsonParser;
using PanoramaApp2.Resources;
using PanoramaApp2.Utility;
using System.Threading.Tasks;

namespace PanoramaApp2.JsonParser
{
    class SearchJsonParser
    {
        public ObservableCollection<Movie> movieCollection { get; set; }
        public int resultNumber { get; set; }
        private string searchText;
        private WebClient client;
        private Downloader downloader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="text"></param>
        public SearchJsonParser(String text)
        {
            searchText = text;
            downloader = new Downloader(Movie.apiSearchHeader + "?apikey=" + App.apikey + "&q=" + text);
            movieCollection = new ObservableCollection<Movie>();
            resultNumber = 0;
        }

        /// <summary>
        /// Search
        /// </summary>
        /// <returns></returns>
        public async Task search()
        {
            String searchJson = await downloader.downloadString();
            JObject obj = JObject.Parse(searchJson);
            JArray array = (JArray)obj["subjects"];
            for (int i = 0; i < array.Count; i++)
            {
                Movie movie = new Movie();
                movie.id = JsonParsers.getValue(array[i], "id");
                movie.posterUrl = JsonParsers.getDouble(array[i], "images", "small");
                movie.rating = JsonParsers.getDouble(array[i], "rating", "average");
                movie.title = JsonParsers.getValue(array[i], "title");
                movie.star = Util.getStarPath(movie.rating);
                movieCollection.Add(movie);
            }
            resultNumber = array.Count;
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
        /// <returns></returns>
        public bool isCanceled()
        {
            if (downloader != null)
            {
                return downloader.isCanceled();
            }
            return false;
        }
    }

}
