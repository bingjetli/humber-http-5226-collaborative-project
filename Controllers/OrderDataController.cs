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
using humber_http_5226_collaborative_project.Models;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class OrderDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all orders in the system that matches the search key (orderid)
        /// </summary>
        /// <returns>
        /// All orders in the database that matches the search key (orderid)
        /// </returns>
        /// <example>
        /// GET: api/OrderData/ListOrders
        /// </example>
        [HttpGet]
        [Route("api/OrderData/ListOrders/{SearchKey?}")]
        public IEnumerable<OrderDto> ListOrders(string SearchKey = null)
        {
            //List<Order> Orders = db.Orders.ToList();
            List<Order> Orders = new List<Order>();

            if (SearchKey == null)
            {
                Orders = db.Orders.ToList();
            }
            else
            {
                Orders = db.Orders.Where(o => o.OrderId.Contains(SearchKey)).ToList();
            }
            List<OrderDto> OrderDtos = new List<OrderDto>();

            Orders.ForEach(o => OrderDtos.Add(new OrderDto()
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                //How to incorporate orderitems from dto??
            }));

            return OrderDtos;
        }

        /// <summary>
        /// Returns all orders in the system
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A order in the system matching up to the order ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the order</param>
        /// <example>
        /// GET: api/OrderData/FindOrder/5
        /// </example>
        [ResponseType(typeof(OrderDto))]
        [HttpGet]
        public IHttpActionResult FindOrder(int id)
        {
            Order Order = db.Orders.Find(id);
            OrderDto OrderDto = new OrderDto()
            {
                OrderId = Order.OrderId,
                CreatedAt = Order.CreatedAt,
                //How to incorporate orderitems from dto??
            };
            if (Order == null)
            {
                return NotFound();
            }

            return Ok(OrderDto);
        }

        /// <summary>
        /// Updates a particular order in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the order ID primary key</param>
        /// <param name="Order">JSON FORM DATA of a order</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/OrderData/UpdateOrder/5
        /// FORM DATA: Order JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateOrder(int id, Order Order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Order.OrderId)
            {
                return BadRequest();
            }

            db.Entry(Order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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
        /// Adds a order to the system
        /// </summary>
        /// <param name="Order">JSON FORM DATA of an order</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Order ID, Order Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/OrderData/AddOrder
        /// FORM DATA: Order JSON Object
        /// </example>
        [ResponseType(typeof(Order))]
        [HttpPost]
        public IHttpActionResult AddOrder(Order Order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(Order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Order.OrderId }, Order);
        }

        /// <summary>
        /// Deletes a order from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the order</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/DistrctData/DeleteOrder/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Order))]
        [HttpPost]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order Order = db.Orders.Find(id);
            if (Order == null)
            {
                return NotFound();
            }

            db.Orders.Remove(Order);
            db.SaveChanges();

            return Ok();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.OrderId == id) > 0;
        }
    }
}
