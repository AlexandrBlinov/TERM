using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using YstProject.Services;
using YstStore.Domain;
using YstStore.Domain.Models;
using YstTerm.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Configuration;
using System.Net;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.Serialization;

namespace ConsoleApp
{
        

    static class PropertyInfoExtensions
    {
        public static  IList<string> GetPropsNotFilled(Object obj, params string[] PropertyNames)
        {

            List<string> propsNotFilled =  new List<string>();

            var properties = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //var i=default(properties[0].PropertyType);
            var objType=obj.GetType();

            foreach (var propName in PropertyNames)
            {   PropertyInfo propInfo= objType.GetProperty(propName);
            if (propInfo!=null )
            {
                var defaultValue = propInfo.PropertyType.IsValueType ? Activator.CreateInstance(propInfo.PropertyType):null;
                var value = propInfo.GetValue(obj, null);


                if ((propInfo.PropertyType.IsValueType && defaultValue.Equals(value))||
                !propInfo.PropertyType.IsValueType && value==null)
                {
                    Attribute attr = propInfo.GetCustomAttribute(typeof(DisplayNameAttribute));
                    if (attr!=null)
                        propsNotFilled.Add(((DisplayNameAttribute)(attr)).DisplayName);
                    else
                        propsNotFilled.Add(propInfo.Name);
                }
                
            }}
            
           return propsNotFilled;
            }
           
              
        

        public static int PropertyOrder(this PropertyInfo propInfo)
        {
            int output;
            var orderAttr = (DisplayAttribute)propInfo.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            output = orderAttr != null ? orderAttr.Order : Int32.MaxValue;
            return output;
        }
    }

    class Program
    {

       static Func<string, byte> sortInCart = (str) =>
        {  byte byteValue;
            var sortOrder =  new Dictionary<string, byte> { { "akb", 3 }, { "acc", 4 }, { "disk", 1 }, { "tyre", 2 } };
          
            return (byte)(sortOrder.TryGetValue(str, out byteValue) ? byteValue : 5);
            
        };

       static Func<string, byte> sortInCart1 =  delegate (string str){
       {
           byte byteValue;
           var sortOrder = new Dictionary<string, byte> { { "akb", 3 }, { "acc", 4 }, { "disk", 1 }, { "tyre", 2 } };

           return (byte)(sortOrder.TryGetValue(str, out byteValue) ? byteValue : 5);
       }
       };

        static string[] retStr(string name)
        {
            return new string[] {new String(name.Reverse().ToArray()) };
            //return new string[]{"1", "2"};
        }
        private static int bitnumber=2;

