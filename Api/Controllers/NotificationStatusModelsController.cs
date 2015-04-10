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
    public class NotificationStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/NotificationStatusModels
        public IQueryable<NotificationStatusModel> GetNotificationStatuses()
        {
            return db.NotificationStatuses;
        }

        // GET: api/NotificationStatusModels/5
        [ResponseType(typeof(NotificationStatusModel))]
        public async Task<IHttpActionResult> GetNotificationStatusModel(int id)
        {
            NotificationStatusModel notificationStatusModel = await db.NotificationStatuses.FindAsync(id);
            if (notificationStatusModel == null)
            {
                return NotFound();
            }

            return Ok(notificationStatusModel);
        }

        // PUT: api/NotificationStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotificationStatusModel(int id, NotificationStatusModel notificationStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notificationStatusModel.NotificationStatusId)
            {
                return BadRequest();
            }

            db.Entry(notificationStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationStatusModelExists(id))
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

        // POST: api/NotificationStatusModels
        [ResponseType(typeof(NotificationStatusModel))]
        public async Task<IHttpActionResult> PostNotificationStatusModel(NotificationStatusModel notificationStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NotificationStatuses.Add(notificationStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = notificationStatusModel.NotificationStatusId }, notificationStatusModel);
        }

        // DELETE: api/NotificationStatusModels/5
        [ResponseType(typeof(NotificationStatusModel))]
        public async Task<IHttpActionResult> DeleteNotificationStatusModel(int id)
        {
            NotificationStatusModel notificationStatusModel = await db.NotificationStatuses.FindAsync(id);
            if (notificationStatusModel == null)
            {
                return NotFound();
            }

            db.NotificationStatuses.Remove(notificationStatusModel);
            await db.SaveChangesAsync();

            return Ok(notificationStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationStatusModelExists(int id)
        {
            return db.NotificationStatuses.Count(e => e.NotificationStatusId == id) > 0;
        }
    }
}