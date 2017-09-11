using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term.Soapmodels
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class SoapProduct
    {


        private string codeField;

        private int quantityField;

        private string storageField;

        private int supplierIdField = 0;

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
        [System.Xml.Serialization.XmlElementAttribute("Storage")]
        public string Storage
        {
            get
            {
                return this.storageField;
            }
            set
            {
                this.storageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("SupplierId")]
        public int SupplierId
        {
            get
            {
                return this.supplierIdField;
            }
            set
            {
                this.supplierIdField = value;
            }
        }
    }



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class SoapProductSeasonOrder
    {

        private int rowNumberField;

        private string codeField;

        private int quantityField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("RowNumber")]
        public int RowNumber
        {
            get
            {
                return this.rowNumberField;
            }
            set
            {
                this.rowNumberField = value;
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
    }
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ResultDel
    {

        private bool successField;

        private string errorField;

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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ProductResult
    {

        private string codeField;

        private int quantityField;

        private string storageField;

        private string orderGUIDField;

        private string orderNumberIn1S;

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
        [System.Xml.Serialization.XmlElementAttribute("Storage")]
        public string Storage
        {
            get
            {
                return this.storageField;
            }
            set
            {
                this.storageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("OrderGUID", IsNullable = true)]
        public string OrderGUID
        {
            get
            {
                return this.orderGUIDField;
            }
            set
            {
                this.orderGUIDField = value;
            }
        }
        [System.Xml.Serialization.XmlElementAttribute("OrderNumber", IsNullable = true)]
        public string OrderNumberIn1S
        {
            get
            {
                return this.orderNumberIn1S;
            }
            set
            {
                this.orderNumberIn1S = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class Result
    {

        public string OrdersFromSuppliers { get; set; }


        private ProductResult[] productsField;

        private bool successField;

        private string errorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ProductsResult", IsNullable = false)]
        public ProductResult[] Products
        {
            get
            {
                return this.productsField;
            }
            set
            {
                this.productsField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ResultSeasonOrder
    {
        private ProductResult[] productsField;

        private bool successField;

        private string errorField;

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

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ProductsResult", IsNullable = false)]
        public ProductResult[] Products
        {
            get
            {
                return this.productsField;
            }
            set
            {
                this.productsField = value;
            }
        }

    }


    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class SeasonOrderAnalyseResult
    {

        private bool successField;
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


        private ProductAnalyseSeasonOrder[] products;



        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ProductAnalyseSeasonOrder", IsNullable = false)]
        public ProductAnalyseSeasonOrder[] Products
        {
            get
            {
                return this.products;
            }
            set
            {
                this.products = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.0.30319.34234")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ProductAnalyseSeasonOrder
    {

        [System.Xml.Serialization.XmlElementAttribute("Code")]
        public string Code { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Quantity")]
        public int Quantity { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("Reserve")]
        public int Reserve { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("Quantityfact")]
        public int QuantityFact { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Rest")]
        public int Rest { get; set; }



    }

    /*    <xs:complexType name="DeliferyInfo">
    <xs:sequence>
    <xs:element name="ContactFio" type="xs:string" nillable="true"/>
    <xs:element name="ContactPhone" type="xs:string" nillable="true"/>
    <xs:element name="CostOfDelivery" type="xs:integer" nillable="true"/>
    <xs:element name="RegionId" type="xs:string" nillable="true"/>
    <xs:element name="CityId" type="xs:string" nillable="true"/>
    <xs:element name="TerminalOrAddress" type="xs:boolean"/>
    <xs:element name="TerminalCode" type="xs:string" nillable="true"/>
    <xs:element name="PostalCode" type="xs:string" nillable="true"/>
    <xs:element name="StreetType" type="xs:string" nillable="true"/>
    <xs:element name="Street" type="xs:string" nillable="true"/>
    <xs:element name="BlockType" type="xs:string" nillable="true"/>
    <xs:element name="House" type="xs:string" nillable="true"/>
    </xs:sequence>
    </xs:complexType> */

    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1067.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class DeliveryInfo
    {

        [System.Xml.Serialization.XmlElementAttribute("ContactFio")]
        public string ContactFio { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ContactPhone")]
        public string ContactPhone { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("CostOfDelivery")]
        public int CostOfDelivery { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("RegionId")]
        public string RegionId { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("CityId")]
        public string CityId { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("TerminalOrAddress")]
        public bool TerminalOrAddress { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("TerminalCode")]
        public string TerminalCode { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("PostalCode")]
        public string PostalCode { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("StreetType")]
        public string StreetType { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Street")]
        public string Street { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("BlockType")]
        public string BlockType { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("House")]
        public string House { get; set; }


    }



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ProductPurchaseReturn
    {

        [System.Xml.Serialization.XmlElementAttribute("RowNumber")]
        public int RowNumber { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Code")]
        public string Code { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Count")]
        public int Count { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("SaleDate", IsNullable = true)]
        public string SaleDate { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("SaleNumber", IsNullable = true)]
        public string SaleNumber { get; set; }

    }



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ProductPurchaseReturnResult
    {

        [System.Xml.Serialization.XmlElementAttribute("RowNumber")]
        public int RowNumber { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Code")]
        public string Code { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("Count")]
        public int Count { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("Price", IsNullable = true)]
        public decimal? Price { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("SaleNumber", IsNullable = true)]
        public string SaleNumber { get; set; }


        [System.Xml.Serialization.XmlElementAttribute("Error")]
        public int Error { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("ErrorDescription")]
        public string ErrorDescription { get; set; }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ResultPurchaseReturn
    {

        private ProductPurchaseReturnResult[] productsField;

        private bool successField;

        private int errorField;

        private string purchaseReturnNumberField;

        private string purchaseReturnGUIDField;

        private string errorDescriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Products", IsNullable = false)]
        public ProductPurchaseReturnResult[] Products
        {
            get
            {
                return this.productsField;
            }
            set
            {
                this.productsField = value;
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
        public int Error
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
        public string PurchaseReturnNumber
        {
            get
            {
                return this.purchaseReturnNumberField;
            }
            set
            {
                this.purchaseReturnNumberField = value;
            }
        }

        /// <remarks/>
        public string PurchaseReturnGUID
        {
            get
            {
                return this.purchaseReturnGUIDField;
            }
            set
            {
                this.purchaseReturnGUIDField = value;
            }
        }

        /// <remarks/>
        public string ErrorDescription
        {
            get
            {
                return this.errorDescriptionField;
            }
            set
            {
                this.errorDescriptionField = value;
            }
        }
    }



    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class Debt
    {

        private System.DateTime dateField;

        private string numOrderField;

        private string numSaleField;

        private float sumSaleField;

        private float sumDebtField;

        private System.Nullable<float> sumPeniField;

        private System.DateTime datePayField;

        private string colDayDebtField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime Date
        {
            get
            {
                return this.dateField;
            }
            set
            {
                this.dateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public string NumOrder
        {
            get
            {
                return this.numOrderField;
            }
            set
            {
                this.numOrderField = value;
            }
        }

        /// <remarks/>
        public string NumSale
        {
            get
            {
                return this.numSaleField;
            }
            set
            {
                this.numSaleField = value;
            }
        }

        /// <remarks/>
        public float SumSale
        {
            get
            {
                return this.sumSaleField;
            }
            set
            {
                this.sumSaleField = value;
            }
        }

        /// <remarks/>
        public float SumDebt
        {
            get
            {
                return this.sumDebtField;
            }
            set
            {
                this.sumDebtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<float> SumPeni
        {
            get
            {
                return this.sumPeniField;
            }
            set
            {
                this.sumPeniField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime DatePay
        {
            get
            {
                return this.datePayField;
            }
            set
            {
                this.datePayField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "integer", IsNullable = true)]
        public string ColDayDebt
        {
            get
            {
                return this.colDayDebtField;
            }
            set
            {
                this.colDayDebtField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.6.1099.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ResultDebt
    {

        private Debt[] expiredDebtField;

        private Debt[] planDebtField;

        private bool successField;

        private string errorField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public Debt[] ExpiredDebt
        {
            get
            {
                return this.expiredDebtField;
            }
            set
            {
                this.expiredDebtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(IsNullable = true)]
        [System.Xml.Serialization.XmlArrayItemAttribute(IsNullable = false)]
        public Debt[] PlanDebt
        {
            get
            {
                return this.planDebtField;
            }
            set
            {
                this.planDebtField = value;
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class ResaltJoinOrder
    {

        private JoinOrderProduct[] productsField;

        private bool successField;

        private int errorField;

        private string errorDescriptionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Products", IsNullable = false)]
        public JoinOrderProduct[] Products
        {
            get
            {
                return this.productsField;
            }
            set
            {
                this.productsField = value;
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
        public int Error
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
        public string ErrorDescription
        {
            get
            {
                return this.errorDescriptionField;
            }
            set
            {
                this.errorDescriptionField = value;
            }
        }
    }


    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2053.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://37.1.84.50:8080/Terminal")]
    public partial class JoinOrderProduct
    {

        private string orderGuidField;

        private string codeField;

        private int quantityField;

        private System.Nullable<bool> isJoinedField;

        private bool isJoinedFieldSpecified;

        private string oldGuidField;

        private string orderNumberIn1SField;

        /// <remarks/>
        public string OrderGuid
        {
            get
            {
                return this.orderGuidField;
            }
            set
            {
                this.orderGuidField = value;
            }
        }

        /// <remarks/>
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
        [System.Xml.Serialization.XmlElementAttribute(IsNullable = true)]
        public System.Nullable<bool> IsJoined
        {
            get
            {
                return this.isJoinedField;
            }
            set
            {
                this.isJoinedField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IsJoinedSpecified
        {
            get
            {
                return this.isJoinedFieldSpecified;
            }
            set
            {
                this.isJoinedFieldSpecified = value;
            }
        }

        /// <remarks/>
        public string OldGuid
        {
            get
            {
                return this.oldGuidField;
            }
            set
            {
                this.oldGuidField = value;
            }
        }

        /// <remarks/>
        public string OrderNumberIn1S
        {
            get
            {
                return this.orderNumberIn1SField;
            }
            set
            {
                this.orderNumberIn1SField = value;
            }
        }
    }
}


