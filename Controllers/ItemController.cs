﻿using humber_http_5226_collaborative_project.Models;
using humber_http_5226_collaborative_project.Models.viewmodel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace humber_http_5226_collaborative_project.Controllers {
    public class ItemController : Controller {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ItemController() {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/itemdata/");
        }

        /**   BASIC CRUD   */

        // GET: Item/List
        public ActionResult List() {
            HttpResponseMessage response = client.GetAsync("listall").Result;
            if (response.IsSuccessStatusCode) {
                var items = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;
                return View(items);
            }
            return View("Error");
        }

        // GET: Item/Details/5
        public ActionResult Details(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var item = response.Content.ReadAsAsync<ItemDto>().Result;
                return View(item);
            }
            return View("Error");
        }

        // GET: Item/New
        [System.Web.Mvc.Authorize]
        public ActionResult New() {
            return View();
        }

        // POST: Item/Create
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Create(ItemDto item) {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("createnew", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Item/Edit/5
        [System.Web.Mvc.Authorize]
        public ActionResult Edit(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var SelectedItem = response.Content.ReadAsAsync<ItemDto>().Result;
                DetailsItem di = new DetailsItem {
                    SelectedItem = SelectedItem
                };
                return View(di);
            }
            return View("Error");
        }

        // POST: Item/Update/5
        [System.Web.Mvc.Authorize]
        [HttpPost]
        public ActionResult Update(int id, ItemDto item) {
            var json = JsonConvert.SerializeObject(item);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync($"update/{id}", content).Result;
            if (response.IsSuccessStatusCode) {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: Item/DeleteConfirm/5
        [System.Web.Mvc.Authorize]
        public ActionResult DeleteConfirm(int id) {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode) {
                var item = response.Content.ReadAsAsync<ItemDto>().Result;
                return View(item);
            }
            return View("Error");
        }

        // POST: Item/Delete/5
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
