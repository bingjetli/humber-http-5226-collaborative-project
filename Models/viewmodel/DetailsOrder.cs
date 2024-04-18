using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace humber_http_5226_collaborative_project.Models.viewmodel
{
    public class DetailsOrder
    {
        public OrderDto SelectedOrder { get; set; }
        public IEnumerable<CafeDto> CafeOptions { get; set; }
    }
}