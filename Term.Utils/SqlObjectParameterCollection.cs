using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Term.Utils
{
    
    /// <summary>
    /// helper class for working with string- object pairs
    /// </summary>
    public class NameObjectValueCollection : Dictionary<string, object>
    {
       

        /// <summary>
        /// add new key-value object to dictionary chained invocation
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public new NameObjectValueCollection Add(string key, object value=null)
        {
            if (!this.ContainsKey(key))   base.Add(key, (object) value ?? DBNull.Value);

            return this;
        }

        /// <summary>
        /// populate array 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="keys"></param>
        public void GetParametersFromObject(object source, params string[] keys)
        {
            foreach (var propertyInfo in source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (keys.Any(key => key == propertyInfo.Name))
                    Add(propertyInfo.Name, propertyInfo.GetValue(source));

            }
        }
      

        

       
    }

    public sealed class SqlObjectParameterCollection : NameObjectValueCollection
    {
        /// <summary>
        /// transforms dictionary(this) to sqlparameter
        /// </summary>
        /// <returns></returns>
        public SqlParameter[] ToArray()
        {

            return this.Select(p => new SqlParameter(p.Key, (object)p.Value ?? DBNull.Value)).ToArray();
        }
    }

   

}
