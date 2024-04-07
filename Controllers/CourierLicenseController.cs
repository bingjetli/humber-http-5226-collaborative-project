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
    public class CourierLicenseController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static CourierLicenseController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44328/api/courierlicensedata/");
        }

        /**   BASIC CRUD   */

        // GET: CourierLicense
        public ActionResult Index()
        {
            //objective: communicate with our courier license data api to retrieve a list of courier licenses
            //curl https://localhost:44328/api/courierlicensedata/listall

            string url = "listall";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<CourierLicenseDto> CourierLicenses = response.Content.ReadAsAsync<IEnumerable<CourierLicenseDto>>().Result;

            //returns Views/CourierLicense/List.cshtml
            return View(CourierLicenses);
        }

        // GET: CourierLicense/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our  courier license data api to retrieve one  courier license
            //curl https://localhost:44328/api/courierlicensedata/findbyid/{id}

            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            CourierLicenseDto SelectedCourierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
            return View(SelectedCourierLicense);
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

        // POST: CourierLicense/Create
        [HttpPost]
        public ActionResult Create(CourierLicense CourierLicense)
        {
            //objective: add a new courier license into our system using the API
            //curl -H "Content-Type:application/json" -d @Order.json https://localhost:44328/api/courierlicensedata/createnew 
            string url = "createnew";


            string jsonpayload = jss.Serialize(CourierLicense);

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

        // GET: CourierLicense/Edit/5
        public ActionResult Edit(int id)
        {
            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CourierLicenseDto SelectedCourierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
            return View(SelectedCourierLicense);
        }

        // POST: CourierLicense/Update/5
        [HttpPost]
        public ActionResult Update(int id, CourierLicense CourierLicense)
        {
            string url = "update/" + id;
            string jsonpayload = jss.Serialize(CourierLicense);
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

        // GET: CourierLicense/Delete/5
        public ActionResult DeleteConfirm(int id)
        {
            string url = "findbyid/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;
            CourierLicenseDto SelectedCourierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
            return View(SelectedCourierLicense);
        }

        // POST: CourierLicense/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, CourierLicense CourierLicense)
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
