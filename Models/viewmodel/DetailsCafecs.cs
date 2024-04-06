using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models.ViewModels {
  public class DetailsCafe {
    //public CafeDto SelectedCafe { get; set; }
    public IEnumerable<OrderDto> AvailableOrders { get; set; }

        public IEnumerable<OrderDto> CurrentOrders { get; set; }
    }
}