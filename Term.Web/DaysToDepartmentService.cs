using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Yst.Context;
using YstStore.Domain.Models;

namespace YstProject.Services
{
    /// <summary>
    /// Класс для определения количества дней доставки
    /// </summary>
    public class DaysToDepartmentService 
    {
        
    private static int MaxDaysFromPortToStock  =7;
    private static int MaxDaysFromProductionToStock = 45;

        private readonly AppDbContext _dbContext ;
        public DaysToDepartmentService( AppDbContext dbContext)
        {
            _dbContext=dbContext;
        }

        public DaysToDepartmentService() : this(new AppDbContext()) { }


        // вычислить число дней до подразделения

        virtual public int GetDaysToDepartment(int pointId, int departmentId,int productid)
        {

            // use DbFunctions

           PartnerPoint pp = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);

            // if from stock
            if (departmentId!=0)  return departmentId == Defaults.MainDepartment ? pp.DaysToMainDepartment : pp.DaysToDepartment;

            // if from on way
            var listOfItems = _dbContext.Set<OnWayItem>().Where(p => p.ProductId == productid).AsEnumerable();
           
           DateTime maxDateOfArrival =listOfItems.Select(p => p.ProdOrWay == ProdOrWay.InProduction ? p.DateOfArrival.AddDays(MaxDaysFromProductionToStock) : p.DateOfArrival.AddDays(MaxDaysFromPortToStock)).Max();


           int result = (maxDateOfArrival.Date - DateTime.Now.Date).Days;

           return result>0 ? result:0;
            

        }


       


    }
}