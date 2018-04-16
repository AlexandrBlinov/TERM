using System;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Term.Soapmodels;

namespace Term.Services
{
   
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="ServiceTerminalSoapBinding", Namespace="http://37.1.84.50:8080/ServiceTerminal")]
    public partial class ServiceTerminal : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        
        public static string FormatForDate {
            get { return "yyyyMMdd"; }
        }
        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public ServiceTerminal() {
          
            this.Url = "http://37.1.84.50:8080/YST/ws/serviceterminal"; 
            //this.Url = "http://37.1.84.50:8080/test/ws/ServiceTerminal";
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /*
        protected override XmlWriter GetWriterForMessage(SoapClientMessage message,int bufferSize)
        {
            
            Debug.Write(message.ToString());
            var writer= base.GetWriterForMessage(message,bufferSize);

            return writer;


        }

        protected override XmlReader GetReaderForMessage(SoapClientMessage message, int bufferSize)
        {
            Debug.Write(message.ToString());
            // convert stream to string
            StreamReader reader = new StreamReader(message.Stream);
            string text = reader.ReadToEnd();

            return base.GetReaderForMessage(message, bufferSize);


        } */

        /// <summary>
        /// Вызов создания заказа (без информации о доставке)
        /// </summary>
        
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:CreateOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public Result CreateOrder(string Partner, int PointId, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods, string Comment, string ShippingDay, bool isReserve, bool TransportCompany)
        {
            object[] results = this.Invoke("CreateOrder", new object[] {
                        Partner,
                        PointId,
                        Goods,
                        Comment,
            ShippingDay,isReserve,TransportCompany});
            return ((Result)(results[0]));
        }



        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:CreateOrder3", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public Result CreateOrder3(string Partner, int PointId, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string Comment, 
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string ShippingDay, bool isReserve, bool TransportCompany, 
            bool Prepay, bool IsStar, int WayOfDelivery, string AddressId , string TkId, string DeliveryDate2,int CaseForLogistik ,bool IsSeasonAdjournment , int DayOfWeekToDeliver=0)
        {
            object[] results = this.Invoke("CreateOrder3", new object[] {
                        Partner,
                        PointId,
                        Goods,
                        Comment,
                        ShippingDay,
                        isReserve,
                        TransportCompany,
                        Prepay,
                        IsStar,
                        WayOfDelivery,
                        AddressId,
                        TkId,
                        DeliveryDate2,
                        CaseForLogistik,
                        IsSeasonAdjournment,
                        DayOfWeekToDeliver
            });
            return ((Result)(results[0]));
        }

        /// <summary>
        /// Вызов создания заказа (с информацией о доставке)
        /// </summary>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:CreateOrderWithTransportDa" +
            "ta", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public Result CreateOrderWithTransportData(string Partner, int PointId, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string Comment, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string ShippingDay, bool isReserve, bool TransportCompany, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] DeliveryInfo DeliveryInfo, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string DeliveryDay)
        {
            object[] results = this.Invoke("CreateOrderWithTransportData", new object[] {
                        Partner,
                        PointId,
                        Goods,
                        Comment,
                        ShippingDay,
                        isReserve,
                        TransportCompany,DeliveryInfo,
                        DeliveryDay
            });
            return ((Result)(results[0]));
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:DeleteOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDel DeleteOrder(string OrderGUID)
        {
            object[] results = this.Invoke("DeleteOrder", new object[] {
                        OrderGUID});
            return ((ResultDel)(results[0]));
        }

    


        //-- begin added manually
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:ChangeOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDel ChangeOrder(string OrderGUID, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods, string Comment, bool isReserve, string ShippingDay, bool TransportCompany, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] DeliveryInfo DeliveryInfo, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string DeliveryDay, int WayOfDelivery,bool IsStar , string AddressId, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string TkId)
        {
            object[] results = this.Invoke("ChangeOrder", new object[] {
                        OrderGUID,
                        Goods,
                        Comment,
                        isReserve,
                        ShippingDay,
                        TransportCompany,
                        DeliveryInfo,
                        DeliveryDay,
                        WayOfDelivery,
                        IsStar,
                        AddressId,
                        TkId });
            return ((ResultDel)(results[0]));
        }




        //-- begin added manually
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:ChangeOrderStatus", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDel ChangeOrderStatus(string OrderGUID, int  Status)
        {
            object[] results = this.Invoke("ChangeOrderStatus", new object[] {
                        OrderGUID,Status});
            return ((ResultDel)(results[0]));
        }


        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:CreatePurchaseReturn", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultPurchaseReturn CreatePurchaseReturn(string PartnerId, int PointId, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] ProductPurchaseReturn[] Goods, bool isTest=false)
        {
            object[] results = this.Invoke("CreatePurchaseReturn", new object[] {
                        PartnerId,
                        PointId,
                        Goods,
                        isTest
            });
            return ((ResultPurchaseReturn)(results[0]));
        }


        /// <summary>
        /// Получить долги
        /// </summary>
        /// <param name="PartnerId"></param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:GetDebt", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDebt GetDebt([System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string PartnerId)
        {
            object[] results = this.Invoke("GetDebt", new object[] {
                        PartnerId});
            return ((ResultDebt)(results[0]));
        }


        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:GetDatesOfShipment", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Date", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)]
        public string[] GetDatesOfShipment(string Partner, string AddressId)
        {
            object[] results = this.Invoke("GetDatesOfShipment", new object[] {
                        Partner,
                        AddressId});
            return ((string[])(results[0]));
        }




        /// <summary>
        /// Объединить заказы
        /// </summary>
        /// <param name="Goods"></param>
        /// <param name="Address"></param>
        /// <param name="ShippingDay"></param>
        /// <param name="Comment"></param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:JoinOrders", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResaltJoinOrder JoinOrders([System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] JoinOrderProduct[] Goods, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string Address, string ShippingDay, string Comment)
        {
            object[] results = this.Invoke("JoinOrders", new object[] {
                        Goods,
                        Address,
                        ShippingDay,
                        Comment});
            return ((ResaltJoinOrder)(results[0]));
        }

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:ReturnOfDefectiveReport", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ReturnOfDefectiveResult ReturnOfDefectiveReport(string Partner, [System.Xml.Serialization.XmlElementAttribute(DataType = "date")] System.DateTime StartDate, [System.Xml.Serialization.XmlElementAttribute(DataType = "date")] System.DateTime StopDate)
        {
            object[] results = this.Invoke("ReturnOfDefectiveReport", new object[] {
                    Partner,
                    StartDate,
                    StopDate});
            return ((ReturnOfDefectiveResult)(results[0]));
        }

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:WheelsTestReport", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("Element", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)]
        public ReturnWheelsTest[] WheelsTestReport(string Partner, [System.Xml.Serialization.XmlElementAttribute(DataType = "date")] System.DateTime StartDate, [System.Xml.Serialization.XmlElementAttribute(DataType = "date")] System.DateTime StopDate)
        {
            object[] results = this.Invoke("WheelsTestReport", new object[] {
                    Partner,
                    StartDate,
                    StopDate});
            return ((ReturnWheelsTest[])(results[0]));
        }




        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:ChangeOrders", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlArrayAttribute("return")]
        [return: System.Xml.Serialization.XmlArrayItemAttribute("OrderGuid", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)]
        public string[] ChangeOrders(string DeliveryDate, int WayOfDelivery, string AddressId, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string TkId, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] System.Nullable<bool> DayOfWeekToDeliver, [System.Xml.Serialization.XmlArrayItemAttribute("OrderGuid", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] string[] OrderGuids)
        {
            object[] results = this.Invoke("ChangeOrders", new object[] {
                        DeliveryDate,
                        WayOfDelivery,
                        AddressId,
                        TkId,
                        DayOfWeekToDeliver,
                        OrderGuids});
            return ((string[])(results[0]));
        }


        private bool IsLocalFileSystemWebService(string url)
        {
            if (((url == null)
                        || (url == string.Empty)))
            {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024)
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0)))
            {
                return true;
            }
            return false;
        }
        
    }

   

}
