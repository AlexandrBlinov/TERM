using System;
using Yst.ViewModels;
using Term.DAL;
using YstProject.Services;

namespace Yst.Services
{
    public class ClaimLocalService : BaseService
    {
        public bool CreateClaimInLocal(ClaimsContainer cc, Guid guid, int number)
        {
            try
            {
                var newClaim = new Claims
                {
                    GuidIn1S = guid,
                    NumberIn1S = number,
                    PartnerId = GetPartnerId(),
                    ClaimDate = DateTime.Now,
                    Status = "Новая"
                };
                DbContext.Claims.Add(newClaim);
                foreach (var item in cc.ClaimItems)
                {
                    string AdditionalInfo = string.Empty;
                    if (item.Condition == 1) AdditionalInfo += "Неставленый"; else AdditionalInfo += "Ставленый";
                    if (item.Auto != null) AdditionalInfo += ", Марка авто: " + item.Auto;
                    if (item.TireRunning != null) AdditionalInfo += ", Пробег шины: " + item.TireRunning;
                    if (item.Position != null) AdditionalInfo += ", Расположение: " + item.Position;
                    var newItem = new ClaimsDetails
                    {
                        GuidIn1S = guid,
                        ProductId = item.ProductId,
                        Name = item.Name,
                        ProductKind = item.ProductType.ToString(),
                        Count = item.Count,
                        SaleNumber = item.DocNumber,
                        SaleDate = item.DocDate,
                        Condition = item.ConditionDescription,
                        Defect = item.DefectDescription,
                        DefectComment = item.DetailedDescriptionDefect,
                        DefectCome = String.Empty,
                        InspectionDate = new DateTime(1900, 1, 1),
                        Resolution = String.Empty,
                        ProductionDate = item.DateOfManufacture.ToShortDateString(),
                        AdditionalInfo = AdditionalInfo
                    };
                    DbContext.ClaimsDetails.Add(newItem);
                }
                DbContext.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

    }
}