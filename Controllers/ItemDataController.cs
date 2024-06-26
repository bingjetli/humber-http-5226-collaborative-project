﻿using humber_http_5226_collaborative_project.Models;
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


        /** BASIC CRUD ROUTES
         */

        /// <summary>
        /// Standard route to fetch a list of all the elements in this database table.
        /// </summary>
        /// <returns>
        /// A list of Items in the form ItemDto.
        /// </returns>
        /// <example>
        /// GET: api/ItemData/ListAll
        /// </example>
        [ResponseType(typeof(IEnumerable<ItemDto>))]
        [HttpGet]
        public IEnumerable<ItemDto> ListAll() {
            return db.Items.AsEnumerable().Select(i => i.ToDto());
        }


        /// <summary>
        /// Standard route to find an element by their id in this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the Item doesn't exist. HTTP 200 with the Item in ItemDto form otherwise.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// GET: api/ItemData/FindById/{id}
        /// </example>
        [ResponseType(typeof(ItemDto))]
        [HttpGet]
        public IHttpActionResult FindById(int id) {
            Item result = db.Items.Find(id);

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
        /// <param name="item">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/ItemData/CreateNew
        /// </example>
        [ResponseType(typeof(ItemDto))]
        [HttpPost]
        public IHttpActionResult CreateNew(Item item) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Item added = db.Items.Add(item);
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
        /// <param name="item">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/ItemData/Update/{id}
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult Update(int id, Item item) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }


            if (id != item.ItemId) {
                return BadRequest();
            }


            db.Entry(item).State = EntityState.Modified;


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
        /// Standard route to delete an element inside this database table.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The primary key of the element in this database.</param>
        /// <example>
        /// POST: api/ItemData/Delete/{id}
        /// </example>
        [ResponseType(typeof(ItemDto))]
        [HttpPost]
        public IHttpActionResult Delete(int id) {
            Item item = db.Items.Find(id);
            if (item == null) {
                return NotFound();
            }

            db.Items.Remove(item);
            db.SaveChanges();

            return Ok();
        }


        /** ASSOCIATIVE ROUTES
         */

        /// <summary>
        /// Standard associative route to link a Item to an Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="item_id">The Item to link.</param>
        /// <param name="cafe_id">The Cafe to link to.</param>
        /// <example>
        /// POST: api/ItemData/LinkToCafe/{item_id}/{cafe_id}
        /// </example>
        [HttpPost]
        [Route("api/ItemData/LinkToCafe/{item_id}/{cafe_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult LinkToCafe(int item_id, int cafe_id) {
            Item item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();
            Cafe target_cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();


            if (item == null || target_cafe == null) {
                return BadRequest();
            }


            //Only create the link if it doesn't already exist.
            if (target_cafe.Menu.Contains(item) == false) {
                target_cafe.Menu.Add(item);
            }
            if (item.CafesWithThisItem.Contains(target_cafe) == false) {
                item.CafesWithThisItem.Add(target_cafe);
            }
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to unlink a Item with an Cafe.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="item_id">The Item to link.</param>
        /// <param name="cafe_id">The Cafe to link to.</param>
        /// <example>
        /// POST: api/ItemData/UnlinkToCafe/{item_id}/{cafe_id}
        /// </example>
        [HttpPost]
        [Route("api/ItemData/UnlinkWithCafe/{item_id}/{cafe_id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult UnlinkWithCafe(int item_id, int cafe_id) {
            Item item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();
            Cafe target_cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();


            if (item == null || target_cafe == null) {
                return BadRequest();
            }


            //Only destroy the link if it exists.
            if (target_cafe.Menu.Contains(item) == true) {
                target_cafe.Menu.Remove(item);
            }
            if (item.CafesWithThisItem.Contains(target_cafe) == true) {
                item.CafesWithThisItem.Remove(target_cafe);
            }
            db.SaveChanges();


            return Ok();
        }


        /// <summary>
        /// Standard associative route to view all the Cafes linked to the Item.
        /// </summary>
        /// <returns>
        /// HTTP 404 if the id doesn't exist.
        /// HTTP 200 if the delete was successful.
        /// </returns>
        /// <param name="id">The Item to link.</param>
        /// <example>
        /// POST: api/ItemData/GetLinkedCafes/{id}
        /// </example>
        [HttpGet]
        [ResponseType(typeof(IEnumerable<CafeDto>))]
        public IHttpActionResult GetLinkedCafes(int id) {
            Item item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == id).FirstOrDefault();


            if (item == null) {
                return BadRequest();
            }


            return Ok(item.CafesWithThisItem.AsEnumerable().Select(c => c.ToDto()));
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