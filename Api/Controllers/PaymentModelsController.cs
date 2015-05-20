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
    public class PaymentModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/PaymentModels
        public IQueryable<PaymentModel> GetPaymentModels()
        {
            return db.PaymentModels;
        }

        // GET: api/PaymentModels/5
        [ResponseType(typeof(PaymentModel))]
        public async Task<IHttpActionResult> GetPaymentModel(int id)
        {
            PaymentModel paymentModel = await db.PaymentModels.FindAsync(id);
            if (paymentModel == null)
            {
                return NotFound();
            }

            return Ok(paymentModel);
        }

        // PUT: api/PaymentModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPaymentModel(int id, PaymentModel paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentModel.PaymentId)
            {
                return BadRequest();
            }

            db.Entry(paymentModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentModelExists(id))
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

        // POST: api/PaymentModels
        [ResponseType(typeof(PaymentModel))]
        public async Task<IHttpActionResult> PostPaymentModel(PaymentModel paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PaymentModels.Add(paymentModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = paymentModel.PaymentId }, paymentModel);
        }

        // DELETE: api/PaymentModels/5
        [ResponseType(typeof(PaymentModel))]
        public async Task<IHttpActionResult> DeletePaymentModel(int id)
        {
            PaymentModel paymentModel = await db.PaymentModels.FindAsync(id);
            if (paymentModel == null)
            {
                return NotFound();
            }

            db.PaymentModels.Remove(paymentModel);
            await db.SaveChangesAsync();

            return Ok(paymentModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentModelExists(int id)
        {
            return db.PaymentModels.Count(e => e.PaymentId == id) > 0;
        }
    }
}