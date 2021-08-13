using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using DataAccess;
using MTDataAccess.Models;

namespace MTDataAccess
{
    /// <summary>
    /// Implements <see cref="IDataAccess"/> on top of the SQL class/>
    /// </summary>
    public class SQLDecorator : IDataAccess
    {
        private readonly SQL sql;

        public SQLDecorator(SQL sql)
        {
            this.sql = sql;
        }

        private Artist populateArtist(Func<string, object> rowSelector, bool lazy, DataTable albumTable = null, DataTable songTable = null, DataTable artistTable = null)
        {
            Artist artist = new Artist()
            {
                ArtistId = (int)rowSelector("artistID"),
                Biography = rowSelector("biography").ToString(),
                DateCreation = (DateTime)rowSelector("dateCreation"),
                HeroUrl = new Uri(rowSelector("heroURL").ToString()),
                ImageUrl = new Uri(rowSelector("ImageURL").ToString()),
                Title = rowSelector("title").ToString()
            };
            if (!lazy)
            {
                List<Album> albums = new List<Album>(albumTable.RowCount());
                foreach (DataRow albumRow in albumTable.Rows)
                {
                    albums.Add(populateAlbum(value => albumRow[value]));
                }

                List<Song> songs = new List<Song>(songTable.RowCount());
                foreach (DataRow songRow in songTable.Rows)
                {
                    songs.Add(populateSong(key => songRow[key], false, albumTable, artistTable));
                }
                artist.Albums = albums.ToArray();
                artist.Songs = songs.ToArray();
            }

            return artist;
        }

        public IEnumerable<Artist> GetArtists()
        {
            using (DataTable table = sql.ExecuteStoredProcedureDT("GetAllArtists", true))
            {
                foreach (DataRow row in table.Rows)
                {
                    yield return populateArtist(value => row[value], false);
                }
            }
        }
        public Artist GetArtistById(int artistId, bool lazy = false)
        {
            //Setup stored procedure parameters.
            SqlParameter artistIdParameter = new SqlParameter("@artistId", SqlDbType.Int) {Value = artistId};
            sql.Parameters.Add(artistIdParameter);
            if (!lazy)
            {
                sql.Parameters.Add(new SqlParameter("@includeAlbum", SqlDbType.Bit) {Value = true});
                sql.Parameters.Add(new SqlParameter("@includeSong", SqlDbType.Bit) {Value = true});
            }
            using (DataSet set = sql.ExecuteStoredProcedureDS("GetArtistDetails", true))
            {
                if (set.Tables[0].Rows.Count > 0)
                {
                    if (!lazy)
                    {
                        return populateArtist(value => set.Tables[0].Rows[0][value], false, set.Tables[1], set.Tables[2], set.Tables[0]);
                    }
                    return populateArtist(value => set.Tables[0].Rows[0][value], true);
                }
            }
            return null;
        }

        public IEnumerable<Artist> GetArtistsByName(string artistName, bool exact = true)
        {
            //Setup stored procedure parameters.
            SqlParameter artistIdParameter = new SqlParameter("@artistName", SqlDbType.VarChar) {Value = artistName};
            sql.Parameters.Add(artistIdParameter);
            sql.Parameters.Add(new SqlParameter("@exact", SqlDbType.Bit) {Value = exact});
            using (DataTable table = sql.ExecuteStoredProcedureDT("GetArtistDetailsByName", true))
            {
                foreach (DataRow row in table.Rows)
                {
                    yield return populateArtist(value => row[value], true);
                }
            }
        }

        public Artist AddArtist(Artist artist)
        {
            if (artist == null)
                throw new ArgumentException("You must supply an artist model.", nameof(artist));

            //Setup parameters
            sql.Parameters.Add(new SqlParameter("@dateCreation", SqlDbType.SmallDateTime) {Value = artist.DateCreation ?? DateTime.Now});
            sql.Parameters.Add(new SqlParameter("@title", SqlDbType.VarChar) {Value = artist.Title});
            sql.Parameters.Add(new SqlParameter("@biography", SqlDbType.VarChar) {Value = artist.Biography});
            sql.Parameters.Add(new SqlParameter("@imageURL", SqlDbType.VarChar) {Value = artist.ImageUrl.ToString()});
            sql.Parameters.Add(new SqlParameter("@heroURL", SqlDbType.VarChar) {Value = artist.HeroUrl.ToString()});

            sql.OpenConnection();
            sql.BeginTransaction();
            var row = sql.ExecuteStoredProcedureDT("AddArtist", true);
            //The stored-procedure will return the added row.
            Artist result = null;

            if (row.Rows.Count > 0)
            {
                result = populateArtist(value => row.Rows[0][value], true);
            }
            row.Dispose();
            sql.Commit();
            sql.CloseConnection();
            return result ?? throw new Exception("Artist could not be returned from server.");
        }

