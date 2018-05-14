using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Term.DAL
{
    

    /// <summary>
    /// Файлы
    /// </summary>
    public class File
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        [StringLength(100)]
        public string ContentType { get; set; }
        public byte[] Content { get; set; }

        public Guid GuidIn1S { get; set; }


    }


    public class DbUserActionLog
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }


        [MaxLength(200)]
        public string UserName { get; set; }
        
        [MaxLength(Byte.MaxValue)]
        public string UserAction { get; set; }

        [MaxLength(Byte.MaxValue)]
        public string IpAddress { get; set; }

        [DataType(DataType.DateTime)]
        public System.DateTime Date { get; set; }

    }

    /// <summary>
    /// Регламентные действия системы
    /// </summary>
    public class DbActionLogs
    {

        public DbActionLogs() { }

        public DbActionLogs(string action)
        {
            Date = DateTime.Now;
            DbAction = action;
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index]
        [MaxLength(200)]
        public string DbAction { get; set; }

        [Index]
        [DataType(DataType.DateTime)]
        public System.DateTime Date { get; set; }
    }


    public enum CartEventType
    {
     Added=1,
     Updated=0,
     Removed=-1
    }

    /// <summary>
    /// Фиксирует события связанные с добавлением/изменением/удалением из корзины в базе
    /// </summary>
    public class CartActionLog
    {

        public CartActionLog()
        {
            Date = DateTime.Now;
        }
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        public int ProductId { get; set; }

        public CartEventType Action { get; set; }

        public decimal Price { get; set; }
        public decimal PriceOfPoint { get; set; }
        public decimal PriceOfClient { get; set; }

        
        [DataType(DataType.DateTime)]
        public System.DateTime Date { get; set; }

    }


    public enum StatusOfNotification
    {
        Unread = 0,
        Read
    }

    /// <summary>
    /// Класс для уведомления
    /// </summary>
    /// 
    [Table("NotificationsForUsers")]
    public class NotificationForUser
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [MaxLength(50)]
        public string UserName { get; set; }

        [MaxLength(255)]
        public string Message { get; set; }

        [DataType(DataType.DateTime)]
        public System.DateTime Date { get; set; }

        /// <summary>
        /// 0 - not processed ,1 - processed
        /// </summary>
        public StatusOfNotification Status { get; set; }

    }

    [Table("HistoryOfOrderStatuses")]
    public class HistoryOfOrderstatus
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Index]
        public Guid GuidIn1S { get; set; }
        
        public OrderStatuses OrderStatus { get; set; }
        
        [DataType(DataType.DateTime)]
        public System.DateTime Date { get; set; }
        
        
    }

}
