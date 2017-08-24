using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Ajax.Utilities;
using Term.DAL;
using Yst.Context;
//using YstProject.WebReferenceTerm;
using Yst.Utils;
using Yst.ViewModels;
using YstProject.Services;
using Term.Utils;
using Term.Soapmodels;



namespace Yst.Services
{
    public partial class ShoppingCart : BaseService
    {
        public event EventHandler<ShoppingCartInfoArgs> ItemAddedToCart = delegate { };
        private string ShoppingCartId = null; 
        public const string CartSessionKey = "CartId";

        public DaysToDepartmentWithSuppliersService _daysToDepartmentService = null;

        public ShoppingCart(AppDbContext dbcontext)
            : this(dbcontext, new DaysToDepartmentWithSuppliersService()) 
        {
   
        }

        ShoppingCart(AppDbContext dbcontext, DaysToDepartmentWithSuppliersService daysToDepartmentService)
            : base(dbcontext)
        {
            _daysToDepartmentService = daysToDepartmentService;
            ShoppingCartId = GetCartId();
        }

     //   public DaysToDepartmentService DaysToDepartmentService { get { return _daysToDepartment ?? new DaysToDepartmentService(); } }



        /// <summary>
        /// Возвращает Идентификатор пользователя
        /// </summary>
        /// <returns></returns>
        private string GetCartId()
        {
            if (_context.Session[CartSessionKey] == null)    _context.Session[CartSessionKey] = _context.User.Identity.Name;
            return _context.Session[CartSessionKey].ToString();

        }


        private static readonly Func<ProductType, byte> SortInCart = delegate(ProductType productType)
        {
            {
                byte byteValue;
                var sortOrder = new Dictionary<ProductType, byte> { { ProductType.Akb, 3 }, { ProductType.Acc, 4 }, { ProductType.Disk, 1 }, { ProductType.Tyre, 2 } };

                return (byte)(sortOrder.TryGetValue(productType, out byteValue) ? byteValue : 5);
            }
        };

     //   private  readonly Expression<Func<Cart,  bool>> _predicate = (cart) => cart.CartId == ShoppingCartId;

        
        /// <summary>
        /// Очистить корзину
        /// </summary>
        public void EmptyCart()
        {

            var cartItems = DbContext.Carts.Where(cart => cart.CartId == ShoppingCartId);
               /* foreach (var cartItem in cartItems)
                {
                    DbContext.Carts.Remove(cartItem);
                }*/
                DbContext.Carts.RemoveRange(cartItems);
                // Save changes
                DbContext.SaveChanges();
            
        }

        public IList<Cart> GetCartItems()
        {

            return DbContext.Carts.Where(cart => cart.CartId == ShoppingCartId).Include(p=>p.Product).ToList().OrderBy(cart => SortInCart(cart.Product.ProductType)).ToList();
            
        }

        public async Task<IList<Cart>> GetCartItemsAsync()
        {

            var resultList=await DbContext.Carts.Where(cart => cart.CartId == ShoppingCartId).ToListAsync() ;

            return resultList.OrderBy(cart => SortInCart(cart.Product.ProductType)).ToList();

        }

        /// <summary>
        /// Вес товаров в корзине
        /// </summary>
        /// <returns></returns>
        public decimal GetCartWeight()
        {
            var query= DbContext.Carts.Where(cart => cart.CartId == ShoppingCartId);
            if (query.Any()) return query.Sum(p => p.Product.Weight*p.Count);
            return 0;

        }

        /// <summary>
        /// Обновляет количество товаров в корзине
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        public void UpdateItemCount(int productId, int count)
        {

            var cartItem = DbContext.Carts.FirstOrDefault(
                    c => c.CartId == ShoppingCartId
                    && c.ProductId == productId);

                if (cartItem != null)
                {
                    if (cartItem.Count != count)
                    {
                        cartItem.Count = count;
                        DbContext.SaveChanges();
                    }
                }
            
        }



        /// <summary>
        /// Добавить товар в корзину
        /// </summary>
        /// <param name="product"></param>
        /// <param name="departmentId"></param>
        /// <param name="days"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>
        /// <param name="priceOfPoint"></param>
        /// <param name="priceOfClient"></param>
        public void AddToCart(Product product, int departmentId, int days, ref int count, decimal price = 0, decimal priceOfPoint=0, decimal priceOfClient = 0, int supplierId=0)
        {
            
            var cartItem = DbContext.Carts.FirstOrDefault(c => c.CartId == ShoppingCartId && c.ProductId == product.ProductId);

            bool itemAdded = false;
                if (cartItem == null)
                {
                    // Create a new cart item if no cart item exists
                    cartItem = new Cart
                    {
                        ProductId = product.ProductId,
                        CartId = ShoppingCartId,
                        Count = count,
                        DepartmentId = departmentId,
                        DaysToDepartment=days,
                        PriceOfClient = priceOfClient,
                        PriceOfPoint = priceOfPoint,
                        Price = price,
                        DateCreated = DateTime.Now,
                        SupplierId=supplierId
                    };
                    itemAdded = true;
                    DbContext.Carts.Add(cartItem);
                }
                else
                {
                    // If the item does exist in the cart, then add one to the quantity
                    cartItem.Count++;
                    count = 1;
                }
                // Save changes
                DbContext.SaveChanges();
                if (itemAdded) ItemAddedToCart(this, new ShoppingCartInfoArgs(ShoppingCartId,product, count, price, priceOfPoint, priceOfClient));  
        }


