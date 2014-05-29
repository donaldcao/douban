using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Controls;
using System.Windows;
using PanoramaApp2.JsonParser;
using PanoramaApp2.Resources;
using Phone.Controls;
using PanoramaApp2.Utility;

namespace PanoramaApp2
{
    class MovieJsonParser
    {
        private Movie movie;
        private Downloader downloader;

        public MovieJsonParser(Movie m)
        {
            movie = m;
            downloader = new Downloader(Movie.apiMovieHeader + movie.id + "?apikey=" + App.apikey);
        }

        public async Task<Tuple<Movie, List<People>>> getMovieByID()
        {
            // Don't use cache now
            /*
            Movie result = Cache.getMovie(movie.id);
            if (result != null)
            {
                movie = result;
                setUI();
                peopleList.ItemsSource = movie.peopleList;
                if (progressBar != null)
                {
                    progressBar.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
             */
            String movieJson = await downloader.downloadString();

            return parseMovieJson(movieJson);
        }

        /// <summary>
        /// Parse movie JSON
        /// </summary>
        /// <param name="movieJson">Movie JSON String</param>
        private Tuple<Movie, List<People>> parseMovieJson(String movieJson)
        {
            JObject obj = JObject.Parse(movieJson);
            movie.summary = JsonParsers.getValue(obj, "summary");
            if (movie.genre == "" || movie.genre == null)
            {
                movie.genre = JsonParsers.getArray(obj, "genres");
            }
            if (movie.title == "" || movie.title == null)
            {
                movie.title = JsonParsers.getValue(obj, "title");
            }
            if (movie.year == "" || movie.year == null)
            {
                movie.year = JsonParsers.getValue(obj, "year");
            }
            if (movie.rating == "" || movie.rating == null)
            {
                movie.rating = JsonParsers.getDouble(obj, "rating", "average");
            }
            movie.star = Util.getStarPath(movie.rating);
            if (movie.rateNumber == "" || movie.rateNumber == null)
            {
                movie.rateNumber = JsonParsers.getValue(obj, "ratings_count");
            }
            if (movie.posterUrl == "" || movie.posterUrl == null)
            {
            movie.posterUrl = JsonParsers.getDouble(obj, "images", "large");
            }
            object[] countries = obj["countries"].ToArray();
            if (movie.region == "" || movie.region == null)
            {
                movie.region = JsonParsers.getArray(obj, "countries");
            }

            if (movie.posterUrl == "")
            {
                movie.posterUrl = App.imagePath + "default.png";
            }

            List<People> peoples = new List<People>();
            JArray array = (JArray)obj["directors"];
            for (int i = 0; i < array.Count; i++)
            {
                People people = new People();
                people.posterUrl = JsonParsers.getDouble(array[i], "avatars", "small");
                if (people.posterUrl == "")
                {
                    people.posterUrl = App.imagePath + "default.png";
                }
                people.id = JsonParsers.getValue(array[i], "id");
                people.name = JsonParsers.getValue(array[i], "name");
                people.positionName = "导演";
                people.position = People.DIRECTOR;
                peoples.Add(people);
            }
            array = (JArray)obj["casts"];
            for (int i = 0; i < array.Count; i++)
            {
                People people = new People();
                people.posterUrl = JsonParsers.getDouble(array[i], "avatars", "small");
                if (people.posterUrl == "")
                {
                    people.posterUrl = App.imagePath + "default.png";
                }
                people.id = JsonParsers.getValue(array[i], "id");
                people.name = JsonParsers.getValue(array[i], "name");
                people.positionName = "";
                people.position = People.ACTOR;
                peoples.Add(people);
            }

            return Tuple.Create(movie, peoples);
        }

        /// <summary>
        /// Cancel download
        /// </summary>
        public void cancelDownload()
        {
            downloader.cancelDownload();
        }

        /// <summary>
        /// If the download is canceled
        /// </summary>
        /// <returns>If download is canceled</returns>
        public bool isCanceled()
        {
            return downloader.isCanceled();
        }
        
    }
}
