using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Term.DAL;
using Term.Services;
using Term.Utils;
using Yst.Context;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTestDpdServices
    {

        DPDGeography2Service service ;

        private readonly auth _authdata = new auth
            {
                clientKey = "89CF47BC79EDDE98122968E978B4176783A774BC",
                clientNumber = 1007003132
            };

        private readonly string  moskowCode = "77000000000";
        private readonly string  yarCode = "76000001000";

        [TestInitialize]
        public void Initialize()
        {

         service = new DPDGeography2Service();
        
            

        }

        [TestMethod]
        public async void TestMethodGetTerminalsReturnsArray()
        {

            var result = service.getTerminalsSelfDelivery2(_authdata);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Length>0);
        }


        [TestMethod]
        public void TestMethodGetTerminalsAnalizeResultsEquals5()
        {
            service.Proxy = new WebProxy("127.0.0.1", 8888);
           

            var results = service.getTerminalsSelfDelivery2(_authdata);

            var moskowResults = results.Where(p => p.address.cityCode == moskowCode).ToArray();
            var yarResults = results.Where(p => p.address.cityCode == yarCode).ToArray();

           /* if (moskowResults.Count() == 4)
            {
            } */
            Assert.IsTrue(moskowResults.Count()>0);
            Assert.IsTrue(yarResults.Length==1);
        }

        [TestMethod]
        public void FillTerminalsWithDelivery()
        {
       

            var results = service.getTerminalsSelfDelivery2(_authdata);

            var moskowResults = results.ToArray();
            var db = new AppDbContext();
            foreach (var record in moskowResults)
            {
               
             var   term= new DpdTerminal();
                
             term.TerminalCode=   record.terminalCode;
             term.TerminalName=   record.terminalName;
               term.CountryCode=  record.address.countryCode;
                term.RegionCode=record.address.regionCode;
               term.CityCode= record.address.cityCode;
                term.CityName= record.address.cityName;
               term.Index= record.address.index;
               term.Street= record.address.street;
               term.StreetAbbr = record.address.streetAbbr;
               term.Structure= record.address.structure;
               term.Description= record.address.descript;
               term.HouseNo = record.address.houseNo; 

                foreach (var schedule in record.schedule)
                {
                    string str = "";
                    if (schedule.operation=="SelfDelivery")
                    foreach (var time1 in schedule.timetable)
                    {
                        str +="  "+time1.weekDays + " " + time1.workTime;
                     
                    }
                    term.Schedule = str;
                }
                db.DpdTerminals.Add(term);
                db.SaveChanges();
                //  record.schedule

            }
            
     //       var yarResults = results.Where(p => p.address.cityCode == yarCode).ToArray();

            /* if (moskowResults.Count() == 4)
             {
             } */
        
        }

        [TestMethod]
       public void GetSupposedDateReturnTrue()
        {
            DateTime startDate =new DateTime(2017, 1, 12);
            DateTime endDate=DateTimeHelper.AddDaysWithoutDaysOff(startDate, 1);

            Assert.IsTrue(endDate.Day==13);
            

        }

    }
}
