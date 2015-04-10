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
    public class PaymentStatusModelsController : ApiController
    {
        private fetchitEntities db = new fetchitEntities();

        // GET: api/PaymentStatusModels
        public IQueryable<PaymentStatusModel> GetPaymentStatuses()
        {
            return db.PaymentStatuses;
        }

        // GET: api/PaymentStatusModels/5
        [ResponseType(typeof(PaymentStatusModel))]
        public async Task<IHttpActionResult> GetPaymentStatusModel(int id)
        {
            PaymentStatusModel paymentStatusModel = await db.PaymentStatuses.FindAsync(id);
            if (paymentStatusModel == null)
            {
                return NotFound();
            }

            return Ok(paymentStatusModel);
        }

        // PUT: api/PaymentStatusModels/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutPaymentStatusModel(int id, PaymentStatusModel paymentStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != paymentStatusModel.PaymentStatusId)
            {
                return BadRequest();
            }

            db.Entry(paymentStatusModel).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PaymentStatusModelExists(id))
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

        // POST: api/PaymentStatusModels
        [ResponseType(typeof(PaymentStatusModel))]
        public async Task<IHttpActionResult> PostPaymentStatusModel(PaymentStatusModel paymentStatusModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PaymentStatuses.Add(paymentStatusModel);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = paymentStatusModel.PaymentStatusId }, paymentStatusModel);
        }

        // DELETE: api/PaymentStatusModels/5
        [ResponseType(typeof(PaymentStatusModel))]
        public async Task<IHttpActionResult> DeletePaymentStatusModel(int id)
        {
            PaymentStatusModel paymentStatusModel = await db.PaymentStatuses.FindAsync(id);
            if (paymentStatusModel == null)
            {
                return NotFound();
            }

            db.PaymentStatuses.Remove(paymentStatusModel);
            await db.SaveChangesAsync();

            return Ok(paymentStatusModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PaymentStatusModelExists(int id)
        {
            return db.PaymentStatuses.Count(e => e.PaymentStatusId == id) > 0;
        }
    }
}