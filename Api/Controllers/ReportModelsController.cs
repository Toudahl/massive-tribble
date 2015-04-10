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
    public class ReportModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/ReportModels
        public IQueryable<ReportModel> GetReportModels()
        {
            return db.ReportModels;
        }

        // GET: api/ReportModels/5
        [ResponseType(typeof(ReportModel))]
        public async Task<IHttpActionResult> GetReportModel(int id)
        {
            ReportModel reportModel = await db.ReportModels.FindAsync(id);
            if (reportModel == null)
            {
                return NotFound();
            }

            return Ok(reportModel);
        }

        // PUT: api/ReportModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutReportModel(int id, ReportModel reportModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != reportModel.ReportId)
            {
                return BadRequest();
            }

            db.Entry(reportModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReportModelExists(id))
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

        // POST: api/ReportModels
        [ResponseType(typeof(ReportModel))]
        public async Task<IHttpActionResult> PostReportModel(ReportModel reportModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ReportModels.Add(reportModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = reportModel.ReportId }, reportModel);
        }

        // DELETE: api/ReportModels/5
        [ResponseType(typeof(ReportModel))]
        public async Task<IHttpActionResult> DeleteReportModel(int id)
        {
            ReportModel reportModel = await db.ReportModels.FindAsync(id);
            if (reportModel == null)
            {
                return NotFound();
            }

            db.ReportModels.Remove(reportModel);
            await db.SaveChangesAsync();

            return Ok(reportModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ReportModelExists(int id)
        {
            return db.ReportModels.Count(e => e.ReportId == id) > 0;
        }
    }
}