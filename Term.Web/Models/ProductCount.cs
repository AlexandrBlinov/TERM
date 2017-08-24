using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace YstProject.Models
{
    public class ProductCount
    {
       
        public int ProductId { get; set; }
        
        public int Count { get; set; }
    }

    public class ProductWithRestCount : ProductCount {
        public int Rest { get; set; }
    }

    public class ProductCountWithSaleInfoDto 
    {
        [Required(ErrorMessage = "Заполните код товара")]
        public int? ProductId { get; set; }

        
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Заполните количество")]
        public int? Count { get; set; }

        [Required(ErrorMessage = "Заполните дату реализации")]
        [DataType(DataType.Date)]
       // [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Required(ErrorMessage = "Заполните номер реализации")]
        public string SaleNumber { get; set; }

        public string ProductIdTo7Simbols
        {
            get
            {
                return (ProductId.ToString().PadLeft(7, '0'));

            }
        }
    }


    public class PurchaseReturnDto
    {
        public IList<ProductCountWithSaleInfoDto> Items { get; set; }
        public PurchaseReturnDto()
        {
            Items = new List<ProductCountWithSaleInfoDto>()
            {
                new ProductCountWithSaleInfoDto {SaleDate = DateTime.Now},
            };
        }
    }





}