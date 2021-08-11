using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private Artist populateArtistFromDataRow(DataRow row, bool lazy, DataTable albumTable = null, DataTable songTable = null, DataTable artistTable = null)
        {
            Artist artist = new Artist()
            {
                ArtistId = (int)row["artistID"],
                Biography = row["biography"].ToString(),
                DateCreation = (DateTime)row["dateCreation"],
                HeroUrl = new Uri(row["heroURL"].ToString()),
                ImageUrl = new Uri(row["ImageURL"].ToString()),
                Title = row["title"].ToString()
            };
            if (!lazy)
            {
                List<Album> albums = new List<Album>(albumTable.RowCount());
                foreach (DataRow albumRow in albumTable.Rows)
                {
                    albums.Add(populateAlbumFromDataRow(albumRow));
                }

                List<Song> songs = new List<Song>(songTable.RowCount());
                foreach (DataRow songRow in songTable.Rows)
                {
                    songs.Add(populateSongFromDataRow(songRow, false, albumTable, artistTable));
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
                    yield return populateArtistFromDataRow(row, false);
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
                        return populateArtistFromDataRow(set.Tables[0].Rows[0], false, set.Tables[1], set.Tables[2], set.Tables[0]);
                    }
                    return populateArtistFromDataRow(set.Tables[0].Rows[0], true);
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
                    yield return populateArtistFromDataRow(row, true);
                }
            }
        }

        public Artist AddArtist(Artist artist, bool lazy = false)
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
                result = populateArtistFromDataRow(row.Rows[0], lazy);
            }
            row.Dispose();
            sql.Commit();
            sql.CloseConnection();
            return result ?? throw new Exception("Artist could not be returned from server.");
        }

        private Album populateAlbumFromDataRow(DataRow row)
        {
            return new Album()
            {
                AlbumId = (int) row["albumId"],
                ArtistId = (int) row["artistId"],
                DateCreation = (DateTime) row["dateCreation"],
                ImageUrl = new Uri(row["imageURL"].ToString()),
                Title = row["title"].ToString(),
                Year = (int) row["year"]
            };
        }

        public Album GetAlbumById(int albumId)
        {
            sql.Parameters.Add(new SqlParameter("@albumId", SqlDbType.Int) {Value = albumId});
            using (DataTable table = sql.ExecuteStoredProcedureDT("GetAlbumById", true))
            {
                if (table.Rows.Count == 0)
                    return null;

                return populateAlbumFromDataRow(table.Rows[0]);
            }
        }

        public IEnumerable<Album> GetAlbumsByArtistId(int artistId)
        {
            sql.Parameters.Add(new SqlParameter("@artistId", SqlDbType.Int) {Value = artistId});
            using (DataTable result = sql.ExecuteStoredProcedureDT("GetAlbumsByArtistId", true))
            {
                foreach (DataRow row in result.Rows)
                {
                    yield return populateAlbumFromDataRow(row);
                }
            }
        }

        public Song populateSongFromDataRow(DataRow row, bool lazy = false, DataTable albumTable = null, DataTable artistTable = null, DataTable songTable = null)
        {
            Song song = new Song()
            {
                SongId = (int) row["songId"],
                Bpm = (decimal) row["bpm"],
                Title = row["title"].ToString(),
                DateCreation = (DateTime) row["dateCreation"],
                TimeSignature = row["timeSignature"].ToString(),
                Chart = (bool) row["chart"],
                MultiTracks = (bool) row["multitracks"],
                CustomMix = (bool) row["customMix"],
                RehearsalMix = (bool) row["rehearsalMix"],
                Patches = (bool) row["patches"],
                SongSpecificPatches = (bool) row["songSpecificPatches"],
                ProPresenter = (bool) row["proPresenter"],
                AlbumId = (int)row["albumID"],
                ArtistId = (int)row["artistID"]
            };
            if (!lazy)
            {
                foreach (DataRow albumRow in albumTable.Rows)
                {
                    if ((int) albumRow["albumID"] == song.AlbumId)
                    {
                        song.Album = populateAlbumFromDataRow(albumRow);
                    }
                }
                song.Artist = populateArtistFromDataRow(artistTable.Rows[0], true, albumTable, songTable);
            }

            return song;
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
                        yield return populateSongFromDataRow(row, lazy, set.Tables[2], set.Tables[1], set.Tables[0]);
                    }
                    yield return populateSongFromDataRow(row, lazy);
                }
            }
        }
    }
}