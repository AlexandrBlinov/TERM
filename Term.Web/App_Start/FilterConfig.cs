using System.Web;
using System.Web.Mvc;
using Term.Web.Filters;

namespace Term.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Фильтр для попадания exeption в Event log
          filters.Add(new LoggedExceptionFilter());

            // Фильтр для отображения Shared ->Error
          filters.Add(new HandleErrorAttribute());
        }
    }
}