        private int PropertyOrder(PropertyInfo propInfo)
        {
            int output;
            var orderAttr = (DisplayAttribute)propInfo.GetCustomAttributes(typeof(DisplayAttribute), true).SingleOrDefault();
            output = orderAttr != null ? orderAttr.Order : Int32.MaxValue;
            return output;
        }

        
        static void Main(string[] args)
        {
            int i = 10;

            Console.WriteLine(String.Concat("123", i));

            PartnerPoint[] ppoints = new[] {new PartnerPoint{PartnerId="1",Address="123123",CompanyName="test"},
            new PartnerPoint{PartnerId="1",Address="123123",CompanyName=null},
            };

            using (var stream = new FileStream("c:\\points.txt", FileMode.Create))
            { 
            DataContractSerializer ser = new DataContractSerializer(typeof(PartnerPoint[]));
                ser.WriteObject(stream,ppoints);
            
            }

            return;

            string url = @"http://yst.ru/photos/" + Defaults.PathToFullImages["tyre"] + "01981s.jpg";
            

          
             uint width = 75; uint height = 75;
             DateTime t1 = DateTime.Now;
            WebRequest wr = WebRequest.Create(url);
            try
            {
                HttpWebResponse response = (HttpWebResponse)(wr.GetResponse());
                var remoteImage = (Bitmap)Bitmap.FromStream(response.GetResponseStream());


                var finalBmp = new Bitmap((int)width, (int)height);
                using (var g = Graphics.FromImage(finalBmp))
                {
                    g.InterpolationMode = InterpolationMode.High;
                    g.FillRectangle(Brushes.White, 0, 0, width, height);
                    g.DrawImage(remoteImage, 0, 0, width, height);

                }
                finalBmp.Save(@"c:\img1.jpg", ImageFormat.Jpeg);

            }
            catch { }

             DateTime t2 = DateTime.Now;

             Console.WriteLine((t2 - t1).Milliseconds);

             Console.ReadKey();

             return;
            Dictionary<string, string> dic = new Dictionary<string, string>() { {"first","first1" }, {"second", "second2" } };

           

            string[] ActionNamesForPartner = { "CreateOwn", "CreateOwnPost" };
            string[] ActionNamesForPoint = { "KeyWordEnter", "Edit", "EditPost", "SavePointProfile", "useragreement" };
            string ActionNameAgreement = "UserAgreement";
            string[] joined = ActionNamesForPartner.ToList().Union(ActionNamesForPoint).ToArray();

           bool isture = ActionNamesForPoint.Any(p=>p.Equals(ActionNameAgreement));

            PartnerPoint pp = new PartnerPoint {ContactFIO="test1", DaysToMainDepartment=-1} ;

            var list = PropertyInfoExtensions.GetPropsNotFilled(pp, "CompanyName","SaleDirection", "CompanyName2", "DaysToMainDepartment");
            

           // String[] pavParams = { pp.Address, pp.CompanyName, pp.ContactFIO, pp.Country, pp.PhoneNumber };

           /* var t = typeof(PartnerPoint);

            var propInfo=t.GetProperty("CompanyName");

            var attr = propInfo.GetCustomAttribute(typeof(DisplayNameAttribute));

            Console.Write(((DisplayNameAttribute)(attr)).DisplayName); */
            

           // return;


            //Console.WriteLine(PropertyInfoExtensions.PropertiesAreFilled(new PriceListPartnerTyreResult { Name = "First", Price = 10, PriceOfClient = 12 },  "row_number"));

            Console.ReadKey();

            return;


            PriceListTyreBase[] plarr = new PriceListPartnerTyreResult[] {
                new PriceListPartnerTyreResult{Name="First",Price=10,PriceOfClient=12},
                new PriceListPartnerTyreResult{Name="Second",Price=10,PriceOfClient=12},
                new PriceListPartnerTyreResult{Name="Third",Price=10,PriceOfClient=12},
            };
            
            //plarr
            List<PropertyInfo> pi=(plarr[0].GetType()).GetProperties().ToList();

            var newpi = pi.Where(p => p.PropertyOrder() != Int32.MaxValue).OrderBy(p => p.PropertyOrder()).ToArray();
           
           // TagBuilder
            return;

            List<string> strList = new List<string>(){ "akb", "acc", "disk", "tyre", "tyre" };

           var sortedList= strList.OrderBy(sortInCart1).ToList();

         /*   Enum.GetValues(typeof(OrderStatuses)).Cast<OrderStatuses>().Select(p => new
            {
                Id = (int)p,
                Name = EnumDescriptionProvider.GetDescription(p)

            }); */


            var nvc = new NameValueCollection{{"374", "Armenia"}, {"994", "Azerbaijan"},{"375", "Belarus"},{"32", "Belgium"},{"387", "Bosnia and Herzegovina"},
           {"359", "Bulgaria"},{"86", "China"},{"385", "Croatia"},{"420", "Czech Republic"},{"45", "Denmark"},{"372", "Estonia"},
           {"358", "Finland"},{"33", "France"},{"995", "Georgia"},{"49", "Germany"},{"36", "Hungary"}, {"7", "Kazakhstan"},
           {"371", "Latvia"},{"370", "Lithuania"},{"389", "Macedonia"},{"60", "Malaysia"},{"373", "Moldova"},{"31", "Netherlands"},
           {"47", "Norway"},{"48", "Poland"},{"40", "Romania"},{"381", "Serbia"},
           {"46", "Sweden"},{"992", "Turkmenistan"},{"380", "Ukraine"},{"1", "USА"},{"998", "Uzbekistan"}};


            var items = nvc.AllKeys.SelectMany(nvc.GetValues, (k, v) => new { Id = k, Value = v }).ToArray();

            var items2 = nvc.AllKeys.SelectMany(retStr, (k, v) => new { Id = k, Value = v }).ToArray();

        //   int [] a1= new int[]{1,2,3,4,5,6};
        //    int [] a2= new int[]{10,20,30,40,50,60};

         //  var itemsInt = a1.SelectMany(x => x).ToArray();

          //  var items1 = nvc.AllKeys.Select(p => new { Id = p, Value = nvc.Get(p) }).ToArray();

            //  byte b = 255;
            Department[] dep = { new Department { DepartmentId = 1, Name = "First" }, new Department { DepartmentId = 2, Name = "Second" } };

            Department[] dep1 = { new Department { DepartmentId = 1, Name = "First" }, new Department { DepartmentId = 2, Name = "Second" } };

            var dep3 = dep1.Union(dep);

            byte b2 = 3;

            


            var bit = (b2 & (1 << bitnumber - 1)) != 0;

            Console.Write(bit);
            Console.ReadKey();

            return;
            TyresPodborView tvm = new TyresPodborView();

            tvm.Width = "10";
            tvm.Diametr = "all";

            PropertyInfo propertyInfo = tvm.GetType().GetProperty("Width");
            propertyInfo.SetValue(tvm, null);

            /*
                        var web = new WebClient();
                        var result1 =web.DownloadStringTaskAsync("http://localhost:9090/TestHandler.ashx");

                        var result2 = web.DownloadStringTaskAsync("http://localhost:9090/TestHandler.ashx");

                        await Task.WhenAll(result1, result2);

                        Console.WriteLine(result);*/



        }

    }
}
