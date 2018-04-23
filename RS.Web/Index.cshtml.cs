using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RS.Web
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            IUrlHelper s = Url;

        }
        public void OnPost()
        {

        }
    }
}