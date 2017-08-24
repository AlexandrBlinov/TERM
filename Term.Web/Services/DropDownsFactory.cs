using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.Context;
using System.Web.Mvc;

using YstTerm.Models;
using System.Collections.Specialized;
using YstProject.Services;
using System.Resources;
using System.Linq.Expressions;
using Term.Utils;
using Term.Web.Views.Resources;

namespace Yst.DropDowns
{
    public static class DropDownsFactory
    {
        private static readonly AppDbContext _dbcontext = new AppDbContext();



        public static Countries[] Countries
        {
            get
            {
                return _dbcontext.Countries.Where(p => p.Name != String.Empty).OrderBy(p => p.Name).ToArray();
            }
        }

        public static Languages[] Languages
        {
            get
            {
                return _dbcontext.Languages.Where(p => p.LanguageName != String.Empty).ToArray();
            }
        }

        public static Department[] Departments
        {
            get
            {
                return _dbcontext.Departments.Where(p => p.DepartmentId != Defaults.MainDepartment).ToArray();

            }

        }

        public static Department[] DepartmentsInclude(PartnerPoint partnerPoint)
        {
            {
                return _dbcontext.Departments.Where(p => p.DepartmentId == Defaults.MainDepartment || p.DepartmentId == partnerPoint.DepartmentId).ToArray();
            }

        }
       
   

        private static Expression<Func<T, TReturn>> GetSelector<T, TReturn>(string fieldName)
            where T : class
            where TReturn : class
        {
            var t = typeof(TReturn);
            ParameterExpression p = Expression.Parameter(typeof(T), "t");
            var body = Expression.Property(p, fieldName);
            return Expression.Lambda<Func<T, TReturn>>(body, new ParameterExpression[] { p });
        }

        


        public static string[] GetCargoProperties(ProductType typeOfProduct, string propertyname)
        {
            var selector = GetSelector<Tiporazmer, string>(propertyname.ToLower());
            var models = _dbcontext.Set<Model>().Where(p => p.Season == "cargo" || p.Season == "agricult").Select(p => p.ModelId);
            var ids = _dbcontext.Set<RestOfProduct>().Select(p => p.ProductId);
            var tids = _dbcontext.Set<Product>().Where(p => p.ProductType == typeOfProduct && models.Any(id => id == p.ModelId) && ids.Any(id => id == p.ProductId)).Select(tip => tip.TiporazmerId); // типоразмеры выбранного типа на остатках
            string[] resultarr = _dbcontext.Set<Tiporazmer>().Where(tip => tids.Any(tid => tid == tip.TiporazmerId)).Select(selector).Distinct().ToArray();
            resultarr = resultarr.Where(x => x != null && x != "1100*400*5").ToArray();
            return resultarr;

        }

        

        public static SelectList GetOrderStatuses()
        {
            return (new SelectList(Enum.GetValues(typeof(OrderStatuses)).Cast<OrderStatuses>().Where(p=>((int)p<10||(int)p>15) && p!=OrderStatuses.ShippedForSale ).Select(p => new
            {
                Id = (int)p,
                Name = EnumDescriptionProvider.GetDescription(p)

            }), "Id", "Name"));
        }


        public static SelectList GetSaleStatuses()
        {
            return (new SelectList(Enum.GetValues(typeof(OrderStatuses)).Cast<OrderStatuses>().Where(p=>p == OrderStatuses.ShippedForSale||p==OrderStatuses.DeliveredToClient).Select(p => new
            {
                Id = (int)p,
                Name = EnumDescriptionProvider.GetDescription(p)

            }), "Id", "Name"));
        }

    }
}