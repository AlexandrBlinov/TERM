using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;

namespace YstProject.Services
{
    /// <summary>
    /// helper class on how to use properties
    /// </summary>
    public class SeasonProductService :BaseService
    {
        PersistedKeyValueStorage _constants;
        /// <summary>
        /// term constants private constants for russian clients
        /// </summary>
        private static readonly string StartDateSteel = "season.stockitems.startdate.steel";
        private static readonly string EndDateSteel = "season.stockitems.enddate.steel";
        private static readonly string StartDateAlloy = "season.stockitems.startdate.alloy";
        private static readonly string EndDateAlloy = "season.stockitems.enddate.alloy";

        private  bool? _hasAlloyOffers;
        private  bool? _hasSteelOffers;



        public SeasonProductService()
        {
            _constants= new PersistedKeyValueStorage(DbContext);
        }

      

        private bool SeasonStockItemsExist(WheelType wheeltype)
        {
            return DbContext.Set<SeasonStockItem>().Any(p => p.Product.WheelType == wheeltype && p.Active);
        }

        public int AmountOfOffers
        {
            get {
                return ((HasAlloyOffers ? 1 : 0) + (HasSteelOffers ? 1 : 0));
            }
        }
        public bool HasSteelOffers { get {
                if (_hasSteelOffers.HasValue) return _hasSteelOffers.Value;
                if (!IsPartner)  {  _hasSteelOffers = false; return _hasSteelOffers.Value; };
                bool itemsAreExist = SeasonStockItemsExist(WheelType.Steel);
                if (!itemsAreExist) { _hasSteelOffers = false; return _hasSteelOffers.Value; }
                /// foreign partner
                if (IsForeignPartner)   _hasSteelOffers = true; 
                 else _hasSteelOffers = DateTime.Now > _constants.Get<DateTime>(StartDateSteel) && DateTime.Now < _constants.Get<DateTime>(EndDateSteel);
                return _hasSteelOffers.Value;
        } }

        public bool HasAlloyOffers
        {
            get
            {
                if (_hasAlloyOffers.HasValue) return _hasAlloyOffers.Value;
                if (!IsPartner) { _hasAlloyOffers = false; return _hasAlloyOffers.Value; };
                bool itemsAreExist = SeasonStockItemsExist(WheelType.Alloy);
                if (!itemsAreExist) { _hasAlloyOffers = false; return _hasAlloyOffers.Value; }
                /// foreign partner
                if (this.IsForeignPartner) _hasAlloyOffers = true;
                else _hasAlloyOffers= DateTime.Now > _constants.Get<DateTime>(StartDateAlloy) && DateTime.Now < _constants.Get<DateTime>(EndDateAlloy);
                return _hasAlloyOffers.Value;

            }
        }
    }
}