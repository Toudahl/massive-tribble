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
    public class SessionModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/SessionModels
        public IQueryable<SessionModel> GetSessionModels()
        {
            return db.SessionModels;
        }

        // GET: api/SessionModels/5
        [ResponseType(typeof(SessionModel))]
        public async Task<IHttpActionResult> GetSessionModel(int id)
        {
            SessionModel sessionModel = await db.SessionModels.FindAsync(id);
            if (sessionModel == null)
            {
                return NotFound();
            }

            return Ok(sessionModel);
        }

        // PUT: api/SessionModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutSessionModel(int id, SessionModel sessionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sessionModel.SessionId)
            {
                return BadRequest();
            }

            db.Entry(sessionModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SessionModelExists(id))
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

        // POST: api/SessionModels
        [ResponseType(typeof(SessionModel))]
        public async Task<IHttpActionResult> PostSessionModel(SessionModel sessionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.SessionModels.Add(sessionModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = sessionModel.SessionId }, sessionModel);
        }

        // DELETE: api/SessionModels/5
        [ResponseType(typeof(SessionModel))]
        public async Task<IHttpActionResult> DeleteSessionModel(int id)
        {
            SessionModel sessionModel = await db.SessionModels.FindAsync(id);
            if (sessionModel == null)
            {
                return NotFound();
            }

            db.SessionModels.Remove(sessionModel);
            await db.SaveChangesAsync();

            return Ok(sessionModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SessionModelExists(int id)
        {
            return db.SessionModels.Count(e => e.SessionId == id) > 0;
        }
    }
}