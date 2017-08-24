using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace YstProject.Models
{
    [DataContract(Namespace = "", Name = "order")]
    public class OrderNumberWithStatusDTO
    {

        [DataMember(Name = "date", EmitDefaultValue = false)]
        public string Date { get; set; }


        public DateTime DateInternal { get; set; }

        [DataMember(Name = "st", EmitDefaultValue = false)]
        public int OrderStatus { get; set; }

        [DataMember(Name = "num", EmitDefaultValue = false)]
        public string NumberIn1S { get; set; }

        [DataMember(Name = "sum", EmitDefaultValue = false)]
        public decimal Totals { get; set; }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            Date = DateInternal.ToShortDateString();
        }

    }

    [DataContract(Namespace = "", Name = "sale")]
    public class SaleDTO
    {
        [DataMember(Name = "date", EmitDefaultValue = false)]
        public string Date { get; set; }


        public DateTime DateInternal { get; set; }

        [DataMember(Name = "num", EmitDefaultValue = false)]
        public string NumberIn1S { get; set; }

        [DataMember(Name = "sum", EmitDefaultValue = false)]
        public decimal Totals { get; set; }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            Date = DateInternal.ToShortDateString();
        }

    }

    [DataContract(Namespace = "", Name = "sale")]
    public class SeasonOrderDTO
    {
        [DataMember(Name = "date", EmitDefaultValue = false)]
        public string Date { get; set; }


        public DateTime DateInternal { get; set; }

        [DataMember(Name = "st", EmitDefaultValue = false)]
        public int OrderStatus { get; set; }

        [DataMember(Name = "num", EmitDefaultValue = false)]
        public string NumberIn1S { get; set; }

        [DataMember(Name = "sum", EmitDefaultValue = false)]
        public decimal Totals { get; set; }

        [OnSerializing]
        public void OnSerializing(StreamingContext context)
        {
            Date = DateInternal.ToShortDateString();
        }

    }
}
