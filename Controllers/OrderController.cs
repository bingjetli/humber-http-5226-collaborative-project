using humber_http_5226_collaborative_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static humber_http_5226_collaborative_project.Models.Cafe;
using System.Web.Script.Serialization;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class OrderController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static OrderController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44321/api/");
        }

        // GET: Order/List
        public ActionResult List(string SearchKey = null)
        {
            //objective: communicate with our order data api to retrieve a list of orders
            //curl https://localhost:44321/api/orderdata/listorders

            //DetailsOrder ViewModel = new DetailsOrder();

            string url = "orderdata/listorders/" + SearchKey;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<OrderDto> Orders = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;


            return View(Orders);
        }

        // GET: Order/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our order data api to retrieve one order
            //curl https://localhost:44321/api/orderdata/findorders/{id}

            DetailsOrder ViewModel = new DetailsOrder();

            string url = "orderdata/findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;

            ViewModel.SelectedOrder = SelectedOrder;

            url = "cafedata/ListCafesForOrder/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CafeDto> RelatedCafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;

            ViewModel.RelatedCafes = RelatedCafes;


            return View(ViewModel);
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
            //curl -H "Content-Type:application/json" -d @Order.json https://localhost:44324/api/orderdata/addOrder 
            string url = "orderdata/addorder";


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
            string url = "orderdata/findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(SelectedOrder);
        }

        // POST: Order/Update/5
        [HttpPost]
        public ActionResult Update(int id, Order Order)
        {

            string url = "orderdata/updateorder/" + id;
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
            string url = "orderdata/findorder/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            OrderDto SelectedOrder = response.Content.ReadAsAsync<OrderDto>().Result;
            return View(SelectedOrder);
        }

        // POST: Order/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "orderdata/deleteorder/" + id;
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