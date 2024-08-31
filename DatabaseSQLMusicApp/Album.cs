using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSQLMusicApp
{
    internal class Album
    {
        public int ID { get; set; }
        public required string AlbumName { get; set; }
        public required string ArtistName { get; set; }
        public int Year { get; set; }
        public required string ImageURL { get; set; }
        public string Description { get; set; }
        public List<Track> Tracks { get; set; } // Add Tracks property for holding related tracks
    }
}
