using System;

namespace MTDataAccess.Models
{
    public sealed class Album
    {
        public int AlbumId { get; set; }
        public DateTime DateCreation { get; set; }
        public string Title { get; set; }
        public int ArtistId { get; set; }
        public Uri ImageUrl { get; set; }
        public int Year { get; set; }
    }
}