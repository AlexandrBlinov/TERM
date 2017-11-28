namespace Term.ClaimServiceSoap
{
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ServiceClaimSoapBinding", Namespace = "http://yst.ru/claim")]
    public partial class ServiceClaim : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        private System.Threading.SendOrPostCallback SetClaimOperationCompleted;

        private bool useDefaultCredentialsSetExplicitly;

        /// <remarks/>
        public ServiceClaim(string url = "http://37.1.84.50:8080/YST/ws/ServiceClaim")
        {
            this.Url = url;
            if ((this.IsLocalFileSystemWebService(this.Url) == true))
            {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else
            {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        public new string Url
        {
            get
            {
                return base.Url;
            }
            set
            {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true)
                && (this.useDefaultCredentialsSetExplicitly == false))
                && (this.IsLocalFileSystemWebService(value) == false)))
                {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }

        public new bool UseDefaultCredentials
        {
            get
            {
                return base.UseDefaultCredentials;
            }
            set
            {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }

        /// <remarks/>
        public event SetClaimCompletedEventHandler SetClaimCompleted;

        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://yst.ru/claim#ServiceClaim:SetClaim", RequestNamespace = "http://yst.ru/claim", ResponseNamespace = "http://yst.ru/claim", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute("return", IsNullable = true)]
        public ResultAll SetClaim([System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)] [System.Xml.Serialization.XmlArrayItemAttribute("Claims", IsNullable = false)] Claim[] ArrayOfClaims, bool IsProbe, bool IsFormula)
        {
            object[] results = this.Invoke("SetClaim", new object[] {
ArrayOfClaims,
IsProbe,
IsFormula});
            return ((ResultAll)(results[0]));
        }

        /// <remarks/>
        public void SetClaimAsync(Claim[] ArrayOfClaims, bool IsProbe, bool IsFormula)
        {
            this.SetClaimAsync(ArrayOfClaims, IsProbe, IsFormula, null);
        }

        /// <remarks/>
        public void SetClaimAsync(Claim[] ArrayOfClaims, bool IsProbe, bool IsFormula, object userState)
        {
            if ((this.SetClaimOperationCompleted == null))
            {
                this.SetClaimOperationCompleted = new System.Threading.SendOrPostCallback(this.OnSetClaimOperationCompleted);
            }
            this.InvokeAsync("SetClaim", new object[] {
ArrayOfClaims,
IsProbe,
IsFormula}, this.SetClaimOperationCompleted, userState);
        }

        private void OnSetClaimOperationCompleted(object arg)
        {
            if ((this.SetClaimCompleted != null))
            {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.SetClaimCompleted(this, new SetClaimCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
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

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://yst.ru/claim")]
    public partial class Claim
    {

        private string iNNField;

        private string responsibleField;

        private string codeField;

        private int quantityField;

        private string numberOfDocField;

        private System.DateTime dateOfDocField;

        private int conditionField;

        private string dateOfProductionField;

        private int typeOfDefectField;

        private string commentaryField;

        private string hTTPLinkToAPictureField;

        private string runField;

        private string modelField;

        private string serialNumberField;

        private System.Nullable<float> diameterDiskField;

        private System.Nullable<float> widthRimDiskField;

        private System.Nullable<float> pressureField;

        private string locationField;

        private System.Nullable<System.DateTime> yearCarField;

        private string engineCapacityField;

        private System.Nullable<System.DateTime> dateDefectField;

        private System.Nullable<System.DateTime> dateSaleKlientField;

        private string infoKlientField;

        private string infoFormulaField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("INN")]
        public string INN
        {
            get
            {
                return this.iNNField;
            }
            set
            {
                this.iNNField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Responsible")]
        public string Responsible
        {
            get
            {
                return this.responsibleField;
            }
            set
            {
                this.responsibleField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Code")]
        public string Code
        {
            get
            {
                return this.codeField;
            }
            set
            {
                this.codeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Quantity")]
        public int Quantity
        {
            get
            {
                return this.quantityField;
            }
            set
            {
                this.quantityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("NumberOfDoc")]
        public string NumberOfDoc
        {
            get
            {
                return this.numberOfDocField;
            }
            set
            {
                this.numberOfDocField = value;
            }
        }

        /// <remarks/>
        public System.DateTime DateOfDoc
        {
            get
            {
                return this.dateOfDocField;
            }
            set
            {
                this.dateOfDocField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Condition")]
        public int Condition
        {
            get
            {
                return this.conditionField;
            }
            set
            {
                this.conditionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DateOfProduction")]
        public string DateOfProduction
        {
            get
            {
                return this.dateOfProductionField;
            }
            set
            {
                this.dateOfProductionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TypeOfDefect")]
        public int TypeOfDefect
        {
            get
            {
                return this.typeOfDefectField;
            }
            set
            {
                this.typeOfDefectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Commentary")]
        public string Commentary
        {
            get
            {
                return this.commentaryField;
            }
            set
            {
                this.commentaryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("HTTPLinkToAPicture")]
        public string HTTPLinkToAPicture
        {
            get
            {
                return this.hTTPLinkToAPictureField;
            }
            set
            {
                this.hTTPLinkToAPictureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Run")]
        public string Run
        {
            get
            {
                return this.runField;
            }
            set
            {
                this.runField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Model")]
        public string Model
        {
            get
            {
                return this.modelField;
            }
            set
            {
                this.modelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string SerialNumber
        {
            get
            {
                return this.serialNumberField;
            }
            set
            {
                this.serialNumberField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> DiameterDisk
        {
            get
            {
                return this.diameterDiskField;
            }
            set
            {
                this.diameterDiskField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> WidthRimDisk
        {
            get
            {
                return this.widthRimDiskField;
            }
            set
            {
                this.widthRimDiskField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> Pressure
        {
            get
            {
                return this.pressureField;
            }
            set
            {
                this.pressureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string Location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> YearCar
        {
            get
            {
                return this.yearCarField;
            }
            set
            {
                this.yearCarField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string EngineCapacity
        {
            get
            {
                return this.engineCapacityField;
            }
            set
            {
                this.engineCapacityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> DateDefect
        {
            get
            {
                return this.dateDefectField;
            }
            set
            {
                this.dateDefectField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date", IsNullable = true)]
        public System.Nullable<System.DateTime> DateSaleKlient
        {
            get
            {
                return this.dateSaleKlientField;
            }
            set
            {
                this.dateSaleKlientField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string InfoKlient
        {
            get
            {
                return this.infoKlientField;
            }
            set
            {
                this.infoKlientField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string InfoFormula
        {
            get
            {
                return this.infoFormulaField;
            }
            set
            {
                this.infoFormulaField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://yst.ru/claim")]
    public partial class Result
    {

        private bool errorField;

        private string contractorErrorField;

        private string sNPErrorField;

        private string nomenclatureErrorField;

        private string quantityErrorField;

        private string conditionErrorField;

        private string typeErrorField;

        private string numberErrorField;

        /// <remarks/>
        public bool Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }

        /// <remarks/>
        public string ContractorError
        {
            get
            {
                return this.contractorErrorField;
            }
            set
            {
                this.contractorErrorField = value;
            }
        }

        /// <remarks/>
        public string SNPError
        {
            get
            {
                return this.sNPErrorField;
            }
            set
            {
                this.sNPErrorField = value;
            }
        }

        /// <remarks/>
        public string NomenclatureError
        {
            get
            {
                return this.nomenclatureErrorField;
            }
            set
            {
                this.nomenclatureErrorField = value;
            }
        }

        /// <remarks/>
        public string QuantityError
        {
            get
            {
                return this.quantityErrorField;
            }
            set
            {
                this.quantityErrorField = value;
            }
        }

        /// <remarks/>
        public string ConditionError
        {
            get
            {
                return this.conditionErrorField;
            }
            set
            {
                this.conditionErrorField = value;
            }
        }

        /// <remarks/>
        public string TypeError
        {
            get
            {
                return this.typeErrorField;
            }
            set
            {
                this.typeErrorField = value;
            }
        }

        /// <remarks/>
        public string NumberError
        {
            get
            {
                return this.numberErrorField;
            }
            set
            {
                this.numberErrorField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://yst.ru/claim")]
    public partial class ArrayOfResults
    {

        private Result[] resultsField;

        private System.Xml.XmlElement[] anyField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Results")]
        public Result[] Results
        {
            get
            {
                return this.resultsField;
            }
            set
            {
                this.resultsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyElementAttribute()]
        public System.Xml.XmlElement[] Any
        {
            get
            {
                return this.anyField;
            }
            set
            {
                this.anyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAnyAttributeAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get
            {
                return this.anyAttrField;
            }
            set
            {
                this.anyAttrField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://yst.ru/claim")]
    public partial class ResultAll
    {

        private ArrayOfResults arrayOfResultsField;

        private bool successField;

        private string[] claimGUIDField;

        private string[] claimNumberField;

        private string errorField;

        /// <remarks/>
        public ArrayOfResults ArrayOfResults
        {
            get
            {
                return this.arrayOfResultsField;
            }
            set
            {
                this.arrayOfResultsField = value;
            }
        }

        /// <remarks/>
        public bool Success
        {
            get
            {
                return this.successField;
            }
            set
            {
                this.successField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ClaimGUID", IsNullable = true)]
        public string[] ClaimGUID
        {
            get
            {
                return this.claimGUIDField;
            }
            set
            {
                this.claimGUIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ClaimNumber", IsNullable = true)]
        public string[] ClaimNumber
        {
            get
            {
                return this.claimNumberField;
            }
            set
            {
                this.claimNumberField = value;
            }
        }

        /// <remarks/>
        public string Error
        {
            get
            {
                return this.errorField;
            }
            set
            {
                this.errorField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    public delegate void SetClaimCompletedEventHandler(object sender, SetClaimCompletedEventArgs e);

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.6.1055.0")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class SetClaimCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs
    {

        private object[] results;

        internal SetClaimCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
        base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ResultAll Result
        {
            get
            {
                this.RaiseExceptionIfNecessary();
                return ((ResultAll)(this.results[0]));
            }
        }
    }
}
