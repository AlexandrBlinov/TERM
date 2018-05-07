using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using YstTerm.Models;

namespace Term.Web.Services
{
    /// <summary>
    /// Нужен для сортировок в подборах
    /// </summary>
    public static class OrderedCollectionService
    {
        
        //
        // Подборы по параметрам (нет вложенной сортировки)
        //
        public static IQueryable<SearchResult> GetOrderedResults(this IQueryable<SearchResult> queryNotOrdered, SortBy  sortBy)
        {
            IQueryable<SearchResult> query;
            switch (sortBy)
            {
                case SortBy.NameAsc: query = queryNotOrdered.OrderBy(p => p.Name); break;
                case SortBy.NameDesc: query = queryNotOrdered.OrderByDescending(p => p.Name); break;
                case SortBy.AmountAsc: query = queryNotOrdered.OrderBy(p => p.Rest); break;
                case SortBy.AmountDesc: query = queryNotOrdered.OrderByDescending(p => p.Rest); break;
                case SortBy.DeliveryAsc: query = queryNotOrdered.OrderBy(p => p.DaysToDepartment); break;
                case SortBy.DeliveryDesc: query = queryNotOrdered.OrderByDescending(p => p.DaysToDepartment); break;
                case SortBy.PriceAsc: query = queryNotOrdered.OrderBy(p => p.PriceOfClient); break;
                case SortBy.PriceDesc: query = queryNotOrdered.OrderByDescending(p => p.PriceOfClient); break;
                default: query = queryNotOrdered.OrderBy(p => p.Name); break;

            }
            return query;
        }


        //
        // Подборы по авто (есть вложенная сортировка)
        //

        public static IOrderedEnumerable<DiskSearchResult> GetOrderedResultsThenBy(this IOrderedEnumerable<DiskSearchResult> queryOrdered, SortBy sortBy)
        {
         
            switch (sortBy)
            {
                case SortBy.NameAsc: return queryOrdered.ThenBy(p => p.Name); 
                case SortBy.NameDesc: return queryOrdered.ThenByDescending(p => p.Name); 
                case SortBy.AmountAsc: return queryOrdered.OrderBy(p => p.Rest); 
                case SortBy.AmountDesc: return queryOrdered.OrderByDescending(p => p.Rest); 
                case SortBy.DeliveryAsc: return queryOrdered.OrderBy(p => p.DaysToDepartment); 
                case SortBy.DeliveryDesc: return  queryOrdered.OrderByDescending(p => p.DaysToDepartment); 
                case SortBy.PriceAsc: return queryOrdered.OrderBy(p => p.PriceOfClient); 
                case SortBy.PriceDesc: return queryOrdered.OrderByDescending(p => p.PriceOfClient); 
                default: return queryOrdered.OrderBy(p => p.Name); 

            }
           
        }
    }
}