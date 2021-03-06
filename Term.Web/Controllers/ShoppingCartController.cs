﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Yst.ViewModels;
using Yst.Context;
using Yst.Services;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.Collections;

using System.Threading.Tasks;
using YstProject.Services;
using System.Data.Entity;
using System.Xml;
using Term.Services;
using Term.Web.Views.Resources;
using Term.Soapmodels;
using Term.Utils;
using Term.Web.Services;
using YstProject.Models;


namespace Term.Web.Controllers
{
    /// <summary>
    /// Класс контроллера корзины
    /// </summary>
    public class ShoppingCartController : BaseController
    {
        
        private readonly DaysToDepartmentWithSuppliersService _daysGetterService;
        private readonly OrderService _orderService;
        private readonly CheckerCountExeedsRest _checkerCount;
        private readonly DeliveryCostCalculatorService _deliveryCostService;
        private readonly NotificationForUserService _notificationForUserService;


        public ShoppingCartController() : this(new DaysToDepartmentWithSuppliersService(), new OrderService(), 
            new CheckerCountExeedsRest(), new DeliveryCostCalculatorService(),
            new NotificationForUserService()
            ) { }

        public ShoppingCartController(DaysToDepartmentWithSuppliersService daysGetterService, OrderService orderService, 
            CheckerCountExeedsRest checkerCount, DeliveryCostCalculatorService deliveryCostService,
            NotificationForUserService notificationForUserService)
        {
            _daysGetterService = daysGetterService;
            _orderService = orderService;
            _checkerCount=checkerCount;
            _deliveryCostService = deliveryCostService;
            _notificationForUserService = notificationForUserService;
        }


        private void FillViewModel(ShoppingCartViewModelExtended vm)
        {
            bool isForeign = this.Partner.IsForeign;
            bool hasStar = this.Partner.HasStar;

            var partnerId = this.Partner.PartnerId;

            var cart = this.Cart;

            vm.CartItems = cart.GetCartItems();
            vm.CartTotal = cart.GetTotal();
            vm.CartTotalOfClient = cart.GetTotalOfClient();
            vm.CartCount = cart.GetCount();
            vm.TotalWeight = cart.GetCartWeight();
            vm.CanUserUseDpdDelivery = ServicePP.CanUserUseDpdDelivery;
            vm.AddressesIds = _orderService.AddressesOfDelivery;
            vm.TkIds = _orderService.TkIds;
            vm.IsPrepay = cart.IsPrepay || Partner.PrePay;
            // viewModel.IsStar = false;
            vm.HasStar = hasStar && !isForeign;

            vm.LogistikDepartment = !isForeign && !String.IsNullOrEmpty(Partner.LogistikDepartment) ? Partner.LogistikDepartment : null;

            vm.SelfDeliveryIds = _orderService.SelfDeliveryAddresses;

            // возможность сезонной отсрочки только у головных,  у которых есть признак HasSeasonAdjournment и , если в корзине цены НЕ Prepay 

            vm.HasSeasonAdjournment = Partner.HasSeasonAdjournment && ServicePP.IsPartner &&!vm.IsPrepay;

              
       //     vm.WayOfDelivery = this.Partner.WayOfDelivery;

        }

        /// <summary>
        /// Show shopping cart
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Index()
        {
            var cart = this.Cart;
            
            // у иностранных клиентов не показываем в резерв и на отгрузку
            
            ViewBag.IsForeign = Partner.IsForeign;

            var viewModel = new  ShoppingCartViewModelExtended();
            FillViewModel(viewModel);

            viewModel.IsStar = Partner.IsStar;
            viewModel.WayOfDelivery = Partner.WayOfDelivery;

            viewModel.AddressId = _orderService.GetDefaultAddressId(Partner.PartnerId,Point.PartnerPointId);

            // если хотя бы одна позиция в пути - не показываем  в резерв и на отгрузку
            bool hasItemsOnWay = viewModel.CartItems.Any(p => p.DepartmentId == 0);
            ViewBag.HasitemsOnWay = hasItemsOnWay;
            

            return View(viewModel);
        }

