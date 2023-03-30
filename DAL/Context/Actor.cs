using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Context
{
    public class Actor
    {
        [Key]
        public int Id { get; set; }
        public string ImdbId { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string SmallInfo { get; set; }
        public virtual List<Film> Films { get; set; }
    }
}
