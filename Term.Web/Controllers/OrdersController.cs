using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Term.DAL;
using Yst.ViewModels;
//using YstProject.WebReferenceTerm;
using YstTerm.Models;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Term.Services;
using Term.Web.Views.Resources;
using YstProject.Services;
using Term.Soapmodels;
using Term.Utils;
using Term.Web.Models;
using System.Data.Entity;

namespace Term.Web.Controllers
{
    [Authorize]
    [CheckSettings]
    public class OrdersController : BaseController
    {
        private bool _isPartner;
        private string errorMessage;
        private static readonly int maxDaysToChangeOrder = Defaults.MaxDaysToChangeOrder;
       
        readonly Func<PartnerPoint, string> _propertyToDisplay = p => p.InternalName ?? String.Empty;

        private readonly OrderService _orderService;
        private readonly DeliveryCostCalculatorService _deliveryCostService;



        public OrdersController() : this(new OrderService(),new DeliveryCostCalculatorService()) { }
       

        public OrdersController(OrderService orderService,DeliveryCostCalculatorService deliveryCostService)
        {
            _orderService = orderService;
            _deliveryCostService = deliveryCostService;
        }


        private  ActionResult ReturnModelErrorsAsJson()
        {
            var errorList = ModelState
           .Where(x => x.Value.Errors.Count > 0)
           .ToDictionary(
               kvp => kvp.Key,
               kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
           );
            return Json(
           new
           { Success = false, Element = errorList.First().Key, Text = errorList.First().Value[0] });
        }

        private void InitiateViewBag()
        {
            _isPartner = ServicePP.IsPartner;
            ViewBag.IsPartner = _isPartner;
            ViewBag.IsForeign = Partner.IsForeign;
        }

        //
        // FOR USER TO SEE OWN ORDERS
       //[CheckPassThroughKeyword]

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult List(OrdersViewModelForList model)
        {
            InitiateViewBag();
           
          
            ViewBag.CurrentPointId = base.Point.PartnerPointId;

            model.AddressesIds = _orderService.AddressesOfDelivery;
            model.TkIds = _orderService.TkIds;
            model.SelfDeliveryIds = _orderService.SelfDeliveryAddresses;
            model.AddressId = _orderService.GetDefaultAddressId(Partner.PartnerId, Point.PartnerPointId);
            model.LogistikDepartment = !Partner.IsForeign && !String.IsNullOrEmpty(Partner.LogistikDepartment) ? Partner.LogistikDepartment : null;

            if (_isPartner) // PartnerId is set too
                   {
                       model.PartnerPoints = ServicePP.GetPointNamesByPartner(base.Partner.PartnerId, _propertyToDisplay);
                      _orderService.GetListOfOrdersByPartnerId(model, base.Partner.PartnerId, ViewBag.CurrentPointId);

                   }
                   else
                   {
                       model.PartnerPoints = null;
                       if (base.Point.PartnerPointId > 0)  _orderService.GetListOfOrdersByPointId(model, ViewBag.CurrentPointId);
                   }
                   return View(model); 


      
        }


        /// <summary>
        /// Заказ в режиме просмотра
        /// </summary>
        /// <param name="guid">идентификатов заказа 1С</param>
        /// <returns></returns>

        public async Task<ActionResult> Details(Guid guid)
        {
            InitiateViewBag();

            
                ViewBag.OrderInfo = String.Empty;
                if (TempData["neworder"] != null) ViewBag.OrderInfo = CartAndOrders.NewOrderCreated;

               

               var order =  await  _orderService.GetOrderByGuidAsync(guid);
                ViewBag.OrderStatus = order.OrderStatus;
            
            
                if (_orderService.CheckIfCanCancelOrder(order, out errorMessage)) ViewBag.DisplayEditButton = true;


                OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);
               model.HistoryOfOrderStatuses= await _orderService.GetHistoryOfOrderStatusesAsync(guid);
                model.CanUserChangeDpdOrder = true;
                if (model.Order.IsDeliveryByTk)
                {
                    var date = model.Order.DeliveryDate.Value.AddHours(14);
                    model.CanUserChangeDpdOrder = date > DateTime.Now ? true : false;
                }
                ViewBag.SaleIsReady = _orderService.GetSaleNumberByOrderGuid(order.GuidIn1S) != String.Empty ? true : false;

