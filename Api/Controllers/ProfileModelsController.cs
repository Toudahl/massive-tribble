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
    public class ProfileModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/ProfileModels
        public IQueryable<ProfileModel> GetProfileModels()
        {
            return db.ProfileModels;
        }

        // GET: api/ProfileModels/5
        [ResponseType(typeof(ProfileModel))]
        public async Task<IHttpActionResult> GetProfileModel(int id)
        {
            ProfileModel profileModel = await db.ProfileModels.FindAsync(id);
            if (profileModel == null)
            {
                return NotFound();
            }

            return Ok(profileModel);
        }

        // PUT: api/ProfileModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfileModel(int id, ProfileModel profileModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profileModel.ProfileId)
            {
                return BadRequest();
            }

            db.Entry(profileModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileModelExists(id))
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

        // POST: api/ProfileModels
        [ResponseType(typeof(ProfileModel))]
        public async Task<IHttpActionResult> PostProfileModel(ProfileModel profileModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProfileModels.Add(profileModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = profileModel.ProfileId }, profileModel);
        }

        // DELETE: api/ProfileModels/5
        [ResponseType(typeof(ProfileModel))]
        public async Task<IHttpActionResult> DeleteProfileModel(int id)
        {
            ProfileModel profileModel = await db.ProfileModels.FindAsync(id);
            if (profileModel == null)
            {
                return NotFound();
            }

            db.ProfileModels.Remove(profileModel);
            await db.SaveChangesAsync();

            return Ok(profileModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileModelExists(int id)
        {
            return db.ProfileModels.Count(e => e.ProfileId == id) > 0;
        }
    }
}