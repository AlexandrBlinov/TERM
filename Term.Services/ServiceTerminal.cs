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
        public ResultDel ChangeOrder(string OrderGUID, [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods, string Comment, bool isReserve, string ShippingDay, bool TransportCompany, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] DeliveryInfo DeliveryInfo, [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string DeliveryDay)
        {
            object[] results = this.Invoke("ChangeOrder", new object[] {
                        OrderGUID,
                        Goods,
                        Comment,isReserve,ShippingDay,
            TransportCompany,DeliveryInfo,DeliveryDay});
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


        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:GetDebt", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDebt GetDebt([System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string PartnerId)
        {
            object[] results = this.Invoke("GetDebt", new object[] {
                        PartnerId});
            return ((ResultDebt)(results[0]));
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
