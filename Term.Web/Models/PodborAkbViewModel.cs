using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Term.DAL;
using System.ComponentModel;
using Yst.DropDowns;
using System.Configuration;
using PagedList;
using Yst.Context;
using Yst.Services;
using System.Web.Mvc;
using System.Collections.Specialized;


namespace YstTerm.Models
{
    public class PodborAkbViewModel
    {
        public PodborAkbViewModel()
        {
            Brands = Enumerable.Empty<string>();
            Models = Enumerable.Empty<string>();
            Years = Enumerable.Empty<int>();
            Capacities = Enumerable.Empty<int>();
            Modifications = Enumerable.Empty<string>();
            PodborAkbViewResults = Enumerable.Empty<PodborAkbViewResult>();
            AkbSearchResults = new List<AkbSearchResult>();
            GroupedResults = new Dictionary<int, IList<AkbSearchResult>>();
            SelectedVolumes = new HashSet<int>();
            Sizes = Enumerable.Empty<string>();
            SelectedSz = new List<string>();
            ShowProps = false;

        }
        public string brand { get; set; }
        public string carModel { get; set; }
        public int year { get; set; }
        public string engine { get; set; }
        public string volumes { get; set; }
        public string selectedsizes { get; set; }

        public IEnumerable<string> Brands { get; set; }
        public IEnumerable<string> Models { get; set; }
        public IEnumerable<int> Years { get; set; }
        public IEnumerable<string> Modifications { get; set; }
        public IEnumerable<PodborAkbViewResult> PodborAkbViewResults { get; set; }
        public IList<AkbSearchResult> AkbSearchResults { get; set; }
        public IDictionary<int, IList<AkbSearchResult>> GroupedResults { get; set; }
        public IEnumerable<int> Capacities { get; set; }
        public HashSet<int> SelectedVolumes { get; set; }
        public Size3d MaxSize { get; set; }
        public bool Connection { get; set; }
        public IEnumerable<string> Sizes { get; set; }
        public List<string> SelectedSz { get; set; }
        public bool ShowProps { get; set; }



    }
}