        /// <summary>
        /// Создает заказы в локальной базе
        /// </summary>
        /// <param name="soapResult">параметр, получаемый из веб-сервиса</param>
        /// <param name="viewModel"></param>
        /// <param name="seasonOrderGuid"> возвращает Guid сезонного заказа, если его надо создать по товарам в пути</param>
        /// <param name="costOfDeliveryByDepartments"> Словарь в который будет передаваться стоимость доставки с каждого подразделения</param>
        /// <returns>guid заказов</returns>
        public IEnumerable<string> CreateOrdersInLocal(Result soapResult, ShoppingCartViewModelExtended viewModel, out Guid seasonOrderGuid, IDictionary<int, int> costOfDeliveryByDepartments, string deliveryString, string rangeDeliveryDays)
        {

            var guidsOfOrdersFromSuppliers = StringUtils.GetArrayOfGuidsFromString(soapResult.OrdersFromSuppliers);

            seasonOrderGuid = Guid.Empty;

            var pointId = CurrentPoint.PartnerPointId; // getPointID();

            var partnerId = IsPartner ? GetPartnerId() : GetPartnerIdByPointId(pointId);

            var userName = IsPartner ? String.Format("Point{0}", pointId) : _context.User.Identity.Name;
            
            
            // получаем из ответа 1С товары и guid заказов
            var productsAndGuids = soapResult.Products.Select(p => new { ProductId = StringUtils.GetProductId(p.Code), OrderGuid = p.OrderGUID, NumberIn1S = p.OrderNumberIn1S });

            var cartItems = viewModel.CartItems;

            // товары и guid заказов, количество и цены
            var alldata = from productAndGuid in productsAndGuids
                        join cartItem in cartItems on productAndGuid.ProductId equals cartItem.ProductId
                        select new { 
                            productAndGuid.OrderGuid,
                            productAndGuid.NumberIn1S, 
                            cartItem.ProductId, 
                            cartItem.DepartmentId, 
                            cartItem.Count, 
                            cartItem.Price, 
                            cartItem.PriceOfClient, 
                            cartItem.PriceOfPoint,
                            cartItem.SupplierId
                        };

            // для уменьшения на остатакх берем товары с количеством на складах 
            var productWithCountOnDeps = alldata.Select(p => new ProductWithCountOnDep { DepartmentId = p.DepartmentId, ProductId = p.ProductId, Count = p.Count }).AsEnumerable();




            // получаем различные Guidы всех заказов кроме сезонного (DepartmentId != 0)

            var distinctGuids = alldata.Select(p => new { p.OrderGuid, p.DepartmentId, p.NumberIn1S,p.SupplierId }).Where(p => p.DepartmentId != 0).Distinct();

            //
            // for orders from stock (DepartmentId != 0)
            //

            foreach (var orderGuid in distinctGuids)
            {
                int costOfDelivery=0;

                if (viewModel.IsDeliveryByTk)   costOfDeliveryByDepartments.TryGetValue(orderGuid.DepartmentId,out costOfDelivery);

                int rowCounter = 0;

                var guidIn1S = Guid.Parse(orderGuid.OrderGuid);

                var newOrder = new Order
                {

                    GuidIn1S = guidIn1S,
                    PartnerId = partnerId,
                    PointId = pointId,
                    DepartmentId = orderGuid.DepartmentId,
                    // для сторонних поставщиков (supplierId>0 добавляем их срок доставки до склада)
                    DaysToDepartment = _daysToDepartmentService.GetDaysToDepartment(pointId, orderGuid.DepartmentId) + (orderGuid.SupplierId > 0 ? _daysToDepartmentService.GetDaysFromSupplierToMainDepartment(orderGuid.SupplierId) : 0),
                    ContactFIOOfClient = viewModel.ContactFIOOfClient ?? String.Empty,
                    PhoneNumberOfClient = viewModel.PhoneNumberOfClient ?? String.Empty,
                    Username = userName,
                    OrderStatus = guidsOfOrdersFromSuppliers.Any(item => item == guidIn1S) ? OrderStatuses.BeingConfirmedBySupplier : OrderStatuses.Confirmed, 
                    Comments = viewModel.Comments ?? String.Empty,
                    DeliveryDate = viewModel.DeliveryDate,
                    isReserve =!viewModel.IsDelivery,
                    OrderDate = DateTime.Now,
                    NumberIn1S = orderGuid.NumberIn1S,
                    IsDeliveryByTk = viewModel.IsDeliveryByTk,
                    CostOfDelivery = costOfDelivery,
                    DeliveryDataString = deliveryString,
                    RangeDeliveryDays = rangeDeliveryDays,
                    SupplierId=orderGuid.SupplierId,
                    OrderDetails = alldata.Where(p => p.OrderGuid == orderGuid.OrderGuid).Select(p => new OrderDetail
                    {
                        RowNumber = ++rowCounter,ProductId = p.ProductId, Count = p.Count, 
                        Price = p.Price, PriceOfClient = p.PriceOfClient, PriceOfPoint = p.PriceOfPoint, PriceInitial = p.Price
                    }).ToList()

                };

               
                newOrder.CalculateTotals();

                DbContext.Orders.Add(newOrder);

            }
            //
            // for orders in way (DepartmentId == 0)
            //
             var orderOnWay = alldata.Where(p => p.DepartmentId == 0).Select(p => new { p.OrderGuid, p.NumberIn1S }).FirstOrDefault();
             if (orderOnWay != null)
             {
                 seasonOrderGuid = Guid.Parse(orderOnWay.OrderGuid);
                 int rowCounter = 0;
                 var newSeasonOrder = new SeasonOrder
                 {
                     // season from on way 
                     FromOnWay=true,
                     //
                     OrderGuid = Guid.Parse(orderOnWay.OrderGuid),
                     PartnerId = partnerId,
                     Username = userName,
                     OrderStatus = SeasonOrderStatus.Confirmed,
                     Comments = viewModel.Comments ?? String.Empty,
                     DeliveryDate = DateTime.Now.AddDays(120),
                     OrderDate = DateTime.Now,
                     NumberIn1S = orderOnWay.NumberIn1S,
                     OrderDetails = alldata.Where(p => p.OrderGuid == orderOnWay.OrderGuid).Select(p => new SeasonOrderDetail { RowNumber = ++rowCounter, ProductId = p.ProductId, Count = p.Count, Price = p.Price }).ToList()

                 };

                 newSeasonOrder.Total = newSeasonOrder.OrderDetails.Sum(p => p.Count * p.Price);

                 DbContext.Set<SeasonOrder>().Add(newSeasonOrder);
             }
             




            DbContext.SaveChanges();

            EmptyCart();

            // уменьшаем корзину
            DbContext.ExecuteTableValueProcedure(productWithCountOnDeps, "SubtractFromRests", "@ProdWithCount", "ProductWithCountOnDeps");

            return distinctGuids.Select(g => g.OrderGuid).Distinct().AsEnumerable();
            
        }

      

       

