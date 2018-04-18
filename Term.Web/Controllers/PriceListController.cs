using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using YstProject.Models;
using YstProject.Services;
using YstTerm.Models;
using Yst.Utils;
using System.Drawing;
using OfficeOpenXml.Style;
using Term.Utils;
using Term.Web.Filters;
using Term.Web.Views.Resources;

namespace Term.Web.Controllers
{
    [ErrorIfPricesAreBeingUpdatedMvcFilter]
    [TrackUserAction]
    public class PriceListController : BaseController
    {
        //
        // GET: /PriceList/
        /// <summary>
        /// Формирует эксель файл общий для головного терминала и точки
        /// </summary>
        /// <returns></returns>
        [CheckPassThroughKeyword]
        public async Task Index()
        {
            byte[] result;
            bool IsPartner = ServicePP.IsPartner;      

            int PointId = ServicePP.getPointID();
            string partnerId = ServicePP.GetPartnerIdByPointId(PointId);

            string parameters = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Height, @SeasonId, @Article, @ProductName, @SortBy";
            string sqltext = (ServicePP.IsPartner ? "exec spGetTyresPartnerToClient " :
            "exec spGetTyresPointToClient ") + parameters;

            Type typeOftyreResult = IsPartner ? typeof(PriceListPartnerTyreResult) : typeof(PriceListPointTyreResult);
            Type typeOfdiskResult = IsPartner ? typeof(PriceListPartnerDiskResult) : typeof(PriceListPointDiskResult);
            Type typeOfakbResult = typeof(PriceListAkbResult);
            Type typeOfaccResult = typeof(PriceListAccResult);

          

            
            var sqlparameters = new SqlObjectParameterCollection{
                {"PartnerID", partnerId},
                {"PartnerPointID", PointId},
                {"SortBy", "Name"}
            };


            String[] tyreParameters = { "ProducerID", "Diametr", "Width", "Height", "SeasonId", "Article", "ProductName"};
            Array.ForEach(tyreParameters, str => sqlparameters.Add(str));

        

            PriceListTyreBase[] tyres;
            if (IsPartner)
                tyres = (DbContext.Database.SqlQuery<PriceListPartnerTyreResult>(sqltext, sqlparameters.ToArray()).ToArray());
            else
                tyres = (DbContext.Database.SqlQuery<PriceListPointTyreResult>(sqltext, sqlparameters.ToArray()).ToArray());

            // упорядочиваем массивы свойств, которые затем передаем в Excel
           
            var tyrePropsInfo=typeOftyreResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
            var diskPropsInfo = typeOfdiskResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
            var akbPropsInfo = typeOfakbResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
            var accPropsInfo = typeOfaccResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
           

             parameters = "@PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Hole, @Dia,@PCD, @ET, @Article, @ProductName, @DiskColor,@ExactSize, @SortBy,@ForPriceExcel";

             sqltext = (ServicePP.IsPartner? @"exec spGetDisksPartnerToClient " : "exec spGetDisksPointToClient ") + parameters;

            

             sqlparameters = new SqlObjectParameterCollection{
                {"PartnerID", partnerId},
                {"PartnerPointID", PointId},
                {"ExactSize", 1},
                {"SortBy", "Name"},
                {"ForPriceExcel", 1},
            };


             String[] disksParameters = { "ProducerID", "Diametr", "Width", "Hole", "Dia", "PCD", "ET", "Article", "ProductName", "DiskColor" };
             Array.ForEach(disksParameters, str => sqlparameters.Add(str));

             PriceListDiskBase[] disks;
             if (IsPartner)
                 disks = DbContext.Database.SqlQuery<PriceListPartnerDiskResult>(sqltext, sqlparameters.ToArray()).ToArray();
             else
                 disks = DbContext.Database.SqlQuery<PriceListPointDiskResult>(sqltext, sqlparameters.ToArray()).ToArray();

            //akb

            sqltext = (IsPartner ? "exec spGetAkb @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy" : "exec spGetAkbPointToClient @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy");

           

            sqlparameters = new SqlObjectParameterCollection{
                {"PartnerID", partnerId},
                {"PartnerPointID", PointId},
                {"SortBy", "Name"}
            };

            String[] akbParameters = { "ProducerID", "Inrush_Current", "Volume", "Polarity", "Brand", "Size", "Article",  "ProductName"};


            Array.ForEach(akbParameters, str => sqlparameters.Add(str));

            var akb = DbContext.Database.SqlQuery<PriceListAkbResult>(sqltext, sqlparameters.ToArray()).ToArray();

            //acc

            sqltext = (IsPartner ? "exec spGetAcc @PartnerId, @PartnerPointId, @Categories, @Article,  @ProducerId,@ProductName, @SortBy" :
                "exec spGetAccPointToClient @PartnerId, @PartnerPointId, @Categories, @Article,@ProducerId, @ProductName, @SortBy");

           
            sqlparameters = new SqlObjectParameterCollection{
                {"PartnerID", partnerId},
                {"PartnerPointID", PointId},
                {"Categories", null},
                {"Article", null},
                {"ProducerId", null},
                {"ProductName", null},
                {"SortBy", "Name"}
            };
            
            var acc = DbContext.Database.SqlQuery<PriceListAccResult>(sqltext, sqlparameters.ToArray()).ToArray();
           
          using (ExcelPackage pck = new ExcelPackage())
          {
              ExcelWorksheet wsTyres = pck.Workbook.Worksheets.Add(Header.Tyres);

              wsTyres.Cells["A1"].LoadFromCollection(tyres, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, tyrePropsInfo);
              wsTyres.Cells["A1:L1"].AutoFilter = true;
              //wsTyres.Cells["A1"].LoadFromCollection(tyres, true);

              for (int i = 1; i < wsTyres.Dimension.Columns; i++)
                  wsTyres.Column(i).AutoFit();


              

              ExcelWorksheet wsDisks = pck.Workbook.Worksheets.Add(Header.Wheels);
              wsDisks.Cells["A1"].LoadFromCollection(disks, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, diskPropsInfo);
              wsDisks.Cells["A1:P1"].AutoFilter = true;
              
              //wsDisks.Cells["A1"].LoadFromCollection(disks, true);

              for (int i = 1; i < wsDisks.Dimension.Columns; i++)
                  wsDisks.Column(i).AutoFit();

              ExcelWorksheet wsAkb = pck.Workbook.Worksheets.Add(Header.CarBatteries);
              wsAkb.Cells["A1"].LoadFromCollection(akb, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, akbPropsInfo);
              wsAkb.Cells["A1:P1"].AutoFilter = true;
              //wsDisks.Cells["A1"].LoadFromCollection(disks, true);

              for (int i = 1; i < wsAkb.Dimension.Columns; i++)
                  wsAkb.Column(i).AutoFit();

              //Create the worksheet
              
               ExcelWorksheet wsAcc = pck.Workbook.Worksheets.Add(Header.Accessories);
               wsAcc.Cells["A1"].LoadFromCollection(acc, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, accPropsInfo);
               wsAcc.Cells["A1:P1"].AutoFilter = true;
               //wsDisks.Cells["A1"].LoadFromCollection(disks, true);

               for (int i = 1; i < wsAcc.Dimension.Columns; i++)
                   wsAcc.Column(i).AutoFit();
              
               result = pck.GetAsByteArray();
          }
                   
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment;  filename=ExcelPrice.xlsx");
            Response.BinaryWrite(result);
 
          
        }
     

