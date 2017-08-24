using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.ViewModels;
using Yst.Context;
using System.Data.SqlClient;
using Term.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data;
using YstTerm.Models;
using YstProject.Services;
using PagedList;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Yst.Services
{
    public class XMLService : BaseService, IDisposable
    {
        private bool _allocDBContext = false;
        public const string PartnerIdSessionKey = "PartnerId";

        public XMLService()
            : this(new AppDbContext())
        {
            _allocDBContext = true;
        }

        public XMLService(AppDbContext dbcontext)
            : base(dbcontext)
        {

        }


        public IQueryable<PriceListDiskXml> getDiskForPriceList(int PointId)
        {
            string partnerId = GetPartnerIdByPointId(PointId);


            string sqltext;
            string parameters = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Hole, @Dia,@PCD, @ET, @Article, @ProductName, @DiskColor,@ExactSize, @SortBy";
            
            sqltext = @"exec spGetDisksForXml " + parameters;
            
            var sqlparams = new List<SqlParameter>
            {new SqlParameter("PartnerID", partnerId),  new SqlParameter("PartnerPointID", PointId),
              new SqlParameter { ParameterName = "@ProducerID", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Diametr", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Width", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Hole", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Dia", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@PCD", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@ET", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@Article", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@ProductName", Value = DBNull.Value } ,
            new SqlParameter { ParameterName = "@DiskColor", Value =  DBNull.Value } ,
            new SqlParameter { ParameterName = "@ExactSize", Value = 0   } ,
             new SqlParameter { ParameterName = "@SortBy", Value = "Name" } 
            };

            //IEnumerable<PriceListDiskXml> disk;
            return DbContext.Database.SqlQuery<PriceListDiskXml>(sqltext, sqlparams.ToArray()).AsQueryable();

            
        }


        public IQueryable<PriceListTyreXml> getTyreForPriceList(int PointId)
        {
            string partnerId = GetPartnerIdByPointId(PointId);


            string parameters = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Height, @SeasonId, @Article, @ProductName, @SortBy";
            string sqltext = "exec spGetTyresForXml " + parameters;


            List<SqlParameter> sqlparams = new List<SqlParameter>
            {new SqlParameter("PartnerID", partnerId),  new SqlParameter("PartnerPointID", PointId),
              new SqlParameter { ParameterName = "@ProducerID", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Diametr", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Width", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Height", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@SeasonId", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@Article", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@ProductName", Value = DBNull.Value } ,
            new SqlParameter { ParameterName = "@SortBy", Value = "Name" } 
            };

          
            return DbContext.Database.SqlQuery<PriceListTyreXml>(sqltext, sqlparams.ToArray()).AsQueryable();

         
        }

        public IQueryable<PriceListAccXml> getAccsForPriceList(int PointId)
        {
            string partnerId = GetPartnerIdByPointId(PointId);


            string sqltext = "exec spGetAccForXml @PartnerId, @PartnerPointId, @Categories, @Article, @ProductName, @SortBy" ;

            List<SqlParameter> sqlparams = new List<SqlParameter>
            {new SqlParameter("PartnerID", partnerId),  new SqlParameter("PartnerPointID", PointId),
            new SqlParameter ( "Categories", DBNull.Value ),
              new SqlParameter { ParameterName = "@Article", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@ProductName", Value = DBNull.Value } ,
             new SqlParameter { ParameterName = "@SortBy", Value = "Name" } 
            };

            
            return DbContext.Database.SqlQuery<PriceListAccXml>(sqltext, sqlparams.ToArray()).AsQueryable();

            
        }

        public IQueryable<PriceListAkbXml> getAkbForPriceList(int PointId)
        {
            string partnerId = GetPartnerIdByPointId(PointId);


            string sqltext = "exec spGetAkbForXml @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy" ;

            List<SqlParameter> sqlparams = new List<SqlParameter>
            {new SqlParameter("PartnerID", partnerId),  new SqlParameter("PartnerPointID", PointId),
                new SqlParameter ( "IsPartner", IsPartner ),
                 new SqlParameter { ParameterName = "@ProducerID", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Inrush_Current", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Volume", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Polarity", Value = DBNull.Value },
              new SqlParameter { ParameterName = "@Brand", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@Size", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@Article", Value = DBNull.Value },
            new SqlParameter { ParameterName = "@ProductName", Value = DBNull.Value } ,
             new SqlParameter { ParameterName = "@SortBy", Value = "Name" } 
            };

            
            return  DbContext.Database.SqlQuery<PriceListAkbXml>(sqltext, sqlparams.ToArray()).AsQueryable();

            
        }

        public void Dispose()
        {
            if (_allocDBContext)
                DbContext.Dispose();
        }


       
    }
}