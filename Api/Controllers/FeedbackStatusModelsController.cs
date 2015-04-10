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
    public class FeedbackStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/FeedbackStatusModels
        public IQueryable<FeedbackStatusModel> GetFeedbackStatuses()
        {
            return db.FeedbackStatuses;
        }

        // GET: api/FeedbackStatusModels/5
        [ResponseType(typeof(FeedbackStatusModel))]
        public async Task<IHttpActionResult> GetFeedbackStatusModel(int id)
        {
            FeedbackStatusModel feedbackStatusModel = await db.FeedbackStatuses.FindAsync(id);
            if (feedbackStatusModel == null)
            {
                return NotFound();
            }

            return Ok(feedbackStatusModel);
        }

        // PUT: api/FeedbackStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutFeedbackStatusModel(int id, FeedbackStatusModel feedbackStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != feedbackStatusModel.FeedbackStatusId)
            {
                return BadRequest();
            }

            db.Entry(feedbackStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FeedbackStatusModelExists(id))
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

        // POST: api/FeedbackStatusModels
        [ResponseType(typeof(FeedbackStatusModel))]
        public async Task<IHttpActionResult> PostFeedbackStatusModel(FeedbackStatusModel feedbackStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.FeedbackStatuses.Add(feedbackStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = feedbackStatusModel.FeedbackStatusId }, feedbackStatusModel);
        }

        // DELETE: api/FeedbackStatusModels/5
        [ResponseType(typeof(FeedbackStatusModel))]
        public async Task<IHttpActionResult> DeleteFeedbackStatusModel(int id)
        {
            FeedbackStatusModel feedbackStatusModel = await db.FeedbackStatuses.FindAsync(id);
            if (feedbackStatusModel == null)
            {
                return NotFound();
            }

            db.FeedbackStatuses.Remove(feedbackStatusModel);
            await db.SaveChangesAsync();

            return Ok(feedbackStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FeedbackStatusModelExists(int id)
        {
            return db.FeedbackStatuses.Count(e => e.FeedbackStatusId == id) > 0;
        }
    }
}