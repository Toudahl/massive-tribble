using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api.EntityFramework;

namespace Api.Controllers
{
    public class LogModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/LogModels
        public IQueryable<LogModel> GetLogModels()
        {
            return db.LogModels;
        }

        // GET: api/LogModels/5
        [ResponseType(typeof(LogModel))]
        public async Task<IHttpActionResult> GetLogModel(int id)
        {
            LogModel logModel = await db.LogModels.FindAsync(id);
            if (logModel == null)
            {
                return NotFound();
            }

            return Ok(logModel);
        }

        // PUT: api/LogModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutLogModel(int id, LogModel logModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != logModel.LogId)
            {
                return BadRequest();
            }

            db.Entry(logModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogModelExists(id))
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

        // POST: api/LogModels
        [ResponseType(typeof(LogModel))]
        public async Task<IHttpActionResult> PostLogModel(LogModel logModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.LogModels.Add(logModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = logModel.LogId }, logModel);
        }

        // DELETE: api/LogModels/5
        [ResponseType(typeof(LogModel))]
        public async Task<IHttpActionResult> DeleteLogModel(int id)
        {
            LogModel logModel = await db.LogModels.FindAsync(id);
            if (logModel == null)
            {
                return NotFound();
            }

            db.LogModels.Remove(logModel);
            await db.SaveChangesAsync();

            return Ok(logModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool LogModelExists(int id)
        {
            return db.LogModels.Count(e => e.LogId == id) > 0;
        }
    }
}