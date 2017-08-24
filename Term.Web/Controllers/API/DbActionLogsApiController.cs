using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using Term.DAL;
using Yst.Context;

namespace Term.Web.Controllers.API
{

    //  временный контроллер для тестирования моб. приложений. Потом - удалить
   
    public class DbActionLogsApiController : ApiController
    {
        private AppDbContext db = new AppDbContext();

        // GET: api/DbActionLogsApi
        public IQueryable<DbActionLogs> GetDbActionLogs()
        {
            return db.DbActionLogs.Take(10000);
        }

        // GET: api/DbActionLogsApi/5
        [ResponseType(typeof(DbActionLogs))]
        public IHttpActionResult GetDbActionLogs(long id)
        {
            DbActionLogs dbActionLogs = db.DbActionLogs.Find(id);
            if (dbActionLogs == null)
            {
                return NotFound();
            }
            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");
            return Ok(dbActionLogs);
        }

        // PUT: api/DbActionLogsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDbActionLogs(long id, DbActionLogs dbActionLogs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != dbActionLogs.Id)
            {
                return BadRequest();
            }

            db.Entry(dbActionLogs).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DbActionLogsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/DbActionLogsApi
        [ResponseType(typeof(DbActionLogs))]
        public IHttpActionResult PostDbActionLogs(DbActionLogs dbActionLogs)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.DbActionLogs.Add(dbActionLogs);
            db.SaveChanges();

            HttpContext.Current.Response.AppendHeader("Access-Control-Allow-Origin", "*");

            return CreatedAtRoute("DefaultApi", new { id = dbActionLogs.Id }, dbActionLogs);
        }

        // DELETE: api/DbActionLogsApi/5
        [ResponseType(typeof(DbActionLogs))]
        public IHttpActionResult DeleteDbActionLogs(long id)
        {
            DbActionLogs dbActionLogs = db.DbActionLogs.Find(id);
            if (dbActionLogs == null)
            {
                return NotFound();
            }

            db.DbActionLogs.Remove(dbActionLogs);
            db.SaveChanges();

            return Ok(dbActionLogs);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool DbActionLogsExists(long id)
        {
            return db.DbActionLogs.Count(e => e.Id == id) > 0;
        }
    }
}