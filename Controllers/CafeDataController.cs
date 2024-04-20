using humber_http_5226_collaborative_project.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using static System.Data.Entity.Infrastructure.Design.Executor;


namespace humber_http_5226_collaborative_project.Controllers {
    public class CafeDataController : ApiController {
        private ApplicationDbContext db = new ApplicationDbContext();


        /** BASIC CRUD ROUTES
         */

        /// <summary>
        /// Standard route to fetch a list of all the elements in this database table.
        /// </summary>
        /// <returns>
        /// A list of Cafes in the form CafeDto.
        /// </returns>
        /// <example>
        /// GET: api/CafeData/ListAll
        /// </example>
        [ResponseType(typeof(IEnumerable<CafeDto>))]
        [HttpGet]
        public IEnumerable<CafeDto> ListAll() {
            return db.Cafes.AsEnumerable().Select(c => c.ToDto());
        }


        /// <summary>
        /// Standard route to find an element by their id in this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the Cafe doesn't exist. HTTP 200 with the Cafe in CafeDto form otherwise.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// GET: api/CafeData/FindById/{id}
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpGet]
        public IHttpActionResult FindById(int id) {
            Cafe result = db.Cafes.Find(id);

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
        /// <param name="cafe">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/CafeData/CreateNew
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpPost]
        public IHttpActionResult CreateNew(Cafe cafe) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Cafe added = db.Cafes.Add(cafe);
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
        /// <param name="cafe">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/CafeData/Update/{id}
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult Update(int id, Cafe cafe) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }


            if (id != cafe.CafeId) {
                return BadRequest();
            }


            db.Entry(cafe).State = EntityState.Modified;


            try {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CafeExists(id)) {
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
        /// POST: api/CafeData/Delete/{id}
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpPost]
        public IHttpActionResult Delete(int id) {
            Cafe cafe = db.Cafes.Find(id);
            if (cafe == null) {
                return NotFound();
            }

            db.Cafes.Remove(cafe);
            db.SaveChanges();

            return Ok();
        }


        /** ASSOCIATIVE ROUTES
         */

        /// <summary>
        /// Standard associative route to link a Cafe to an Item.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="cafe_id">The Cafe to link.</param>
        /// <param name="item_id">The Item to link to.</param>
        /// <example>
        /// POST: api/CafeData/LinkToItem/{cafe_id}/{item_id}
        /// </example>
        [HttpPost]
        [Route("api/CafeData/LinkToItem/{cafe_id}/{item_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LinkToItem(int cafe_id, int item_id) {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Item target_item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();


            if (target_item == null || cafe == null) {
                return BadRequest();
            }


            //Only create the link if it doesn't already exist.
            if (cafe.Menu.Contains(target_item) == false) {
                cafe.Menu.Add(target_item);
            }
            if (target_item.CafesWithThisItem.Contains(cafe) == false) {
                target_item.CafesWithThisItem.Add(cafe);
            }
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to unlink a Cafe with an Item.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="cafe_id">The Cafe to link.</param>
        /// <param name="item_id">The Item to link to.</param>
        /// <example>
        /// POST: api/CafeData/UnlinkToItem/{cafe_id}/{item_id}
        /// </example>
        [HttpPost]
        [Route("api/CafeData/UnlinkWithItem/{cafe_id}/{item_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UnlinkWithItem(int cafe_id, int item_id) {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Item target_item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();


            if (target_item == null || cafe == null) {
                return BadRequest();
            }


            //Only destroy the link if it exists.
            if (cafe.Menu.Contains(target_item) == true) {
                cafe.Menu.Remove(target_item);
            }
            if (target_item.CafesWithThisItem.Contains(cafe) == true) {
                target_item.CafesWithThisItem.Remove(cafe);
            }
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to view all the Items linked to the Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The Cafe to link.</param>
        /// <example>
        /// POST: api/CafeData/GetLinkedItems/{id}
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<ItemDto>))]
        public IHttpActionResult GetLinkedItems(int id) {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == id).FirstOrDefault();


            if (cafe == null) {
                return BadRequest();
            }


            return Ok(cafe.Menu.AsEnumerable().Select(i => i.ToDto()));
        }


        /// <summary>
        /// Standard associative route to link a Cafe to an Order.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="cafe_id">The Cafe to link.</param>
        /// <param name="order_id">The Order to link to.</param>
        /// <example>
        /// POST: api/CafeData/LinkToOrder/{cafe_id}/{order_id}
        /// </example>
        [HttpPost]
        [Route("api/CafeData/LinkToOrder/{cafe_id}/{order_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LinkToOrder(int cafe_id, int order_id) {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order target_order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (target_order == null || cafe == null) {
                return BadRequest();
            }


            if (target_order.Cafe.Equals(null)) {

                //If the target order is already linked to a cafe, then the user must
                //unlink with the previous cafe before linking to a new one.
                return BadRequest();
            }


            //Only create the link if it doesn't already exist.
            if (cafe.Orders.Contains(target_order) == false) {
                cafe.Orders.Add(target_order);
                target_order.CafeId = cafe_id;
                target_order.Cafe = cafe;


                db.SaveChanges();
            }


            return Ok();
        }


        /// <summary>
        /// Standard associative route to unlink a Cafe with an Order.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="cafe_id">The Cafe to link.</param>
        /// <param name="order_id">The Order to link to.</param>
        /// <example>
        /// POST: api/CafeData/UnlinkToOrder/{cafe_id}/{order_id}
        /// </example>
        [HttpPost]
        [Route("api/CafeData/UnlinkWithOrder/{cafe_id}/{order_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UnlinkWithOrder(int cafe_id, int order_id) {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order target_order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (target_order == null || cafe == null) {
                return BadRequest();
            }


            if (cafe.Orders.Contains(target_order) == true) {
                cafe.Orders.Remove(target_order);
                target_order.CafeId = null;
                target_order.Cafe = null;


                db.SaveChanges();
            }


            return Ok();
        }


        /// <summary>
        /// Standard associative route to view all the Orders linked to the Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The Cafe to link.</param>
        /// <example>
        /// POST: api/CafeData/GetLinkedOrders/{id}
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<OrderDto>))]
        public IHttpActionResult GetLinkedOrders(int id) {
            Cafe cafe = db.Cafes.Include(c => c.Orders).Where(c => c.CafeId == id).FirstOrDefault();


            if (cafe == null) {
                return BadRequest();
            }


            return Ok(cafe.Orders.AsEnumerable().Select(i => i.ToDto()));
        }


        protected override void Dispose(bool disposing) {
            if (disposing) {
                db.Dispose();
            }

            base.Dispose(disposing);
        }


        private bool CafeExists(int id) {
            return db.Cafes.Count(e => e.CafeId == id) > 0;
        }

    }
}