        /// <summary>
        /// Формирует эксель файл остатков для головного терминала по всем подразделениям
        /// </summary>
        /// <returns></returns>
        public async Task RestsOnAllDepartments()
            {
            byte[] result;
            

            int PointId = ServicePP.getPointID();
            string partnerId = ServicePP.GetPartnerIdByPointId(PointId);

            
            /////// шины
             string sqltext = "exec spGetTyresForXml @PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Height, @SeasonId, @Article, @ProductName, @SortBy";

            Type typeOftyreResult =  typeof(PriceListTyreXml) ;
            var tyrePropsInfo = typeOftyreResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
            
            var sqlparameters = new SqlObjectParameterCollection();
            sqlparameters.Add("PartnerID", partnerId).Add("PartnerPointID", PointId).Add("ProducerID")
                .Add("Diametr").Add("Width").Add("Height").Add("SeasonId").Add("Article").Add("ProductName")
                .Add("SortBy", "Name");



            var tyres = DbContext.Database.SqlQuery<PriceListTyreXml>(sqltext, sqlparameters.ToArray()).ToArray();


            /////// диски
            Type typeOfdiskResult =  typeof(PriceListDiskXml);
            var diskPropsInfo = typeOfdiskResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();

            
            sqltext = "exec spGetDisksForXml @PartnerId, @PartnerPointId, @ProducerId,@Diametr, @Width, @Hole, @Dia,@PCD, @ET, @Article, @ProductName, @DiskColor,@ExactSize, @SortBy";


            
            sqlparameters.Clear();
            sqlparameters.Add("PartnerID", partnerId).Add("PartnerPointID", PointId).Add("ProducerID")
                .Add("Diametr").Add("Width").Add("Hole").Add("Dia").Add("PCD").Add("ET").Add("Article").Add("ProductName").Add("DiskColor").Add("ExactSize",1). 
                Add("SortBy", "Name");
             //   .Add("SortBy", "Name");

            var disks = DbContext.Database.SqlQuery<PriceListDiskXml>(sqltext, sqlparameters.ToArray()).ToArray();


            /////// аккумуляторы
            Type typeOfakbResult = typeof(PriceListAkbXml);
            var akbPropsInfo = typeOfakbResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();

            sqltext ="exec spGetAkbForXml @PartnerId, @PartnerPointId, @ProducerID,@Inrush_Current,@Volume, @Polarity, @Brand, @Size,@Article, @ProductName, @SortBy";

            sqlparameters = new SqlObjectParameterCollection{
                {"PartnerID", partnerId},
                {"PartnerPointID", PointId},
                {"SortBy", "Name"}
            };
            

            String[] akbParameters = {"ProducerID", "Inrush_Current", "Volume", "Polarity", "Brand", "Size", "Article", "ProductName" };
            Array.ForEach(akbParameters,str=> sqlparameters.Add(str));
            
            var akb = DbContext.Database.SqlQuery<PriceListAkbXml>(sqltext, sqlparameters.ToArray()).ToArray();

            /////// аксессуары

            Type typeOfaccResult = typeof(PriceListAccXml);
            var accPropsInfo = typeOfaccResult.GetProperties().Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();

            sqltext = "exec spGetAccForXml @PartnerId, @PartnerPointId, @Categories, @Article, @ProductName, @SortBy";

          sqlparameters = new SqlObjectParameterCollection{{"PartnerID", partnerId},{"PartnerPointID", PointId},{"Categories",null},{"Article",null},{"ProductName",null},{"SortBy","Name"}};
           
           

            
             var acc = DbContext.Database.SqlQuery<PriceListAccXml>(sqltext, sqlparameters.ToArray()).ToArray();


            //////////// загрузка в excel
           
            using (ExcelPackage pck = new ExcelPackage())
            {
            ExcelWorksheet wsTyres = pck.Workbook.Worksheets.Add(Header.Tyres);

            wsTyres.Cells["A1"].LoadFromCollection(tyres, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, tyrePropsInfo);
            wsTyres.Cells["A1:L1"].AutoFilter = true;

            for (int i = 1; i < wsTyres.Dimension.Columns; i++)
            wsTyres.Column(i).AutoFit();

            ExcelWorksheet wsDisks = pck.Workbook.Worksheets.Add(Header.Wheels);
            wsDisks.Cells["A1"].LoadFromCollection(disks, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, diskPropsInfo);
            wsDisks.Cells["A1:P1"].AutoFilter = true;
            //wsDisks.Cells["A1"].LoadFromCollection(disks, true);
                string brand = String.Empty;
                string model = String.Empty;
                for (int i = 1; i < wsDisks.Dimension.Rows; i++)
                {
                    brand = wsDisks.GetValue(i, 2).ToString();
                    model = wsDisks.GetValue(i, 3).ToString();
                    if (brand == "VISSOL")
                    {
                        var restYar = wsDisks.GetValue(i, 14).ToString();
                        if (restYar == "50" && model.Contains("F"))
                        {
                            wsDisks.Cells[i, 14].Value = "Под заказ";
                        }
                        for (int j = 1; j < 22; j++)
                        {
                            wsDisks.Cells[i, j].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            wsDisks.Cells[i, j].Style.Fill.BackgroundColor.SetColor(Color.IndianRed);
                        }
                    }
                }

                for (int i = 1; i < wsDisks.Dimension.Columns; i++)
                wsDisks.Column(i).AutoFit();

            ExcelWorksheet wsAkb = pck.Workbook.Worksheets.Add(Header.CarBatteries);
            wsAkb.Cells["A1"].LoadFromCollection(akb, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, akbPropsInfo);
            wsAkb.Cells["A1:P1"].AutoFilter = true;
            //wsDisks.Cells["A1"].LoadFromCollection(disks, true);

            for (int i = 1; i < wsAkb.Dimension.Columns; i++)
                wsAkb.Column(i).AutoFit();

            ExcelWorksheet wsAcc = pck.Workbook.Worksheets.Add(Header.Accessories);
            wsAcc.Cells["A1"].LoadFromCollection(acc, true, OfficeOpenXml.Table.TableStyles.None, BindingFlags.Public | BindingFlags.GetProperty, accPropsInfo);
            wsAcc.Cells["A1:P1"].AutoFilter = true;
            //wsDisks.Cells["A1"].LoadFromCollection(disks, true);

            for (int i = 1; i < wsAcc.Dimension.Columns; i++)
                wsAcc.Column(i).AutoFit();

            result = pck.GetAsByteArray();

            }
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            Response.AddHeader("content-disposition", "attachment; filename=ExcelPriceAllDep.xlsx");
            Response.BinaryWrite(result);
            }


    }
}
