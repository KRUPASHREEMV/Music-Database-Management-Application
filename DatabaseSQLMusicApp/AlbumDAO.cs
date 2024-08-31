using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseSQLMusicApp
{
    internal class AlbumDAO
    {
        private readonly string connectionString = "datasource=localhost;port=3306;username=root;password=root;database=music;";

        public List<Album> getAllAlbums()
        {
            var returnThese = new List<Album>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "SELECT * FROM ALBUMS";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var album = new Album
                                {
                                    ID = reader.GetInt32("ID"),
                                    AlbumName = reader.GetString("ALBUM_TITLE"),
                                    ArtistName = reader.GetString("ARTIST"),
                                    Year = reader.GetInt32("YEAR"),
                                    ImageURL = reader.GetString("IMAGE_NAME"),
                                    Description = reader.GetString("DESCRIPTION"),
                                    Tracks = getTracksForAlbum(reader.GetInt32("ID"))
                                };
                                returnThese.Add(album);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in getAllAlbums: " + ex.Message);
                }
            }

            return returnThese;
        }

        public List<Album> searchTitles(string searchTerm)
        {
            var returnThese = new List<Album>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var searchWildPhrase = "%" + searchTerm + "%";
                    var query = "SELECT * FROM ALBUMS WHERE ALBUM_TITLE LIKE @search";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@search", searchWildPhrase);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var album = new Album
                                {
                                    ID = reader.GetInt32("ID"),
                                    AlbumName = reader.GetString("ALBUM_TITLE"),
                                    ArtistName = reader.GetString("ARTIST"),
                                    Year = reader.GetInt32("YEAR"),
                                    ImageURL = reader.GetString("IMAGE_NAME"),
                                    Description = reader.GetString("DESCRIPTION")
                                };
                                returnThese.Add(album);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in searchTitles: " + ex.Message);
                }
            }

            return returnThese;
        }

        internal int addOneAlbum(Album album)
        {
            int newRows = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "INSERT INTO albums (ALBUM_TITLE, ARTIST, YEAR, IMAGE_NAME, DESCRIPTION) VALUES (@albumtitle, @artist, @year, @imageURL, @description)";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@albumtitle", album.AlbumName);
                        command.Parameters.AddWithValue("@artist", album.ArtistName);
                        command.Parameters.AddWithValue("@year", album.Year);
                        command.Parameters.AddWithValue("@imageURL", album.ImageURL);
                        command.Parameters.AddWithValue("@description", album.Description);
                        newRows = command.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in addOneAlbum: " + ex.Message);
                }
            }

            return newRows;
        }

        public List<Track> getTracksForAlbum(int albumID)
        {
            var returnThese = new List<Track>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "SELECT ID, track_title AS Name, number AS Number, video_url AS VideoURL, lyrics AS Lyrics FROM TRACKS WHERE albums_ID = @albumid";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@albumid", albumID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var track = new Track
                                {
                                    ID = reader.GetInt32("ID"),
                                    Name = reader.GetString("Name"),
                                    Number = reader.GetInt32("Number"),
                                    VideoURL = reader.GetString("VideoURL"),
                                    Lyrics = reader.GetString("Lyrics")
                                };
                                returnThese.Add(track);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in getTracksForAlbum: " + ex.Message);
                }
            }

            return returnThese;
        }

        public List<JObject> getTracksUsingJoin(int albumID)
        {
            var returnThese = new List<JObject>();

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "SELECT tracks.ID AS ID, tracks.track_title AS Name, tracks.number AS Number, tracks.video_url AS VideoURL, tracks.lyrics AS Lyrics FROM tracks JOIN albums ON tracks.albums_ID = albums.ID WHERE tracks.albums_ID = @albumid";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@albumid", albumID);
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var newTrack = new JObject();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    newTrack.Add(reader.GetName(i), reader.GetValue(i).ToString());
                                }
                                returnThese.Add(newTrack);
                            }
                        }
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in getTracksUsingJoin: " + ex.Message);
                }
            }

            return returnThese;
        }

        internal int deleteTrack(int trackID)
        {
            int result = 0;

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    var query = "DELETE FROM tracks WHERE ID = @trackID";
                    using (var command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@trackID", trackID);
                        result = command.ExecuteNonQuery();
                    }
                }
                catch (MySqlException ex)
                {
                    Console.WriteLine("An error occurred in deleteTrack: " + ex.Message);
                }
            }

            return result;
        }
    }
}



