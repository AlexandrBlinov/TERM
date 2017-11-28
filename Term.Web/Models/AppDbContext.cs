using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using Term.DAL;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Yst.ViewModels;
using YstTerm.Models;
using System.Data.Entity.Infrastructure.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using Term.Utils;


namespace Yst.Context

{   

    public class AppDbContext : IdentityDbContext<ApplicationUser> 
    {

        public AppDbContext() : base("YstTerminal") {
         //  Database.SetInitializer<AppDbContext>(null);
        }


        /// <summary>
        /// Keys stored in database like constants
        /// </summary>
        public DbSet<StoredKeyValueItem> StoredKeyValueItems { get; set; }
        
        
        /// <summary>
        /// Public member related to base functionality
        /// </summary>

        #region Common Members  
        public DbSet<Manager> Managers { get; set; }
        public DbSet<Countries> Countries { get; set; }
        public DbSet<ReplicaDisksForCars> ReplicaDisksForCars { get; set; }

        // public DbSet<ManagersOfPartner> ManagersOfPartners { get; set; }

        public DbSet<AssistantOfManager> AssistantsOfManagers { get; set; }


        public DbSet<Producer> Producers { get; set; }
        public DbSet<Model> Models { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Tiporazmer> Tiporazmers { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<PartnerPropertyDescription> PartnerPropertyDescriptions { get; set; }
        public DbSet<PartnerPropertyValue> PartnerPropertyValues { get; set; }
        public DbSet<AddressOfPartner> AddressOfPartners { get; set; }

        public DbSet<ProducerNotDisplayedFromPartner> ProducerNotDisplayedFromPartners { get; set; }



        public DbSet<PriceOfPartner> PriceOfPartners { get; set; }
        public DbSet<PriceOfProduct> PriceOfProducts { get; set; }


        public DbSet<ProductPropertyDescription> ProductPropertyDescriptions { get; set; }
        public DbSet<ProductPropertyValue> ProductProperties { get; set; }
        public DbSet<RestOfProduct> Rests { get; set; }
        public DbSet<RestOfPartner> RestOfPartners { get; set; }
         
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<RestOfSupplier> RestsOfSuppliers { get; set; }
        

        public DbSet<Department> Departments { get; set; }
        public DbSet<DiskColour> DiskColours { get; set; }
        public DbSet<Languages> Languages { get; set; }

       public DbSet<CarRecord> CarRecords { get; set; }
       public DbSet<CarAkbRecord> CarAkbRecords { get; set; }
       public DbSet<CargoWheelsVehicle> CargoWheelsVehicles { get; set; }
     //   public DbSet<CarRecord> CarRecords { get; set; }
       public DbSet<News> News { get; set; }
       public DbSet<NewsNotifications> NewsNotifications { get; set; }
       public DbSet<PhotoForProducts> PhotoForProducts { get; set; }
       /// ////////////////

       public DbSet<PartnerPriceRule> PartnerPriceRules { get; set; }
       public DbSet<PartnerPoint> PartnerPoints { get; set; }
              

        public DbSet<Cart> Carts { get; set; }
       public DbSet<Order> Orders { get; set; }
       public DbSet<NotificationOfOrder> NotificationOfOrders { get; set; }
       public DbSet<ProductForSale> ProductForSales { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }  
       public DbSet<Sale> Sales { get; set; } 
        public DbSet<SaleDetail> SaleDetails { get; set; }
        public DbSet<Claims> Claims { get; set; }
        public DbSet<ClaimsDetails> ClaimsDetails { get; set; }

        #endregion
        /// <summary>
        /// season entities
        /// </summary>

        #region Season Members

        public DbSet<SeasonStockItem> SeasonStockItems { get; set; }
        public DbSet<SeasonCart> SeasonCarts { get; set; }
        public DbSet<SeasonOrder> SeasonOrders { get; set; }
        public DbSet<SeasonOrderDetail> SeasonOrderDetails { get; set; }
        public DbSet<SeasonStockItemOfPartner> SeasonStockItemOfPartners { get; set; }

        #endregion


        #region Dpd Members
        public DbSet<Region> Regions { get; set; }
        public DbSet<City> Cities { get; set; }
       public DbSet<RateToMainCity> RatesToMainCity { get; set; }
        public DbSet<RateToAdditionalCity> RatesToAdditionalCity { get; set; }

        public DbSet<DpdTerminal> DpdTerminals { get; set; }
        public DbSet<TimeOfDelivery> TimesOfDelivery { get; set; }
        #endregion


        public DbSet<SelfDeliveryAddress> SelfDeliveryAddresses { get; set; }

        public DbSet<SaleReturn> SaleReturns { get; set; }


        public DbSet<SaleReturnDetail> SaleReturnDetails { get; set; }
        

        public DbSet<OnWayItem> OnWayItems { get; set; }
        public DbSet<DbActionLogs> DbActionLogs { get; set; }
        public DbSet<DbUserActionLog> DbUserActionLogs { get; set; }
        public DbSet<CartActionLog> CartActionLogs { get; set; }

        public DbSet<TransportCompany> TransportCompanies { get; set; }


        public DbSet<FeedbackForm> FeedbackFormDbSet { get; set; }
        public DbSet<File> Files { get; set; }

        public DbSet<NotificationForUser> NotificationsForUsers { get; set; }
     //   public DbSet<FilesForDocument> FilesForDocuments { get; set; }

        public DbSet<HistoryOfOrderstatus> HistoryOfOrderstatuses { get; set; }
     

        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
           modelBuilder.Entity<PriceOfPartner>().HasKey(t => new { t.PartnerId, t.ProductId});
           modelBuilder .Entity<Product>()  .Property(t => t.Name)   .HasColumnAnnotation("Index", new IndexAnnotation(new IndexAttribute()));

           modelBuilder.Entity<IdentityUser>()
                .ToTable("Users");
            modelBuilder.Entity<ApplicationUser>()
                .ToTable("Users");

  
        }

          public void ExecuteTableValueProcedure<T>( IEnumerable<T> data, string procedureName, string paramName, string typeName)
          {

              DataTable table = AppDbContext.ToDataTable(data);
             //// convert source data to DataTable 
             //DataTable table = data.ToDataTable();

             //// create parameter 
             SqlParameter parameter = new SqlParameter(paramName, table);
             parameter.SqlDbType = SqlDbType.Structured;
             parameter.TypeName = typeName;

             //// execute sp sql 
             string sql = String.Format("EXEC {0} {1};", procedureName, paramName);

             //// execute sql 
             this.Database.ExecuteSqlCommand(sql, parameter);
         }


         /// <summary> 
         /// Creates data table from source data. 
         /// </summary> 
        
       
         private static DataTable ToDataTable<T>( IEnumerable<T> source)
         {
             DataTable table = new DataTable();

             //// get properties of T 
             var binding = BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty;
             var options = PropertyReflectionOptions.IgnoreEnumerable | PropertyReflectionOptions.IgnoreIndexer;

             var properties = ReflectionExtensions.GetProperties<T>(binding, options).ToList();

             //// create table schema based on properties 
             foreach (var property in properties)
             {
                 table.Columns.Add(property.Name, property.PropertyType);
             }

             //// create table data from T instances 
             object[] values = new object[properties.Count];

             foreach (T item in source)
             {
                 for (int i = 0; i < properties.Count; i++)
                 {
                     values[i] = properties[i].GetValue(item, null);
                 }

                 table.Rows.Add(values);
             }

             return table;
         } 
    }
}