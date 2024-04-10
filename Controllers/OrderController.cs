﻿using humber_http_5226_collaborative_project.Models;
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

        static OrderController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/orderdata/");
        }

        /**   BASIC CRUD   */

        // GET: Order/List
        public ActionResult List()
        {
            //objective: communicate with our order data api to retrieve a list of orders
            //curl https://localhost:44321/api/orderdata/listall

            //DetailsOrder ViewModel = new DetailsOrder();

            string url = "listall";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<OrderDto> Orders = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;


            return View(Orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our order data api to retrieve one order
            //curl https://localhost:44328/api/orderdata/findbyid/{id}

            //DetailsOrder ViewModel = new DetailsOrder();

            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;

            //ViewModel.SelectedOrder = SelectedOrder;

            //url = "cafedata/ListCafesForOrder/" + id;
            //response = client.GetAsync(url).Result;
            //IEnumerable<CafeDto> RelatedCafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;

            //ViewModel.RelatedCafes = RelatedCafes;


            //return View(ViewModel);

            return View(SelectedOrder);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Order/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Order/Create
        [HttpPost]
        public ActionResult Create(Order Order)
        {
            //objective: add a new order into our system using the API
            //curl -H "Content-Type:application/json" -d @Order.json https://localhost:44328/api/orderdata/createnew 
            string url = "createnew";


            string jsonpayload = jss.Serialize(Order);

            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // POST: Order/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(SelectedOrder);
        }

        // POST: Order/Update/5
        [HttpPost]
        public ActionResult Update(int id, Order Order)
        {

            string url = "update/" + id;
            string jsonpayload = jss.Serialize(Order);
            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }

        // GET: Order/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(SelectedOrder);
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "delete/" + id;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                return RedirectToAction("Error");
            }
        }
    }
}