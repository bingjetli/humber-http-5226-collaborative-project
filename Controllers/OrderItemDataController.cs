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

    [ResponseType(typeof(IEnumerable<OrderItemDto>))]
    [HttpGet]
    public IEnumerable<OrderItemDto> ListAll() {
      return db.OrderItems.AsEnumerable().Select(o => o.ToDto());
    }


    [ResponseType(typeof(OrderItemDto))]
    [HttpGet]
    public IHttpActionResult FindById(int id) {
      OrderItem result = db.OrderItems.Find(id);

      if (result == null) {
        return NotFound();
      }

      return Ok(result.ToDto());
    }


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


    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult GetLinkedItem(int id) {
      OrderItem order_item = db.OrderItems.Include(o => o.Item).Where(o => o.OrderItemId == id).FirstOrDefault();


      if (order_item == null) {
        return BadRequest();
      }


      return Ok(order_item.Item.ToDto());
    }



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