using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using humber_http_5226_collaborative_project.Models;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CourierLicenseDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/CourierLicenseData
        public IQueryable<CourierLicense> GetCourierLicenses()
        {
            return db.CourierLicenses;
        }

        // GET: api/CourierLicenseData/5
        [ResponseType(typeof(CourierLicense))]
        public IHttpActionResult GetCourierLicense(int id)
        {
            CourierLicense courierLicense = db.CourierLicenses.Find(id);
            if (courierLicense == null)
            {
                return NotFound();
            }

            return Ok(courierLicense);
        }

        // PUT: api/CourierLicenseData/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutCourierLicense(int id, CourierLicense courierLicense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != courierLicense.CourierLicenseId)
            {
                return BadRequest();
            }

            db.Entry(courierLicense).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourierLicenseExists(id))
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

        // POST: api/CourierLicenseData
        [ResponseType(typeof(CourierLicense))]
        public IHttpActionResult PostCourierLicense(CourierLicense courierLicense)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.CourierLicenses.Add(courierLicense);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = courierLicense.CourierLicenseId }, courierLicense);
        }

        // DELETE: api/CourierLicenseData/5
        [ResponseType(typeof(CourierLicense))]
        public IHttpActionResult DeleteCourierLicense(int id)
        {
            CourierLicense courierLicense = db.CourierLicenses.Find(id);
            if (courierLicense == null)
            {
                return NotFound();
            }

            db.CourierLicenses.Remove(courierLicense);
            db.SaveChanges();

            return Ok(courierLicense);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool CourierLicenseExists(int id)
        {
            return db.CourierLicenses.Count(e => e.CourierLicenseId == id) > 0;
        }
    }
}