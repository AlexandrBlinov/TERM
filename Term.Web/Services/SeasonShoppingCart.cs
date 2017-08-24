
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.Context;
//using YstProject.WebReferenceTerm;
using Yst.Utils;
using Yst.ViewModels;
using System.Data.Entity;
using YstProject.Services;
using System.Linq.Expressions;
using YstTerm.Models;
using Term.Utils;
using Term.Soapmodels;
using Term.Web.Views.Resources;

#if !not_compile
namespace Yst.Services
{
    /// <summary>
    /// Базовый тип событий
    /// </summary>
    public class BaseCartInfoArgs : EventArgs
    {
        public string Username { get; set; }
        public Product Product { get; set; }
        public int  Count { get; set; }

        public BaseCartInfoArgs(string username, Product product, int count)
        {
            Username = username;
            Product = product; 
            Count = count;
        }
        
    }

    public class ShoppingCartInfoArgs : BaseCartInfoArgs
    {
        public ShoppingCartInfoArgs(string partnerId,Product product, int count, decimal price, decimal priceOfPoint, decimal priceOfClient)
            : base(partnerId,product, count)
        {
            Price = price;
            PriceOfPoint = priceOfPoint;
            PriceOfClient = priceOfClient;
        }

        public decimal Price { get; set; }
        public decimal PriceOfPoint { get; set; }
        public decimal PriceOfClient { get; set; }
    }


    /// <summary>
    /// Managing by season shopping cart
    /// </summary>
    /// 
    public partial class SeasonShoppingCart : BaseService
    {

        public event EventHandler<BaseCartInfoArgs> ItemAddedToCart = delegate { };
        public event EventHandler<BaseCartInfoArgs> ItemRemovedFromCart = delegate { };
        private static readonly int MAX_COUNT = 1000;


        private readonly string ShoppingCartId = null; 
        public const string CartSessionKey = "SeasonCartId";

       
        public SeasonShoppingCart()
        {
            ShoppingCartId = GetCartId();
        }

        /// <summary>
        /// Проверка, может ли товар быть добавлен в сезонную корзину
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        /// <param name="message"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        public bool CheckIfItemCanBeAddedToCart (int productId,int count,ref string message,string partnerId)
        {
            if (count < 0 || count > MAX_COUNT)
            {
                message = SeasonOrdersTexts.Error_AmountOfCountShouldBeBetween0And1000;
            return false;
            }


            bool hasOffer = DbContext.Set<SeasonStockItem>().Any(ssi => ssi.ProductId == productId) ||
                            (partnerId != null &&
                             DbContext.Set<SeasonStockItemOfPartner>()
                                 .Any(ssi => ssi.ProductId == productId && ssi.PartnerId == partnerId));

            //var item=DbContext.Set<SeasonStockItem>().FirstOrDefault(ssi => ssi.ProductId == productId);
            if (!hasOffer) { message = SeasonOrdersTexts.Error_NoSeasonItemsFoundForThisProduct; return false; }
            

            return true;
        }

        public string GetCartId()
        {
            if (_context.Session[CartSessionKey] == null)    _context.Session[CartSessionKey] = _context.User.Identity.Name;
            return _context.Session[CartSessionKey].ToString();

        }


        private static Func<ProductType, byte> sortInCart = delegate(ProductType productType)
        {
            {
                byte byteValue;
                var sortOrder = new Dictionary<ProductType, byte> { { ProductType.Akb, 3 }, { ProductType.Acc, 4 }, { ProductType.Disk, 1 }, { ProductType.Tyre, 2 } };

                return (byte)(sortOrder.TryGetValue(productType, out byteValue) ? byteValue : 5);
            }
        };

        public void EmptyCart(WheelType? wheeltype=null)
        {
            
                var cartItems = DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId && (wheeltype==null||cart.Product.WheelType==wheeltype));
               
                DbContext.Set<SeasonCart>().RemoveRange(cartItems);
                // Save changes
                DbContext.SaveChanges();
            
        }