        /// <summary>
        /// Функция проверяет обязательные поля при доставке транспортной компанией (DPD)
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task CheckTransportFields(ShoppingCartViewModelExtended viewModel)
        {
            

                //ShoppingCartErrors.
                if (String.IsNullOrEmpty(viewModel.CityId)) ModelState.AddModelError("City", ShoppingCartErrors.EnterCityOfDelivery);

                if (!viewModel.TerminalOrAddress && String.IsNullOrEmpty(viewModel.Address)) ModelState.AddModelError("Address", ShoppingCartErrors.EnterAddressOfDelivery);


                if (String.IsNullOrEmpty(viewModel.ContactFIOOfClient)) ModelState.AddModelError("ContactFIOOfClient", ShoppingCartErrors.EnterFioOfClient);

                if (String.IsNullOrEmpty(viewModel.PhoneNumberOfClient)) ModelState.AddModelError("PhoneNumberOfClient", ShoppingCartErrors.EnterPhoneNumber);

              // если выбрана доставка до терминала
                if (viewModel.TerminalOrAddress && String.IsNullOrEmpty(viewModel.TerminalsDpd)) ModelState.AddModelError("TerminalsDpd", ShoppingCartErrors.ChooseTerminal);

                // если выбрана доставка до адреса
                if (!viewModel.TerminalOrAddress && (String.IsNullOrEmpty(viewModel.PostalCode) || String.IsNullOrEmpty(viewModel.StreetType)
                    || String.IsNullOrEmpty(viewModel.Street) || String.IsNullOrEmpty(viewModel.House))) ModelState.AddModelError("TerminalsDpd", ShoppingCartErrors.EnterCityAndAddressOfDelivery);


                if (!String.IsNullOrEmpty(viewModel.CityId))
                {
                    viewModel.CostOfDelivery = await _deliveryCostService.GetCostOfDelivery(viewModel.CityId, viewModel.TerminalOrAddress);
                    

                }
            
        }



        /// <summary>
        /// Create orders in remote 1S and local orders, separated by Storage = p.Department
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>


