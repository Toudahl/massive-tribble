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
    public class PaymentTypeModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/PaymentTypeModels
        public IQueryable<PaymentTypeModel> GetPaymentTypeModels()
        {
            return db.PaymentTypeModels;
        }

        // GET: api/PaymentTypeModels/5
        [ResponseType(typeof(PaymentTypeModel))]
        public async Task<IHttpActionResult> GetPaymentTypeModel(int id)
        {
            PaymentTypeModel paymentTypeModel = await db.PaymentTypeModels.FindAsync(id);
            if (paymentTypeModel == null)
            {
                return NotFound();
            }

            return Ok(paymentTypeModel);
        }

        // PUT: api/PaymentTypeModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPaymentTypeModel(int id, PaymentTypeModel paymentTypeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentTypeModel.PaymentTypeId)
            {
                return BadRequest();
            }

            db.Entry(paymentTypeModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentTypeModelExists(id))
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

        // POST: api/PaymentTypeModels
        [ResponseType(typeof(PaymentTypeModel))]
        public async Task<IHttpActionResult> PostPaymentTypeModel(PaymentTypeModel paymentTypeModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PaymentTypeModels.Add(paymentTypeModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = paymentTypeModel.PaymentTypeId }, paymentTypeModel);
        }

        // DELETE: api/PaymentTypeModels/5
        [ResponseType(typeof(PaymentTypeModel))]
        public async Task<IHttpActionResult> DeletePaymentTypeModel(int id)
        {
            PaymentTypeModel paymentTypeModel = await db.PaymentTypeModels.FindAsync(id);
            if (paymentTypeModel == null)
            {
                return NotFound();
            }

            db.PaymentTypeModels.Remove(paymentTypeModel);
            await db.SaveChangesAsync();

            return Ok(paymentTypeModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentTypeModelExists(int id)
        {
            return db.PaymentTypeModels.Count(e => e.PaymentTypeId == id) > 0;
        }
    }
}