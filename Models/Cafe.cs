using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models
{
    public class Cafe
    {

        //A Cafe stores basic details about itself, along with all the Items it sells
        //and all the Orders that have been made to the Cafe.

        [Key]
        public int CafeId { get; set; }
        public long OverpassId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Description { get; set; }
        public string Website { get; set; }

        //A Cafe can have many Items on the menu, and each Item may be referenced by
        //multiple Cafes. This is true if the item is a generic item like coffee or tea.

        //FORESIGHT: Duplicates may still occur if different cafes sell the same item
        //but under another price. Maybe there's an opportunity to improve this?
        public virtual ICollection<Item> Menu { get; set; }

        //A Cafe can have many Orders associated with it, but an Order can only be
        //associated to 1 Cafe.
        public virtual ICollection<Order> Orders { get; set; }
    }

        //CafeDto -Sarah
        public class CafeDto
        {
            public int CafeId { get; set; }
            public long OverpassId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string Name { get; set; }
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Description { get; set; }
            public string Website { get; set; }

            // Inclusion of menu items in DTO
            public ICollection<ItemDto> Menu { get; set; }

            // Inclusion of order history in DTO
            public ICollection<OrderDto> Orders { get; set; }
    }
    }