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
    public class TaskStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/TaskStatusModels
        public IQueryable<TaskStatusModel> GetTaskStatuses()
        {
            return db.TaskStatuses;
        }

        // GET: api/TaskStatusModels/5
        [ResponseType(typeof(TaskStatusModel))]
        public async Task<IHttpActionResult> GetTaskStatusModel(int id)
        {
            TaskStatusModel taskStatusModel = await db.TaskStatuses.FindAsync(id);
            if (taskStatusModel == null)
            {
                return NotFound();
            }

            return Ok(taskStatusModel);
        }

        // PUT: api/TaskStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTaskStatusModel(int id, TaskStatusModel taskStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskStatusModel.TaskStatusId)
            {
                return BadRequest();
            }

            db.Entry(taskStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskStatusModelExists(id))
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

        // POST: api/TaskStatusModels
        [ResponseType(typeof(TaskStatusModel))]
        public async Task<IHttpActionResult> PostTaskStatusModel(TaskStatusModel taskStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskStatuses.Add(taskStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taskStatusModel.TaskStatusId }, taskStatusModel);
        }

        // DELETE: api/TaskStatusModels/5
        [ResponseType(typeof(TaskStatusModel))]
        public async Task<IHttpActionResult> DeleteTaskStatusModel(int id)
        {
            TaskStatusModel taskStatusModel = await db.TaskStatuses.FindAsync(id);
            if (taskStatusModel == null)
            {
                return NotFound();
            }

            db.TaskStatuses.Remove(taskStatusModel);
            await db.SaveChangesAsync();

            return Ok(taskStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskStatusModelExists(int id)
        {
            return db.TaskStatuses.Count(e => e.TaskStatusId == id) > 0;
        }
    }
}