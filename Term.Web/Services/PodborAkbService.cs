using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.DAL;
using Yst.Context;
using System.Web.Mvc;

using YstTerm.Models;
using System.Collections.Specialized;
using YstProject.Services;
using System.Resources;
using System.Linq.Expressions;
using Term.Utils;

namespace YstProject.Services
{
    public class PodborAkbService
    {
        private readonly AppDbContext _dbcontext;

        public PodborAkbService(): this( new AppDbContext())
        {
                
        }

        public PodborAkbService(AppDbContext dbContext)
        {
            _dbcontext = dbContext;
        }

      //  private static readonly AppDbContext _dbcontext = new AppDbContext();


        private IQueryable<CarAkbRecord> _GetByAuto(string brand, string car, int year, string engine)
        {
            return _dbcontext.CarAkbRecords.Where(p => p.Manufacturer == brand && p.Model == car && p.Year == year && p.Modification == engine);
        }

        public IEnumerable<string> GetBrands()
        {
            return _dbcontext.CarAkbRecords.Select(p => p.Manufacturer).Distinct().OrderBy(p => p).ToList();
        }

        public IEnumerable<string> GetCars(string brand)
        {
            return _dbcontext.CarAkbRecords.Where(p => p.Manufacturer == brand).Select(p => p.Model).Distinct().OrderBy(p => p).ToList();
        }

        public IEnumerable<int> GetYears(string brand, string car)
        {
            return _dbcontext.CarAkbRecords.Where(p => p.Manufacturer == brand && p.Model == car).Select(p => p.Year).Distinct();
        }

        public IEnumerable<string> GetEngines(string brand, string car, int year)
        {
            return _dbcontext.CarAkbRecords.Where(p => p.Manufacturer == brand && p.Model == car && p.Year == year).Select(p => p.Modification).Distinct().OrderBy(p => p);
        }

        public IEnumerable<PodborAkbViewResult> getViewResults(string brand, string car, int year, string engine)
        {
            return _GetByAuto(brand, car, year, engine).Select(p => new PodborAkbViewResult { Connection = p.Connection, Height = p.Height, Length = p.Length, Width = p.Width }).Distinct();
        }

        public IEnumerable<int> getCapacitiesFromCube(string brand, string car, int year, string engine, Size3d size)
        {
            return _GetByAuto(brand, car, year, engine).Where(p => p.Height <= size.Height && p.Width <= size.Width && p.Length <= size.Length).Select(p => p.Capacity).OrderBy(p => p).Distinct().ToArray();
        }

        public IEnumerable<Size3d> getSizes(string brand, string car, int year, string engine, Size3d maxsize)
        {
            return _GetByAuto(brand, car, year, engine).Where(p => p.Height <= maxsize.Height && p.Width <= maxsize.Width && p.Length <= maxsize.Length).Select(p => new Size3d { Height = p.Height, Width = p.Width, Length = p.Length }).ToArray();
        }

        public Size3d getMaxSize(string brand, string car, int year, string engine)
        {
            Size3d result = new Size3d();
            var resultsbyAuto = _GetByAuto(brand, car, year, engine);

            result = resultsbyAuto.GroupBy(p => 1).Select(p => new Size3d { Length = p.Max(res => res.Length), Height = p.Max(res => res.Height), Width = p.Max(res => res.Width) }).First();

            return result;
        }

        public bool getConnection(string brand, string car, int year, string engine)
        {
            var firstItem = _GetByAuto(brand, car, year, engine).First();
            return firstItem.Connection;
        }

    }
}
