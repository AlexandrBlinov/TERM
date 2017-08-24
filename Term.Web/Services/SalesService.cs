using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using PagedList;
using Term.DAL;
using Term.Web.Models;
using Yst.ViewModels;
using YstProject.Services;
using Term.Web.Services;

namespace Term.Services
{
    /// <summary>
    /// Сервис для получения данных из реализаций
    /// </summary>
    public class SalesService : BaseService
    {

        private readonly Expression<Func<Sale, SaleViewModel>> _selector = o => new SaleViewModel
        {
            GuidIn1S = o.GuidIn1S,
            PartnerId = o.PartnerId,
            NumberIn1S = o.NumberIn1S,
            Total = o.Total,
            SaleDate = o.SaleDate,
            GuidOfOrderIn1S = o.GuidOfOrderIn1S,
            DepartmentId = o.DepartmentId,
            Driver = o.Driver,
            PhoneNumberOfDriver = o.PhoneNumberOfDriver,
            BrandOfAuto = o.BrandOfAuto,
            RegNumOfAuto = o.RegNumOfAuto,
            Comments = o.Comments,
            PointId = o.PointId,
            SaleDetails = o.SaleDetails
        };

        /// <summary>
        /// Получить список реализаций для партнера
        /// </summary>
        /// <param name="model"></param>
        /// <param name="partnerId">Партнер</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public SalesViewModel GetListOfSalesByPartnerId(SalesViewModel model, string partnerId)
        {

            Expression<Func<Sale, bool>> predicate =
                o =>
                    o.PartnerId == partnerId && (model.PointId == null || o.PointId == model.PointId) &&
                    (o.SaleDate >= model.BeginDate || model.BeginDate == null) &&
                    (o.SaleDate <= model.EndDate || model.EndDate == null);
           
             
            if (!String.IsNullOrEmpty(model.ProductName))
        
            {
                Expression<Func<Sale, bool>> predicatebyProductId = s => (s.SaleDetails.Any(p => model.ProductName == null ||  p.ProductId.ToString().Contains(model.ProductName) || (p.Product.Name!=null &&   p.Product.Name.Contains(model.ProductName)) || (p.Product.Article!=null && p.Product.Article.Contains(model.ProductName))));
                predicate = predicate.And(predicatebyProductId);

            }
                


            if (model.StatusId == (int?)OrderStatuses.ShippedForSale) predicate=predicate.And(p => !p.IsDelivered);
            if (model.StatusId == (int?)OrderStatuses.DeliveredToClient) predicate=predicate.And(p => p.IsDelivered);

            model.Sales = DbContext.Sales.Where(predicate)
                .Include(p => p.SaleDetails)
                .Include(i => i.SaleDetails.Select(p => p.Product))
                .Select(_selector).OrderByDescending(o => o.SaleDate).ToPagedList(model.Page, model.ItemsPerPage);



            return model;
        }

        /// <summary>
        /// Получить список реализаций для точки
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pointId">Id точки</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public SalesViewModel GetListOfSalesByPointId(SalesViewModel model, int? pointId)
        {
            Expression<Func<Sale, bool>> predicate =
                o =>o.PointId == pointId && (o.SaleDate >= model.BeginDate || model.BeginDate == null) &&
                        (o.SaleDate <= model.EndDate || model.EndDate == null);


            if (!String.IsNullOrEmpty(model.ProductName))
            {
                Expression<Func<Sale, bool>> predicatebyProductId = s => (s.SaleDetails.Any(p => model.ProductName == null ||  p.ProductId.ToString().Contains(model.ProductName) || (p.Product.Name!=null &&   p.Product.Name.Contains(model.ProductName)) || (p.Product.Article!=null && p.Product.Article.Contains(model.ProductName))));
                predicate = predicate.And(predicatebyProductId);

            }

            if (model.StatusId == (int?)OrderStatuses.ShippedForSale) predicate=predicate.And(p => !p.IsDelivered);
            if (model.StatusId == (int?)OrderStatuses.DeliveredToClient) predicate=predicate.And(p => p.IsDelivered);

            model.Sales = DbContext.Sales.Where(predicate).Select(o => new SaleViewModel
            {
                GuidIn1S = o.GuidIn1S,
                PartnerId = o.PartnerId,
                NumberIn1S = o.NumberIn1S,
                SaleDate = o.SaleDate,
                GuidOfOrderIn1S = o.GuidOfOrderIn1S,
                DepartmentId = o.DepartmentId,
                Driver = o.Driver,
                PhoneNumberOfDriver = o.PhoneNumberOfDriver,
                BrandOfAuto = o.BrandOfAuto,
                RegNumOfAuto = o.RegNumOfAuto,
                Comments = o.Comments,
                PointId = o.PointId


            }).OrderByDescending(o => o.SaleDate).ToPagedList(model.Page, model.ItemsPerPage);

            return model;
        }

        /// <summary>
        /// Получить реализацию по Guid
        /// </summary>
        /// <param name="guidSaleIn1S"></param>
        /// <returns></returns>
        public SaleViewWithDetails GetSaleByGuid(Guid guidSaleIn1S)
        {

            Sale sale = DbContext.Sales.FirstOrDefault(o => o.GuidIn1S == guidSaleIn1S);
            if (sale == null) return null;

             

            var model = DbContext.Sales.Where(o => o.GuidIn1S == guidSaleIn1S).Select(p => new SaleViewWithDetails
            {
                SaleData = new SaleViewModel { NumberIn1S = p.NumberIn1S, Total = p.Total, DepartmentId = p.DepartmentId, DischargePoint = p.DischargePoint, Driver = p.Driver, GuidIn1S = p.GuidIn1S, PhoneNumberOfDriver = p.PhoneNumberOfDriver, Comments = p.Comments, 
                    RegNumOfAuto = p.RegNumOfAuto, SaleDate = p.SaleDate, BrandOfAuto = p.BrandOfAuto,IsDelivered=p.IsDelivered},
                SaleDetails = (p.SaleDetails.Select(detail => new SaleViewDetail { RowNumber = detail.RowNumber, Count = detail.Count, Price = detail.Price, ProductId = detail.ProductId, ProductName = detail.Product.Name }).OrderBy(sort => sort.RowNumber)),


            }).FirstOrDefault();

            Order order = DbContext.Orders.FirstOrDefault(o => o.GuidIn1S == sale.GuidOfOrderIn1S);
            if (order != null && model.SaleData!=null) model.SaleData.DpdDeliveryStatus = order.DpdDeliveryStatus;

            /* if (order != null && model.SaleDetails != null)
                 model.SaleData.OrderNumberIn1S = order.NumberIn1S;
             model.SaleData.OrderTotal = order.Total;*/



            /* if (model.SaleDetails == null)
                 return null;
             model.OrderDetails = _orderDetails.GetAll(od => od.OrderId == order.OrderId).Include(p => p.Product).Select(detail => new OrderViewDetail { RowNumber = detail.RowNumber, Count = detail.Count, Price = detail.Price, ProductId = detail.ProductId, ProductName = detail.Product.Name });*/

            return model;


        }
    }
}