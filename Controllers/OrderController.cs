using humber_http_5226_collaborative_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using static humber_http_5226_collaborative_project.Models.Cafe;

namespace humber_http_5226_collaborative_project.Controllers {
    public class OrderController : Controller {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static OrderController() {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/orderdata/");
        }

        /**   BASIC CRUD   */

        // GET: Order/List
        public ActionResult List() {
            HttpResponseMessage response = client.GetAsync("listall").Result;
            if (response.IsSuccessStatusCode) {
                var orders = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
                return View(orders);
            }
            return View("Error");
        }

        // GET: Order/Details/5
        public ActionResult Details(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var order = response.Content.ReadAsAsync<OrderDto>().Result;
                return View(order);
            }
            return View("Error");
        }

        // GET: Order/New
        [System.Web.Mvc.Authorize]
        public ActionResult New() {
            //information about all cafes in the system
            //GET api/cafedata/listall

            string url = "cafedata/listall/";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<CafeDto> CafeOptions = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;


            return View(CafeOptions);
        }

        // POST: Order/Create
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Create(OrderDto order) {
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("createnew", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Order/Edit/5
        [System.Web.Mvc.Authorize]
        public ActionResult Edit(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;
                return View(SelectedOrder);
            }
            return View("Error");
        }

        // POST: Order/Update/5
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Update(int id, OrderDto order) {
            var json = JsonConvert.SerializeObject(order);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync($"update/{id}", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Order/DeleteConfirm/5
        [System.Web.Mvc.Authorize]
        public ActionResult DeleteConfirm(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var order = response.Content.ReadAsAsync<OrderDto>().Result;
                return View(order);
            }
            return View("Error");
        }

        // POST: Order/Delete/5
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Delete(int id) {
            HttpResponseMessage response = client.DeleteAsync($"delete/{id}").Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }
    }
}