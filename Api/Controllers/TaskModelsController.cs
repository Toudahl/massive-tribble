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
    public class TaskModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/TaskModels
        public IQueryable<TaskModel> GetTaskModels()
        {
            return db.TaskModels;
        }

        // GET: api/TaskModels/5
        [ResponseType(typeof(TaskModel))]
        public async Task<IHttpActionResult> GetTaskModel(int id)
        {
            TaskModel taskModel = await db.TaskModels.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }

            return Ok(taskModel);
        }

        // PUT: api/TaskModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTaskModel(int id, TaskModel taskModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != taskModel.TaskId)
            {
                return BadRequest();
            }

            db.Entry(taskModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TaskModelExists(id))
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

        // POST: api/TaskModels
        [ResponseType(typeof(TaskModel))]
        public async Task<IHttpActionResult> PostTaskModel(TaskModel taskModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TaskModels.Add(taskModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = taskModel.TaskId }, taskModel);
        }

        // DELETE: api/TaskModels/5
        [ResponseType(typeof(TaskModel))]
        public async Task<IHttpActionResult> DeleteTaskModel(int id)
        {
            TaskModel taskModel = await db.TaskModels.FindAsync(id);
            if (taskModel == null)
            {
                return NotFound();
            }

            db.TaskModels.Remove(taskModel);
            await db.SaveChangesAsync();

            return Ok(taskModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TaskModelExists(int id)
        {
            return db.TaskModels.Count(e => e.TaskId == id) > 0;
        }
    }
}