        /// <summary>
        /// returns sum by factory
        /// </summary>
        /// <returns></returns>
        public decimal GetSumByFactory(string factory)
        {
            Expression<Func<SeasonCart,bool>> predicate = cart => cart.CartId == ShoppingCartId &&cart.Factory==factory;

            return  (DbContext.Set<SeasonCart>().Any(predicate) ?DbContext.Set<SeasonCart>().Where(predicate).Sum(p => p.Price * p.Count):0);
        }

        /// <summary>
        /// returns sum by factory
        /// </summary>
        /// <returns></returns>
        public int GetCountByFactory(string factory)
        {

            return DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId && cart.Factory == factory).Sum(p => (int?)p.Count) ??0;
        }

        /// <summary>
        /// Returns factory by  product
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public string GetFactoryByProduct(int productId)
        {
            return DbContext.Set<Product>().First(p => p.ProductId == productId).Factory;
        
        }


        /// <summary>
        /// Create season orders in local db
        /// </summary>
        /// <param name="r"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public IEnumerable<Guid> CreateOrdersInLocalDb(ResultSeasonOrder r, SeasonCartViewModel viewModel)
        {


            var partnerId = GetPartnerId() ;

            var userName = _context.User.Identity.Name;


            var ProductsAndGuids = r.Products.Select(p => new { ProductId = StringUtils.GetProductId(p.Code), OrderGuid = p.OrderGUID, NumberIn1S = p.OrderNumberIn1S });

            var CartItems = viewModel.CartItems;

            var query = from ProductGuid in ProductsAndGuids
                        join cartItem in CartItems on ProductGuid.ProductId equals cartItem.ProductId
                        select new { ProductGuid.OrderGuid, ProductGuid.NumberIn1S, cartItem.ProductId,  cartItem.Count, cartItem.Price };

            var guids = query.Select(p => new { p.OrderGuid,  p.NumberIn1S }).Distinct();

            foreach (var orderGuid in guids)
            {
                int rowCounter = 0;
                SeasonOrder newOrder = new SeasonOrder
                {
                    
                    OrderGuid = Guid.Parse(orderGuid.OrderGuid),
                    PartnerId = partnerId,
                    Username = userName,
                    OrderStatus = SeasonOrderStatus.Confirmed,
                    Comments = viewModel.Comments ?? String.Empty,
                    DeliveryDate = viewModel.DeliveryDate,
                    OrderDate = DateTime.Now,
                    NumberIn1S = orderGuid.NumberIn1S,
                    OrderDetails = query.Where(p => p.OrderGuid == orderGuid.OrderGuid).Select(p => new SeasonOrderDetail { RowNumber = ++rowCounter, ProductId = p.ProductId, Count = p.Count, Price = p.Price}).ToList()

                };


                newOrder.Total=newOrder.OrderDetails.Sum(p=>p.Count*p.Price);

                DbContext.Set<SeasonOrder>().Add(newOrder);

            }

            DbContext.SaveChanges();

            EmptyCart(viewModel.ActiveWheelType);

            return guids.Select(g => Guid.Parse(g.OrderGuid)).Distinct().AsEnumerable<Guid>();
            
        }

        /// <summary>
        /// Create one season order for russian client. All guids and 1s numbers in result are equal
        /// </summary>
        /// <param name="r"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        public Guid CreateOneOrderInLocalDb(ResultSeasonOrder r, SeasonCartViewModel viewModel)
        {
             var orderGuid = Guid.Parse(r.Products.First().OrderGUID);
            var numberIn1S = r.Products.First().OrderNumberIn1S;

             var partnerId = GetPartnerId() ;

              var userName = _context.User.Identity.Name;
              var CartItems = viewModel.CartItems;


              int rowCounter = 0;
               var newOrder = new SeasonOrder
                {
                    OrderGuid = orderGuid,
                    PartnerId = partnerId,
                    Username = userName,
                    OrderStatus = SeasonOrderStatus.Confirmed,
                    Comments = viewModel.Comments ?? String.Empty,
                    DeliveryDate = viewModel.DeliveryDate,
                    OrderDate = DateTime.Now,
                    NumberIn1S = numberIn1S,
                    OrderDetails = CartItems.Select(p => new SeasonOrderDetail { RowNumber = ++rowCounter, ProductId = p.ProductId, Count = p.Count, Price = p.Price}).ToList()

                };


                newOrder.Total=newOrder.OrderDetails.Sum(p=>p.Count*p.Price);

                DbContext.Set<SeasonOrder>().Add(newOrder);

            
            DbContext.SaveChanges();

            EmptyCart(viewModel.ActiveWheelType);

        return orderGuid;
        
        }
        /// <summary>
        /// returns cart items with product related fields
        /// </summary>
        /// <returns></returns>
        public List<SeasonCart> GetCartItems( WheelType? wheeltype=null )
        {
            
            return DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId && (wheeltype==null || cart.Product.WheelType==wheeltype)).Include(p=>p.Product).ToList();
            
        }

        /*
        public List<SeasonCart> GetCartItemsByWheelType(WheelType wheeltype)
        {

            return DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId && cart.Product.WheelType==wheeltype).Include(p => p.Product).ToList();

        }
         * */

        /// <summary>
        /// Update items in cart
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>
        public void UpdateItemCount(int productId, int count,out decimal price)
        {
            price = 0;
            var cartItem = DbContext.Set<SeasonCart>().FirstOrDefault(
                    c => c.CartId == ShoppingCartId
                    && c.ProductId == productId);

                if (cartItem != null)
                {
                    if (cartItem.Count != count)
                    {
                        cartItem.Count = count;
                        DbContext.SaveChanges();
                    }
                    price = cartItem.Price;
                }
            
        }


        /// <summary>
        /// Add product to season cart
        /// </summary>
        /// <param name="product"></param>
        /// <param name="count"></param>
        /// <param name="price"></param>

        public void AddToCart(Product product,  ref int count, decimal price = 0)
        {

            var factory=DbContext.Set<Product>().First(ssi => ssi.ProductId == product.ProductId).Factory;
            // Get the matching cart and album instances
            bool itemAdded = false;

            var cartItem = DbContext.Set<SeasonCart>().FirstOrDefault(c => c.CartId == ShoppingCartId&& c.ProductId == product.ProductId);
                       

                if (cartItem == null)
                {
                    // Create a new cart item if no cart item exists
                    cartItem = new SeasonCart
                    {
                        ProductId = product.ProductId,
                        CartId = ShoppingCartId,
                        Count = count,
                        Price = price,
                        Factory= product.Factory
                       
                    };
                    itemAdded = true;
                    DbContext.Set<SeasonCart>().Add(cartItem);
                }
                else
                {
                    // If the item does exist in the cart, then add one to the quantity
                    cartItem.Count++;
                    count = 1;
                }
                // Save changes
                DbContext.SaveChanges();
                if (itemAdded) ItemAddedToCart(this, new BaseCartInfoArgs(ShoppingCartId,product, count));  
            
        }

     
      

      

       

        public int RemoveFromCart(int id)
        { // Get the cart

            var cartItem = DbContext.Set<SeasonCart>().FirstOrDefault(
                cart => cart.CartId == ShoppingCartId
                && cart.ProductId == id);

                int itemCount = 0;
                if (cartItem != null)
                {
                    // by default Remove whole product

                    DbContext.Set<SeasonCart>().Remove(cartItem);

                    
                    // Save changes
                    DbContext.SaveChanges();
                    
                }
                return itemCount;
            
        }



        public decimal GetTotal()
        {

            decimal? total = (from cartItems in DbContext.Set<SeasonCart>()
                                  where cartItems.CartId == ShoppingCartId
                                  select (int?)cartItems.Count * cartItems.Price).Sum();
                return total ?? decimal.Zero;

            
        }

    
        /// <summary>
        /// Service function returns totals by custom prop of SeasonCart
        /// </summary>
        /// <param name="selector"></param>
        /// <returns></returns>
        public decimal GetTotalsByProperty(Expression<Func<SeasonCart, decimal>> selector)
        {
            var recordset = DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId).Include(p => p.Product).Select(selector);
                if (recordset.Any()) return recordset.Sum();

            return 0;
        }

       
        public int GetCount()
        {
            var cartCounts = DbContext.Set<SeasonCart>().Where(cart => cart.CartId == ShoppingCartId).Select(cart => cart.Count).ToArray();

                int count = cartCounts.Any() ? cartCounts.Sum() : 0;
            
                return count;
            
        }

      

    }
}
#endif