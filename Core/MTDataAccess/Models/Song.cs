using System;

namespace MTDataAccess.Models
{
    public sealed class Song
    {
        public int SongId { get; set; }
        public DateTime DateCreation { get; set; }
        public Album Album { get; internal set; }
        public int AlbumId { get; set; }
        public Artist Artist { get; internal set; }
        public int ArtistId { get; set; }
        public string Title { get; set; }
        public decimal Bpm { get; set; }
        public string TimeSignature { get; set; }

        public TimeSignature TimeSignatureObject => Models.TimeSignature.ParseEncodedNumber(uint.Parse(TimeSignature));
        public bool MultiTracks { get; set; }
        public bool CustomMix { get; set; }
        public bool Chart { get; set; }
        public bool RehearsalMix { get; set; }
        public bool Patches { get; set; }
        public bool SongSpecificPatches { get; set; }
        public bool ProPresenter { get; set; }
    }
}