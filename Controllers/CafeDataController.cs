using humber_http_5226_collaborative_project.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CafeDataController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        /// Returns all cafes in the system.
        /// </summary>
        /// <returns>
        /// All cafes in the database with names containing the seach key, including their associated districts and addresses.
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
                Name = c.Name,
                OverpassId = c.OverpassId,
                Address = c.Address,
                Phone = c.Phone,
                Description = c.Description,
                Website = c.Website,
                Latitude = c.Latitude,
                Longitude = c.Longitude
            }));

            return CafeDtos;
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
                Name = Cafe.Name,
                OverpassId = Cafe.OverpassId,
                Address = Cafe.Address,
                Phone = Cafe.Phone,
                Description = Cafe.Description,
                Website = Cafe.Website,
                Latitude = Cafe.Latitude,
                Longitude = Cafe.Longitude
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