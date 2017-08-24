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

namespace Term.Web.Controllers
{
    [Authorize]
    [CheckSettings]
    public class OrdersController : BaseController
    {

        private bool _isPartner;
      
       private string errorMessage;

        readonly Func<PartnerPoint, string> _propertyToDisplay = p => p.InternalName ?? String.Empty;

        private readonly OrderService _orderService;
        private readonly DeliveryCostCalculatorService _deliveryCostService;



        public OrdersController() : this(new OrderService(),new DeliveryCostCalculatorService()) { }
       

        public OrdersController(OrderService orderService,DeliveryCostCalculatorService deliveryCostService)
        {
            _orderService = orderService;
            _deliveryCostService = deliveryCostService;
        }

       

        //
        // FOR USER TO SEE OWN ORDERS
       //[CheckPassThroughKeyword]

        /// <summary>
        /// Список заказов
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult List(OrdersViewModel model)
        {
         
            _isPartner = ServicePP.IsPartner;
            ViewBag.IsPartner = _isPartner;
        
            ViewBag.IsForeign =Partner.IsForeign;
            ViewBag.CurrentPointId = base.Point.PartnerPointId;

                if (_isPartner) // PartnerId is set too
                {
                    model.PartnerPoints = ServicePP.GetPointNamesByPartner(base.Partner.PartnerId, _propertyToDisplay);
                    model = _orderService.GetListOfOrdersByPartnerId(model, base.Partner.PartnerId, ViewBag.CurrentPointId);

                }
                else
                {
                    model.PartnerPoints = null;
                    if (base.Point.PartnerPointId > 0) model = _orderService.GetListOfOrdersByPointId(model, ViewBag.CurrentPointId);
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
            _isPartner = ServicePP.IsPartner;
                ViewBag.OrderInfo = String.Empty;
                if (TempData["neworder"] != null) ViewBag.OrderInfo = CartAndOrders.NewOrderCreated;

           
                ViewBag.IsPartner = ServicePP.IsPartner;
                ViewBag.IsForeign = Partner.IsForeign;
             
               

               var order =  await  _orderService.GetOrderByGuidAsync(guid);
                ViewBag.OrderStatus = order.OrderStatus;
            
            
                if (_orderService.CheckIfCanCancelOrder(order, out errorMessage)) ViewBag.DisplayEditButton = true;


                OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);
               model.HistoryOfOrderStatuses= await _orderService.GetHistoryOfOrderStatusesAsync(guid);
                model.CanUserChangeDpdOrder = true;
                if (model.OrderData.IsDeliveryByTk)
                {
                    var date = model.OrderData.DeliveryDate.Value.AddHours(14);
                    model.CanUserChangeDpdOrder = date > DateTime.Now ? true : false;
                }
                ViewBag.SaleIsReady = _orderService.GetSaleNumberByOrderGuid(order.GuidIn1S) != String.Empty ? true : false;

            // заполняем склад для подтверждения
            if (!Partner.IsForeign) model.OrderData.DepartmentName = order.Department!=null ? order.Department.Name:"";


            // Если доставка транспортной компанией, то считаем дату планируемой доставки
            if (model.OrderData.IsDeliveryByTk && !String.IsNullOrEmpty(model.OrderData.RangeDeliveryDays) && model.OrderData.DeliveryDate.HasValue)
            {
                var lastDay =Int32.Parse(StringUtils.GetLastDayFromRange(model.OrderData.RangeDeliveryDays));

              ViewBag.SupposedDateOfDelivery = DateTimeHelper.AddDaysWithoutDaysOff((DateTime) model.OrderData.DeliveryDate, lastDay);

            }
            
           
                 return  View("Details", model);
               
            

        }

     
        


        

