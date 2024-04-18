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

        // GET: CourierLicense/List
        public ActionResult List()
        {
            HttpResponseMessage response = client.GetAsync("listall").Result;
            if (response.IsSuccessStatusCode)
            {
                var courierLicenses = response.Content.ReadAsAsync<IEnumerable<CourierLicenseDto>>().Result;
                return View(courierLicenses);
            }
            return View("Error");
        }

        // GET: CourierLicense/Details/5
        public ActionResult Details(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var courierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
                return View(courierLicense);
            }
            return View("Error");
        }

        // GET: CourierLicense/New
        public ActionResult New()
        {
            return View();
        }

        // POST: CourierLicense/Create
        [HttpPost]
        public ActionResult Create(CourierLicenseDto courierLicense)
        {
            var json = JsonConvert.SerializeObject(courierLicense);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync("createnew", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: CourierLicense/Edit/5
        public ActionResult Edit(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                CourierLicenseDto SelectedCourierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
                return View(SelectedCourierLicense);
            }
            return View("Error");
        }

        // POST: CourierLicense/Update/5
        [HttpPost]
        public ActionResult Update(int id, CourierLicenseDto courierLicense)
        {
            var json = JsonConvert.SerializeObject(courierLicense);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PutAsync($"update/{id}", content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            return View("Error");
        }

        // GET: CourierLicense/DeleteConfirm/5
        public ActionResult DeleteConfirm(int id)
        {
            HttpResponseMessage response = client.GetAsync($"findbyid/{id}").Result;
            if (response.IsSuccessStatusCode)
            {
                var courierLicense = response.Content.ReadAsAsync<CourierLicenseDto>().Result;
                return View(courierLicense);
            }
            return View("Error");
        }

        // POST: CourierLicense/Delete/5
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
