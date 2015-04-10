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
    public class IssueModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/IssueModels
        public IQueryable<IssueModel> GetIssueModels()
        {
            return db.IssueModels;
        }

        // GET: api/IssueModels/5
        [ResponseType(typeof(IssueModel))]
        public async Task<IHttpActionResult> GetIssueModel(int id)
        {
            IssueModel issueModel = await db.IssueModels.FindAsync(id);
            if (issueModel == null)
            {
                return NotFound();
            }

            return Ok(issueModel);
        }

        // PUT: api/IssueModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutIssueModel(int id, IssueModel issueModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != issueModel.IssueId)
            {
                return BadRequest();
            }

            db.Entry(issueModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueModelExists(id))
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

        // POST: api/IssueModels
        [ResponseType(typeof(IssueModel))]
        public async Task<IHttpActionResult> PostIssueModel(IssueModel issueModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.IssueModels.Add(issueModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = issueModel.IssueId }, issueModel);
        }

        // DELETE: api/IssueModels/5
        [ResponseType(typeof(IssueModel))]
        public async Task<IHttpActionResult> DeleteIssueModel(int id)
        {
            IssueModel issueModel = await db.IssueModels.FindAsync(id);
            if (issueModel == null)
            {
                return NotFound();
            }

            db.IssueModels.Remove(issueModel);
            await db.SaveChangesAsync();

            return Ok(issueModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IssueModelExists(int id)
        {
            return db.IssueModels.Count(e => e.IssueId == id) > 0;
        }
    }
}