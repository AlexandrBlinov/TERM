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
        public ReturnWheelsTest[] ReturnWheelsTest { get; set; }
    }
    
    public class ClaimsViewModel : BaseViewPodborModel
    {
        public string ProductId { get; set; }
        public string SaleNumber { get; set; }
        public int? NumberIn1S { get; set; }
        public IPagedList<Claims> Claims { get; set; }
    }

    public class ClaimsViewWithDetails
    {
        public Claims Claim { get; set; }
        public IEnumerable<ClaimsDetails> ClaimDetails { get; set; }
    }

    public class NewClaimViewModel
    {
        public string PartnerName { get; set; }
        public string Inn { get; set; }
        public string Address { get; set; }
        public string Fio { get; set; }
        public string Phone { get; set; }
    }

    public class ClaimItemData
    {
        public string DocNumber { get; set; }
        public DateTime DocDate { get; set; }
        public DateTime DefectDate { get; set; }
        public DateTime EndSaleDate { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public int Count { get; set; }
        public DateTime DateOfManufacture { get; set; }
        public string SerialNumber { get; set; }
        public int Defect { get; set; }
        public string DefectDescription { get; set; }
        public string DetailedDescriptionDefect { get; set; }
        public int Condition { get; set; }
        public string ConditionDescription { get; set; }
        public string Auto { get; set; }
        public string TireRunning { get; set; }
        public string Pressure { get; set; }
        public string Position { get; set; }
        public string WheelWidth { get; set; }
        public string WheelDiametr { get; set; }
        public Term.DAL.ProductType ProductType { get; set; }
        public string AutoYear { get; set; }
        public string AutoEngine { get; set; }
    }

    public class ClaimsContainer
    {
        public string ContactFIO { get; set; }
        public string ContactPhone { get; set; }
        public IEnumerable<ClaimItemData> ClaimItems { get; set; }
    }
}