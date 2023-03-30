using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilmLibrary.Models
{
    public class RequestOptions
    {
        static string mostPopular = "https://imdb-api.com/en/API/MostPopularMovies/";
        public static string MostPopular
        {
            get => mostPopular;
            set => mostPopular = "https://imdb-api.com/en/API/MostPopularMovies/" + value;
        }
        static string title = "";
        public static string Title
        {
            get => title + "/Images,Trailer,";
            set => title = "https://imdb-api.com/en/API/Title/" + value;
        }
    }
}
