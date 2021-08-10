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
        private SQL sql;

        public SQLDecorator(SQL sql)
        {
            this.sql = sql;
        }

        private Artist populateArtistFromDataReader(SqlDataReader reader)
        {
            return new Artist()
            {
                ArtistId = (int)reader["artistId"],
                Biography = reader["biography"].ToString(),
                DateCreation = (DateTime)reader["dateCreation"],
                HeroUrl = new Uri(reader["heroURL"].ToString()),
                ImageUrl = new Uri(reader["ImageURL"].ToString()),
                Title = reader["title"].ToString()
            };
        }

        public IEnumerable<Artist> GetArtists()
        {
            using (SqlDataReader reader = sql.ExecuteStoredProcedureDataReader("GetAllArtists"))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return populateArtistFromDataReader(reader);
                    }
                }
            }
        }
        public Artist GetArtistById(int artistId)
        {
            //Setup stored procedure parameters.
            SqlParameter artistIdParameter = new SqlParameter("@artistId", SqlDbType.Int) {Value = artistId};
            sql.Parameters.Add(artistIdParameter);
            using (SqlDataReader reader = sql.ExecuteStoredProcedureDataReader("GetArtistDetails"))
            {
                if (reader.HasRows)
                {
                    reader.Read();
                    return populateArtistFromDataReader(reader);
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
            using (SqlDataReader reader = sql.ExecuteStoredProcedureDataReader("GetArtistDetailsByName"))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        yield return populateArtistFromDataReader(reader);
                    }
                }
            }
        }

        public Artist AddArtist(Artist artist)
        {
            if (artist == null)
                throw new ArgumentException("You must supply an artist model.", nameof(artist));

            SQL sql = new SQL();
            //Setup parameters
            sql.Parameters.Add(new SqlParameter("@dateCreation", SqlDbType.SmallDateTime) {Value = artist.DateCreation ?? DateTime.Now});
            sql.Parameters.Add(new SqlParameter("@title", SqlDbType.VarChar) {Value = artist.Title});
            sql.Parameters.Add(new SqlParameter("@biography", SqlDbType.VarChar) {Value = artist.Biography});
            sql.Parameters.Add(new SqlParameter("@imageURL", SqlDbType.VarChar) {Value = artist.ImageUrl.ToString()});
            sql.Parameters.Add(new SqlParameter("@heroURL", SqlDbType.VarChar) {Value = artist.HeroUrl.ToString()});

            sql.OpenConnection();
            sql.BeginTransaction();
            var reader = sql.ExecuteStoredProcedureDataReader("AddArtist");
            //The stored-procedure will return the added row.
            Artist result = null;
            if (reader.HasRows)
            {
                reader.Read();
                result = populateArtistFromDataReader(reader);
            }
            reader.Close();
            sql.Commit();
            sql.CloseConnection();
            return result ?? throw new Exception("Artist could not be returned from server.");
        }

        public Album GetAlbumById(int albumId)
        {
            throw new NotImplementedException();
        }

        public Song GetSongById(int songId)
        {
            throw new NotImplementedException();
        }
    }
}