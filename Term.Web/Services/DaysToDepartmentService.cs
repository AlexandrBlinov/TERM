using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Yst.Context;
using Term.DAL;

namespace YstProject.Services
{


    public struct DepartmentInfo
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; }
        public int Days { get; set; }
    }

    /// <summary>
    /// Data transfer object for products on way
    /// </summary>
   public  struct NumberOfDaysWithCount {
    public int NumberOfDays { get; set; }
    public string Count { get; set; }
    }

    /// <summary>
    /// Get department, days to it and rest on it for partner point
    /// </summary>
    public class DepartmentWithRests
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int Rest { get; set; }
        public int Days { get; set; }
    }

    /// <summary>
    /// Класс для определения количества дней доставки
    /// </summary>
    public class DaysToDepartmentService 
    {
        
    private static int MaxDaysFromPortToStock  =7;
    private static int MaxDaysFromProductionToStock = 45;
    private static int DefaultNumberOfDays = 1;
    private static int MaxItemsToShowOnWays = 100;
        

        protected readonly AppDbContext _dbContext ;
        public DaysToDepartmentService( AppDbContext dbContext)
        {
            _dbContext=dbContext;
         

        }

        public DaysToDepartmentService() : this(new AppDbContext()) { }



        private static Func<OnWayItem, DateTime> selector3(int days)
        {
            return item =>
            {
                DateTime dateOfArrivalToStock = item.DateOfArrival.AddDays(item.ProdOrWay == ProdOrWay.InProduction ? MaxDaysFromProductionToStock : MaxDaysFromPortToStock);


                dateOfArrivalToStock = dateOfArrivalToStock < DateTime.Now ? DateTime.Now : dateOfArrivalToStock;


                DateTime dateToDepartment = dateOfArrivalToStock.AddDays(days);

                return dateToDepartment;
            };
        }

        private static Func<OnWayItem, NumberOfDaysWithCount> selector4(int days)
        {
            return item =>
            {
                DateTime dateOfArrivalToStock = item.DateOfArrival.AddDays(item.ProdOrWay == ProdOrWay.InProduction ? MaxDaysFromProductionToStock : MaxDaysFromPortToStock);


                dateOfArrivalToStock = dateOfArrivalToStock < DateTime.Now ? DateTime.Now : dateOfArrivalToStock;

                
                DateTime dateToDepartment = dateOfArrivalToStock.AddDays(days);

                var count_str = item.Count > MaxItemsToShowOnWays ? "> 100" : item.Count.ToString();

                return new NumberOfDaysWithCount { NumberOfDays = (dateToDepartment - DateTime.Now.Date).Days, Count=count_str };
            };
        }

       
        /// <summary>
        /// Возвращает коллекцию записей для точки число дней и количество
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="productid"></param>
        /// <returns></returns>

       public  IEnumerable<NumberOfDaysWithCount> GetDaysProductFromOnWays(int pointId, int productid)
        {
            PartnerPoint pp = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);

        var listOfItems = _dbContext.Set<OnWayItem>().Where(p => p.ProductId == productid).Select(selector4(pp.DaysToMainDepartment)).AsEnumerable();

        return listOfItems;
        
        }
       
        
        /// <summary>
       /// // вычислить число дней до подразделения c учетом сезонных заказов
        /// </summary>
        /// <param name="pointId">код точки</param>
        /// <param name="departmentId">код подразделения</param>
        /// <param name="productid">код товара (не передается для сезонного заказа)</param>
        /// <returns></returns>
        public virtual int GetDaysToDepartment(int pointId, int departmentId,int productid=0)
        {   

           var pp = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);
            

            // if from stock
            if (departmentId!=0)  return departmentId == Defaults.MainDepartment ? pp.DaysToMainDepartment : pp.DaysToDepartment;

            // if from on way
            var listOfItems = _dbContext.Set<OnWayItem>().Where(p => p.ProductId == productid).AsEnumerable();
           
            if (listOfItems.Any())
            {
                DateTime maxDateOfArrival = listOfItems.Select(selector3(pp.DaysToMainDepartment)).Max();
         
                
           int result = (maxDateOfArrival.Date - DateTime.Now.Date).Days;

           return result>0 ? result:DefaultNumberOfDays;

                }
            //else 
                return DefaultNumberOfDays;
        }


        /// <summary>
        /// В карточке товара определяет число дней доставки до этого товара и подразделение
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="productId"></param>
        /// <param name="days"></param>
        /// <returns></returns>
        public int GetDepartmentIdForProduct(int pointId, int productId, ref int days)
        {
        //    Department dep_result = null;
            int dep_result = days= -1;
            var mainDepartment = _dbContext.Departments.First(p => p.DepartmentId == Defaults.MainDepartment);
            var point = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);

            var departmentId = point.DepartmentId ?? -1;
            if (departmentId > 0 && point.DaysToDepartment > 0)
            {
                var availableDepartments = _dbContext.Rests.Where(p => p.ProductId == productId && (p.DepartmentId == departmentId || p.DepartmentId == Defaults.MainDepartment)).Select(p => p.DepartmentId).ToList();

                // товар есть фактически в 2-х подразделениях
                if (availableDepartments.Count > 1)
                {
                    if (point.DaysToDepartment > point.DaysToMainDepartment)
                    {
                        days = point.DaysToMainDepartment;
                        dep_result= mainDepartment.DepartmentId;
                        return dep_result;
                      //  dep_result= mainDepartment;

                        //return mainDepartment;

                    }
                    else
                    {
                        days = point.DaysToDepartment;
                        dep_result= departmentId;
                        return dep_result;
                        //return _dbContext.Departments.First(p => p.DepartmentId == DepartmentId)
                        
                    }
                }
                    // товар есть в одном подразделении
                else if (availableDepartments.Count == 1)
                {
                    var oneDepartmentFound = availableDepartments.First();
                    if (oneDepartmentFound == Defaults.MainDepartment)
                    {
                        days = point.DaysToMainDepartment;
                        return Defaults.MainDepartment; // dep_result = mainDepartment;
                    }
                    else
                    {
                        days = point.DaysToDepartment;
                        return departmentId;
                        //dep_result = _dbContext.Departments.First(p => p.DepartmentId == DepartmentId);
                    }
                }
                    // товара нет
                /*else
                {
                    days = -1;
                    dep_result = -1;
                
                } */

            }else
            {
                // в головном подразделении есть остатки
                bool hasRests = _dbContext.Rests.Any(p => p.ProductId == productId && p.DepartmentId == Defaults.MainDepartment);
                if (hasRests)
                {
                    days = point.DaysToMainDepartment;
                    dep_result = Defaults.MainDepartment;
                }
                // в головном подразделении нет остатков
                /*else
                {
                    days = -1;
                    dep_result = null;
                } */

            }

           
            return dep_result;
          

        }



        /// <summary>
        /// В карточке товара возвращает подразделения точки с остатками товара 
        /// </summary>
        /// <param name="pointId">Id точки</param>
        /// <param name="productId">Id товара</param>
        /// <returns>Коллекцию подразделений с товарами </returns>
        public IEnumerable<DepartmentWithRests> GetDepartmentsWithRests(int pointId, int productId)
        {
         //   var mainDepartment = _dbContext.Departments.First(p => p.DepartmentId == Defaults.MainDepartment);
            var point = _dbContext.PartnerPoints.FirstOrDefault(p => p.PartnerPointId == pointId);
            var partnerId = point.PartnerId;

            
            var departmentId = point.DepartmentId ?? -1;

            var availableDepIds = new List<int> {Defaults.MainDepartment};

            if (departmentId > 0) availableDepIds.Add(departmentId);

            // сюда надо дабавить резерв покупателя
            var restsavailable =_dbContext.Rests.Where(p => p.ProductId == productId && availableDepIds.Contains(p.DepartmentId));
                        

                var results=from rests in restsavailable
                join deps in _dbContext.Departments on rests.DepartmentId equals deps.DepartmentId
                orderby rests.DepartmentId
                select
                    new DepartmentWithRests
                    {
                        DepartmentId = deps.DepartmentId,
                        DepartmentName = deps.Name,
                        Rest = rests.Rest,
                        Days = deps.DepartmentId == Defaults.MainDepartment ?point.DaysToMainDepartment :point.DaysToDepartment
                    };
                
                return results.AsEnumerable();
            }


        /// <summary>
        /// Возвращает подразделения и сроки доставки 
        /// </summary>
        /// <param name="pointId"></param>
        /// <returns></returns>
        public DepartmentInfo GetDepartmentInfoWithMaxDaysForPoint(int pointId)
        {
            var mainDepartment=_dbContext.Set<Department>().FirstOrDefault(p => p.DepartmentId == Defaults.MainDepartment);

            var point = _dbContext.Set<PartnerPoint>().FirstOrDefault(p => p.PartnerPointId == pointId);
            
           IList<DepartmentInfo> list = new List<DepartmentInfo>(); 
            var mainDepartmentInfo= new DepartmentInfo{DepartmentId = mainDepartment.DepartmentId,Name = mainDepartment.Name,Days = point.DaysToMainDepartment};

            if (point.DepartmentId.HasValue && point.DepartmentId > 0)
            {
                var addDepartment = _dbContext.Set<Department>().FirstOrDefault(p => p.DepartmentId == (int)point.DepartmentId);
                var addDepartmentInfo = new DepartmentInfo { DepartmentId = (int)point.DepartmentId, Name = addDepartment.Name, Days = point.DaysToDepartment };
              return addDepartmentInfo.Days > mainDepartmentInfo.Days ? addDepartmentInfo : mainDepartmentInfo;
            } else return mainDepartmentInfo;
            
        }

    }

    /// <summary>
    /// Определяет число дней доставки с учетом сторонних поставщиков
    /// </summary>
    public class DaysToDepartmentWithSuppliersService: DaysToDepartmentService
    {

        public DaysToDepartmentWithSuppliersService(AppDbContext dbContext) : base(dbContext)
        {
        }

        public DaysToDepartmentWithSuppliersService()
        {
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="departmentId"></param>
        /// <param name="supplierId">Не ноль, если добавляется товар с привязкой к поставщику</param>
        /// <param name="productid"></param>
        /// <returns></returns>
        public  int GetDaysToDepartment(int pointId, int departmentId, int? supplierId, int productid = 0)
        {
             var result =base.GetDaysToDepartment(pointId, departmentId, productid);

            return result + GetDaysFromSupplierToMainDepartment( supplierId??0);
            if(supplierId==null || (int)supplierId==0) return result;
            
            
             var supplierFound =_dbContext.Suppliers.Find((int) supplierId);
            if (supplierFound != null) return supplierFound.Days + result;
            return result;

        }

        /// <summary>
        /// Возвращает число дней доставки от поставщика до головного подразделения
        /// </summary>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        public int GetDaysFromSupplierToMainDepartment(int supplierId)
        {
            if (supplierId == 0) return 0;
            var supplierFound = _dbContext.Suppliers.Find(supplierId);
            return supplierFound != null?  supplierFound.Days :0;
           
        }
    }




}