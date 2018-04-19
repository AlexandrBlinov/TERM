<%@ WebHandler Language="C#" Class="PricesOfSuppliersDataHandler" %>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;


public class PricesOfSuppliersDataHandler : IHttpHandler
{
        static string specifier = "G29";

    class SupplierData
    {
        public string Supplier {get; set;}
        public string Article {get; set;}
        public string Productname {get; set;}
        public decimal Price { get; set; }
            public string Date { get; set; }


    }

    public void ProcessRequest(HttpContext context)
    {
        
        string connectionString = "Data Source=193.107.181.75;Initial Catalog=PricesOfSuppliers;User Id=pricesofsuppliers;Password=facility123;MultipleActiveResultSets=True;";
        IList<SupplierData> list = new List<SupplierData>();

        using (var conn = new SqlConnection(connectionString))
        {

            using (SqlCommand command = conn.CreateCommand())
            {
                conn.Open();
                command.CommandText = "GetPrices";
                command.CommandType = CommandType.StoredProcedure;


                SqlDataReader rdr = command.ExecuteReader();
                while (rdr.Read())
                {
                    list.Add(new SupplierData
                    {
                        Supplier = (string)rdr["Supplier"],
                        Article = (string)rdr["Article"],
                        Productname = (string)rdr["Productname"],
                        Price = (decimal)rdr["Price"],
                        Date= ((DateTime)rdr["Date"]).ToString("dd.MM.yyyy"),
                        /*    RequestDate = ((DateTime)rdr["RequestDate"]).ToString("dd-MM-yyyy HH:mm:ss"),
                            LocationDate = ((DateTime)rdr["LocationDate"]).ToString("dd-MM-yyyy HH:mm:ss"), */

                    });
                }
            }
            conn.Close();

            var sb = new StringBuilder();
            foreach (var item in list)

                sb.AppendFormat("{0};{1};{2};{3};{4}", item.Supplier, item.Article, item.Productname, item.Price.ToString(specifier),item.Date).AppendLine();

            // JavaScriptSerializer serializer = new JavaScriptSerializer();

            // string json = serializer.Serialize(list);
            //  context.Response.ContentType = "application/json";
            context.Response.ContentType = "text/csv";
             context.Response.ContentType = "text/plain";
            
            context.Response.Charset = "UTF-8";
            context.Response.Write(sb.ToString());

        }


    }

    public bool IsReusable {
        get {
            return false;
        }
    }

}