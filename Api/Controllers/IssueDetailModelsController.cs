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
    public class IssueDetailModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/IssueDetailModels
        public IQueryable<IssueDetailModel> GetIssueDetailModels()
        {
            return db.IssueDetailModels;
        }

        // GET: api/IssueDetailModels/5
        [ResponseType(typeof(IssueDetailModel))]
        public async Task<IHttpActionResult> GetIssueDetailModel(int id)
        {
            IssueDetailModel issueDetailModel = await db.IssueDetailModels.FindAsync(id);
            if (issueDetailModel == null)
            {
                return NotFound();
            }

            return Ok(issueDetailModel);
        }

        // PUT: api/IssueDetailModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutIssueDetailModel(int id, IssueDetailModel issueDetailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != issueDetailModel.IssueDetailId)
            {
                return BadRequest();
            }

            db.Entry(issueDetailModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!IssueDetailModelExists(id))
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

        // POST: api/IssueDetailModels
        [ResponseType(typeof(IssueDetailModel))]
        public async Task<IHttpActionResult> PostIssueDetailModel(IssueDetailModel issueDetailModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.IssueDetailModels.Add(issueDetailModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = issueDetailModel.IssueDetailId }, issueDetailModel);
        }

        // DELETE: api/IssueDetailModels/5
        [ResponseType(typeof(IssueDetailModel))]
        public async Task<IHttpActionResult> DeleteIssueDetailModel(int id)
        {
            IssueDetailModel issueDetailModel = await db.IssueDetailModels.FindAsync(id);
            if (issueDetailModel == null)
            {
                return NotFound();
            }

            db.IssueDetailModels.Remove(issueDetailModel);
            await db.SaveChangesAsync();

            return Ok(issueDetailModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool IssueDetailModelExists(int id)
        {
            return db.IssueDetailModels.Count(e => e.IssueDetailId == id) > 0;
        }
    }
}