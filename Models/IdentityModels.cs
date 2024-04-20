using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;

namespace humber_http_5226_collaborative_project.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }


        //Apparently, `.PhoneNumber` is already inherited from IdentityUser.
        //public string PhoneNumber {  get; set; }

        //`.UserName` is also already inherited from IdentityUser.
        //public string UserName { get; set; }

        //`.Email` is also already inherited from IdentityUser.
        //public string Email {  get; set; }

        //`Id` seems to be the inherited Id property for ApplicationUser.
        //public string Id {  get; set; }


        //public DateTime DateOfBirth { get; set; }
        //public string Address { get; set; }

        //[ForeignKey("CourierLicense")]
        //public int? CourierLicenseId { get; set; }
        //public virtual CourierLicense CourierLicense { get; set; }

        //A very hack-y way of establishing a relationship between CourierLicense
        //and ApplicationUser. TODO: Fix this at some point.
        public virtual ICollection<CourierLicense> CourierLicenses { get; set; }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false) {
        }

        public DbSet<Cafe> Cafes { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<CourierLicense> CourierLicenses { get; set; }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }
    }
}