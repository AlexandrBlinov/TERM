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
    public class PodborAkbViewResult
    {
        public int Length { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public bool Connection { get; set; }

        public string Size
        {
            get
            {
                return String.Format("{0}x{1}x{2}", Length, Width, Height);
            }
        }

        public override string ToString()
        {
            return String.Format("{0}x{1}x{2} {3}", Length, Width, Height, Connection ? "п.п." : "о.п.");
        }

        public string Stamp
        {
            get { return String.Format("{0}x{1}x{2}_{3}", Length, Width, Height, Connection ? 1 : 0); }
        }
    }

    public class Size3d
    {
        public int Length { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public override string ToString()
        {
            return String.Format("{0}x{1}x{2}", this.Length, this.Width, this.Height);
        }
    }
}