        [HttpPost]
        public async Task<ActionResult> CreateOrder(ShoppingCartViewModelExtended viewModel )
        {
            
            bool isForeign = this.Partner.IsForeign;
            bool hasStar = this.Partner.HasStar;


            string deliveryDataString=null;
            string rangeDeliveryDays = null;
            
            var costOfDeliveryByDepartments = new Dictionary<int, int>();

            FillViewModel(viewModel);
            var cart = this.Cart;


            viewModel.IsDeliveryByTk = viewModel.WayOfDelivery == (int)WaysOfDelivery.ByDpd && viewModel.IsDelivery;

            ViewBag.IsForeign = this.Partner.IsForeign;

            ViewBag.HasitemsOnWay = viewModel.CartItems.Any(p => p.DepartmentId == 0);
           // ViewBag.HasitemsOnWay = hasItemsOnWay;

            if (viewModel.CartCount == 0)    ModelState.AddModelError("", CartAndOrders.ErrorCartEmpty);

                       

            if (viewModel.DeliveryDate == null && viewModel.IsDelivery) ModelState.AddModelError("DeliveryDate", CartAndOrders.ErrorShipmentDate);


            // проверяем обязательное заполнение адреса если доставка наша и не 

            if (viewModel.IsDelivery && viewModel.WayOfDelivery == (int)WaysOfDelivery.Delivery && !isForeign && !ViewBag.HasitemsOnWay)
            {
                if (String.IsNullOrEmpty(viewModel.AddressId))      ModelState.AddModelError("AddressId", "Выберите адрес доставки");
            }


            // проверяем заполнение необходимых полей при доставке транспортной компанией
                if (viewModel.IsDeliveryByTk)
            { 
                await CheckTransportFields(viewModel);

                if (!ModelState.IsValid) return View("Index", viewModel);

                // Заполняем словарь costOfDeliveryByDepartments
                foreach (var depId in viewModel.CartItems.Where(p=>p.DepartmentId!=0).Select(p => p.DepartmentId).Distinct()) 
                costOfDeliveryByDepartments.Add(depId, await _deliveryCostService.GetCostOfDelivery(viewModel.CityId, viewModel.TerminalOrAddress,depId));
                
            }



            if (!ModelState.IsValid) return View("Index", viewModel);

            
           var pointId = base.Point.PartnerPointId;
            var partnerId = base.Point.PartnerId;


            bool successStatus = true;
            var result = new Result();

            //   Контроль превышения количества в сезонном заказе по товарам в пути
            var listOfErrors = _checkerCount.Check(viewModel.CartItems, ref successStatus);

            string available = CartAndOrders.Available;
            // проверка по заказам в пути            
            if (listOfErrors.Any())
            {
                available = SeasonOrdersTexts.QuantityOnWay;    
                result.Products = listOfErrors.ToArray();

                successStatus = false;
            }


           
            if (successStatus )
           {
               var productItems = viewModel.CartItems.Select(p => 
                   new SoapProduct { 
                   Code = p.Product.ToString(), 
                   Storage = p.DepartmentName, 
                   Quantity = p.Count,
                   SupplierId = p.SupplierId}).ToArray();

              

               var deliveryDateString = viewModel.DeliveryDate.HasValue ? ((DateTime)viewModel.DeliveryDate).ToString(ServiceTerminal.FormatForDate) : String.Empty;

              var deliveryDate2String = viewModel.DeliveryDate2.HasValue ? ((DateTime)viewModel.DeliveryDate2).ToString(ServiceTerminal.FormatForDate) : String.Empty;

                int caseForLogistik = viewModel.CaseForLogistik.HasValue ? (int)viewModel.CaseForLogistik : 0;
                // Если иностранный клиент, резерв или способ доставки , то адрес передаем пустой
                string addressId = isForeign || viewModel.WayOfDelivery > 0 || !viewModel.IsDelivery ? String.Empty : viewModel.AddressId;

                if (!viewModel.IsDeliveryByTk)
                    result = await Task.Run(
                    () => WS.CreateOrder3(
                    partnerId,
                    pointId,
                    productItems,
                    viewModel.Comments ?? String.Empty,
                    deliveryDateString,
                    !viewModel.IsDelivery,
                    viewModel.IsDeliveryByTk,
                    viewModel.IsPrepay,
                    viewModel.IsStar,
                    viewModel.WayOfDelivery,
                    addressId,
                    viewModel.TkId ?? String.Empty,
                    deliveryDate2String,
                   caseForLogistik,
                   viewModel.IsSeasonAdjournment,
                  (int) viewModel.DayOfWeekToDeliver
                    ));


                else
                {
                     DeliveryInfo di = FillInDeliveryInfo(viewModel);
                     deliveryDataString = await _deliveryCostService.PrepareDeliveryDataString(di);
                    
                     int minNumOfDays =  await _deliveryCostService.GetNumberOfDaysOfDeliveryAsync(di.CityId, 0);
                    int maxNumOfDays = await _deliveryCostService.GetNumberOfDaysOfDeliveryAsync(di.CityId,1);

                    rangeDeliveryDays = StringUtils.PrepareRangeDays(minNumOfDays, maxNumOfDays);

                   // нужна для рассчета даты получения товаров
                    var lastDay = Int32.Parse(StringUtils.GetLastDayFromRange(rangeDeliveryDays));
                    var dateToDeliverToEnd = DateTimeHelper.AddDaysWithoutDaysOff((DateTime)viewModel.DeliveryDate, lastDay);

                    result = await Task.Run(() => WS.CreateOrderWithTransportData(partnerId, pointId, productItems, viewModel.Comments ?? String.Empty, deliveryDateString, false, true, di, dateToDeliverToEnd.ToString(ServiceTerminal.FormatForDate)));
                }
              
            successStatus = result.Success;

            }

            
            // проверка по товарам на складах
            if (!successStatus)
            {
                if (String.IsNullOrEmpty(result.Error))
                {
                    var dictErrors = new Dictionary<string, string>();

                    foreach (var errorItem in result.Products)
                    {
                        dictErrors.Add(errorItem.Code.TrimStart('0'),
                            String.Format(available + " {0}", errorItem.Quantity > 0 ? errorItem.Quantity : 0));
                    }


                    viewModel.Errors = dictErrors;

                    ModelState.AddModelError("", CartAndOrders.ErrorAmountOnStock);
                }
                else
                
                    ModelState.AddModelError("", CartAndOrders.ShipmentNotAlolowed);
                
                
                return View("Index", viewModel);


            }
            else
            {
                Guid seasonOrderGuid;
                var orderGuids = cart.CreateOrdersInLocal(result, viewModel, out seasonOrderGuid, costOfDeliveryByDepartments, deliveryDataString, rangeDeliveryDays);

                var modelOrdersInView = new List<OrderViewWithDetails>();
                //var modelOrdersInView = orderGuids.Select(orderGuid => _orderService.GetOrderWithDetailsByGuid(Guid.Parse(orderGuid))).ToList();

                foreach (var orderGuid in orderGuids)
                {
                  var orderfound= await _orderService.GetOrderWithDetailsByGuidAsync(Guid.Parse(orderGuid));
                    modelOrdersInView.Add(orderfound);
                }

                // для сторонних поставщиков создаем уведомления
                foreach (var supplierId in modelOrdersInView.Where(p => p.Order.SupplierId > 0).Select(p=>p.Order.SupplierId).Distinct())
                {
                    var userName= _notificationForUserService.GetUserFromSupplierId(supplierId);

                    if (!String.IsNullOrEmpty(userName)) // Для автоматизируемых поставщиков не делаем уведомления
                    _notificationForUserService.AddIfNotExists(userName,"New orders received");

                }
                

                SeasonOrder seasonOrder =null;
                if (seasonOrderGuid!=Guid.Empty) seasonOrder =DbContext.Set<SeasonOrder>().Include(p=>p.OrderDetails).First(p=>p.OrderGuid==seasonOrderGuid);
                ViewBag.SeasonOrder = seasonOrder;
                    
              // после содания заказа кэш принудительно очищается  
             await   Task.Run(()=>CachedCollectionsService.RemoveCacheWheels(pointId.ToString()));
                
                return View("OrdersCreated", modelOrdersInView);


            }


        }

