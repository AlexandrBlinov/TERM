using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;

namespace Term.Web.Services
{
    /// <summary>
    /// Класс определяет количество товаров на остатках (Rest)
    /// </summary>
    public class ProductOnRestsService
    {

          private readonly AppDbContext _dbContext ;
        public ProductOnRestsService( AppDbContext dbContext)
        {
            _dbContext=dbContext;
         

        }

        public ProductOnRestsService() : this(new AppDbContext()) { }

        
    }
}