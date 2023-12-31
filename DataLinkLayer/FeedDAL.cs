﻿using Blogger_C_.Models;
using DataAccessLayer.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class FeedDAL
    {
        public async Task<SqlConnection?> StartConnection()
        {
            string connectionString = @"Data Source=DESKTOP-H9T9SKL;Initial Catalog=tester; User ID=sa;Password=RPSsql12345;TrustServerCertificate=True;";
            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                await connection.OpenAsync();
                return connection;
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
                return null;
            }
        }
        public async Task<List<FeedModel>?> ExecuteSQLQueryAsync(SqlConnection connection, SqlCommand command)
        {
            SqlDataReader reader = await command.ExecuteReaderAsync();
            List<FeedModel> feedList = new List<FeedModel>();

            if (reader.HasRows)
                while (reader.Read())
                {
                    FeedModel feed = new FeedModel();

                    feed.id = reader.GetInt32(0);
                    feed.title = reader.GetString(1);
                    feed.author_id = reader.GetInt32(2);
                    feed.content = reader.GetString(3);
                    feed.category_id = reader.GetInt32(4);
                    feed.image_link = reader.GetString(5);
                    feed.author_name = reader.GetString(6);
                    feed.category_name = reader.GetString(7);

                    feedList.Add(feed);
                }
            else
            {
                return null;
            }
            reader.Close();
            connection.Close();
            return feedList;
        }
        public async Task<List<FeedModel>?> GetListDALAsync(int skipNum, int category_id)
        {
            SqlConnection? connection = await StartConnection();
            if (connection == null)
            {
                return null;
            }
            string query = "";
            if (category_id != 0)
            {
                query = $"select proj3_feed.id,proj3_feed.title,proj3_feed.author_id,proj3_feed.content,proj3_feed.category_id,proj3_feed.image_link,proj3_userdata.username,proj3_categorydata.category_name " +
                                $"from proj3_feed " +
                                $"inner join proj3_userdata on proj3_feed.author_id=proj3_userdata.id " +
                                $"inner join proj3_categorydata on proj3_feed.category_id=proj3_categorydata.id " +
                                $" where proj3_feed.category_id=@CATEGORY_ID " +
                                $"order by id offset @SKIPNUM rows fetch next 5 rows only ";
            }
            else
            {
                query = $"select proj3_feed.id,proj3_feed.title,proj3_feed.author_id,proj3_feed.content,proj3_feed.category_id,proj3_feed.image_link,proj3_userdata.username,proj3_categorydata.category_name " +
                $"from proj3_feed " +
                $"inner join proj3_userdata on proj3_feed.author_id=proj3_userdata.id " +
                $"inner join proj3_categorydata on proj3_feed.category_id=proj3_categorydata.id " +
                $"order by id offset @SKIPNUM rows fetch next 5 rows only ";
            }

            try
            {
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@SKIPNUM", skipNum);
                command.Parameters.AddWithValue("@CATEGORY_ID", category_id);

                var data = await ExecuteSQLQueryAsync(connection, command);    //why are you sending connection bruh
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }

        }
        public async Task<List<FeedModel>?> GetFeedDataFavDALAsync(string feedIds)
        {
            SqlConnection? connection = await StartConnection();
            if (connection == null)
            {
                return null;
            }
            string query = $"select proj3_feed.id,proj3_feed.title,proj3_feed.author_id,proj3_feed.content,proj3_feed.category_id,proj3_feed.image_link,proj3_userdata.username,proj3_categorydata.category_name " +
                $"from proj3_feed " +
                $"inner join proj3_userdata on proj3_feed.author_id=proj3_userdata.id " +
                $"inner join proj3_categorydata on proj3_feed.category_id=proj3_categorydata.id " +
                $"where proj3_feed.id in (" + feedIds + ")";
            try
            {
                SqlCommand command = new SqlCommand(query, connection);

                var data = await ExecuteSQLQueryAsync(connection, command);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }
        public async Task<List<FeedModel>?> GetTopFeedsDALAsync()
        {
            SqlConnection? connection = await StartConnection();
            if (connection == null)
            {
                return null;
            }
            string query = $"select proj3_feed.id,proj3_feed.title,proj3_feed.author_id,proj3_feed.content,proj3_feed.category_id,proj3_feed.image_link,proj3_userdata.username,proj3_categorydata.category_name " +
                $"from proj3_feed " +
                $"inner join proj3_userdata on proj3_feed.author_id=proj3_userdata.id " +
                $"inner join proj3_categorydata on proj3_feed.category_id=proj3_categorydata.id " +
                $"order by proj3_feed.hits desc offset 0 rows fetch next 6 rows only";
            try
            {
                SqlCommand command = new SqlCommand(query, connection);
                var data = await ExecuteSQLQueryAsync(connection, command);
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }


        public async Task<ImageModel?> RecieveAsync(int id)   

        {
            SqlConnection? connection = await StartConnection();
            if (connection == null)
            {
                return null;
            }
            else
            {
                string query = $"select * from proj3_image where image_id="+id;

                try
                {
                   SqlCommand command = new SqlCommand(query, connection);
                    
                   SqlDataReader reader=await command.ExecuteReaderAsync();
                   ImageModel imageData = new();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            imageData.image_id = reader.GetInt32(0);
                            imageData.image_data = (byte[])reader["image_data"];
                            imageData.file_name = (string)reader["file_name"];    
                            imageData.description = reader.GetString(3);
                        }
                    }
                    reader.Close();
                    connection.Close();
                   // File.WriteAllBytes("A:\\Abay\\Coding\\Projects\\Proj1-EmployeeDept\\C#\\EmployeeDepartment\\x.jpg", imageData.image_data);
                    return imageData;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
        }

        public async Task<bool?> SendAsync(ImageModel image)
        {
            SqlConnection? connection = await StartConnection();
            if (connection == null)
            {
                return null;
            }
            else
            {
                string query = $"insert into proj3_image values(convert(varbinary(max),@IMAGE_DATA),'file','file is allright')";
                try
                {
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@IMAGE_DATA", image.image_data);

                    int affected = await command.ExecuteNonQueryAsync();

                    connection.Close();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }              
            }
        }

    }
}
