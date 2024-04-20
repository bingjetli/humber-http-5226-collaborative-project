using humber_http_5226_collaborative_project.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class OrderItemController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static OrderItemController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/orderitemdata/");
        }

        /**   BASIC CRUD   */

        // GET: OrderItem/List
        public ActionResult List()
        {
            HttpResponseMessage response = client.GetAsync("listall").Result;
            if (response.IsSuccessStatusCode)
            {
                var orderItems = response.Content.ReadAsAsync<IEnumerable<OrderItemDto>>().Result;
                return View(orderItems);
            }
            return View("Error");
        }

        // GET: OrderItem/Details/5
        public ActionResult Details(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var orderItem = response.Content.ReadAsAsync<OrderItemDto>().Result;
                return View(orderItem);
            }
            return View("Error");
        }

        // GET: OrderItem/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderItem/Create
        [HttpPost]
        public ActionResult Create(OrderItemDto orderItem)
        {
            var json = JsonConvert.SerializeObject(orderItem);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("createnew", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: OrderItem/Edit/5
        public ActionResult Edit(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                OrderItemDto SelectedOrderItem = response.Content.ReadAsAsync<OrderItemDto>().Result;
                return View(SelectedOrderItem);
            }
            return View("Error");
        }

        // POST: OrderItem/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, OrderItemDto orderItem)
        {
            var json = JsonConvert.SerializeObject(orderItem);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync($"update/{id}", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: OrderItem/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var orderItem = response.Content.ReadAsAsync<OrderItemDto>().Result;
                return View(orderItem);
            }
            return View("Error");
        }

        // POST: OrderItem/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            HttpResponseMessage response = client.DeleteAsync($"delete/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }
    }
}