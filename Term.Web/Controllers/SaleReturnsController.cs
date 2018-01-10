using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using PagedList;
using Term.DAL;
using Term.Services;
using Term.Soapmodels;
using YstProject.Models;
using Term.Utils;
using Term.Web.Models;
using Yst.ViewModels;
using YstProject.Services;

namespace Term.Web.Controllers
{

    
    /// <summary>
    /// Возрат товаров от покупателя
    /// </summary>
    public class SaleReturnsController : BaseController
    {
      public  class SaleDataForReturnDto
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
            public string SaleNumber { get; set; }
            public DateTime? SaleDate { get; set; }
            public bool SaleFound { get; set; }

        }
        /// <summary>
        /// Функция для ввода данных клиентом
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {

            return View(new PurchaseReturnDto());
        }

        /// <summary>
        /// Обработка формы от клиента
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> Index(PurchaseReturnDto model)
        {
            if (Request.IsAjaxRequest()) return PartialView("_ReturnItems", model);


            int daysAllowedToReturn = base.Partner.NumberOfDaysForReturn ?? Defaults.DaysAllowedToReturn;
            // 1.Проверка данных периода

            for (int i = 0; i < model.Items.Count(); i++)
            {

                ModelState.AddModelErrorIf<PurchaseReturnDto, DateTime>(
                    x => x.Items[i].SaleDate,
                    x => (DateTime.Now - x.Items[i].SaleDate).Days > daysAllowedToReturn,
                    model,
                   String.Format( "Возврат не может превышать {0} дней с момента отгрузки",daysAllowedToReturn)
                    );

                ModelState.AddModelErrorIf<PurchaseReturnDto, DateTime>(
                    x => x.Items[i].SaleDate,
                    x => (DateTime.Now.Date < x.Items[i].SaleDate.Date),
                    model,
                    "Вы ввели будущую дату, отгрузка еще не существует"
                    );

            }

            if (ModelState.IsValid)
            {

                // 2. Вызов веб-сервиса
                int rowNumber = 0;
                var paramProducts = model.Items.Select(
                        p =>
                            new ProductPurchaseReturn
                            {
                                RowNumber = ++rowNumber,
                                Code = p.ProductIdTo7Simbols,
                                SaleDate = p.SaleDate.ToString(ServiceTerminal.FormatForDate),
                                SaleNumber = p.SaleNumber,
                                Count = p.Count.Value
                            }).ToArray();

                var result = base.WS.CreatePurchaseReturn(base.Partner.PartnerId, base.Point.PartnerPointId, paramProducts,false);


                // 3. Обработка результатов запроса

                if (result.Success)
                {
                     rowNumber = 0;
                    var dictPriceByRowNumber = result.Products.ToDictionary(x => x.RowNumber, x => x.Price);   
                    
                    // создание документа в базе
                    Guid newGuid = Guid.Parse(result.PurchaseReturnGUID);
                    var saleReturn = new SaleReturn
                    {
                        GuidIn1S = newGuid,
                        NumberIn1S = result.PurchaseReturnNumber,
                        PartnerId = base.Partner.PartnerId,
                        PointId = base.Point.PartnerPointId,
                        DocDate = DateTime.Now,
                        SaleReturnDetails =
                            model.Items.Select(
                                p =>
                                    new SaleReturnDetail
                                    {
                                        GuidIn1S = newGuid,
                                        SaleDate = p.SaleDate,

                                        SaleNumber = p.SaleNumber,
                                        Count = p.Count.Value,
                                        RowNumber = ++rowNumber,
                                        Price = dictPriceByRowNumber.ContainsKey(rowNumber)? (decimal) dictPriceByRowNumber[rowNumber] : 0,
                                        ProductId = p.ProductId.Value,
                                    }).ToList()

                    };
                    DbContext.SaleReturns.Add(saleReturn);
                    await DbContext.SaveChangesAsync();

                    TempData["isNewSaleReturn"] = true;

                    return RedirectToAction("Details", "SaleReturns", new {guid = newGuid});
                }
                else
                {
                 
                    // сперва проверяем ошибку на корневом уровне

                    if (!String.IsNullOrEmpty(result.ErrorDescription))
                    {
                        ModelState.AddModelError("",result.ErrorDescription);
                    }

                    
                    // проверка на уровне таб. части

                    var   errors = result.Products.ToArray(); 
                    
                    for (int i = 0; i < model.Items.Count(); i++)
                    {
                       var errorFound=errors.FirstOrDefault(p => p.RowNumber == i + 1);

                        if(errorFound!=null)

                            switch (errorFound.Error)
                            {
                                //1. Не найдена номенклатура по коду
                                case 1:
                                {
                                    ModelState.AddModelError<PurchaseReturnDto, int?>(
                                     x => x.Items[i].ProductId,
                                        errorFound.ErrorDescription
                                    );
                                    break;
                                }
                                //2. Не удалось найти реализацию
                                case 2:
                                {
                                    ModelState.AddModelError<PurchaseReturnDto, string>(
                                    x => x.Items[i].SaleNumber,
                                       errorFound.ErrorDescription
                                   );
                                    break;
                                }
                                  //  3. Возвращается больше допустимого количества
                                case 3:
                                {
                                    ModelState.AddModelError<PurchaseReturnDto, int?>(
                                    x => x.Items[i].Count,
                                       errorFound.ErrorDescription
                                   );
                                    break;
                                }
                                //  4. Превышен срок возврата по дискам.
                                default:
                                {
                                    ModelState.AddModelError<PurchaseReturnDto, DateTime>(
                                    x => x.Items[i].SaleDate,
                                       errorFound.ErrorDescription
                                   );
                                    break;
                                }
                            }
                        

                    }
                }
            {
                }

            }

            return View(model);

        }

