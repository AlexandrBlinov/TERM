using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term.DAL
{
    /// <summary>
    /// Документ из 1С
    /// </summary>
  public  interface IDocument
    {
        
        Guid GuidIn1S { get; set; }
        string NumberIn1S { get; set; }
        DateTime DocDate { get; set; }
        
    }

    /// <summary>
    /// Табличная часть документа из 1С
    /// </summary>
    public interface IDocumentDetails {
        
        long Id { get; set; }        
        Guid GuidIn1S { get; set; }
    }
}
