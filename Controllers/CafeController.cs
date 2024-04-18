using humber_http_5226_collaborative_project.Models;
using humber_http_5226_collaborative_project.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static humber_http_5226_collaborative_project.Models.Cafe;
using humber_http_5226_collaborative_project.Migrations;
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CafeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CafeController()
        { 
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/cafedata/");
        }

        /**   BASIC CRUD   */


        // GET: Cafe/List
        public ActionResult List()
        {
            HttpResponseMessage response = client.GetAsync("cafedata/listall").Result;
            if (response.IsSuccessStatusCode)
            {
                var cafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;
                return View(cafes);
            }
            return View("Error");
        }

        // GET: Cafe/Details/5
        public ActionResult Details(int id)
        {
            var response = client.GetAsync($"cafedata/findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var cafe = response.Content.ReadAsAsync<CafeDto>().Result;
                return View(cafe);
            }
            return View("Error");
        }

        // GET: Cafe/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Cafe/Create
        [HttpPost]
        public ActionResult Create(CafeDto cafe)
        {
            var json = JsonConvert.SerializeObject(cafe);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = client.PostAsync("cafedata/createnew", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Cafe/Edit/5
        public ActionResult Edit(int id)
        {
            UpdateCafe ViewModel = new UpdateCafe();

            //existing cafe info
            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.SelectedCafe = SelectedCafe;

            return View(ViewModel);
        }

        // POST: Cafe/Update/5
        [HttpPost]
        public ActionResult Update(int id, CafeDto cafe)
        {
            var json = JsonConvert.SerializeObject(cafe);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = client.PutAsync($"cafedata/update/{id}", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Cafe/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            HttpResponseMessage response = client.GetAsync($"cafedata/findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var cafe = response.Content.ReadAsAsync<CafeDto>().Result;
                return View(cafe);
            }
            return View("Error");
        }

        // POST: Cafe/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            HttpResponseMessage response = client.DeleteAsync($"cafedata/delete/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // POST: Cafe/LinkToItem
        [HttpPost]
        public ActionResult LinkToItem(int cafeId, int itemId)
        {
            var response = client.PostAsync($"cafedata/linktoitem/{cafeId}/{itemId}", null).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/UnlinkWithItem
        [HttpPost]
        public ActionResult UnlinkWithItem(int cafeId, int itemId)
        {
            var response = client.PostAsync($"cafedata/unlinkwithitem/{cafeId}/{itemId}", null).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/LinkToOrder
        [HttpPost]
        public ActionResult LinkToOrder(int cafeId, int orderId)
        {
            var response = client.PostAsync($"cafedata/linktoorder/{cafeId}/{orderId}", null).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/UnlinkWithOrder
        [HttpPost]
        public ActionResult UnlinkWithOrder(int cafeId, int orderId)
        {
            var response = client.PostAsync($"cafedata/unlinkwithorder/{cafeId}/{orderId}", null).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }
    }
}