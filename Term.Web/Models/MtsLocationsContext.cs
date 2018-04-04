using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Term.Web.Models
{
    public class MtsLocationsContext :DbContext
    {
        public MtsLocationsContext()
        : base("MtsLocations")
    {
        }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<LocationsRecord> LocationsRecords { get; set; }
    }
}