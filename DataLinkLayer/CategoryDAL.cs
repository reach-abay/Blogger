﻿using Blogger_C_.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CategoryDAL
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
        public async Task<List<CategoryModel>?> ExecuteQueryAsync(SqlConnection connection, string query)
        {
            List<CategoryModel> categoryList = new List<CategoryModel>();
            try
            {
                SqlCommand command = new SqlCommand(query, connection);

                SqlDataReader reader =await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    CategoryModel category = new CategoryModel();
                    category.id = reader.GetInt32(0);
                    category.category_name = reader.GetString(1);
                    category.hits = reader.GetInt32(2);
                    category.image_link = reader.GetString(3);
                    categoryList.Add(category);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            connection.Close();
            return categoryList;
        }
        public async Task<List<CategoryModel>?> GetTopDALAsync()
        {
            SqlConnection? connection=await StartConnection();
            if (connection == null){
                return null;
            }
            else{
                string query = $"select top 10 * from proj3_categorydata order by 'hits' desc ;";
                var data=await ExecuteQueryAsync(connection, query);
                return data;
            }           
        }
        public async Task<List<CategoryModel>?> GetAllDALAsync()
        {
            SqlConnection? connection =await StartConnection();
            if (connection == null){
                return null;
            }
            else{
                string query = $"select * from proj3_categorydata order by 'category_name' ;";
                var data = await ExecuteQueryAsync(connection, query);
                return data;
            }
        }

    }
}