        /// <summary>
        /// подготовить информаию по доставке DPD
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private DeliveryInfo FillInDeliveryInfo(ShoppingCartViewModelExtended viewModel)
        {
            return new DeliveryInfo
            {
                ContactFio = viewModel.ContactFIOOfClient ?? "",
                ContactPhone = viewModel.PhoneNumberOfClient ?? "",
                RegionId = viewModel.RegionId ?? "",
                CityId = viewModel.CityId ?? "",
                CostOfDelivery = (int)viewModel.CostOfDelivery,
                TerminalOrAddress = viewModel.TerminalOrAddress,
                PostalCode = viewModel.PostalCode ?? "",
                StreetType = viewModel.StreetType ?? "",
                Street = viewModel.Street ?? "",
                BlockType = viewModel.BlockType ?? "",
                House = viewModel.House ?? "",
                TerminalCode = viewModel.TerminalsDpd ?? ""
            }; 
        }

       

        [HttpPost]
        public ActionResult UpdateToCart(int id, int count = 1)
        {
            bool success = true;
            string message = String.Empty;


            var cart = this.Cart;
            cart.UpdateItemCount(id, count);
            int cartCount = cart.GetCount();

            return Json(new { success, CartCount = cartCount, CartTotal = cart.GetTotal(), CartTotalOfClient = cart.GetTotalOfClient(),TotalWeight=cart.GetCartWeight() });

        }

        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        /// <param name="id">productId товара</param>
        /// <param name="departmentId">подразделение</param>
        /// <param name="supplierId">поставщик</param>
        /// <param name="count">количество</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult AddToCart(int id, int departmentId, int supplierId, int count = 1)
        {
            string message=null;

            int pointId = this.Point.PartnerPointId;
            Partner partner = this.Point.Partner;
            string partnerId = partner.PartnerId;
          



            if (count < 0 || count > Defaults.MaxNumberAddToCart) message = ShoppingCartErrors.IncorrectNumberAddedToCart;
            
            if (String.IsNullOrEmpty(partnerId)) message = ShoppingCartErrors.PartnerIdentifierNotAssigned;

            var addedProduct = Products.GetProduct(id);
            if (addedProduct == null) message = ShoppingCartErrors.CantFindProductForThisCode;

            
            

            var cart = this.Cart;

            var usePrepay = partner.UsePrepayPrices;

            if (cart.GetCount()>0)
            { 
           

            // если есть товары в корзине с другим Prepay то ругаемся
            
            bool isPrepay= cart.IsPrepay;
            if (isPrepay != usePrepay)
            {
                    
                var priceType = isPrepay ? CartAndOrders.Prepay : CartAndOrders.Adjournment;
                message = "Нельзя добавлять в корзину товары с разными типами цен (предоплата и отсрочка). В корзине уже есть товары с типом цен "+ priceType;
                message+=". Оформите заказ или вернуте тип цен "+ priceType;
            }

            }
            if (message != null)
                return Json(new 
                {   Message = message,
                    Success = false
                });


            var price = Products.GetPriceOfProduct(id, partnerId);

            var priceOfPoint = Products.GetPriceOfPoint(id, pointId, partnerId);

            var priceOfClient = Products.GetPriceOfClient(id, pointId, partnerId);

            if (addedProduct.ProductType == ProductType.Other)
            {
                priceOfPoint = price;
                priceOfClient = price;
            }

            // есть товары в резерве под покупателя - значит берем с головного склада
          //  bool existsOnRestsOfPartner = _checkerCount.CheckIfExistsRestOfPartner(partnerId, id);
          //  if (existsOnRestsOfPartner) departmentId = Defaults.MainDepartment;
            

            // updated by Lapenkov 2016-06-22
            int days = _daysGetterService.GetDaysToDepartment(pointId, departmentId,supplierId,id);

                       


            // проверка, если не сезонный ассортимент и нет на текущем складе, то 
            // если 2 склада и запрашивается количество больше чем на текущем складе, и меньше чем на доп. складе, то
            // берем с доп. склада
            if (departmentId != 0 && supplierId == 0 /* && !existsOnRestsOfPartner */)
            {
             var departmentsWithRests   =_daysGetterService.GetDepartmentsWithRests(pointId, id);

                // IDictionary <int, >
              var dictRests=  departmentsWithRests.ToDictionary(dr => dr.DepartmentId, dr => dr);

                // количество складов = 2, и запрашиваемое количество больше чем есть на текущем складе
                if (dictRests.Count > 1 && count > dictRests[departmentId].Rest)

                {
                    var recordOtherStock=dictRests.First(p => p.Key != departmentId).Value;

                    // остатка хватает на другом складе , меняем подразделение и дни
                    if (recordOtherStock.Rest >= count)
                    {
                        days = recordOtherStock.Days;
                        departmentId = recordOtherStock.DepartmentId;

                    }
                }

            }

            cart.ItemAddedToCart += cart_ItemAddedToCart;

            cart.AddToCart(addedProduct, departmentId, days, ref count, price, priceOfPoint, priceOfClient, supplierId, usePrepay);


            

            message = $"{ForSearchResult.MsgAddToCart1} {addedProduct.Name} {ForSearchResult.MsgAddToCart2} {count} {ForSearchResult.MsgAddToCart3}";

            var results = new ShoppingCartAddRemoveViewModel
            {
                Message = message,
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                CartTotalOfClient = cart.GetTotalOfClient(),
                TotalWeight = cart.GetCartWeight(),
                Success = true
            };
            return Json(results);
        }

