using humber_http_5226_collaborative_project.Migrations;
using humber_http_5226_collaborative_project.Models;
using humber_http_5226_collaborative_project.Models.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static humber_http_5226_collaborative_project.Models.Cafe;

namespace humber_http_5226_collaborative_project.Controllers {
    public class CafeController : Controller {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CafeController() {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/cafedata/");
        }

        /**   BASIC CRUD   */


        // GET: Cafe/List
        public ActionResult List() {
            HttpResponseMessage response = client.GetAsync("listall").Result;
            if (response.IsSuccessStatusCode) {
                var cafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;
                return View(cafes);
            }
            return View("Error");
        }

        // GET: Cafe/Details/5
        public ActionResult Details(int id) {
            var response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var cafe = response.Content.ReadAsAsync<CafeDto>().Result;
                return View(cafe);
            }
            return View("Error");
        }

        // GET: Cafe/New
        [System.Web.Mvc.Authorize]
        public ActionResult New() {
            return View();
        }

        // POST: Cafe/Create
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Create(CafeDto cafe) {
            var json = JsonConvert.SerializeObject(cafe);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = client.PostAsync("createnew", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Cafe/Edit/5
        [System.Web.Mvc.Authorize]
        public ActionResult Edit(int id) {
            UpdateCafe ViewModel = new UpdateCafe();

            //existing cafe info
            string url = "findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.SelectedCafe = SelectedCafe;

            return View(ViewModel);
        }

        // POST: Cafe/Update/5
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Update(int id, CafeDto cafe) {
            var json = JsonConvert.SerializeObject(cafe);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = client.PutAsync($"update/{id}", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Cafe/DeleteConfirm/5
        [System.Web.Mvc.Authorize]
        public ActionResult DeleteConfirm(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var cafe = response.Content.ReadAsAsync<CafeDto>().Result;
                return View(cafe);
            }
            return View("Error");
        }

        // POST: Cafe/Delete/5
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Delete(int id) {
            HttpResponseMessage response = client.DeleteAsync($"delete/{id}").Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // POST: Cafe/LinkToItem
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult LinkToItem(int cafeId, int itemId) {
            var response = client.PostAsync($"linktoitem/{cafeId}/{itemId}", null).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/UnlinkWithItem
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult UnlinkWithItem(int cafeId, int itemId) {
            var response = client.PostAsync($"unlinkwithitem/{cafeId}/{itemId}", null).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/LinkToOrder
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult LinkToOrder(int cafeId, int orderId) {
            var response = client.PostAsync($"linktoorder/{cafeId}/{orderId}", null).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }

        // POST: Cafe/UnlinkWithOrder
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult UnlinkWithOrder(int cafeId, int orderId) {
            var response = client.PostAsync($"unlinkwithorder/{cafeId}/{orderId}", null).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("Details", new { id = cafeId });
            }
            return View("Error");
        }
    }
}