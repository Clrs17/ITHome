﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ITHome.Core.Models
{
    public class NewsList
    {
        public  ObservableCollection<News> NewsItem { get; set; }
        public static NewsList CreateFromJson(JToken token)
        {
            var newsList = new NewsList();
            newsList.NewsItem = new ObservableCollection<News>();
            foreach (var item in token)
                newsList.NewsItem.Add(News.CreateFromJson(item));
            return newsList;
        }
    }
    public class News
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public int CommentCount { get; set; }
        public DateTime PostDate { get; set; }
        public List<string> ImageList { get; set; }
        public static News CreateFromJson(JToken token)
        {
            var news = new News
            {
                Id = token.Value<int>("newsid"),
                Url = token.Value<string>("url"),
                Title = token.Value<string>("title"),
                Description = token.Value<string>("description"),
                Image = token.Value<string>("image"),
                CommentCount = token.Value<int>("commentcount"),
                PostDate = token.Value<DateTime>("postdate"),
            };
            if(news.Title == null)
            {
                news.Title = token.Value<string>("newstitle");
            }
            if (news.Image == null)
            {
                news.Image = token.Value<string>("img");
            }
            if (token["imagelist"]!= null)
            {
                news.ImageList = new List<string>();
                foreach (var image in token["imagelist"])
                    news.ImageList.Add(image.ToString());
            }
            return news;
        }
    }
    public class NewsSearch
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Detail { get; set; }
        public string Author { get; set; }
        public string Editer { get; set; }
        public DateTime? PostDate { get; set; }
        public List<NewsTag> Tags { get; set; }
        public static NewsSearch CreateFromJson(JToken token)
        {
            var newssearch = new NewsSearch
            {
                Id = token.Value<int>("newsid"),
                Title = token.Value<string>("title"),
                Author = token.Value<string>("newsauthor"),
                Editer = token.Value<string>("z"),
                PostDate = token.Value<DateTime>("postdate"),
            };
            newssearch.Detail = token.Value<string>("detail").Replace("&nbsp;", "");
            if (token["newstags"] != null)
            {
                newssearch.Tags = new List<NewsTag>();
                foreach (var tag in token["newstags"])
                    newssearch.Tags.Add(NewsTag.CreateFromJson(tag));
            }
            return newssearch;
        }
    }
    public class NewsTag
    { 
        public int Id { get; set; }
        public string Keyword { get; set; }
        public string Link { get; set; }
        public static NewsTag CreateFromJson(JToken token)
        {
            var newstag = new NewsTag
            {
                Id = token.Value<int>("id"),
                Keyword = token.Value<string>("keyword"),
                Link = token.Value<string>("link"),
            };
            return newstag;
        }
    }
    public class NewsGrade
    {
        public string Message { get; set; }
        public int MyGrade { get; set; }
        public  int Grade { get; set; }
        public string GradeStr { get; set; }
        public int GradeCount { get; set; }
        public int WorthLessCount { get; set; }
        public int AcceptableCount { get; set; }
        public int ValuableCount { get; set; }

        public static NewsGrade CreateFromJson(JToken token)
        {
            var grade = new NewsGrade
            {
                MyGrade = token.Value<int>("myGrade"),
                Grade = token.Value<int>("grade"),
                GradeStr = token.Value<string>("gradeStr"),
                WorthLessCount = token.Value<int>("worthLessCount"),
                AcceptableCount = token.Value<int>("acceptableCount"),
                ValuableCount = token.Value<int>("valuableCount"),
                GradeCount = token.Value<int>("gradeCount"),
            };
            return grade;
        }
    }
}
