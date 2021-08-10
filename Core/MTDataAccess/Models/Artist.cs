using System;

namespace MTDataAccess.Models
{
    public sealed class Artist
    {
        public int ArtistId { get; set; }
        public DateTime? DateCreation { get; set; }
        public static int TITLE_MAX = 100;
        public string Title { get; set; }
        public string Biography { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri HeroUrl { get; set; }
    }
}