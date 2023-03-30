using DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO
{
    public class FilmDTO
    {
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string Poster { get; set; }
        public string Title { get; set; }
        public DateTime DateOfPublishing { get; set; }
        public string RunTime { get; set; }
        public string Trailer { get; set; }
        public string Plot { get; set; }
        public virtual List<Genre> Genres { get; set; }
        public virtual List<Producer> Producers { get; set; }
        public virtual List<Actor> Actors { get; set; }
        public virtual List<User> Users { get; set; }
    }
}
