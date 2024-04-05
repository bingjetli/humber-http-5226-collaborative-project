using humber_http_5226_collaborative_project.Models;
using humber_http_5226_collaborative_project.Models.viewmodel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using static humber_http_5226_collaborative_project.Models.Cafe;
using System.Web.Script.Serialization;

namespace humber_http_5226_collaborative_project.Controllers
{
    public class CafeController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();


        static CafeController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44321/api/");
        }

        // GET: Cafe/List
        public ActionResult List(string SearchKey = null)
        {
            //objective: with our cafe data api to retrieve a list of cafes
            //curl https://localhost:44321/api/cafedata/listcafes

            string url = "cafedata/listcafes/" + SearchKey;
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CafeDto> Cafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;

            //returns Views/Cafe/List.cshtml
            return View(Cafes);

        }

        // GET: Cafe/Details/5
        public ActionResult Details(int id)
        {
            DetailsCafe ViewModel = new DetailsCafe();

            //objective: with our cafe data api to retrieve one cafe
            //curl https://localhost:44321/api/cafedata/findcafe/{id}

            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.SelectedCafe = SelectedCafe;

            Debug.WriteLine("The cafe recieved is: " + SelectedCafe.CafeId);
            Debug.WriteLine(SelectedCafe.Name);
           
            //show associated items with this cafe
            url = "itemdata/listitemsforcafe/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ItemDto> AvailableItems = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;
            ViewModel.AvailableItems = AvailableItems;


            url = "itemdata/listitemsnotincafe/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<ItemDto> CurrentItems = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;
            ViewModel.CurrentItems = CurrentItems;

            return View(ViewModel);
        }

        //POST: Cafe/Associate/{CafeId}/{ItemId}
        [HttpPost]
        public ActionResult Associate(int id, int ItemId)
        {
            Debug.WriteLine("Attempting to associate cafe :" + id + "with item" + ItemId);

            //call api to associate cafe with item
            string url = "cafedata/associatecafewithitem/" + id + "/" + ItemId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }

        //Get:Cafe/Unassociate/{id}?ItemId={itemId}
        [HttpGet]
        public ActionResult UnAssociate(int id, int ItemId)
        {
            Debug.WriteLine("Attempting to unassociate cafe :" + id + "with item" + ItemId);

            //call api to unassociate cafe with item
            string url = "cafedata/unassociatecafewithitem/" + id + "/" + ItemId;
            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }
        public ActionResult Error()
        {
            return View();
        }

        // GET: Cafe/New
        public ActionResult New()
        {
            //information about all orders in the system
            //GET api/orderdata/listorders

            string url = "orderdata/listorders/";
            HttpResponseMessage response = client.GetAsync(url).Result;
            IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;


            return View(OrderOptions);
        }

        // POST: Cafe/Create
        [HttpPost]
        public ActionResult Create(Cafe Cafe)
        {
            //objective: add a new cafe into the system using the API
            //curl -H "Content-Type:application/json" -d @cafe.json https://localhost:44321/api/cafedata/addcafe
            string url = "cafedata/addcafe";

            string jsonpayload = jss.Serialize(Cafe);
            //Debug.WriteLine(jsonpayload);

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

        // GET: Cafe/Edit/5
        public ActionResult Edit(int id)
        {


            UpdateCafe ViewModel = new UpdateCafe();

            //existing cafe info
            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CafeDto selectedcafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.selectedcafe = selectedcafe;

            //all orders to choose from when updating cafe
            //existing cafe info
            url = "orderdata/listorders/";
            response = client.GetAsync(url).Result;
            IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
    
            ViewModel.OrderOptions = OrderOptions;
            return View(ViewModel);
        }

        // POST: Cafe/Update/5
        [HttpPost]
        public ActionResult Update(int id, Cafe cafe)
        {
            string url = "cafedata/updatecafe/" + id;
            string jsonpayload = jss.Serialize(cafe);
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

        // GET: Cafe/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CafeDto selectedcafe = response.Content.ReadAsAsync<CafeDto>().Result;
            return View(selectedcafe);
        }

        // POST: Cafe/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "cafedata/deletecafe/" + id;
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