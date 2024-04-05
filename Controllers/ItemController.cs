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
    public class ItemController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static ItemController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44321/api/");
        }

        // GET: Item/List
        public ActionResult List()
        {
            //objective: communicate with our item data api to retrieve a list of items
            //curl https://localhost:44321/api/itemdata/listitems


            string url = "itemdata/listitems";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<ItemDto> Items = response.Content.ReadAsAsync<IEnumerable<ItemDto>>().Result;


            return View(Items);
        }

        // GET: Item/Details/5
        public ActionResult Details(int id)
        {
            DetailsItem ViewModel = new DetailsItem();

            //objective: communicate with our itemdata api to retrieve one one item
            //curl https://localhost:44321/api/itemdata/finditem/{id}

            string url = "itemdata/finditem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is: ");
            Debug.WriteLine(response.StatusCode);


            ItemDto SelectedItem = response.Content.ReadAsAsync<ItemDto>().Result;
            Debug.WriteLine("The item recived is: ");
            Debug.WriteLine(SelectedItem.ItemId);



            ViewModel.SelectedItem = SelectedItem;

            //show all cafes with an item
            url = "cafedata/ListCafesWithItem/" + id;
            response = client.GetAsync(url).Result;
            IEnumerable<CafeDto> RelevantCafes = response.Content.ReadAsAsync<IEnumerable<CafeDto>>().Result;

            ViewModel.RelevantCafes = RelevantCafes;


            return View(ViewModel);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Item/New
        public ActionResult New()
        {
            return View();
        }

        // POST: Item/Create
        [HttpPost]
        public ActionResult Create(Item Item)
        {

            //objective: add a new item into our system using the API
            //curl -H "Content-Type:application/json" -d @Item.json https://localhost:44321/api/itemdata/additem 
            string url = "itemdata/additem";


            string jsonpayload = jss.Serialize(Item);

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


        // GET: Item/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "itemdata/finditem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ItemDto SelectedItem = response.Content.ReadAsAsync<ItemDto>().Result;
            return View(SelectedItem);
        }

        // POST: Item/Update/5
        [HttpPost]
        public ActionResult Update(int id, Item Item)
        {
            string url = "itemdata/updateitem/" + id;
            string jsonpayload = jss.Serialize(Item);
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

        // GET: Item/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "itemdata/finditem/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            ItemDto SelectedItem = response.Content.ReadAsAsync<ItemDto>().Result;
            return View(SelectedItem);
        }

        // POST: Item/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            string url = "itemdata/deleteitem/" + id;
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
