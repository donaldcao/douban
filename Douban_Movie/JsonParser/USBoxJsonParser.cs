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

namespace PanoramaApp2.JsonParser
{
    class USBoxJsonParser
    {
        private static ObservableCollection<Movie> movieCollection = new ObservableCollection<Movie>();
        private static Downloader downloader = new Downloader(Movie.apiUSBoxHeader + "?apikey=" + App.apikey);

        public async static Task<ObservableCollection<Movie>> getUSMovie()
        {
            String USHtml = await downloader.downloadString();

            JObject obj = JObject.Parse(USHtml);
            JArray array = (JArray)obj["subjects"];
            for (int i = 0; i < array.Count; i++)
            {
                Movie movie = new Movie();
                movie.id = JsonParsers.getDouble(array[i], "subject", "id");
                movie.posterUrl = JsonParsers.getTriple(array[i], "subject", "images", "small");
                movie.money = JsonParsers.getValue(array[i], "box");
                movie.rating = JsonParsers.getTriple(array[i], "subject", "rating", "average");
                movie.title = JsonParsers.getDouble(array[i], "subject", "title");
                movie.star = Util.getStarPath(movie.rating);
                movieCollection.Add(movie);
            }

            return movieCollection;
        }
    }
}
