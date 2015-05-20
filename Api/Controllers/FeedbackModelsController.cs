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
    public class FeedbackModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/FeedbackModels
        public IQueryable<FeedbackModel> GetFeedbackModels()
        {
            return db.FeedbackModels;
        }

        // GET: api/FeedbackModels/5
        [ResponseType(typeof(FeedbackModel))]
        public async Task<IHttpActionResult> GetFeedbackModel(int id)
        {
            FeedbackModel feedbackModel = await db.FeedbackModels.FindAsync(id);
            if (feedbackModel == null)
            {
                return NotFound();
            }

            return Ok(feedbackModel);
        }

        // PUT: api/FeedbackModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFeedbackModel(int id, FeedbackModel feedbackModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedbackModel.FeedbackId)
            {
                return BadRequest();
            }

            db.Entry(feedbackModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackModelExists(id))
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

        // POST: api/FeedbackModels
        [ResponseType(typeof(FeedbackModel))]
        public async Task<IHttpActionResult> PostFeedbackModel(FeedbackModel feedbackModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FeedbackModels.Add(feedbackModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = feedbackModel.FeedbackId }, feedbackModel);
        }

        // DELETE: api/FeedbackModels/5
        [ResponseType(typeof(FeedbackModel))]
        public async Task<IHttpActionResult> DeleteFeedbackModel(int id)
        {
            FeedbackModel feedbackModel = await db.FeedbackModels.FindAsync(id);
            if (feedbackModel == null)
            {
                return NotFound();
            }

            db.FeedbackModels.Remove(feedbackModel);
            await db.SaveChangesAsync();

            return Ok(feedbackModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FeedbackModelExists(int id)
        {
            return db.FeedbackModels.Count(e => e.FeedbackId == id) > 0;
        }
    }
}