        public int RemoveFromCart(int id)
        { // Get the cart

            var cartItem = DbContext.Carts.FirstOrDefault(
                cart => cart.CartId == ShoppingCartId
                && cart.ProductId == id);

                int itemCount = 0;
                if (cartItem != null)
                {
                    // by default Remove whole product

                    DbContext.Carts.Remove(cartItem);

                    /*if (cartItem.Count > 1)
                    {
                        cartItem.Count--;
                        itemCount = cartItem.Count;
                    }
                    else
                    {
                        storeDB.Carts.Remove(cartItem);
                    } */
                    // Save changes
                    DbContext.SaveChanges();
                }
                return itemCount;
            
        }

        /// <summary>
        /// Возвращает стоимость поставки 
        /// </summary>
        /// <returns></returns>

        public decimal GetTotal()
        {

            decimal? total = (from cartItems in DbContext.Carts
                                  where cartItems.CartId == ShoppingCartId
                                  select (int?)cartItems.Count * cartItems.Price).Sum();
                return total ?? decimal.Zero;

            
        }

        /// <summary>
        /// Возвращает стоимость продажи 
        /// </summary>
        /// <returns></returns>
         public decimal GetTotalOfClient()
        {

            decimal? total = (from cartItems in DbContext.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.PriceOfClient).Sum();
            return total ?? decimal.Zero;


        }
        public int GetCount()
        {

            var cartCounts = DbContext.Carts
                    .Where(cart => cart.CartId == ShoppingCartId)
                    .Select(cart => cart.Count).ToArray();

                int count = cartCounts.Any() ? cartCounts.Sum() : 0;
            
                return count;
            
        }

      

    }
}