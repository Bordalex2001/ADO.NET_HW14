using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamesClassLibrary.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual Studio Studio { get; set; }
        public int StudioId { get; set; }
        public virtual Genre Genre { get; set; }
        public int GenreId { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string GameMode { get; set; }
        public int? CopiesAreSold { get; set; }
    }
}
