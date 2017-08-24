using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Term.DAL
{
    public enum ProdOrWay :byte
    {
        InProduction,
        OnWay
    }

    /// <summary>
    ///     class to hold products on way and in production
    ///     with corresponding dates of arrival or production dates
    /// </summary>
     [DataContract(Namespace = "", Name = "onwayitem")]
    public class  OnWayItem
    {
        [DataMember]
        [Key, Column(Order = 1)]
        public int ProductId { get; set; }

                
        /// <summary>
        /// in production or on way
        /// </summary>
         [DataMember]
         [Key, Column(Order = 2)]
        public ProdOrWay ProdOrWay { get; set; }

        /// <summary>
        ///     date of arrival or production date
        /// </summary>
         [DataMember]
         [Key, Column(Order = 3)]
        public DateTime DateOfArrival { get; set; }

         [DataMember]
         public int Count { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }


    }

    [NotMapped]
    [DataContract(Namespace = "", Name = "onwayitem")]
     public class OnWayItemDto {

         [DataMember]
         public int ProductId { get; set; }
         
         
         [DataMember]
         public ProdOrWay ProdOrWay { get; set; }
                  
         [DataMember]
         public DateTime DateOfArrival { get; set; }

         [DataMember]
         public int Count { get; set; }
     
     }
}
