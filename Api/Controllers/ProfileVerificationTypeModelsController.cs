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
    public class ProfileVerificationTypeModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/ProfileVerificationTypeModels
        public IQueryable<ProfileVerificationTypeModel> GetProfileVerificationTypeModels()
        {
            return db.ProfileVerificationTypeModels;
        }

        // GET: api/ProfileVerificationTypeModels/5
        [ResponseType(typeof(ProfileVerificationTypeModel))]
        public async Task<IHttpActionResult> GetProfileVerificationTypeModel(int id)
        {
            ProfileVerificationTypeModel profileVerificationTypeModel = await db.ProfileVerificationTypeModels.FindAsync(id);
            if (profileVerificationTypeModel == null)
            {
                return NotFound();
            }

            return Ok(profileVerificationTypeModel);
        }

        // PUT: api/ProfileVerificationTypeModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutProfileVerificationTypeModel(int id, ProfileVerificationTypeModel profileVerificationTypeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != profileVerificationTypeModel.ProfileVerificationTypeId)
            {
                return BadRequest();
            }

            db.Entry(profileVerificationTypeModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfileVerificationTypeModelExists(id))
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

        // POST: api/ProfileVerificationTypeModels
        [ResponseType(typeof(ProfileVerificationTypeModel))]
        public async Task<IHttpActionResult> PostProfileVerificationTypeModel(ProfileVerificationTypeModel profileVerificationTypeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ProfileVerificationTypeModels.Add(profileVerificationTypeModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = profileVerificationTypeModel.ProfileVerificationTypeId }, profileVerificationTypeModel);
        }

        // DELETE: api/ProfileVerificationTypeModels/5
        [ResponseType(typeof(ProfileVerificationTypeModel))]
        public async Task<IHttpActionResult> DeleteProfileVerificationTypeModel(int id)
        {
            ProfileVerificationTypeModel profileVerificationTypeModel = await db.ProfileVerificationTypeModels.FindAsync(id);
            if (profileVerificationTypeModel == null)
            {
                return NotFound();
            }

            db.ProfileVerificationTypeModels.Remove(profileVerificationTypeModel);
            await db.SaveChangesAsync();

            return Ok(profileVerificationTypeModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ProfileVerificationTypeModelExists(int id)
        {
            return db.ProfileVerificationTypeModels.Count(e => e.ProfileVerificationTypeId == id) > 0;
        }
    }
}