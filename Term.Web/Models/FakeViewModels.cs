using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Yst.Terminal.FakeViewModels
{
    public class FakeFormModel1
    {
        public int StartPrice { get; set; }
        public int EndPrice { get; set; }

        public string StartPriceString { get; set; }
        public string EndPriceString { get; set; }
    }

    public class FakeSecondViewModel
    {
        [Required]
        public string City { get; set; }

        [RegularExpression(@"^\d{11}$")]
        public string Phone { get; set; }
    }
}