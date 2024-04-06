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
  public class OrderItemDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    /** BASIC CRUD ROUTES
     */

    /// <summary>
    /// Standard route to fetch a list of all the elements in this database table.
    /// </summary>
    /// <returns>
    /// A list of OrderItems in the form OrderItemDto.
    /// </returns>
    /// <example>
    /// GET: api/OrderItemData/ListAll
    /// </example>
    [ResponseType(typeof(IEnumerable<OrderItemDto>))]
    [HttpGet]
    public IEnumerable<OrderItemDto> ListAll() {
      return db.OrderItems.AsEnumerable().Select(o => o.ToDto());
    }


    /// <summary>
    /// Standard route to find an element by their id in this database table.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the OrderItem doesn't exist. HTTP 200 with the OrderItem in OrderItemDto form otherwise.
    /// </returns>
    /// <param name="id">The primary key of the element in this database.</param>
    /// <example>
    /// GET: api/OrderItemData/FindById/{id}
    /// </example>
    [ResponseType(typeof(OrderItemDto))]
    [HttpGet]
    public IHttpActionResult FindById(int id) {
      OrderItem result = db.OrderItems.Find(id);

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
    /// <param name="order_item">The POST payload containing JSON data modeled after this database model.</param>
    /// <example>
    /// POST: api/OrderItemData/CreateNew
    /// </example>
    [ResponseType(typeof(OrderItemDto))]
    [HttpPost]
    public IHttpActionResult CreateNew(OrderItem order_item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      OrderItem added = db.OrderItems.Add(order_item);
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
    /// <param name="order_item">The POST payload containing JSON data modeled after this database model.</param>
    /// <example>
    /// POST: api/OrderItemData/Update/{id}
    /// </example>
    [ResponseType(typeof(void))]
    [HttpPost]
    public IHttpActionResult Update(int id, OrderItem order_item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }


      if (id != order_item.OrderItemId) {
        return BadRequest();
      }


      db.Entry(order_item).State = EntityState.Modified;


      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!OrderItemExists(id)) {
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
    /// POST: api/OrderItemData/Delete/{id}
    /// </example>
    [ResponseType(typeof(OrderItemDto))]
    [HttpPost]
    public IHttpActionResult Delete(int id) {
      OrderItem order_item = db.OrderItems.Find(id);
      if (order_item == null) {
        return NotFound();
      }

      db.OrderItems.Remove(order_item);
      db.SaveChanges();

      return Ok();
    }


    /** ASSOCIATIVE ROUTES
     */

    /// <summary>
    /// Standard associative route to link a OrderItem to an Item.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="order_item_id">The OrderItem to link.</param>
    /// <param name="item_id">The Item to link to.</param>
    /// <example>
    /// POST: api/OrderItemData/LinkToItem/{order_item_id}/{item_id}
    /// </example>
    [HttpPost]
    [Route("api/OrderItemData/LinkToItem/{order_item_id}/{item_id}")]
    [ResponseType(typeof(void))]
    public IHttpActionResult LinkToItem(int order_item_id, int item_id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == order_item_id).FirstOrDefault();
      Item target_item = db.Items.Find(item_id);


      if (target_item == null || order_item == null) {
        return BadRequest();
      }


      order_item.Item = target_item;
      order_item.ItemId = target_item.ItemId;


      db.SaveChanges();


      return Ok();
    }


    /// <summary>
    /// Standard associative route to unlink a OrderItem with an Item.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="order_item_id">The OrderItem to link.</param>
    /// <param name="item_id">The Item to link to.</param>
    /// <example>
    /// POST: api/OrderItemData/UnlinkToItem/{order_item_id}/{item_id}
    /// </example>
    [HttpPost]
    [Route("api/OrderItemData/UnlinkWithItem/{order_item_id}/{item_id}")]
    [ResponseType(typeof(void))]
    public IHttpActionResult UnlinkWithItem(int order_item_id, int item_id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == order_item_id).FirstOrDefault();
      Item target_item = db.Items.Find(item_id);


      if (target_item == null || order_item == null) {
        return BadRequest();
      }


      order_item.Item = null;
      order_item.ItemId = null;


      db.SaveChanges();


      return Ok();
    }


    /// <summary>
    /// Standard associative route to view all the Items linked to the OrderItem.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="id">The OrderItem to link.</param>
    /// <example>
    /// POST: api/OrderItemData/GetLinkedItems/{id}
    /// </example>
    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult GetLinkedItem(int id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == id).FirstOrDefault();


      if (order_item == null) {
        return BadRequest();
      }


      return Ok(order_item.Item.ToDto());
    }



    /// <summary>
    /// Standard associative route to link a OrderItem to an Order.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="order_item_id">The OrderItem to link.</param>
    /// <param name="order_id">The Order to link to.</param>
    /// <example>
    /// POST: api/OrderItemData/LinkToOrder/{order_item_id}/{order_id}
    /// </example>
    [HttpPost]
    [Route("api/OrderItemData/LinkToOrder/{order_item_id}/{order_id}")]
    [ResponseType(typeof(void))]
    public IHttpActionResult LinkToOrder(int order_item_id, int order_id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == order_item_id).FirstOrDefault();
      Order target_order = db.Orders.Find(order_id);


      if (order_item == null || target_order == null) {
        return BadRequest();
      }


      order_item.Order = target_order;
      order_item.OrderId = target_order.OrderId;


      if (target_order.OrderItems.Contains(order_item) == false) {
        target_order.OrderItems.Add(order_item);
      }


      db.SaveChanges();


      return Ok();
    }


    /// <summary>
    /// Standard associative route to unlink a OrderItem with an Order.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="order_item_id">The OrderItem to link.</param>
    /// <param name="order_id">The Order to link to.</param>
    /// <example>
    /// POST: api/OrderItemData/UnlinkToOrder/{order_item_id}/{order_id}
    /// </example>
    [HttpPost]
    [Route("api/OrderItemData/UnlinkWithOrder/{order_item_id}/{order_id}")]
    [ResponseType(typeof(void))]
    public IHttpActionResult UnlinkWithOrder(int order_item_id, int order_id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == order_item_id).FirstOrDefault();
      Order target_order = db.Orders.Find(order_id);


      if (order_item == null || target_order == null) {
        return BadRequest();
      }


      order_item.Order = null;
      order_item.OrderId = null;


      if (target_order.OrderItems.Contains(order_item) == true) {
        target_order.OrderItems.Remove(order_item);
      }


      db.SaveChanges();


      return Ok();
    }


    /// <summary>
    /// Standard associative route to view all the Orders linked to the OrderItem.
    /// </summary>
    /// <returns>
    /// HTTP 404 if the id doesn't exist.
    /// HTTP 200 if the delete was successful.
    /// </returns>
    /// <param name="id">The OrderItem to link.</param>
    /// <example>
    /// POST: api/OrderItemData/GetLinkedOrders/{id}
    /// </example>
    [HttpGet]
    [ResponseType(typeof(OrderDto))]
    public IHttpActionResult GetLinkedOrder(int id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == id).FirstOrDefault();


      if (order_item == null) {
        return BadRequest();
      }


      return Ok(order_item.Order.ToDto());
    }



    //---


    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool OrderItemExists(int id) {
      return db.OrderItems.Count(e => e.OrderItemId == id) > 0;
    }
  }
}