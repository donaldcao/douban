using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Net;

namespace PanoramaApp2.Utility
{
    class Util
    {

        /// <summary>
        /// Get star image path from rating
        /// </summary>
        /// <param name="rate">Movie rating</param>
        /// <returns>Path to corresponding rating picture</returns>
        public static string getStarPath(string rate)
        {
            double rating = 0;
            if (rate == "")
            {
                return "00 star.png";
            }
            try
            {
                rating = double.Parse(rate);
            }
            catch (System.FormatException)
            {
                return "00 star.png";
            }
            double stars = rating / 2.0;
            int baseStar = (int)stars;
            int roundStar = (int)Math.Round(stars);
            bool half = roundStar > baseStar ? true : false;
            string starPath = App.imagePath + baseStar;
            if (half)
            {
                starPath += 5;
            }
            else
            {
                starPath += 0;
            }
            starPath += " star.png";
            return starPath;
        }

        /// <summary>
        /// Format short review
        /// </summary>
        /// <param name="review">Short review</param>
        /// <returns>Formatted short review</returns>
        public static string formatShortReview(string review)
        {
            string newReview = removeNewLine(review.Trim());
            newReview = replaceSpecialChar(newReview);
            return newReview;
        }

        /// <summary>
        /// Format review
        /// </summary>
        /// <param name="review">Review</param>
        /// <returns>Formatted review</returns>
        public static string formatReview(string review)
        {
            string newReview = removeNewLine(review);
            newReview = replaceSpecialChar(newReview);
            return replaceTags(newReview);
        }

        /// <summary>
        /// Replace html tags with corresponding characters
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string replaceTags(string text)
        {
            text = text.Replace("<br/>", Environment.NewLine);
            text = text.Replace("<wbr/>", string.Empty);
            text = text.Replace("<br>", Environment.NewLine);
            return text.Replace("<wbr>", string.Empty);
        }

        /// <summary>
        /// Remove newline characters
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Text without newline</returns>
        public static string removeNewLine(string text)
        {
            text = text.Replace("\r\n", string.Empty);
            text = text.Replace("\r", string.Empty);
            text = text.Replace("\n", string.Empty);
            return text;
        }

        /// <summary>
        /// Decode html
        /// </summary>
        /// <param name="text">Html text</param>
        /// <returns>Decoded html text</returns>
        public static string replaceSpecialChar(string text)
        {
            return HttpUtility.HtmlDecode(text);
        }
    }
}
