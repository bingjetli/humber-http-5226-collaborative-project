using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using humber_http_5226_collaborative_project.Models;
using static humber_http_5226_collaborative_project.Models.Cafe;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CafeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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
        [Route("api/CafeData/ListCafes/{SearchKey?}")]
        public IEnumerable<CafeDto> ListCafes(string SearchKey = null)
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
            List<Cafe> Cafes = db.Cafes.Where(c => c.OrderId == id).ToList();
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

            List<Cafe> Cafes = db.Cafes.Where(
                c => c.Items.Any(
                    i => i.ItemId == id
                    )).ToList();
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
                ItemId = c.ItemId,
                ItemName = c.Item.ItemName //fix in cafedto
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

            Cafe SelectedCafe = db.Cafes.Include(c => c.Items).Where(c => c.CafeId == cafeid).FirstOrDefault();
            Item SelectedItem = db.Item.Find(itemid);

            if (SelectedCafe == null || SelectedItem == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input cafe id is:" + cafeid);
            Debug.WriteLine("selected cafe name is:" + SelectedCafe.Name);


            SelectedCafe.Items.Add(SelectedItem);
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

            Cafe SelectedCafe = db.Cafes.Include(c => c.Items).Where(c => c.CafeId == cafeid).FirstOrDefault();
            Item SelectedItem = db.Items.Find(itemid);

            if (SelectedCafe == null || SelectedItem == null)
            {
                return NotFound();
            }

            Debug.WriteLine("input cafe id is: " + cafeid);

            SelectedCafe.Items.Remove(SelectedItem);
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