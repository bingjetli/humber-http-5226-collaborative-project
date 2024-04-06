using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models {
    public class CourierLicense
    {

        //The CourierLicense contains all the details of the courier, along with the
        //Orders that have been assigned to this license and the associated ApplicationUser.

        //An ApplicationUser may only have 1 CourierLicense, and a CourierLicense may
        //only belong to 1 ApplicationUser.

        //A CourierLicense may be associated with many Orders, but an Order may only
        //be associated with a single Courier.
        [Key]
        public int CourierLicenseId { get; set; }
        public string VehicleType { get; set; }
        public bool IsValid { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime IssueDate { get; set; }

        //Since ApplicationUser uses a string Id by default, this is the exception
        //to our naming conventions.
        //[ForeignKey("ApplicationUser")]
        //public string Id { get; set; }
        //public virtual ApplicationUser ApplicationUser { get; set; }


        public virtual ICollection<Order> AssignedOrders { get; set; }

        //TODO: Fix this relationship at some point...
        public virtual ICollection<ApplicationUser> ApplicationUsers { get; set; }
    }

        //CourierLicenseDto -Sarah
        //ApplicationUser?
        public class CourierLicenseDto
        {
            public int CourierLicenseId { get; set; }
            public string VehicleType { get; set; }
            public bool IsValid { get; set; }
            public bool IsAvailable { get; set; }
            public DateTime IssueDate { get; set; }

            // Inclusion of assigned order in DTO
            public ICollection<Order> AssignedOrders { get; set; }


    }
    }