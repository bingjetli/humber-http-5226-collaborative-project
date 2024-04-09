using humber_http_5226_collaborative_project.Models;
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

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CourierLicenseDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();


        /** BASIC CRUD ROUTES
         */

        /// <summary>
        /// Standard route to fetch a list of all the elements in this database table.
        /// </summary>
        /// <returns>
        /// A list of CourierLicenses in the form CourierLicenseDto.
        /// </returns>
        /// <example>
        /// GET: api/CourierLicenseData/ListAll
        /// </example>
        [ResponseType(typeof(IEnumerable<CourierLicenseDto>))]
        [HttpGet]
        public IEnumerable<CourierLicenseDto> ListAll()
        {
            return db.CourierLicenses.AsEnumerable().Select(o => o.ToDto());
        }


        /// <summary>
        /// Standard route to find an element by their id in this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the CourierLicense doesn't exist. HTTP 200 with the CourierLicense in CourierLicenseDto form otherwise.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// GET: api/CourierLicenseData/FindById/{id}
        /// </example>
        [ResponseType(typeof(CourierLicenseDto))]
        [HttpGet]
        public IHttpActionResult FindById(int id)
        {
            CourierLicense result = db.CourierLicenses.Find(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result.ToDto());
        }


        /// <summary>
        /// Standard route to create a new element inside this database table.
        /// </summary>
        /// <returns>
        /// HTTP 400 If the payload is invalid. HTTP 200 with the added element in Dto
        /// form otherwise.
        /// </returns>
        /// <param name="courier_license">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/CourierLicenseData/CreateNew
        /// </example>
        [ResponseType(typeof(CourierLicenseDto))]
        [HttpPost]
        public IHttpActionResult CreateNew(CourierLicense courier_license)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            CourierLicense added = db.CourierLicenses.Add(courier_license);
            db.SaveChanges();

            return Ok(added.ToDto());
        }


        /// <summary>
        /// Standard route to update an element inside this database table.
        /// </summary>
        /// <returns>
        /// HTTP 400 if the POST payload is invalid, or the id is invalid.
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 204 if the update was successful.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <param name="courier_license">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/CourierLicenseData/Update/{id}
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult Update(int id, CourierLicense courier_license)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (id != courier_license.CourierLicenseId)
            {
                return BadRequest();
            }


            db.Entry(courier_license).State = EntityState.Modified;


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


        /// <summary>
        /// Standard route to delete an element inside this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// POST: api/CourierLicenseData/Delete/{id}
        /// </example>
        [ResponseType(typeof(CourierLicenseDto))]
        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            CourierLicense courier_license = db.CourierLicenses.Find(id);
            if (courier_license == null)
            {
                return NotFound();
            }

            db.CourierLicenses.Remove(courier_license);
            db.SaveChanges();

            return Ok();
        }


        /** ASSOCIATIVE ROUTES
         */

        /// <summary>
        /// Standard associative route to link a CourierLicense to an Order.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="license_id">The CourierLicense to link.</param>
        /// <param name="order_id">The Order to link to.</param>
        /// <example>
        /// POST: api/CourierLicenseData/LinkToOrder/{license_id}/{order_id}
        /// </example>
        [HttpPost]
        [Route("api/CourierLicenseData/LinkToOrder/{license_id}/{order_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LinkToOrder(int license_id, int order_id)
        {
            CourierLicense courier_license = db.CourierLicenses.Include(c => c.AssignedOrders).Where(c => c.CourierLicenseId == license_id).FirstOrDefault();
            Order target_order = db.Orders.Find(order_id);

            if (courier_license == null || target_order == null)
            {
                return BadRequest();
            }

            target_order.CourierLicense = courier_license;
            target_order.CourierLicenseId = courier_license.CourierLicenseId;

            if (courier_license.AssignedOrders.Contains(target_order) == false)
            {
                courier_license.AssignedOrders.Add(target_order);
            }


            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to unlink a CourierLicense with an Order.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="license_id">The CourierLicense to link.</param>
        /// <param name="order_id">The Order to link to.</param>
        /// <example>
        /// POST: api/CourierLicenseData/UnlinkToOrder/{license_id}/{order_id}
        /// </example>
        [HttpPost]
        [Route("api/CourierLicenseData/UnlinkWithOrder/{license_id}/{order_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UnlinkWithOrder(int license_id, int order_id)
        {
            CourierLicense courier_license = db.CourierLicenses.Include(c => c.AssignedOrders).Where(c => c.CourierLicenseId == license_id).FirstOrDefault();
            Order target_order = db.Orders.Find(order_id);

            if (courier_license == null || target_order == null)
            {
                return BadRequest();
            }

            target_order.CourierLicense = null;
            target_order.CourierLicenseId = null;

            if (courier_license.AssignedOrders.Contains(target_order) == true)
            {
                courier_license.AssignedOrders.Remove(target_order);
            }


            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to view all the Orders linked to the CourierLicense.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The CourierLicense to link.</param>
        /// <example>
        /// POST: api/CourierLicenseData/GetLinkedOrders/{id}
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CourierLicenseDto))]
        public IHttpActionResult GetLinkedOrders(int id)
        {
            CourierLicense courier_license = db.CourierLicenses.Include(c => c.AssignedOrders).Where(c => c.CourierLicenseId == id).FirstOrDefault();


            if (courier_license == null)
            {
                return BadRequest();
            }


            return Ok(courier_license.ToDto());
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