        private Album populateAlbum(Func<string, object> rowSelector)
        {
            return new Album()
            {
                AlbumId = (int)rowSelector("albumId"),
                ArtistId = (int)rowSelector("artistId"),
                DateCreation = (DateTime)rowSelector("dateCreation"),
                ImageUrl = new Uri(rowSelector("imageURL").ToString()),
                Title = rowSelector("title").ToString(),
                Year = (int)rowSelector("year")
            };
        }

        public Album GetAlbumById(int albumId)
        {
            sql.Parameters.Add(new SqlParameter("@albumId", SqlDbType.Int) {Value = albumId});
            using (DataTable table = sql.ExecuteStoredProcedureDT("GetAlbumById", true))
            {
                if (table.Rows.Count == 0)
                    return null;

                return populateAlbum(value => table.Rows[0][value]);
            }
        }

        public IEnumerable<Album> GetAlbumsByArtistId(int artistId)
        {
            sql.Parameters.Add(new SqlParameter("@artistId", SqlDbType.Int) {Value = artistId});
            using (DataTable result = sql.ExecuteStoredProcedureDT("GetAlbumsByArtistId", true))
            {
                foreach (DataRow row in result.Rows)
                {
                    yield return populateAlbum(value => row[value]);
                }
            }
        }

        public Song populateSong(Func<string, object> rowSelector, bool lazy = false, DataTable albumTable = null, DataTable artistTable = null, DataTable songTable = null)
        {
            Song song = new Song()
            {
                SongId = (int) rowSelector("songId"),
                Bpm = (decimal) rowSelector("bpm"),
                Title = rowSelector("title").ToString(),
                DateCreation = (DateTime) rowSelector("dateCreation"),
                TimeSignature = rowSelector("timeSignature").ToString(),
                Chart = (bool) rowSelector("chart"),
                MultiTracks = (bool) rowSelector("multitracks"),
                CustomMix = (bool) rowSelector("customMix"),
                RehearsalMix = (bool) rowSelector("rehearsalMix"),
                Patches = (bool) rowSelector("patches"),
                SongSpecificPatches = (bool) rowSelector("songSpecificPatches"),
                ProPresenter = (bool) rowSelector("proPresenter"),
                AlbumId = (int)rowSelector("albumID"),
                ArtistId = (int)rowSelector("artistID")
            };
            if (!lazy)
            {
                foreach (DataRow albumRow in albumTable.Rows)
                {
                    if ((int) albumRow["albumID"] == song.AlbumId)
                    {
                        song.Album = populateAlbum(value => albumRow[value]);
                        break;
                    }
                }
                song.Artist = populateArtist(value => artistTable.Rows[0][value], true, albumTable, songTable);
            }

            return song;
        }

        public IEnumerable<Song> GetAllSongs(uint pageNumber = 1, uint pageSize = uint.MaxValue)
        {
            sql.Parameters.Add(new SqlParameter("@pageSize", SqlDbType.Int) {Value = pageSize});
            sql.Parameters.Add(new SqlParameter("@pageNumber", SqlDbType.Int) {Value = pageNumber});
            //Load rows dynamically
            using (SqlDataReader reader = sql.ExecuteStoredProcedureDataReader("GetAllSongs", true))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return populateSong(key => reader[key], true);
                    }
                }
            };
        }

        public Song GetSongById(int songId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Song> GetSongsByArtistId(int artistId, bool lazy = false)
        {
            sql.Parameters.Add(new SqlParameter("@artistId", SqlDbType.Int) {Value = artistId});
            if (!lazy)
            {
                sql.Parameters.Add(new SqlParameter("@includeArtist", SqlDbType.Bit) {Value = true});
                sql.Parameters.Add(new SqlParameter("@includeAlbum", SqlDbType.Bit) {Value = true});
            }
            using (DataSet set = sql.ExecuteStoredProcedureDS("GetSongsByArtistId", true))
            {
                foreach (DataRow row in set.Tables[0].Rows)
                {
                    if (!lazy)
                    {
                        yield return populateSong(key => row[key], false, set.Tables[2], set.Tables[1], set.Tables[0]);
                    }
                    yield return populateSong(key => row[key], lazy);
                }
            }
        }
    }
}