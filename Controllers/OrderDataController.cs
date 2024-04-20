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

namespace humber_http_5226_collaborative_project.Controllers {
    public class OrderDataController : ApiController {
        private ApplicationDbContext db = new ApplicationDbContext();


        /** BASIC CRUD ROUTES
         */

        /// <summary>
        /// Standard route to fetch a list of all the elements in this database table.
        /// </summary>
        /// <returns>
        /// A list of Orders in the form OrderDto.
        /// </returns>
        /// <example>
        /// GET: api/OrderData/ListAll
        /// </example>
        [ResponseType(typeof(IEnumerable<OrderDto>))]
        [HttpGet]
        public IEnumerable<OrderDto> ListAll() {
            return db.Orders.AsEnumerable().Select(o => o.ToDto());
        }


        /// <summary>
        /// Standard route to find an element by their id in this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the Order doesn't exist. HTTP 200 with the Order in OrderDto form otherwise.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// GET: api/OrderData/FindById/{id}
        /// </example>
        [ResponseType(typeof(OrderDto))]
        [HttpGet]
        public IHttpActionResult FindById(int id) {
            Order result = db.Orders.Find(id);

            if (result == null) {
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
        /// <param name="order">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/OrderData/CreateNew
        /// </example>
        [ResponseType(typeof(OrderDto))]
        [HttpPost]
        public IHttpActionResult CreateNew(Order order) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //Overwrite the CreatedAt field with the current time.
            order.CreatedAt = DateTime.Now;
            Order added = db.Orders.Add(order);
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
        /// <param name="order">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/OrderData/Update/{id}
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult Update(int id, Order order) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }


            if (id != order.OrderId) {
                return BadRequest();
            }


            db.Entry(order).State = EntityState.Modified;


            try {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) {
                if (!OrderExists(id)) {
                    return NotFound();
                }
                else {
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
        /// POST: api/OrderData/Delete/{id}
        /// </example>
        [ResponseType(typeof(OrderDto))]
        [HttpPost]
        public IHttpActionResult Delete(int id) {
            Order order = db.Orders.Find(id);
            if (order == null) {
                return NotFound();
            }

            db.Orders.Remove(order);
            db.SaveChanges();

            return Ok();
        }


        /** ASSOCIATIVE ROUTES
         */

        /// <summary>
        /// Standard associative route to link a Order to an Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="order_id">The Order to link.</param>
        /// <param name="cafe_id">The Cafe to link to.</param>
        /// <example>
        /// POST: api/OrderData/LinkToCafe/{order_id}/{cafe_id}
        /// </example>
        [HttpPost]
        [Route("api/OrderData/LinkToCafe/{order_id}/{cafe_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LinkToCafe(int order_id, int cafe_id) {
            Cafe target_cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (order == null || target_cafe == null) {
                return BadRequest();
            }


            if (order.Cafe.Equals(null)) {

                //If the target order is already linked to a cafe, then the user must
                //unlink with the previous cafe before linking to a new one.
                return BadRequest();
            }


            if (order.CafeId != null) {
                return BadRequest();
            }

            order.CafeId = cafe_id;
            order.Cafe = target_cafe;

            //Only create the link if it doesn't already exist.
            if (target_cafe.Orders.Contains(order) == false) {
                target_cafe.Orders.Add(order);
            }

            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to unlink a Order with an Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="order_id">The Order to link.</param>
        /// <param name="cafe_id">The Cafe to link to.</param>
        /// <example>
        /// POST: api/OrderData/UnlinkToCafe/{order_id}/{cafe_id}
        /// </example>
        [HttpPost]
        [Route("api/OrderData/UnlinkWithCafe/{order_id}/{cafe_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UnlinkWithCafe(int order_id, int cafe_id) {
            Cafe target_cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (order == null || target_cafe == null) {
                return BadRequest();
            }


            target_cafe.Orders.Remove(order);
            order.CafeId = null;
            order.Cafe = null;
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to view all the Cafes linked to the Order.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The Order to link.</param>
        /// <example>
        /// POST: api/OrderData/GetLinkedCafes/{id}
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CafeDto>))]
        public IHttpActionResult GetLinkedCafes(int id) {
            Order order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == id).FirstOrDefault();


            if (order == null) {
                return BadRequest();
            }


            return Ok(order.Cafe.ToDto());
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }

            base.Dispose(disposing);
        }


        private bool OrderExists(int id) {
            return db.OrderItems.Count(e => e.OrderItemId == id) > 0;
        }
    }
}
