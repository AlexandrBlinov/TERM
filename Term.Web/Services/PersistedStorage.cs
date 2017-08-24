using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yst.Context;
using Term.DAL;

namespace YstProject.Services
{
        
    public class PersistedKeyValueStorage
    {
        AppDbContext _dbContext;
        public PersistedKeyValueStorage():this (new AppDbContext())
        {

        }
        public PersistedKeyValueStorage(AppDbContext dbContext)
        { _dbContext = dbContext; }

        public T Get<T>(string Key) {
            var record=_dbContext.Set<StoredKeyValueItem>().FirstOrDefault(p => p.Key == Key);
            if (record != null) return (T)Convert.ChangeType(record.Value, typeof(T));
            //if (record != null) return (T)(object)(record.Value);
                return default(T);                
    }
     

        public void Set<T>(string Key, T Value)
        {
            var record = _dbContext.Set<StoredKeyValueItem>().FirstOrDefault(p => p.Key == Key);
            if (record != null) record.Value = Value.ToString();
            
            else _dbContext.Set<StoredKeyValueItem>().Add(new StoredKeyValueItem { Key = Key, Value = Value.ToString() });

            _dbContext.SaveChanges();

        }


    }
}