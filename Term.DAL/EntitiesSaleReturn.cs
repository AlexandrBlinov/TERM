using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term.DAL
{

    public class SaleReturnDetail
    {
        [Key]
        public long Id { get; set; }

        [ForeignKey("SaleReturn")]
        public Guid GuidIn1S { get; set; }
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "dd.MM.yyyy")]
        public System.DateTime SaleDate { get; set; }
        
        public string SaleNumber { get; set; }
        
        public int RowNumber { get; set; }

        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        

        public virtual Product Product { get; set; }
        public virtual SaleReturn SaleReturn { get; set; }
    }


    public  class SaleReturn
    {
        private ICollection<SaleReturnDetail> _saleReturnDetails;
        public virtual ICollection<SaleReturnDetail> SaleReturnDetails
        {
            get { return _saleReturnDetails ?? (_saleReturnDetails = new List<SaleReturnDetail>()); }
            set { _saleReturnDetails = value; }
        }

        [Key]  
        public Guid GuidIn1S { get; set; }

        public string NumberIn1S { get; set; }

        public DateTime DocDate { get; set; }

        [MaxLength(7)]
        public string PartnerId { get; set; }

        [ForeignKey("Point")]
        public int PointId { get; set; }


        [ForeignKey("PartnerPointId")]
        public virtual PartnerPoint Point { get; set; }
       

    }

   
}
