using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MovieGrab.Models;
using Newtonsoft.Json;

namespace MovieGrab
{
    class Program
    {
        
        static async Task Main(string[] args)
        {

            Console.WriteLine("Hello World!");
            var grab = new DouBanMovieGrab();
            var res = await grab.SearchMovie("天注定");
        }
    }

    interface IMovieGrab
    {
        Task<Movie> SearchMovie(string name);
    }

    class DouBanMovieGrab:IMovieGrab
    {
        private static readonly string Search_API =
            "https://api.douban.com/v2/movie/search?apikey=0dad551ec0f84ed02907ff5c42e8ec70&q=";

        private static readonly string Detail_API =
            "https://api.douban.com/v2/movie/{0}?apikey=0dad551ec0f84ed02907ff5c42e8ec70";

        private readonly HttpClient _httpClient;

        public DouBanMovieGrab()
        {
            _httpClient = new HttpClient();
        }

        public async Task<Movie> SearchMovie(string name)
        {
            var encodeName = UrlEncoder.Default.Encode(name);
            var response = await _httpClient.GetStringAsync(Search_API + encodeName);
            var movieResult = JsonConvert.DeserializeObject<DouBanMovie>(response);

            if (movieResult.Subjects.Any())
            {
                var matchedMovie = movieResult.Subjects.FirstOrDefault(x => x.Title == name && x.Subtype == "movie");
                if (matchedMovie != null)
                {
                    var id = matchedMovie.Id;
                    var url = string.Format(Detail_API, id);
                    var detailResponse = await _httpClient.GetStringAsync(url);
//                    var movieDetail = 
                }
            }

            return null;
        }
    }

    

    class TMDBMovieGrab:IMovieGrab
    {
        private static readonly string Api =
            "https://api.themoviedb.org/3/search/movie?api_key=2d8021f0b7abeb777c802a6a37709575&query=";
        //

        private readonly HttpClient _httpClient;

        public TMDBMovieGrab()
        {
            _httpClient = new HttpClient();
        }

        public Task<Movie> SearchMovie(string name)
        {
            throw new NotImplementedException();
        }
    }

    
}