        /// <summary>
        /// обработчик события при добавлении в корзину - фиксируем в базе даных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cart_ItemAddedToCart(object sender, ShoppingCartInfoArgs e)
        {
            DbContext.CartActionLogs.Add(new CartActionLog()
            {  UserName = e.Username,
                ProductId = e.Product.ProductId,
                Action = CartEventType.Added,
                Price = e.Price,
                PriceOfClient = e.PriceOfClient,
                PriceOfPoint = e.PriceOfPoint
            });
            DbContext.SaveChanges();
            
        }

        /// <summary>
        /// Удалить из корзины
        /// </summary>
        /// <param name="id">productId товара</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {

            string message = null;
            bool success = true;

            var removedProduct = Products.GetProduct(id);

            var cart = this.Cart;

            int itemCount = 0;

            if (removedProduct != null)
            {
                itemCount = cart.RemoveFromCart(id);
                message = Server.HtmlEncode(removedProduct.Name) + " " + ForSearchResult.MsgDelFromCart1;
            }
            else
            {
                message = ForSearchResult.MsgDelFromCart2;
                success = false;
            }

            var results = new ShoppingCartAddRemoveViewModel
            {
                Message = message,
                CartTotal = cart.GetTotal(),
                CartTotalOfClient = cart.GetTotalOfClient(),
                CartCount = cart.GetCount(),
                TotalWeight = cart.GetCartWeight(),
                Success = success
            };
            return Json(results);

        }

