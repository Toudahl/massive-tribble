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
    public class IssueStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/IssueStatusModels
        public IQueryable<IssueStatusModel> GetIssueStatuses()
        {
            return db.IssueStatuses;
        }

        // GET: api/IssueStatusModels/5
        [ResponseType(typeof(IssueStatusModel))]
        public async Task<IHttpActionResult> GetIssueStatusModel(int id)
        {
            IssueStatusModel issueStatusModel = await db.IssueStatuses.FindAsync(id);
            if (issueStatusModel == null)
            {
                return NotFound();
            }

            return Ok(issueStatusModel);
        }

        // PUT: api/IssueStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutIssueStatusModel(int id, IssueStatusModel issueStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != issueStatusModel.IssueStatusId)
            {
                return BadRequest();
            }

            db.Entry(issueStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueStatusModelExists(id))
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

        // POST: api/IssueStatusModels
        [ResponseType(typeof(IssueStatusModel))]
        public async Task<IHttpActionResult> PostIssueStatusModel(IssueStatusModel issueStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.IssueStatuses.Add(issueStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = issueStatusModel.IssueStatusId }, issueStatusModel);
        }

        // DELETE: api/IssueStatusModels/5
        [ResponseType(typeof(IssueStatusModel))]
        public async Task<IHttpActionResult> DeleteIssueStatusModel(int id)
        {
            IssueStatusModel issueStatusModel = await db.IssueStatuses.FindAsync(id);
            if (issueStatusModel == null)
            {
                return NotFound();
            }

            db.IssueStatuses.Remove(issueStatusModel);
            await db.SaveChangesAsync();

            return Ok(issueStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IssueStatusModelExists(int id)
        {
            return db.IssueStatuses.Count(e => e.IssueStatusId == id) > 0;
        }
    }
}