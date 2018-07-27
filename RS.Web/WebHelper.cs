using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Web
{
    public static class WebHelper
    {
        public static HttpContext Current
        {
            get
            {
                object factory = Global.Services.GetService(typeof(IHttpContextAccessor));
                HttpContext context = ((HttpContextAccessor)factory).HttpContext;                
                return context;
            }
        }
        
    }
}