        /// <summary>
        /// Счет на оплату по заказу
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public ActionResult Bill(Guid guid,bool? showPicture)
        {
            _isPartner = ServicePP.IsPartner;
            Order order = _orderService.GetOrderByGuid(guid);

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

            _isPartner = ServicePP.IsPartner;
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
            var deliveryType = (Regex.IsMatch(model.OrderData.DeliveryDataString ?? "", pattern)) ? "ТТ" : "ТД";

            var stringSeparators = new string[] { "г." };
            try
            {
                var citystreet = model.OrderData.DeliveryDataString.Split(stringSeparators, StringSplitOptions.None);
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

          
        }

        public ActionResult DpdCancelOrder(Guid guid)
        {

            _isPartner = ServicePP.IsPartner;
            Order order = _orderService.GetOrderByGuid(guid);


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
            var deliveryType = (Regex.IsMatch(model.OrderData.DeliveryDataString ?? "", pattern)) ? "ТТ" : "ТД";

            var stringSeparators = new string[] { "г." };
            try
            {
                var citystreet = model.OrderData.DeliveryDataString.Split(stringSeparators, StringSplitOptions.None);
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
        public async Task<ActionResult> ChancelOrder(Guid guid)
        {
            if (ModelState.IsValid)
            {
              
                Order order = _orderService.GetOrderByGuid(guid);

                if (!_orderService.CheckIfCanCancelOrder(order, out errorMessage))
                {

                    ModelState.AddModelError("OrderError", errorMessage);

                    OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid);

                    if (model != null)
                    {
                        return View("Details", model);
                    }

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
        /// Страница изменения заказа 
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult ChangeOrder(Guid guid)
        {
            _isPartner = ServicePP.IsPartner;
            ViewBag.IsForeign = base.Partner.IsForeign;

            OrderViewWithDetailsExtended model = _orderService.GetOrderWithDetailsByGuid(guid, _isPartner, base.Partner.PartnerId, base.Point.PartnerPointId);
            model.CanUserUseDpdDelivery = ServicePP.CanUserUseDpdDelivery;

            return View(model);


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

            ViewBag.IsForeign = base.Partner.IsForeign;
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
                        foreach (ModelError error in modelState.Errors)
                        {
                            errMessage = error.ErrorMessage;
                        }
                    }
                    return Json(new { Success = false, Message = errMessage });
                }
                var order = _orderService.GetOrderByGuid(model.OrderGuid);

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
                var order = _orderService.GetOrderByGuid(model.OrderGuid);

                if (order == null)
                    return Json(new { Success = false, Message = "Заказ отсутствует в системе" });

                if (!model.DeliveryDate.HasValue) return Json(new { Success = false, Message = "Введите дату отгрузки" });

                //int totalDays = (int)(oc.DeliveryDate.Date - order.OrderDate.Date).TotalDays;
                int totalDays = (int)(((DateTime)model.DeliveryDate).Date - order.OrderDate.Date).TotalDays;

                if (totalDays < 0 || totalDays > 7)
                {
                    errorMessage = string.Format(CartAndOrders.DateShipmentRange + " {0} - {1}", order.OrderDate.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), order.OrderDate.AddDays(7).ToString("dd.MM.yyyy", CultureInfo.InvariantCulture));
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

            bool Success = false;


            try
            {

                ResultDel res = WS.ChangeOrder(model.OrderGuid.ToString(), products, model.Comments ?? String.Empty, model.IsReserve, !model.IsReserve ? String.Format("{0:yyyyMMdd}", model.DeliveryDate) : String.Empty, model.IsDeliveryByTk, di, deliveryDay);
                Success = res.Success;
                errorMessage = res.Error;
            }
            catch
            {
                Success = false;

            }

            if (Success)
            {
                try
                {
                    DateTime? deliveryDate = model.IsReserve ? (DateTime?)null : model.DeliveryDate;
                    _orderService.ChangeOrder(model.OrderGuid, model.Items, model.Comments ?? String.Empty, model.IsReserve, model.IsDeliveryByTk, deliveryDataString, rangeDeliveryDays, di, deliveryDate);

                }
                catch { Success = false; }
                finally
                { Session["IsOrderChanged"] = true; }

            }
            return Json(new { Success = Success, Message = errorMessage });
        }


        
        [HttpGet]
        public ActionResult JoinOrders(ExtendedOrdersViewModel model)
        {

            _isPartner = ServicePP.IsPartner;
            ViewBag.IsPartner = _isPartner;

            ViewBag.IsForeign = Partner.IsForeign;
            ViewBag.CurrentPointId = base.Point.PartnerPointId;

            if (_isPartner) // PartnerId is set too
            {
                model.PartnerPoints = ServicePP.GetPointNamesByPartner(base.Partner.PartnerId, _propertyToDisplay);
                model = _orderService.GetListOfOrdersByPartnerId(model, base.Partner.PartnerId, ViewBag.CurrentPointId);

            }
            else
            {
                model.PartnerPoints = null;
                if (base.Point.PartnerPointId > 0) model = _orderService.GetListOfOrdersByPointId(model, ViewBag.CurrentPointId);
            }
            return View(model);
            
        }
        

    }
}
