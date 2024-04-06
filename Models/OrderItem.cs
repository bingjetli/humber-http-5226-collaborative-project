using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models {
  public class OrderItem {

    //Each order contains a series of OrderItems. OrderItems are entities that
    //describe which Item is being ordered and how many.

    //An Order may have many OrderItems, but an OrderItem may only belong to 1 Order.
    //An Item may be referenced by many OrderItems, but an OrderItem may only
    //reference 1 Item.

    [Key]
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }

    //The OrderItem will store a reference to an ItemId. The referenced item may
    //also be accessible from the `ReferencedItem` navigation property.
    [ForeignKey("Item")]
    public int? ItemId { get; set; }
    public virtual Item Item { get; set; }

    //The OrderItem will also store a reference to the order that it belongs to.
    //The Order can be accessed from the `ReferencedOrder` navigation property.
    [ForeignKey("Order")]
    public int? OrderId { get; set; }
    public virtual Order Order { get; set; }


    //We don't need to reference the Cafe, because the Cafe can be referenced from
    //the Order, which we already have a reference to.


    public OrderItemDto ToDto() {
      return new OrderItemDto {
        OrderItemId = OrderItemId,
        Quantity = Quantity,
        OrderId = OrderId,
        ItemId = ItemId,
      };
    }

  }


  //OrderItemDto -Sarah
  public class OrderItemDto {
    public int OrderItemId { get; set; }
    public int Quantity { get; set; }
    public int? OrderId { get; set; }
    public int? ItemId { get; set; }
  }
}