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
    public class ProfileStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/ProfileStatusModels
        public IQueryable<ProfileStatusModel> GetProfileStatuses()
        {
            return db.ProfileStatuses;
        }

        // GET: api/ProfileStatusModels/5
        [ResponseType(typeof(ProfileStatusModel))]
        public async Task<IHttpActionResult> GetProfileStatusModel(int id)
        {
            ProfileStatusModel profileStatusModel = await db.ProfileStatuses.FindAsync(id);
            if (profileStatusModel == null)
            {
                return NotFound();
            }

            return Ok(profileStatusModel);
        }

        // PUT: api/ProfileStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfileStatusModel(int id, ProfileStatusModel profileStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profileStatusModel.ProfileStatusId)
            {
                return BadRequest();
            }

            db.Entry(profileStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileStatusModelExists(id))
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

        // POST: api/ProfileStatusModels
        [ResponseType(typeof(ProfileStatusModel))]
        public async Task<IHttpActionResult> PostProfileStatusModel(ProfileStatusModel profileStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProfileStatuses.Add(profileStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = profileStatusModel.ProfileStatusId }, profileStatusModel);
        }

        // DELETE: api/ProfileStatusModels/5
        [ResponseType(typeof(ProfileStatusModel))]
        public async Task<IHttpActionResult> DeleteProfileStatusModel(int id)
        {
            ProfileStatusModel profileStatusModel = await db.ProfileStatuses.FindAsync(id);
            if (profileStatusModel == null)
            {
                return NotFound();
            }

            db.ProfileStatuses.Remove(profileStatusModel);
            await db.SaveChangesAsync();

            return Ok(profileStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileStatusModelExists(int id)
        {
            return db.ProfileStatuses.Count(e => e.ProfileStatusId == id) > 0;
        }
    }
}