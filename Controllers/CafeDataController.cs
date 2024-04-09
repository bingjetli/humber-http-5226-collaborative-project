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


namespace humber_http_5226_collaborative_project.Controllers
{
    public class CafeDataController : ApiController
    {
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
        public IEnumerable<CafeDto> ListAll()
        {
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
        public IHttpActionResult FindById(int id)
        {
            Cafe result = db.Cafes.Find(id);

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
        /// <param name="cafe">The POST payload containing JSON data modeled after this database model.</param>
        /// <example>
        /// POST: api/CafeData/CreateNew
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpPost]
        public IHttpActionResult CreateNew(Cafe cafe)
        {
            if (!ModelState.IsValid)
            {
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
        public IHttpActionResult Update(int id, Cafe cafe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            if (id != cafe.CafeId)
            {
                return BadRequest();
            }


            db.Entry(cafe).State = EntityState.Modified;


            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CafeExists(id))
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
        /// POST: api/CafeData/Delete/{id}
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpPost]
        public IHttpActionResult Delete(int id)
        {
            Cafe cafe = db.Cafes.Find(id);
            if (cafe == null)
            {
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
        public IHttpActionResult LinkToItem(int cafe_id, int item_id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Item target_item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();


            if (target_item == null || cafe == null)
            {
                return BadRequest();
            }


            //Only create the link if it doesn't already exist.
            if (cafe.Menu.Contains(target_item) == false)
            {
                cafe.Menu.Add(target_item);
            }
            if (target_item.CafesWithThisItem.Contains(cafe) == false)
            {
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
        public IHttpActionResult UnlinkWithItem(int cafe_id, int item_id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Item target_item = db.Items.Include(i => i.CafesWithThisItem).Where(i => i.ItemId == item_id).FirstOrDefault();


            if (target_item == null || cafe == null)
            {
                return BadRequest();
            }


            //Only destroy the link if it exists.
            if (cafe.Menu.Contains(target_item) == true)
            {
                cafe.Menu.Remove(target_item);
            }
            if (target_item.CafesWithThisItem.Contains(cafe) == true)
            {
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
        public IHttpActionResult GetLinkedItems(int id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == id).FirstOrDefault();


            if (cafe == null)
            {
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
        public IHttpActionResult LinkToOrder(int cafe_id, int order_id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order target_order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (target_order == null || cafe == null)
            {
                return BadRequest();
            }


            if (target_order.Cafe.Equals(null))
            {

                //If the target order is already linked to a cafe, then the user must
                //unlink with the previous cafe before linking to a new one.
                return BadRequest();
            }


            //Only create the link if it doesn't already exist.
            if (cafe.Orders.Contains(target_order) == false)
            {
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
        public IHttpActionResult UnlinkWithOrder(int cafe_id, int order_id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Menu).Where(c => c.CafeId == cafe_id).FirstOrDefault();
            Order target_order = db.Orders.Include(o => o.Cafe).Where(o => o.OrderId == order_id).FirstOrDefault();


            if (target_order == null || cafe == null)
            {
                return BadRequest();
            }


            if (cafe.Orders.Contains(target_order) == true)
            {
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
        public IHttpActionResult GetLinkedOrders(int id)
        {
            Cafe cafe = db.Cafes.Include(c => c.Orders).Where(c => c.CafeId == id).FirstOrDefault();


            if (cafe == null)
            {
                return BadRequest();
            }


            return Ok(cafe.Orders.AsEnumerable().Select(i => i.ToDto()));
        }


        /** LEGACY ROUTES
         */

        /// <summary>
        /// Returns either all cafes in the system or cafes based on search key if applicable
        /// </summary>
        /// <returns>
        /// All cafes in the database with names containing the seach key.
        /// </returns>
        /// <example>
        /// GET: api/CafeData/ListCafes/creeds
        /// </example>
        [HttpGet]
        [Route("api/CafeData/Search/{SearchKey?}")]
        public IEnumerable<CafeDto> Search(string SearchKey = null)
        {
            List<Cafe> Cafes = new List<Cafe>();

            if (SearchKey == null)
            {
                Cafes = db.Cafes.ToList();
            }
            else
            {
                Cafes = db.Cafes.Where(c => c.Name.Contains(SearchKey)).ToList();
            }



            List<CafeDto> CafeDtos = new List<CafeDto>();
            Cafes.ForEach(c => CafeDtos.Add(new CafeDto()
            {
                CafeId = c.CafeId,
                OverpassId = c.OverpassId,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Description = c.Description,
                Website = c.Website,
            }));

            return CafeDtos;
        }


        /// <summary>
        /// Gathers information about a cafe related to a particular order ID -------> Change to list all orders for cafe
        /// </summary>
        /// <returns>
        ///  Returns a cafe in the database, including the associated order matched with a particular order ID
        /// </returns>
        /// <param name="id">Order Id.</param>
        /// <example>
        /// GET: api/CafeData/ListCafeForOrder/3
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CafeDto))]
        public IHttpActionResult ListCafeForOrder(int id)
        {
            //SQL Equivalent:
            //Select * from cafes where cafes.orderid = {id}
            //List<Cafe> Cafes = db.Cafes.Where(c => c.OrderId == id).ToList();
            List<Cafe> Cafes = db.Cafes.ToList();
            List<CafeDto> CafeDtos = new List<CafeDto>();

            Cafes.ForEach(c => CafeDtos.Add(new CafeDto()
            {
                CafeId = c.CafeId,
                OverpassId = c.OverpassId,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Description = c.Description,
                Website = c.Website,
            }));

            return Ok(CafeDtos);
        }

        /// <summary>
        /// Gathers information about cafes related to a particular item -------> Change to list all items at a cafe (a cafes entire menu)
        /// </summary>
        /// <returns>
        /// All cafes in the database that match to a particular item id
        /// </returns>
        /// <param name="id">Item Id.</param>
        /// <example>
        /// GET: api/CafeData/ListCafesWithItem/1
        /// </example>
        [HttpGet]
        [ResponseType(typeof(CafeDto))]
        public IHttpActionResult ListCafesWithItem(int id)
        {

            //List<Cafe> Cafes = db.Cafes.Where(
            //    c => c.Items.Any(
            //        i => i.ItemId == id
            //        )).ToList();
            List<Cafe> Cafes = db.Cafes.ToList();
            List<CafeDto> CafeDtos = new List<CafeDto>();

            Cafes.ForEach(c => CafeDtos.Add(new CafeDto()
            {
                CafeId = c.CafeId,
                OverpassId = c.OverpassId,
                Latitude = c.Latitude,
                Longitude = c.Longitude,
                Name = c.Name,
                Address = c.Address,
                Phone = c.Phone,
                Description = c.Description,
                Website = c.Website,
                //ItemId = c.ItemId,
                //ItemName = c.Item.ItemName //fix in cafedto
            }));

            return Ok(CafeDtos);
        }

        /// <summary>
        /// Associates a particular item with a particular cafe
        /// </summary>
        /// <param name="cafeid">The cafe ID primary key</param>
        /// <param name="itemid">The item ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/CafeData/AssociateCafeWithItem/9/1
        /// </example>
        [HttpPost]
        [Route("api/CafeData/AssociateCafeWithItem/{cafeid}/{itemid}")]
        public IHttpActionResult AssociateCafeWithItem(int cafeid, int itemid)
        {

            //Cafe SelectedCafe = db.Cafes.Include(c => c.Items).Where(c => c.CafeId == cafeid).FirstOrDefault();
            //Item SelectedItem = db.Item.Find(itemid);

            //if (SelectedCafe == null || SelectedItem == null) {
            //  return NotFound();
            //}

            //Debug.WriteLine("input cafe id is:" + cafeid);
            //Debug.WriteLine("selected cafe name is:" + SelectedCafe.Name);


            //SelectedCafe.Items.Add(SelectedItem);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Removes an association between a particular item and a particular cafe
        /// </summary>
        /// <param name="cafeid">The cafe ID primary key</param>
        /// <param name="itemid">The item ID primary key</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST api/CafeData/UnAssociateCafeWithItem/9/1
        /// </example>
        [HttpPost]
        [Route("api/CafeData/UnAssociateCafeWithItem/{cafeid}/{itemid}")]
        public IHttpActionResult UnAssociateCafeWithItem(int cafeid, int itemid)
        {

            //Cafe SelectedCafe = db.Cafes.Include(c => c.Items).Where(c => c.CafeId == cafeid).FirstOrDefault();
            Item SelectedItem = db.Items.Find(itemid);

            //if (SelectedCafe == null || SelectedItem == null) {
            //  return NotFound();
            //}

            Debug.WriteLine("input cafe id is: " + cafeid);

            //SelectedCafe.Items.Remove(SelectedItem);
            db.SaveChanges();

            return Ok();
        }

        /// <summary>
        /// Returns all cafes in the system.
        /// </summary>
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: A cafe in the system matching up to the cafe ID primary key
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <param name="id">The primary key of the cafe</param>
        /// <example>
        /// GET: api/CafeData/FindCafe/5
        /// </example>
        [ResponseType(typeof(CafeDto))]
        [HttpGet]
        public IHttpActionResult FindCafe(int id)
        {
            Cafe Cafe = db.Cafes.Find(id);
            CafeDto CafeDto = new CafeDto()
            {
                CafeId = Cafe.CafeId,
                OverpassId = Cafe.OverpassId,
                Latitude = Cafe.Latitude,
                Longitude = Cafe.Longitude,
                Name = Cafe.Name,
                Address = Cafe.Address,
                Phone = Cafe.Phone,
                Description = Cafe.Description,
                Website = Cafe.Website,
            };
            if (Cafe == null)
            {
                return NotFound();
            }

            return Ok(CafeDto);
        }

        /// <summary>
        /// Updates a particular cafe in the system with POST Data input
        /// </summary>
        /// <param name="id">Represents the cafe ID primary key</param>
        /// <param name="cafe">JSON FORM DATA of a cafe</param>
        /// <returns>
        /// HEADER: 204 (Success, No Content Response)
        /// or
        /// HEADER: 400 (Bad Request)
        /// or
        /// HEADER: 404 (Not Found)
        /// </returns>
        /// <example>
        /// POST: api/CafeData/UpdateCafe/5
        /// FORM DATA: cafe JSON Object
        /// </example>
        [ResponseType(typeof(void))]
        [HttpPost]
        public IHttpActionResult UpdateCafe(int id, Cafe Cafe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != Cafe.CafeId)
            {
                return BadRequest();
            }

            db.Entry(Cafe).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CafeExists(id))
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
        /// Adds a cafe to the system
        /// </summary>
        /// <param name="cafe">JSON FORM DATA of a cafe</param>
        /// <returns>
        /// HEADER: 201 (Created)
        /// CONTENT: Cafe ID, Cafe Data
        /// or
        /// HEADER: 400 (Bad Request)
        /// </returns>
        /// <example>
        /// POST: api/CafeData/AddCafe
        /// FORM DATA: Cafe JSON Object
        /// </example>
        [ResponseType(typeof(Cafe))]
        [HttpPost]
        public IHttpActionResult AddCafe(Cafe Cafe)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Cafes.Add(Cafe);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = Cafe.CafeId }, Cafe);
        }

        /// <summary>
        /// Deletes an cafe from the system by it's ID.
        /// </summary>
        /// <param name="id">The primary key of the cafe</param>
        /// <returns>
        /// HEADER: 200 (OK)
        /// or
        /// HEADER: 404 (NOT FOUND)
        /// </returns>
        /// <example>
        /// POST: api/CafeData/DeleteCafe/5
        /// FORM DATA: (empty)
        /// </example>
        [ResponseType(typeof(Cafe))]
        [HttpPost]
        public IHttpActionResult DeleteCafe(int id)
        {
            Cafe Cafe = db.Cafes.Find(id);
            if (Cafe == null)
            {
                return NotFound();
            }

            db.Cafes.Remove(Cafe);
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

        private bool CafeExists(int id)
        {
            return db.Cafes.Count(c => c.CafeId == id) > 0;
        }
    }
}