            // заполняем склад для подтверждения
            if (!Partner.IsForeign) model.DepartmentName = order.Department?.Name;


            // Если доставка транспортной компанией, то считаем дату планируемой доставки
            if (model.Order.IsDeliveryByTk && !String.IsNullOrEmpty(model.Order.RangeDeliveryDays) && model.Order.DeliveryDate.HasValue)
            {
                var lastDay =Int32.Parse(StringUtils.GetLastDayFromRange(model.Order.RangeDeliveryDays));

              ViewBag.SupposedDateOfDelivery = DateTimeHelper.AddDaysWithoutDaysOff((DateTime) model.Order.DeliveryDate, lastDay);

            }
            
           ViewBag.Title = $"{CartAndOrders.Order}   {model.Order.NumberIn1S}";
                 return  View("Details", model);
               
            

        }

     
        


        

        /// <summary>
        /// Счет на оплату по заказу
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public async Task<ActionResult> Bill(Guid guid,bool? showPicture)
        {
            //  _isPartner = ServicePP.IsPartner;
            InitiateViewBag();
            Order order = await _orderService.GetOrderByGuidAsync(guid);

            if (_orderService.CheckIfCanCancelOrder(order, out errorMessage))
            {
                ViewBag.DisplayEditButton = true;
            }
            
            if (base.Partner == null) throw new NullReferenceException("Partner not found in db");

            ViewBag.PartnerInfo = base.Partner.ToString();
            ViewBag.ShowPicture = showPicture ?? false;
            
            OrderViewWithDetails model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);
           

            if (model != null)
            {
                return View(model);
            }

