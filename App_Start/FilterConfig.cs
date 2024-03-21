using System.Web;
using System.Web.Mvc;

namespace humber_http_5226_collaborative_project {
  public class FilterConfig {
    public static void RegisterGlobalFilters(GlobalFilterCollection filters) {
      filters.Add(new HandleErrorAttribute());
    }
  }
}
