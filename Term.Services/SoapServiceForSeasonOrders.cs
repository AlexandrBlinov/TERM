using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Term.Soapmodels;


namespace Term.Services
{
   /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.18408")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ServiceTerminalSoapBinding", Namespace = "http://37.1.84.50:8080/ServiceTerminal")]
    public partial class SoapServiceForSeasonOrders : System.Web.Services.Protocols.SoapHttpClientProtocol
    {
        private bool useDefaultCredentialsSetExplicitly;
        /// <remarks/>
        public SoapServiceForSeasonOrders(string url = "http://37.1.84.50:8080/YST/ws/serviceterminal")
        {
            this.Url = url;//global::YstProject.Properties.Settings.Default.YstProject_WebReferenceTerm_ServiceTerminal;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
            this.PreAuthenticate = true;
            
        }

       public void SetAlternateUrl()
       {
           this.Url = @"http://37.1.84.50:8080/YST_TURKEY/ws/ServiceTerminal";
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

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:CreateSeasonOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultSeasonOrder CreateSeasonOrder(string Partner, /* [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProductSeasonOrder[] Goods, */
            int PointId,
            [System.Xml.Serialization.XmlArrayItemAttribute("Products", Namespace = "http://37.1.84.50:8080/Terminal", IsNullable = false)] SoapProduct[] Goods,
            string Storage,
            [System.Xml.Serialization.XmlElementAttribute(DataType = "date")] System.DateTime ActionDate,
            [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)] string Comment,
             string SeasonOrderGUID)
        {


            object[] results = this.Invoke("CreateSeasonOrder", new object[] {
                        Partner,
                        PointId,
                        Goods,
                        Storage,
                        ActionDate,
                        Comment,
                        SeasonOrderGUID});
            return ((ResultSeasonOrder)(results[0]));
        }



        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:DeleteRestoreSeasonOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public ResultDel DeleteRestoreSeasonOrder(string SeasonOrderGUID, bool Delete)
        {
            object[] results = this.Invoke("DeleteRestoreSeasonOrder", new object[] {
                        SeasonOrderGUID,
                        Delete});
            return ((ResultDel)(results[0]));
        }
        /// <remarks/>
        /// 

        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://37.1.84.50:8080/ServiceTerminal#ServiceTerminal:AnalyseSeasonOrder", RequestNamespace = "http://37.1.84.50:8080/ServiceTerminal", ResponseNamespace = "http://37.1.84.50:8080/ServiceTerminal", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return")]
        public SeasonOrderAnalyseResult AnalyseSeasonOrder(string SeasonOrderGUID)
        {
            object[] results = this.Invoke("AnalyseSeasonOrder", new object[] {
                        SeasonOrderGUID});
            return ((SeasonOrderAnalyseResult)(results[0]));
        }


    }
}