            throw new HttpException(404, "Not found");

        }


       


        /// <summary>
        /// Реестр DPD (Поручение Ddp)
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult DpdErrand(Guid guid)
        {
            InitiateViewBag();
           // _isPartner = ServicePP.IsPartner;
            Order order = _orderService.GetOrderByGuid(guid);


            if (base.Partner == null) throw new NullReferenceException("Partner not found in db");


            OrderViewWithDetails model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);

            model.FildsForDpdForm = new FieldsForDpdViewModel
            {
                PartnerInnKpp = base.Partner.INN,
                //Address = (partner.Address ?? String.Empty),
                Address = base.Partner.Address,
                PartnerName = base.Partner.Name ?? String.Empty,
                ContractNumber = base.Partner.ContractNumber ?? String.Empty,
                ContractDate = base.Partner.ContractDate.ToShortDateFormat() ?? String.Empty,
                PartnerManager = base.Partner.MainManager.Fio,
                EMailManager = base.Partner.MainManager.EMail
                //DeliveryType = order.
            };
            int count = 0;
            decimal weight = 0;
            foreach (var item in model.OrderDetails)
            {
                count = count + item.Count;
                Product p = Products.GetProduct(item.ProductId);
                weight = weight + p.Weight * item.Count;
            }

            string pattern = @"\(([A-Z]){3}\)";
            var deliveryType = (Regex.IsMatch(model.OrderData.DeliveryDataString ?? String.Empty, pattern)) ? "ТТ" : "ТД";

            var stringSeparators = new string[] { "г." };
            try
            {
                
                var citystreet = model.OrderData.DeliveryDataString.Split(stringSeparators, StringSplitOptions.None);
                var city = citystreet[1].Split(Defaults.CommaSign);

                model.FildsForDpdForm.TotalCount = count;
                model.FildsForDpdForm.TotalWeight = weight;
                model.FildsForDpdForm.DeliveryType = deliveryType;
                model.FildsForDpdForm.DeliveryCity = city[0];

            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex);

            }


            return View(model);

          
        }

        public async  Task <ActionResult> DpdCancelOrder(Guid guid)
        {

            InitiateViewBag();
         //   _isPartner = ServicePP.IsPartner;
            Order order = await _orderService.GetOrderByGuidAsync(guid);


            if (base.Partner == null) throw new NullReferenceException("Partner not found in db");


            OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);

            model.FildsForDpdForm = new FieldsForDpdViewModel
            {
                PartnerInnKpp = base.Partner.INN ?? String.Empty,
                //Address = (partner.Address ?? String.Empty),
                Address = base.Partner.Address,
                PartnerName = base.Partner.Name,
                SaleNumber = _orderService.GetSaleNumberByOrderGuid(order.GuidIn1S),
                ContractNumber = base.Partner.ContractNumber ?? String.Empty,
                ContractDate = base.Partner.ContractDate.ToShortDateFormat() ?? String.Empty
                //DeliveryType = order.
            };
            int count = 0;
            decimal weight = 0;
            foreach (var item in model.OrderDetails)
            {
                count = count + item.Count;
                Product p = Products.GetProduct(item.ProductId);
                weight = weight + p.Weight * item.Count;
            }

            string pattern = @"\(([A-Z]){3}\)";
            var deliveryType = (Regex.IsMatch(model.DeliveryDataString ?? "", pattern)) ? "ТТ" : "ТД";

            var stringSeparators = new string[] { "г." };
            try
            {
                var citystreet = model.DeliveryDataString.Split(stringSeparators, StringSplitOptions.None);
                var city = citystreet[1].Split(',');

                model.FildsForDpdForm.TotalCount = count;
                model.FildsForDpdForm.TotalWeight = weight;
                model.FildsForDpdForm.DeliveryType = deliveryType;
                model.FildsForDpdForm.DeliveryCity = city[0];

            }
            catch (Exception ex)
            {
                ErrorLogger.Error(ex);

            }


            return View(model);


            //throw new HttpException(404, "Not found");

        }
        
        /// <summary>
        /// Отмена заказа
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CancelOrder(Guid guid)
        {
            InitiateViewBag();
            if (ModelState.IsValid)
            {
               

                Order order = await _orderService.GetOrderByGuidAsync(guid);

                if (!_orderService.CheckIfCanCancelOrder(order, out errorMessage))
                {

                    ModelState.AddModelError("OrderError", errorMessage);

                   var model = _orderService.GetOrderWithDetailsByGuid(guid);
                    
                        return View("Details", model);
                   
                    throw new HttpException(404, "Not found");
                }

                ResultDel result = await Task.Run(() => WS.DeleteOrder(order.GuidIn1S.ToString()));

                if (result.Success)
                {
                    _orderService.ChancelOrder(order);
                    return RedirectToAction("List");
                }
            }
            throw new HttpException(404, "Not found");
        }

       

        /// <summary>
        /// Заполнить поля при создании  через dpd
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private DeliveryInfo FillInDeliveryInfo(ChangeOrderViewModel viewModel)
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

        /// <summary>
        /// Проверить поля при создании заказа через dpd
        /// </summary>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        private async Task CheckTransportFields(ChangeOrderViewModel viewModel)
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
        /// Страница изменения заказа 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeOrder(Guid guid)
        {
            InitiateViewBag();
          //  _isPartner = ServicePP.IsPartner;
            bool hasStar = this.Partner.HasStar;
            bool isForeign = Partner.IsForeign;
            

            OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);

            model.CanUserUseDpdDelivery = ServicePP.CanUserUseDpdDelivery;
            model.AddressesIds = _orderService.AddressesOfDelivery;

            model.TkIds = _orderService.TkIds;
            model.HasStar = hasStar && !isForeign;
            model.IsStar = model.Order.IsStar;
            model.IsPrepay= model.Order.Prepay;


            if (String.IsNullOrEmpty(model.AddressId))
            model.AddressId= _orderService.GetDefaultAddressId(Partner.PartnerId, Point.PartnerPointId);
            model.LogistikDepartment = !isForeign && !String.IsNullOrEmpty(Partner.LogistikDepartment) ? Partner.LogistikDepartment : null;
            return View(model);


        }

        /// <summary>
        /// Изменение заказа 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ChangeOrder")]
        public async Task<ActionResult> ChangeOrder(ChangeOrderViewModel model)
        {
            
            string deliveryDataString = null;
            string rangeDeliveryDays = null;
            string deliveryDay = String.Empty;
            var costOfDeliveryByDepartments = new Dictionary<int, int>();

            InitiateViewBag();

         //   ViewBag.IsForeign = base.Partner.IsForeign;
            if (!ModelState.IsValid)
            {
                if (ModelState["DeliveryDate"].Errors.Count > 0)
                {
                    if (!model.IsReserve)
                        return Json(new { Success = false, Message = CartAndOrders.CorrectShipmentDate });
                }

                if (ModelState["OrderGuid"].Errors.Count > 0) return Json(new { Success = false, Message = "Некорректный guid" });
            }
            DeliveryInfo di = FillInDeliveryInfo(model);
            // проверяем заполнение необходимых полей при доставке транспортной компанией
            if (model.IsDeliveryByTk)
            {
                await CheckTransportFields(model);
                if (!ModelState.IsValid)
                {
                    string errMessage = String.Empty;
                    foreach (ModelState modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            errMessage = error.ErrorMessage;
                        }
                    }
                    return Json(new { Success = false, Message = errMessage });
                }
                var order = await _orderService.GetOrderByGuidAsync(model.OrderGuid);

                di.CostOfDelivery = await _deliveryCostService.GetCostOfDelivery(model.CityId, model.TerminalOrAddress, order.DepartmentId, model.OrderGuid);
                int minNumOfDays = await _deliveryCostService.GetNumberOfDaysOfDeliveryAsync(model.CityId, 0, model.OrderGuid);
                int maxNumOfDays = await _deliveryCostService.GetNumberOfDaysOfDeliveryAsync(model.CityId, 1, model.OrderGuid);

                rangeDeliveryDays = StringUtils.PrepareRangeDays(minNumOfDays, maxNumOfDays);
                deliveryDataString = await _deliveryCostService.PrepareDeliveryDataString(di);

                var lastDay = Int32.Parse(StringUtils.GetLastDayFromRange(rangeDeliveryDays));
                var dateToDeliverToEnd = DateTimeHelper.AddDaysWithoutDaysOff((DateTime)model.DeliveryDate, lastDay);
                deliveryDay = dateToDeliverToEnd.ToString(ServiceTerminal.FormatForDate);
            }
            if (!model.IsDeliveryByTk)
            {
                di.TerminalOrAddress = false;
                di.CostOfDelivery = 0;
            }
            // check if delivery date is valid
            if (!model.IsReserve)
            {
                var order = await _orderService.GetOrderByGuidAsync(model.OrderGuid);

                if (order == null)     return Json(new { Success = false, Message = "Заказ отсутствует в системе" });

                if (!model.DeliveryDate.HasValue) return Json(new { Success = false, Message = "Введите дату отгрузки" });

                //int totalDays = (int)(oc.DeliveryDate.Date - order.OrderDate.Date).TotalDays;
                int totalDays = (int)(((DateTime)model.DeliveryDate).Date - order.OrderDate.Date).TotalDays;

                
                if (totalDays < 0 || totalDays > maxDaysToChangeOrder)
                {
                    errorMessage = $"{ CartAndOrders.DateShipmentRange} { order.OrderDate.Date} - { order.OrderDate.AddDays(maxDaysToChangeOrder).Date}";
                        
                    return Json(new { Success = false, Message = errorMessage });
                }


            }

            //   int row_number = 0;
            var products = model.Items.Select(p => new SoapProduct
            {
                Quantity = p.Count,
                Code = p.ProductIdTo7Simbols,
                Storage = String.Empty
            }).ToArray();

            bool success = false;


            try
            {
                //  int WayOfDelivery, string AddressId
                //"{0:yyyyMMdd}"
                 ResultDel res = WS.ChangeOrder(model.OrderGuid.ToString(), products, model.Comments ?? String.Empty, model.IsReserve, 
                      // пустая строка если резерв, иначе дата доставки
                      !model.IsReserve ? String.Format("{0:yyyyMMdd}", model.DeliveryDate) : String.Empty,
                       model.IsDeliveryByTk, 
                       di, 
                       deliveryDay,
                       model.WayOfDelivery,
                       model.IsStar,
                       model.AddressId ?? String.Empty , 
                       model.TkId 
                      ); 
              
               success = res.Success;
                errorMessage = res.Error;
            }
            catch (Exception e)
            {
                ErrorLogger.Error(e);
                success = false;

            }

            // если вызов веб сервиса прошел удачно
            if (success)
            {
                try
                {
                    DateTime? deliveryDate = model.IsReserve ? (DateTime?)null : model.DeliveryDate;
                    _orderService.ChangeOrder(model.OrderGuid,
                        model.Items,
                        model.Comments ?? String.Empty,
                        model.IsReserve,
                        model.IsDeliveryByTk, 
                        deliveryDataString,
                        rangeDeliveryDays,
                        di, 
                        deliveryDate,
                        model.WayOfDelivery,
                        model.IsStar,
                        model.AddressId,
                        model.TkId);

                }
                catch (Exception e)
                {
                    ErrorLogger.Error(e);
                    success = false;
                }
              

            }
            return Json(new { Success = success, Message = errorMessage });
        }


        /// <summary>
        /// Получить даты отгрузки в корзине 
        /// </summary>
        /// <param name="addressId"></param>
        /// <returns></returns>
        public ActionResult GetDatesOfShipment( string addressId)
        {
            
                var result = WS.GetDatesOfShipment(base.Partner.PartnerId, addressId);
                 return Json(new {result= result } , JsonRequestBehavior.AllowGet );
        }


        /// <summary>
        /// Постановка на отгрузку нескольких заказов 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ProcessOrders(OrdersViewModelToProcess model)
        {

            // 1. Валидация модели
            const int DeliveryByTransportCompany = 3;
            if (model.WayOfDelivery== DeliveryByTransportCompany && String.IsNullOrEmpty(model.TkId)) ModelState.AddModelError("TkId", "необходимо ввести транспортную компанию");

            if (model.OrderGuids==null || !model.OrderGuids.Any()) ModelState.AddModelError("", "не выбраны заказы");
            
            
            if (!ModelState.IsValid) return  ReturnModelErrorsAsJson();


            // 2. Вызов веб-сервиса (получаем заказы которые обновлены)
            var deliveryDateString =  ((DateTime)model.DeliveryDate).ToString(ServiceTerminal.FormatForDate);

            Guid[] guidsOfOrdersChanged= { };
            try
            {
                var guidsOfOrdersChangedStringArray = model.OrderGuids.Select(item => item.ToString()).ToArray();
                var result= WS.ChangeOrders(deliveryDateString, model.WayOfDelivery, model.AddressId, model.TkId, model.DayOfWeekToDeliver == DayOfWeekToDeliver.Monday ? true : false, guidsOfOrdersChangedStringArray);

                if (result.Any())   guidsOfOrdersChanged= result.Select(item => Guid.Parse(item)).ToArray();
            }
            catch
           (Exception e)
            {
                ErrorLogger.Error(e.Message, e.InnerException);
                throw;
            }


            // 3. Заказы которые обновлены меняем в базе
            if (guidsOfOrdersChanged.Any())
            {
                var listOfOrders = await DbContext.Orders.Where(order => guidsOfOrdersChanged.Contains(order.GuidIn1S)).ToListAsync();

                foreach (var order in listOfOrders)
                {
                    order.isReserve = false;
                    order.DeliveryDate = model.DeliveryDate;
                    order.AddressId = model.AddressId;
                    order.WayOfDelivery = model.WayOfDelivery;
                    order.TkId = model.TkId;
                    
                }

                await DbContext.SaveChangesAsync();
            }

            model.OrderGuids = guidsOfOrdersChanged.ToList();
            return Json(model);
        }
        /// <summary>
        /// Объединить заказы (представление)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
    /*    [HttpGet]
        public ActionResult JoinOrders(ExtendedOrdersViewModel model)
        {

            _isPartner = ServicePP.IsPartner;
            ViewBag.IsPartner = _isPartner;

            ViewBag.IsForeign = Partner.IsForeign;
            ViewBag.CurrentPointId = base.Point.PartnerPointId;

            model.AddressesIds = _orderService.GetAddressesOfDeliveryForCurrentPoint();


            if (_isPartner) // PartnerId is set too
            {
                model.PartnerPoints = ServicePP.GetPointNamesByPartner(base.Partner.PartnerId, _propertyToDisplay);
                _orderService.GetListOfOrdersByPartnerId(model, base.Partner.PartnerId, ViewBag.CurrentPointId);

            }
            else
            {
                model.PartnerPoints = null;
                if (base.Point.PartnerPointId > 0)  _orderService.GetListOfOrdersByPointId(model, ViewBag.CurrentPointId);
            }
            return View(model);
            
        }


        /// <summary>
        /// Объединить заказы (обработчик)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>

        [HttpPost]
        public ActionResult JoinOrders(DtoJoinOrders model)
        {
            int maxDaysToAdd = 7;

            var partnerId = this.Partner.PartnerId;
            var results = model;
            string errorMessage = null;

            if (String.IsNullOrEmpty(model.AddressId)) errorMessage = "Не выбран адрес доставки";

            if(model.ShippingDay.Date <DateTime.Now.Date || model.ShippingDay.Date >= DateTime.Now.Date.AddDays(maxDaysToAdd) ) errorMessage = "Нельзя указывать дату ранее текущей даты";

            if(!model.OrdersAndProducts.Any()) errorMessage = "Нет заказов и товаров для объединения";

            if (errorMessage!=null)    return Json(new { Success = false, Message = errorMessage });


            // 0. Подготавливаем структуру для отправки в web-сервис

            var joinOrderProducts = model.OrdersAndProducts.Select(
                p => new JoinOrderProduct
            {
                Code = p.ProductId.ToString(),
                OrderGuid = p.OrderId.ToString(),
                Quantity = p.Count,
                OrderNumberIn1S = String.Empty,
                OldGuid = Guid.Empty.ToString()
            }).ToArray();


            // 1. Вызов  web-сервиса
           
                var result = WS.JoinOrders(joinOrderProducts, model.AddressId, model.ShippingDay.ToString(ServiceTerminal.FormatForDate), model.Comments ?? String.Empty);

                if (!result.Success) errorMessage = result.ErrorDescription;

            if (errorMessage != null) return Json(new { Success = false, Message = errorMessage });


            // 2. Сборка данных
            var initialResultSet =  result.Products.Select(p => new
            {
                OrderGuid = Guid.Parse(p.OrderGuid),
                NumberIn1S = p.OrderNumberIn1S,
                ProductId = Int32.Parse(p.Code),
                Count = p.Quantity,
                IsJoined = p.IsJoined.HasValue ? p.IsJoined : false,
                OldGuid = Guid.Parse(p.OldGuid),
                
            }).ToList();

            // выбираем Guid старых заказов
            var oldGuids = initialResultSet.Select(o=>o.OldGuid).Distinct().ToArray();

            // выбираем табличные данные OrderDetails старых заказов в память
            var orderdetails =  DbContext.OrderDetails.Include(o => o.Order).Where(p => oldGuids.Contains(p.GuidIn1S)).ToList();

            // добавляем столбцы из старых заказов
          var fullResultSet=  (from item in initialResultSet
             from orderDetail in orderdetails.Where(p => p.GuidIn1S == item.OldGuid && p.ProductId == item.ProductId).DefaultIfEmpty()
             select new
             {
                 OrderGuid = item.OrderGuid,
                 NumberIn1S = item.NumberIn1S,
                 ProductId = item.ProductId,
                 Count = item.Count,
                 IsJoined = item.IsJoined,
                 OldGuid = item.OldGuid,
                 Price = orderDetail.Price,
                 PriceOfClient = orderDetail.PriceOfClient,
                 PriceOfPoint = orderDetail.PriceOfPoint,
                 PointId = orderDetail.Order.PointId,
                 DepartmentId = orderDetail.Order.DepartmentId,
                 // 
                 ContactFIOOfClient = orderDetail.Order.ContactFIOOfClient,
                 PhoneNumberOfClient = orderDetail.Order.PhoneNumberOfClient

             }).ToList();


            // 3. СОЗДАНИЕ НОВЫХ ЗАКАЗОВ
            var newOrderDetails = fullResultSet.Where(p => !oldGuids.Contains(p.OrderGuid)).GroupBy(o => o.OrderGuid);
            
            Guid guidOfJoinedOrder = Guid.Empty;

            // обход заказов в памяти (создаем их в базе)
            // новые заказы - это только объединенные либо
            // вновь созданные на те товары которые не надо отгружать 
            // их - в резерв
            foreach (var groupedItem in newOrderDetails)
            {                             
               bool isJoined =groupedItem.First().IsJoined??false;

                if (isJoined) guidOfJoinedOrder = groupedItem.Key;

                var order =  new Order {
                  GuidIn1S =groupedItem.Key,
                  NumberIn1S = groupedItem.First().NumberIn1S,
                  DepartmentId = groupedItem.First().DepartmentId,
                  PointId= groupedItem.First().PointId,
                  PartnerId= partnerId,
                  OrderStatus=OrderStatuses.Confirmed,
                  OrderDate=DateTime.Now,
                  //
                  IsJoined= isJoined,                  
                  DeliveryDate = isJoined ? model.ShippingDay:(DateTime?)null,
                  isReserve= !isJoined
                };

          
                int count = 0;

                // создаем новые заказы сгруппированные по заказу
                foreach (var detail in groupedItem)
                {
                    order.OrderDetails.Add ( new OrderDetail
                    {
                        GuidIn1S = detail.OrderGuid,
                        RowNumber = ++count,
                        ProductId = detail.ProductId,
                        Count = detail.Count,
                        Price = detail.Price,
                        PriceOfClient = detail.PriceOfClient,
                        PriceOfPoint = detail.PriceOfPoint
                    });
                   

                }
                order.CalculateTotals();

                DbContext.Orders.Add(order);
                DbContext.SaveChanges();

            }
           // return Json(new { Success = false, Message = errorMessage });


            // 4. ПЕРЕЗАПИСТЬ ТАБЛИЧНЫХ ЧАСТЕЙ СТАРЫХ ЗАКАЗОВ
            var newOrderDetailsForOldOrders = fullResultSet.Where(p => oldGuids.Contains(p.OrderGuid)).GroupBy(o => o.OrderGuid);

            foreach (var groupedItem in newOrderDetailsForOldOrders)
            {
                var order = DbContext.Orders.Include(o => o.OrderDetails).First(p => p.GuidIn1S == groupedItem.Key);

                DbContext.OrderDetails.RemoveRange(order.OrderDetails);

                int count = 0;

                foreach (var detail in groupedItem)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        GuidIn1S = groupedItem.Key,
                        RowNumber = ++count,
                        ProductId = detail.ProductId,
                        Count = detail.Count,
                        Price = detail.Price,
                        PriceOfClient = detail.PriceOfClient,
                        PriceOfPoint = detail.PriceOfPoint
                    });
                }

                order.CalculateTotals();
                DbContext.SaveChanges();
            }

            return Json(new { Success = true });
            //   return RedirectToAction("Details","Orders", new { guid= guidOfJoinedOrder });

        }

      */



    }
}