        public async Task<ActionResult> Details(Guid guid)
        {

           // ViewBag.is
            var doc = await DbContext.SaleReturns.FirstOrDefaultAsync(o => o.GuidIn1S == guid);

            if (doc == null) return HttpNotFound("Sale Return Not Found");
            return View(doc);
        }

        /// <summary>
        /// Возвращает список имеющихся заявок на возврат
        /// </summary>
        /// <returns></returns>
        public ActionResult List(SaleReturnViewModel model )
        {
            var endDate = model.EndDate.HasValue ? ((DateTime)model.EndDate).AddDays(1).AddTicks(-1) : DateTime.MaxValue;
            var partnerId = base.Partner.PartnerId;
            model.SaleReturns =
               
                    DbContext.SaleReturns.Include(p => p.SaleReturnDetails)
                        .Where(o => o.PartnerId == partnerId &&
                        (model.BeginDate == null || o.DocDate >= model.BeginDate)
                && (model.EndDate == null || o.DocDate <= endDate)
                && (model.NumberIn1S == null || o.NumberIn1S.Contains(model.NumberIn1S.ToString()))
            && (model.ProductId == null || o.SaleReturnDetails.Any(p => p.ProductId.ToString().Contains(model.ProductId)))
            && (model.SaleNumber == null || o.SaleReturnDetails.Any(p => p.SaleNumber.Contains(model.SaleNumber)))
                )
                        .Select(p => new SaleReturnDto
                        {
                            GuidIn1S = p.GuidIn1S,
                            NumberIn1S = p.NumberIn1S,
                            Count = p.SaleReturnDetails.Sum(i => i.Count),
                            Sum = p.SaleReturnDetails.Sum(i => i.Count*i.Price),
                            DocDate = p.DocDate,
                            PartnerId = p.PartnerId,
                            PointId = p.PointId
                        }).OrderByDescending(p => p.DocDate).ToPagedList(model.Page, model.ItemsPerPage);
            

            return View(model);
        }

        /// <summary>
        /// Печать задания на возврат
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Print(Guid guid)
        {
            
            ViewBag.AskToReturnCaption = String.Format(" Прошу отпустить со склада {0} ИНН {1} следующие позиции", base.Partner.FullName, base.Partner.INN);

            var doc = await DbContext.SaleReturns.FirstOrDefaultAsync(o => o.GuidIn1S == guid);

            if (doc == null) return HttpNotFound("Sale Return Not Found");
            return View(doc);
        }

       
        public /*async Task<JsonResult> */ JsonResult GetSaleDataFromParam(SaleDataForReturnDto model)
        {
            var partner = base.Partner;
            var saleDate = model.SaleDate ?? null;
            var sale=DbContext.Sales.Include(s => s.SaleDetails).Where(s => s.PartnerId == partner.PartnerId  
            
            && ( model.SaleNumber==null ||  s.NumberIn1S.Contains(model.SaleNumber))
            && (saleDate == null || s.SaleDate== saleDate)
            && s.SaleDetails.Any(p => model.ProductId == p.ProductId)).OrderByDescending(p => p.SaleDate).FirstOrDefault();

            if (sale == null) return Json(model, JsonRequestBehavior.AllowGet);

            var product = DbContext.Products.FirstOrDefault(p => p.ProductId == model.ProductId);



            var result = new {
                ProductId = model.ProductId,
                ProductName = product == null ? null : product.Name,
                    SaleDate = sale.SaleDate.ToShortDateString(),
                    SaleNumber = sale.NumberIn1S,
                    SaleFound = true,
                    
            };

                               
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        

    }
}
