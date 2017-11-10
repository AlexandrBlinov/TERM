using PagedList;
using System;
using System.Collections.Generic;
using Term.DAL;
using Term.Soapmodels;
using Term.Web.Models;

namespace Yst.ViewModels
{
    public class ReportModel
    {
        public string PartnerId { get; set; }
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ReturnOfDefectiveResult ReturnItems { get; set; }
    }
    
    public class ClaimsViewModel : BaseViewPodborModel
    {
        public int? NumberIn1S { get; set; }
        public IPagedList<Claims> Claims { get; set; }
    }

    public class ClaimsViewWithDetails
    {
        public Claims Claim { get; set; }
        public IEnumerable<ClaimsDetails> ClaimDetails { get; set; }
    }
}