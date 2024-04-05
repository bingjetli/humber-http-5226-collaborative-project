using humber_http_5226_collaborative_project.Models;
using humber_http_5226_collaborative_project.Models.viewmodel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
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
            client.BaseAddress = new Uri("https://localhost:44328/api/");
        }

        // GET: Cafe/List
        public ActionResult List(string SearchKey = null)
        {
            //objective: with our cafe data api to retrieve a list of cafes
            //curl https://localhost:44328/api/cafedata/listcafes

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
            //curl https://localhost:44328/api/cafedata/findcafe/{id}

            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            Debug.WriteLine("The response code is ");
            Debug.WriteLine(response.StatusCode);

            CafeDto SelectedCafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.SelectedCafe = SelectedCafe;

            Debug.WriteLine("The cafe recieved is: " + SelectedCafe.CafeId);
           

            return View(ViewModel);
        }

        public ActionResult Error()
        {
            return View();
        }

        // GET: Cafe/New
        public ActionResult New()
        {
            //curl https://localhost:44357/api/cafedata/Addcafe
            return View();

        }

        // POST: Cafe/Create
        [HttpPost]
        public ActionResult Create(Cafe Cafe)
        {
            //objective: add a new cafe into the system using the API
            //curl -H "Content-Type:application/json" -d @cafe.json https://localhost:44357/api/cafedata/addcafe
            string url = "cafedata/addcafe";

            string jsonpayload = jss.Serialize(Cafe);
      

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


            Models.viewmodel.UpdateCafe ViewModel = new Models.viewmodel.UpdateCafe();

            //existing cafe info
            string url = "cafedata/findcafe/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CafeDto selectedcafe = response.Content.ReadAsAsync<CafeDto>().Result;
            ViewModel.selectedcafe = selectedcafe;

    
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