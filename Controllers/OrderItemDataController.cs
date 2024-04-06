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