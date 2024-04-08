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
using System.Web.Script.Serialization;
using System.Security.Cryptography.X509Certificates;

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
            //objective: with our cafe data api to retrieve a list of cafes
            //curl https://localhost:44328/api/cafedata/listall

            string url = "listall";

            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CafeDto> Cafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;

            //returns Views/Cafe/List.cshtml
            return View(Cafes);

        }

        // GET: Cafe/Details/5
        public ActionResult Details(int id)
        {

            //objective: communicate with our cafe data api to retrieve one cafe
            //curl https://localhost:44328/api/cafedata/findbyid/{id}

            string url = "findbyid/"+id;

            HttpResponseMessage response = client.GetAsync(url).Result;

           // Debug.WriteLine("The response code is ");
           //Debug.WriteLine(response.StatusCode);

            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;

            //ViewModel.SelectedCafe = SelectedCafe;

            //Debug.WriteLine("The cafe recieved is: " + SelectedCafe.CafeId);
            //Debug.WriteLine(SelectedCafe.Name);

            //show AVAILABLE items at this cafe
            url = "itemdata/listavailablecafeitems/" + id;

            response = client.GetAsync(url).Result;

            IEnumerable<ItemDto> AvailableItems = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;
            //ViewModel.AvailableItems = AvailableItems;

            //show UNAVAILABLE items at this cafe
            url = "itemdata/listunavailablecafeitems/" + id;

            response = client.GetAsync(url).Result;

            IEnumerable<ItemDto> UnavailableItems = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;
            //ViewModel.UnavailableItems = UnavailableItems;

            //return View(ViewModel);
            return View(SelectedCafe);

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

            //string url = "orderdata/listorders/";
            //HttpResponseMessage response = client.GetAsync(url).Result;
            //IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;


            //return View(OrderOptions);
            return View();

        }

        // POST: Cafe/Create
        [HttpPost]
        public ActionResult Create(Cafe Cafe)
        {
            //objective: add a new cafe into the system using the API
            //curl -H "Content-Type:application/json" -d @cafe.json https://localhost:44328/api/cafedata/createnew
            string url = "createnew";

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

            //UpdateCafe ViewModel = new UpdateCafe();

            //existing cafe info
            string url = "findbyid/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;
            
            //ViewModel.SelectedCafe = SelectedCafe;

            ////all orders to choose from when updating cafe
            ////existing cafe info
            //url = "orderdata/listorders/";
            //response = client.GetAsync(url).Result;
            //IEnumerable<OrderDto> OrderOptions = response.Content.ReadAsAsync<IEnumerable<OrderDto>>().Result;
    
            //ViewModel.OrderOptions = OrderOptions;
            //return View(ViewModel);

            return View(SelectedCafe);
        }

        // POST: Cafe/Update/5
        [HttpPost]
        public ActionResult Update(int id, Cafe cafe)
        {
            string url = "update/" + id;

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
            string url = "findbyid/" + id;

            HttpResponseMessage response = client.GetAsync(url).Result;

            CafeDto selectedcafe = response.Content.ReadAsAsync<CafeDto>().Result;

            return View(selectedcafe);
        }

        // POST: Cafe/Delete/5
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

        /** ASSOCIATIVE ROUTES */

        ////POST: Cafe/LinkToItem/{CafeId}/{ItemId}
        //[HttpPost]
        public ActionResult LinkToItem(int id, int ItemId)
        {
            Debug.WriteLine("Attempting to link cafe :" + id + "with item" + ItemId);

            //call api to link cafe with item
            string url = "cafedata/linktoitem/" + id + "/" + ItemId;

            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        //Get:Cafe/UnlinkWithItem/{id}?ItemId={itemId}
        [HttpGet]
        public ActionResult UnlinkWithItem(int id, int ItemId)
        {
            //Debug.WriteLine("Attempting to unlink cafe :" + id + "with item" + ItemId);

            //call api to unlink cafe with item
            string url = "cafedata/unlinkwithitem/" + id + "/" + ItemId;

            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        ////POST: Cafe/LinkToOrder/{CafeId}/{OrderId}
        //[HttpPost]
        public ActionResult LinkToOrder(int id, int OrderId)
        {
            Debug.WriteLine("Attempting to link cafe :" + id + "with order" + OrderId);

            //call api to link cafe with order
            string url = "cafedata/linktoorder/" + id + "/" + OrderId;

            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }


        //Get:Cafe/UnlinkWithItem/{id}?ItemId={itemId}
        [HttpGet]
        public ActionResult UnlinkWithOrder(int id, int OrderId)
        {
            //Debug.WriteLine("Attempting to unlink cafe :" + id + "with order" + OrderId);

            //call api to unlink cafe with order
            string url = "cafedata/unlinkwithorder/" + id + "/" + OrderId;

            HttpContent content = new StringContent("");

            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;

            return RedirectToAction("Details/" + id);
        }
    }
}