        [HttpPost]
        [ActionName("uploadexcel")]
        public async Task<ActionResult> UploadExcel(HttpPostedFileBase file)
        {
            ViewBag.IsForeign = ServicePP.CurrentPoint.Partner.IsForeign;
            var cart = this.Cart;
            int PointId = ServicePP.getPointID();
            string PartnerId = ServicePP.GetPartnerIdByPointId(PointId);
            bool addStock = ServicePP.CanPartnerUseAdditionalStock();
            ViewBag.AddStock = addStock;
            ViewBag.HasitemsOnWay = false;
           
            FileUploaderService _fileUploader = DependencyResolver.Current.GetService<FileUploaderService>();

            try
            {
                _fileUploader.SaveFileFromUpload(file, "xlsx");

            }
            catch (FileOperationErrorException exc)
            {
                ModelState.AddModelError("", exc.Message);
                return View("Index");

            }

            // 2. Parse file

            try
            {
                var dictionary = ExcelParser.ParseTwoColumns(_fileUploader.FullPathToFile);
                foreach (var entry in dictionary)
                {
                    var addedProduct = Products.GetProduct(entry.Key);

                    if (addedProduct == null)
                    {
                        ViewBag.Error += "Товар с кодом " + entry.Key + " не найден!";
                    }
                    else
                    {
                        var price = Products.GetPriceOfProduct(entry.Key, PartnerId);

                        var priceOfPoint = Products.GetPriceOfPoint(entry.Key, PointId, PartnerId);

                        var priceOfClient = Products.GetPriceOfClient(entry.Key, PointId, PartnerId);
                                                
                        var departmentId = Defaults.MainDepartment;
                        int count = entry.Value;
                        int days = ServicePP.GetDaysToDepartment(PointId, departmentId);

                        cart.AddToCart(addedProduct, departmentId, days, ref count, price, priceOfPoint, priceOfClient);
                    }
                }

                //_cart.AddFromDictionary(dictionary);
            }

            catch (Exception exc)
            {
                ModelState.AddModelError("", exc.Message);
                return View("Index");
            }

            try
            {
                _fileUploader.DeleteUploadedFile();

            }
            catch { }

            var viewModel = new ShoppingCartViewModelExtended
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal(),
                CartTotalOfClient = cart.GetTotalOfClient(),
                CartCount = cart.GetCount(),
                TotalWeight = cart.GetCartWeight(),
                IsDelivery = false,
                IsDeliveryByTk = false,
                TkIds = _orderService.TkIds
        };


            return View("Index", viewModel);
        }

      //  [ChildActionOnly]
        public ActionResult CartSummary()
        {
           
            var cart = this.Cart;

            ViewData["CartCount"] = cart.GetCount();
            ViewData["CartTotal"] = cart.GetTotal();

            return PartialView("CartSummary");

        }


    }
}
