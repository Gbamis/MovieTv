using UnityEngine;
using System;
using System.Collections.Generic;

namespace Savana.Movie
{
    public class DataModel { }

    [Serializable]
    public struct Response_NowPlaying_List
    {
        public List<Model_Result> results;
    }

    [Serializable]
    public class Model_Result
    {
        public string id;
    }



    [Serializable]
    public class Response_MovieDetail
    {
        public int runtime;
        public List<Model_Genre> genres;
        public bool adult;
        public string backdrop_path;
        public string original_language;
        public string original_title;
        public string overview;
        public int popularity;
        public string poster_path;
        public string release_date;
        public string title;
        public bool video;
        public int vote_average;
        public int vote_count;

        public byte[] imageData;

        public void SetImageData(byte[] data)
        {
            imageData = new byte[data.Length];
            data.CopyTo(imageData, 0);
        }

        public bool CanParse()
        {
            return poster_path != null && title != null && overview != null;
        }
    }

    [Serializable]
    public struct Model_Genre
    {
        public int id;
        public string name;
    }

    [Serializable]
    public struct Response_SearchResult
    {
        public int page;
        public List<Model_SearchResult> results;
        public int total_pages;
        public int total_results;
    }

    [Serializable]
    public struct Model_SearchResult
    {
        public string id;
        public string name;
    }
}