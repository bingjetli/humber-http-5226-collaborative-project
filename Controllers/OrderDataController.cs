using humber_http_5226_collaborative_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class OrderDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        /// <summary>
        /// Returns all orders in the system.
        /// </summary>
        /// <returns>
        /// All orders in the database.
        /// </returns>
        /// <example>
        /// GET: api/OrderData/ListOrders
        /// </example>
        [HttpGet]
        [ResponseType(typeof(OrderDto))]
        public IHttpActionResult ListOrders()
        {
            List<Order> Orders = db.Orders.ToList();
            List<OrderDto> OrderDtos = new List<OrderDto>();

            Orders.ForEach(o => OrderDtos.Add(new OrderDto()
            {
                OrderId = o.OrderId,
                CreatedAt = o.CreatedAt,
                CafeId = o.CafeId,
                CafeName = o.Cafe.Name,
                CourierLicenseId = o.CourierLicenseId,
                Status = o.Status
            }));

            return Ok(OrderDtos);
        }
    }
}
