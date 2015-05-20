using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Api.EntityFramework;

namespace Api.Controllers
{
    public class NotificationModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/NotificationModels
        public IQueryable<NotificationModel> GetNotificationModels()
        {
            return db.NotificationModels;
        }

        // GET: api/NotificationModels/5
        [ResponseType(typeof(NotificationModel))]
        public async Task<IHttpActionResult> GetNotificationModel(int id)
        {
            NotificationModel notificationModel = await db.NotificationModels.FindAsync(id);
            if (notificationModel == null)
            {
                return NotFound();
            }

            return Ok(notificationModel);
        }

        // PUT: api/NotificationModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutNotificationModel(int id, NotificationModel notificationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != notificationModel.NotificationId)
            {
                return BadRequest();
            }

            db.Entry(notificationModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationModelExists(id))
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

        // POST: api/NotificationModels
        [ResponseType(typeof(NotificationModel))]
        public async Task<IHttpActionResult> PostNotificationModel(NotificationModel notificationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.NotificationModels.Add(notificationModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = notificationModel.NotificationId }, notificationModel);
        }

        // DELETE: api/NotificationModels/5
        [ResponseType(typeof(NotificationModel))]
        public async Task<IHttpActionResult> DeleteNotificationModel(int id)
        {
            NotificationModel notificationModel = await db.NotificationModels.FindAsync(id);
            if (notificationModel == null)
            {
                return NotFound();
            }

            db.NotificationModels.Remove(notificationModel);
            await db.SaveChangesAsync();

            return Ok(notificationModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool NotificationModelExists(int id)
        {
            return db.NotificationModels.Count(e => e.NotificationId == id) > 0;
        }
    }
}