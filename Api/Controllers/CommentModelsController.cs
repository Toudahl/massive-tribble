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
    public class CommentModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/CommentModels
        public IQueryable<CommentModel> GetCommentModels()
        {
            return db.CommentModels;
        }

        // GET: api/CommentModels/5
        [ResponseType(typeof(CommentModel))]
        public async Task<IHttpActionResult> GetCommentModel(int id)
        {
            CommentModel commentModel = await db.CommentModels.FindAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }

            return Ok(commentModel);
        }

        // PUT: api/CommentModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutCommentModel(int id, CommentModel commentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != commentModel.CommentId)
            {
                return BadRequest();
            }

            db.Entry(commentModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CommentModelExists(id))
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

        // POST: api/CommentModels
        [ResponseType(typeof(CommentModel))]
        public async Task<IHttpActionResult> PostCommentModel(CommentModel commentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CommentModels.Add(commentModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = commentModel.CommentId }, commentModel);
        }

        // DELETE: api/CommentModels/5
        [ResponseType(typeof(CommentModel))]
        public async Task<IHttpActionResult> DeleteCommentModel(int id)
        {
            CommentModel commentModel = await db.CommentModels.FindAsync(id);
            if (commentModel == null)
            {
                return NotFound();
            }

            db.CommentModels.Remove(commentModel);
            await db.SaveChangesAsync();

            return Ok(commentModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CommentModelExists(int id)
        {
            return db.CommentModels.Count(e => e.CommentId == id) > 0;
        }
    }
}