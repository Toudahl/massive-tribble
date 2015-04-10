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
    public class ProfileLevelModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/ProfileLevelModels
        public IQueryable<ProfileLevelModel> GetProfileLevelModels()
        {
            return db.ProfileLevelModels;
        }

        // GET: api/ProfileLevelModels/5
        [ResponseType(typeof(ProfileLevelModel))]
        public async Task<IHttpActionResult> GetProfileLevelModel(int id)
        {
            ProfileLevelModel profileLevelModel = await db.ProfileLevelModels.FindAsync(id);
            if (profileLevelModel == null)
            {
                return NotFound();
            }

            return Ok(profileLevelModel);
        }

        // PUT: api/ProfileLevelModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfileLevelModel(int id, ProfileLevelModel profileLevelModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profileLevelModel.ProfileLevelId)
            {
                return BadRequest();
            }

            db.Entry(profileLevelModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileLevelModelExists(id))
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

        // POST: api/ProfileLevelModels
        [ResponseType(typeof(ProfileLevelModel))]
        public async Task<IHttpActionResult> PostProfileLevelModel(ProfileLevelModel profileLevelModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProfileLevelModels.Add(profileLevelModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = profileLevelModel.ProfileLevelId }, profileLevelModel);
        }

        // DELETE: api/ProfileLevelModels/5
        [ResponseType(typeof(ProfileLevelModel))]
        public async Task<IHttpActionResult> DeleteProfileLevelModel(int id)
        {
            ProfileLevelModel profileLevelModel = await db.ProfileLevelModels.FindAsync(id);
            if (profileLevelModel == null)
            {
                return NotFound();
            }

            db.ProfileLevelModels.Remove(profileLevelModel);
            await db.SaveChangesAsync();

            return Ok(profileLevelModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileLevelModelExists(int id)
        {
            return db.ProfileLevelModels.Count(e => e.ProfileLevelId == id) > 0;
        }
    }
}