using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Resources;
using YstStore.Domain.Models;
using Yst.ViewModels;
using System.Data.SqlClient;


namespace Yst.Sql
{
    public static class SQLQueryBuilder
    {


        public static string generateSQLForTyresWithParams(FormCollection form, ref List<SqlParameter> sqlparams)
        {

            String query_str = @"SELECT TOP (50) cast(row_number as integer) row_number , ProductId, Name, ProducerName, Rest, Price, PictureUrl, Season FROM
            ( SELECT row_number() OVER (ORDER BY Products.[Name] ASC) AS [row_number],  Products.ProductId ProductId, ISNULL(RestOfProducts.Rest,0) Rest, Products.Name Name,  ISNULL(PriceOfPartners.Price,0) Price,
            ISNULL(Models.PictureUrl,'') PictureUrl, ISNULL(Models.Season,'') Season, ISNULL(Producers.Name,'') ProducerName  FROM Products
            INNER JOIN PriceOfPartners  ON Products.ProductId=PriceOfPartners.ProductId
            INNER JOIN RestOfProducts ON Products.ProductId=RestOfProducts.ProductId
            LEFT JOIN Models ON Products.ModelId=Models.ModelId
            LEFT JOIN Producers ON Products.ProducerID=Producers.ProducerID";


            //List<SqlParameter> sqlparams = new List<SqlParameter>();
            //sqlparams.Add(new SqlParameter("productid", 1));
            
            int page_index = 0;
            try
            {
                page_index = Convert.ToUInt16(form["page"]) - 1;
            }
            catch (Exception)
            {
                page_index = 0;
            }

            if (queryHasTyporazmerParams(form))
                query_str += " left join Tiporazmers ON Products.TiporazmerID=Tiporazmers.TiporazmerID \n";

           

            Dictionary<string, string> requestToQueryFields = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            {"ProducerID","Products.ProducerID"}, {"Article","Products.Article"}, {"producttype","Products.ProductType"}, {"seasonId","Models.Season"},
            {"width","Tiporazmers.width"},  {"height","Tiporazmers.height"},  {"diametr","Tiporazmers.diametr"},  {"name","Products.Name"}      };

            //string where_sql = " where (PartnerId='92884')";
            string where_sql = " where (Products.ProductType='tyre') ";

            foreach (string formparam in form)
            {
                if (!String.IsNullOrEmpty(form[formparam]))
                    if (formparam.ToLower() == "article" || formparam.ToLower() == "name")
                    {
                        where_sql += String.Format(" and ({0} like @{1})", requestToQueryFields[formparam], formparam);
                        sqlparams.Add(new SqlParameter(formparam, String.Format("%{0}%",form[formparam])));        
                    }
                    else
                    {
                        where_sql += requestToQueryFields.ContainsKey(formparam) ? String.Format(" and ({0}=@{1})", requestToQueryFields[formparam], formparam) : "";
                        sqlparams.Add(new SqlParameter(formparam, form[formparam]));
                    }
                
            }

            string orderby_str = ")A";

            if (page_index > 0) orderby_str = String.Format(")A where row_number>{0} ORDER BY Name ASC ", page_index * 50);

            return (query_str + where_sql + orderby_str);
        }




        public static string generateSQLForTyres(FormCollection form)
        {
            
            String query_str = @"SELECT TOP (50) cast(row_number as integer) row_number , ProductId, Name, Rest, Price, PictureUrl, Season FROM
            ( SELECT row_number() OVER (ORDER BY Products.[Name] ASC) AS [row_number],  Products.ProductId ProductId, ISNULL(RestOfProducts.Rest,0) Rest, Products.Name Name,  ISNULL(PriceOfPartners.Price,0) Price,
            ISNULL(Models.PictureUrl,'') PictureUrl, ISNULL(Models.Season,'') Season FROM Products
            inner join PriceOfPartners  ON Products.ProductId=PriceOfPartners.ProductId
            inner join RestOfProducts ON Products.ProductId=RestOfProducts.ProductId
            left join Models ON Products.ModelId=Models.ModelId";
            
            
            int page_index=0;
            try {
                 page_index= Convert.ToUInt16(form["page"])-1;  }
                catch (Exception) {
                 page_index=0;
            }

            if ( queryHasTyporazmerParams(form))
                query_str += " left join Tiporazmers ON Products.TiporazmerID=Tiporazmers.TiporazmerID \n";

          /*  if (queryHasProducerParams(form))
                query_str += " left join Producers ON Products.ProducerID=Producers.ProducerID \n"; */

            Dictionary<string, string> requestToQueryFields = new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase) {
            {"ProducerID","Products.ProducerID"}, {"Article","Products.Article"}, {"producttype","Products.ProductType"}, {"seasonId","Models.Season"},
            {"width","Tiporazmers.width"},  {"height","Tiporazmers.height"},  {"diametr","Tiporazmers.diametr"},  {"name","Products.Name"}      };

            //string where_sql = " where (PartnerId='92884')";
            string where_sql = " where (1=1) ";

         foreach (string formparam in form)
         {
             if (!String.IsNullOrEmpty(form[formparam]))
                 if (formparam.ToLower() =="article" || formparam.ToLower() =="name"  )
                 {
                     where_sql += String.Format(" and ({0} like '%{1}%')", requestToQueryFields[formparam], form[formparam]);
                 }
                 else
             {
                 where_sql += requestToQueryFields.ContainsKey(formparam) ? String.Format(" and ({0}='{1}')", requestToQueryFields[formparam], form[formparam]) : "";
             } 
            
         }

            string orderby_str=")A";

            if (page_index>0 )   orderby_str =String.Format(")A where row_number>{0} ORDER BY Name ASC ",page_index*50);

         return (query_str+where_sql+orderby_str);
     }


        public static bool queryHasTyporazmerParams(FormCollection form)
        {
            List<string> t_array = new List<string> { "width", "height", "diametr", "dia", "pcd", "et" };
            
            foreach (string formparam in form)
            {
                if (t_array.Exists(a => a == formparam.ToLower()))
                { return true; }

         
            }
            return false;
        }

        public static bool queryHasProducerParams(FormCollection form)
        {
            
            foreach (string formparam in form)
            {
                if (formparam == "producer_name")
                return true; 
}
            return false;
        }



    }

   
}