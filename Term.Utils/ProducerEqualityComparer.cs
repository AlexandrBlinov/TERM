using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Term.DAL;

namespace Term.Utils
{
    /// <summary>
    /// Нужен для списка производителей дисков (чтобы не отображать одинаковых производителей (используется в Union))
    /// </summary>
   public class ProducerEqualityComparer : IEqualityComparer<Producer>
{
    public bool Equals(Producer x, Producer y)
    {
        // If reference same object including null then return true
        return object.ReferenceEquals(x, y) || x.ProducerId == y.ProducerId;



    }

    public int GetHashCode(Producer obj)
    {
        return obj.ProducerId;
    }
}
    
}
