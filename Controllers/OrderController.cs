using humber_http_5226_collaborative_project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
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
            client.BaseAddress = new Uri("https://localhost:44328/api/");
        }

        // GET: Order/List
        public ActionResult List(string SearchKey = null)
        {
            //objective: with our order data api to retrieve a list of orders
            //curl https://localhost:44328/api/orderdata/listorders

            string url = "orderdata/listorders/" + SearchKey;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<OrderDto> Orders = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;

            //returns Views/Order/List.cshtml
            return View(Orders);
        }
    }
}