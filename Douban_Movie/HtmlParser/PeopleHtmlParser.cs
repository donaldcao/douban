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
    class PeopleHtmlParser
    {
        private People people;
        private Downloader downloader;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="p"></param>
        public PeopleHtmlParser(People p)
        {
            people = p;
            downloader = new Downloader(People.peopleLinkHeader + people.id);
        }

        /// <summary>
        /// Get people
        /// </summary>
        /// <returns>People from html</returns>
        public async Task<People> getPeople()
        {
            String peopleHtml = await downloader.downloadString();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(peopleHtml);
            try
            {
                getPeople(doc);
            }
            catch (Exception)
            {

            }
            return people;
        }

        /// <summary>
        /// Ge people from html node
        /// </summary>
        /// <param name="doc"></param>
        private void getPeople(HtmlDocument doc)
        {
            string gender = "";
            string birthday = "";
            string constl = "";
            string birthplace = "";
            string occupation = "";
            string summary = "";

            try
            {
                //name = doc.DocumentNode.SelectNodes("//div[@id='content']")[0].SelectNodes("h1")[0].InnerText.Trim();
                HtmlNodeCollection ulNode = doc.DocumentNode.SelectNodes("//div[@id='headline']")[0].SelectNodes("div[@class='info']")[0].SelectNodes("ul");
                if (ulNode != null)
                {
                    HtmlNodeCollection liNodes = ulNode[0].SelectNodes("li");
                    if (liNodes != null)
                    {
                        foreach (HtmlNode liNode in liNodes)
                        {
                            HtmlNode spanNode = liNode.SelectNodes("span")[0];
                            string attr = spanNode.InnerText.Trim();
                            spanNode.Remove();
                            if (attr == "性别")
                            {
                                gender = Util.formatShortReview(liNode.InnerText.Trim().Substring(1, liNode.InnerText.Trim().Length - 1));
                            }
                            if (attr == "星座")
                            {
                                constl = Util.formatShortReview(liNode.InnerText.Trim().Substring(1, liNode.InnerText.Trim().Length - 1));
                            }
                            if (attr == "出生日期")
                            {
                                birthday = Util.formatShortReview(liNode.InnerText.Trim().Substring(1, liNode.InnerText.Trim().Length - 1));
                            }
                            if (attr == "出生地")
                            {
                                birthplace = Util.formatShortReview(liNode.InnerText.Trim().Substring(1, liNode.InnerText.Trim().Length - 1));
                            }
                            if (attr == "职业")
                            {
                                occupation = Util.formatShortReview(liNode.InnerText.Trim().Substring(1, liNode.InnerText.Trim().Length - 1));
                            }
                        }
                    }
                }
                HtmlNodeCollection introNodes = doc.DocumentNode.SelectNodes("//div[@id='intro']")[0].SelectNodes("div[@class='bd']")[0].SelectNodes("span[@class='all hidden']");
                if (introNodes != null)
                {
                    summary = Util.formatReview(introNodes[0].InnerHtml);
                }
                else
                {
                    summary = Util.formatReview(doc.DocumentNode.SelectNodes("//div[@id='intro']")[0].SelectNodes("div[@class='bd']")[0].InnerText);
                }

            }
            catch (Exception)
            {
                throw;
            }
            people.gender = gender;
            people.occupation = occupation;
            people.birthplace = birthplace;
            people.birthday = birthday;
            people.constl = constl;
            people.summary = summary;
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
    }
}
