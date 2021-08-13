using System.Collections.Generic;
using MTDataAccess.Models;

namespace MTDataAccess
{
    /// <summary>
    /// Abstracts away data-access and platform dependent logic
    /// </summary>
    //TODO: Segregate entities into separate repositories
    public interface IDataAccess
    {
        //Can be yielded
        /// <summary>
        /// Gets all artist information from the underlying DB.
        /// </summary>
        /// <param name="lazy">Determines whether to retrieve data from other tables.</param>
        /// <returns>Returns an <see cref="IEnumerable{T}"/> that can be iterated over</returns>
        IEnumerable<Artist> GetArtists();
        /// <summary>
        /// Gets an artist by Id
        /// </summary>
        /// <param name="artistId">The artist Id to search</param>
        /// <param name="lazy">Determines whether to not load additional data from other tables. Default is false</param>
        /// <returns>The returned artist if one is found; returns null if no artist exists with the specified Id</returns>
        Artist GetArtistById(int artistId, bool lazy = false);
        /// <summary>
        /// Gets a list of artists that match a specific name (title). Title is not a primary/candiate key and thus multiple results may be returned.
        /// </summary>
        /// <param name="artistName">The artist name to search</param>
        /// <param name="exact">Determines whether the query will use a LIKE filter instead of equals. Defaults to true</param>
        /// <param name="lazy">Determines whether to retrieve data from other tables</param>
        /// <returns>A list of possible matches</returns>
        IEnumerable<Artist> GetArtistsByName(string artistName, bool exact = true);
        /// <summary>
        /// Adds an artist to the Artist table. ArtistId will be ignored
        /// </summary>
        /// <param name="artist">The artist object to save</param>
        /// <returns>An object that contains a new artist Id</returns>
        Artist AddArtist(Artist artist);
        Album GetAlbumById(int albumId);
        IEnumerable<Album> GetAlbumsByArtistId(int artistId);
        IEnumerable<Song> GetSongsByArtistId(int artistId, bool lazy = false);
        IEnumerable<Song> GetAllSongs(uint pageNumber = 1, uint pageSize = uint.MaxValue);
        Song GetSongById(int songId);
    }
}