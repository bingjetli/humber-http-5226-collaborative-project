using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models {
  public class Order {

    //Each Order will store details about itself, along with a series of OrderItems
    //which define the Items along with the quantity associated with this Order.
    [Key]
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }
    public virtual ICollection<OrderItem> OrderItems { get; set; }

    //The Order will also store a reference to the Cafe that the Order belongs to.
    //The Cafe can be referenced by the `ReferencedCafe` navigation property.
    [ForeignKey("Cafe")]
    public int? CafeId { get; set; }
    public virtual Cafe Cafe { get; set; }

    //This property is nullable, meaning it can be null if there isn't a courier
    //assigned to this order. 
    [ForeignKey("CourierLicense")]
    public int? CourierLicenseId { get; set; }
    public virtual CourierLicense CourierLicense { get; set; }

    public string Status { get; set; }



    public OrderDto ToDto() {
      return new OrderDto {
        OrderId = OrderId,
        CreatedAt = CreatedAt,
        CafeId = CafeId,
        CourierLicenseId = CourierLicenseId,
        Status = Status,
      };
    }

  }

  //OrderDto -Sarah
  public class OrderDto {
    public int OrderId { get; set; }
    public DateTime CreatedAt { get; set; }

    public int? CafeId { get; set; }
    public int? CourierLicenseId { get; set; }
    public string Status { get; set; }

    //public virtual ICollection<OrderItem> OrderItems { get; set; }
  }
}