using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using DocumentFormat.OpenXml.Drawing.Charts;


namespace YstProject.Models
{
    /// <summary>
    /// Модель для  возврата через web-api
    /// </summary>
    [DataContract]
    public class CityWithRegionDto
    {
        [DataMember]
        public string RegionName { get; set; }

        /// <summary>
        /// КЛАДР города
        /// </summary>
        
        [DataMember]
        public string Id { get; set; }

        
        [DataMember]
        public string DpdCode { get; set; }

        /// <summary>
        /// Полное название города
        /// </summary>
        
        [DataMember]
        public string Name { get; set; }


        
        [DataMember]
        public string Abbreviation { get; set; }

        [DataMember]
        public int RegionId { get; set; }

    }


    /// <summary>
    /// Модель терминала для DPD
    /// </summary>
    /// 
    [DataContract]
    public class DpdTerminalDto
    {
        [DataMember]
        public  string Address{ get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Schedule { get; set; }
    }
}