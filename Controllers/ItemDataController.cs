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
  public class ItemDataController : ApiController {
    private ApplicationDbContext db = new ApplicationDbContext();

    /// <summary>
    /// Returns all Items in the system.
    /// </summary>
    /// <returns>
    /// All items in the database.
    /// </returns>
    /// <example>
    /// GET: api/ItemData/ListItems
    /// </example>
    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult ListItems() {
      List<Item> Items = db.Items.ToList();
      List<ItemDto> ItemDtos = new List<ItemDto>();

      Items.ForEach(i => ItemDtos.Add(new ItemDto() {
        ItemId = i.ItemId,
        Name = i.Name,
        Description = i.Description,
        Price = i.Price,

      }));

      return Ok(ItemDtos);
    }

    /// <summary>
    /// Returns all items in the system associated with a particular cafe.
    /// </summary>
    /// <returns>
    /// HEADER: 200 (OK)
    /// CONTENT: all items in the database associated with a particular cafe
    /// </returns>
    /// <param name="id">cafe Primary Key</param>
    /// <example>
    /// GET: api/ItemData/ListItemsForCafe/5
    /// </example>
    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult ListItemsForCafe(int id) {
      //List<Item> Items = db.Items.Where(
      //    i => i.Cafes.Any(
      //        c => c.CafeId == id)
      //    ).ToList();
      List<ItemDto> ItemDtos = new List<ItemDto>();

      //Items.ForEach(i => ItemDtos.Add(new ItemDto() {
      //  ItemId = i.ItemId,
      //  Name = i.Name,
      //  Description = i.Description,
      //  Price = i.Price,
      //}));

      return Ok(ItemDtos);
    }

    /// <summary>
    /// Returns items in the system not available at a cafe.
    /// </summary>
    /// <returns>
    /// HEADER: 200 (OK)
    /// CONTENT: all items in the database not available at a cafe
    /// </returns>
    /// <param name="id">cafe Primary Key</param>
    /// <example>
    /// GET: api/ItemData/ListItemsNotInCafe/5
    /// </example>
    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult ListItemsNotInCafe(int id) {
      //List<Item> Items = db.Items.Where(
      //    i => !i.Cafes.Any(
      //        c => c.CafeId == id)
      //    ).ToList();
      List<ItemDto> ItemDtos = new List<ItemDto>();

      //Items.ForEach(i => ItemDtos.Add(new ItemDto() {
      //  ItemId = i.ItemId,
      //  Name = i.Name,
      //  Description = i.Description,
      //  Price = i.Price,
      //}));

      return Ok(ItemDtos);
    }

    /// <summary>
    /// Returns all items in the system.
    /// </summary>
    /// <returns>
    /// HEADER: 200 (OK)
    /// CONTENT: An item in the system matching up to the item ID primary key
    /// or
    /// HEADER: 404 (NOT FOUND)
    /// </returns>
    /// <param name="id">The primary key of the item</param>
    /// <example>
    /// GET: api/ItemData/FindItem/5
    /// </example>
    [HttpGet]
    [ResponseType(typeof(ItemDto))]
    public IHttpActionResult FindItem(int id) {
      Item Item = db.Items.Find(id);
      ItemDto ItemDto = new ItemDto() {
        ItemId = Item.ItemId,
        Name = Item.Name,
        Description = Item.Description,
        Price = Item.Price,
      };

      if (Item == null) {
        return NotFound();
      }

      return Ok(ItemDto);
    }

    /// <summary>
    /// Updates a particular item in the system with POST Data input
    /// </summary>
    /// <param name="id">Represents the item ID primary key</param>
    /// <param name="Item">JSON FORM DATA of an Item</param>
    /// <returns>
    /// HEADER: 204 (Success, No Content Response)
    /// or
    /// HEADER: 400 (Bad Request)
    /// or
    /// HEADER: 404 (Not Found)
    /// </returns>
    /// <example>
    /// POST: api/ItemData/UpdateItem/5
    /// FORM DATA: Item JSON Object
    /// </example>
    [ResponseType(typeof(void))]
    public IHttpActionResult UpdateItem(int id, Item Item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      if (id != Item.ItemId) {
        return BadRequest();
      }

      db.Entry(Item).State = EntityState.Modified;

      try {
        db.SaveChanges();
      }
      catch (DbUpdateConcurrencyException) {
        if (!ItemExists(id)) {
          return NotFound();
        }
        else {
          throw;
        }
      }

      return StatusCode(HttpStatusCode.NoContent);
    }

    /// <summary>
    /// Adds an item to the system
    /// </summary>
    /// <param name="Item">JSON FORM DATA of an Item</param>
    /// <returns>
    /// HEADER: 201 (Created)
    /// CONTENT: Item ID, Item Data
    /// or
    /// HEADER: 400 (Bad Request)
    /// </returns>
    /// <example>
    /// POST: api/ItemData/AddItem
    /// FORM DATA: Item JSON Object
    /// </example>
    [ResponseType(typeof(Item))]
    [HttpPost]
    public IHttpActionResult AddItem(Item Item) {
      if (!ModelState.IsValid) {
        return BadRequest(ModelState);
      }

      db.Items.Add(Item);
      db.SaveChanges();

      return CreatedAtRoute("DefaultApi", new { id = Item.ItemId }, Item);
    }

    /// <summary>
    /// Deletes an item from the system by it's ID.
    /// </summary>
    /// <param name="id">The primary key of the item</param>
    /// <returns>
    /// HEADER: 200 (OK)
    /// or
    /// HEADER: 404 (NOT FOUND)
    /// </returns>
    /// <example>
    /// POST: api/ItemData/DeleteItem/5
    /// FORM DATA: (empty)
    /// </example>
    [ResponseType(typeof(Item))]
    [HttpPost]
    public IHttpActionResult DeleteItem(int id) {
      Item Item = db.Items.Find(id);
      if (Item == null) {
        return NotFound();
      }

      db.Items.Remove(Item);
      db.SaveChanges();

      return Ok();
    }

    protected override void Dispose(bool disposing) {
      if (disposing) {
        db.Dispose();
      }
      base.Dispose(disposing);
    }

    private bool ItemExists(int id) {
      return db.Items.Count(e => e.ItemId == id) > 0;
    }
  }
}