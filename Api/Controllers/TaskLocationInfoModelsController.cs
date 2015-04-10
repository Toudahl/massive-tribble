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
    public class TaskLocationInfoModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/TaskLocationInfoModels
        public IQueryable<TaskLocationInfoModel> GetTaskLocationInfos()
        {
            return db.TaskLocationInfos;
        }

        // GET: api/TaskLocationInfoModels/5
        [ResponseType(typeof(TaskLocationInfoModel))]
        public async Task<IHttpActionResult> GetTaskLocationInfoModel(int id)
        {
            TaskLocationInfoModel taskLocationInfoModel = await db.TaskLocationInfos.FindAsync(id);
            if (taskLocationInfoModel == null)
            {
                return NotFound();
            }

            return Ok(taskLocationInfoModel);
        }

        // PUT: api/TaskLocationInfoModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTaskLocationInfoModel(int id, TaskLocationInfoModel taskLocationInfoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskLocationInfoModel.TaskLocationInfoId)
            {
                return BadRequest();
            }

            db.Entry(taskLocationInfoModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskLocationInfoModelExists(id))
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

        // POST: api/TaskLocationInfoModels
        [ResponseType(typeof(TaskLocationInfoModel))]
        public async Task<IHttpActionResult> PostTaskLocationInfoModel(TaskLocationInfoModel taskLocationInfoModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskLocationInfos.Add(taskLocationInfoModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taskLocationInfoModel.TaskLocationInfoId }, taskLocationInfoModel);
        }

        // DELETE: api/TaskLocationInfoModels/5
        [ResponseType(typeof(TaskLocationInfoModel))]
        public async Task<IHttpActionResult> DeleteTaskLocationInfoModel(int id)
        {
            TaskLocationInfoModel taskLocationInfoModel = await db.TaskLocationInfos.FindAsync(id);
            if (taskLocationInfoModel == null)
            {
                return NotFound();
            }

            db.TaskLocationInfos.Remove(taskLocationInfoModel);
            await db.SaveChangesAsync();

            return Ok(taskLocationInfoModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskLocationInfoModelExists(int id)
        {
            return db.TaskLocationInfos.Count(e => e.TaskLocationInfoId == id) > 0;
        }
    }
}