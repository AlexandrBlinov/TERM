using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.Context;
using YstProject.Services;
using YstTerm.Models;

namespace Term.Web.Services
{
    public class PodborTyreDiskService
    {
        private readonly AppDbContext _dbcontext;

        public PodborTyreDiskService(): this( new AppDbContext())
        {
                
        }

        public PodborTyreDiskService(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

        public IEnumerable<TiporazmerByCarModelView> GetResults(string brand, string car, int year, string engine)
        {

            var jmt = new List<TiporazmerByCarModelView>();

            if (String.IsNullOrEmpty(brand) || String.IsNullOrEmpty(car) || String.IsNullOrEmpty(engine))
                return jmt;

            ICollection<CarRecord> carrecords = _dbcontext.Set<CarRecord>().Where(p => p.VendorName == brand && p.CarName == car && (p.BeginYear <= year && (p.EndYear >= year || p.EndYear == 0)) && p.ModificationName == engine).OrderBy(p => p.Diameter).ToList();

            foreach (var cr in carrecords)
                jmt.Add(new TiporazmerByCarModelView(cr));

            return jmt;

        }
       
       /* public TiporazmerByCarModelView[] getJsonArray()
        {
            engine = engine.Replace("~", "/");
            arr = _products.getResults(brand, model, year, engine).ToArray();
            return arr;
        } */


        public IEnumerable<string> GetBrands()
        {
            return _dbcontext.Set<CarRecord>().Select(p => p.VendorName).Distinct().OrderBy(p => p).ToList();
        }

        public IEnumerable<string> GetCars(string brand)
        {
            if (String.IsNullOrEmpty(brand)) return Enumerable.Empty<string>();

            return _dbcontext.Set<CarRecord>().Where(p => p.VendorName == brand).Select(p => p.CarName).Distinct().OrderBy(p => p).ToList();
        }


        public IEnumerable<int> GetYears(string brand, string car)
        {
            int endYear = Defaults.Endyear, beginYear = Defaults.BeginYear;

            if (String.IsNullOrEmpty(brand) || String.IsNullOrEmpty(car))
                yield break;
            try
            {
                beginYear = _dbcontext.Set<CarRecord>().Where(p => p.VendorName == brand && p.CarName == car).Select(p => p.BeginYear).Min();
                var endYears = _dbcontext.Set<CarRecord>().Where(p => p.VendorName == brand && p.CarName == car).Select(p => p.EndYear).Distinct().OrderBy(p => p).ToArray();

                if (endYears.Count() > 0 && endYears[0] > 0)
                    endYear = endYears[0];


            }
            catch { yield break; }
            for (int i = beginYear; i <= endYear; i++) yield return i;

        }

        public IEnumerable<string> GetEngines(string brand, string car, int year)
        {
            return _dbcontext.Set<CarRecord>().Where(p => p.VendorName == brand && p.CarName == car && (p.BeginYear <= year && (p.EndYear >= year || p.EndYear == 0))).Select(p => p.ModificationName).Distinct().OrderBy(p => p).ToArray();

        }

    }
}