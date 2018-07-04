using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace RS.Sys
{
    public class PlatformAuthoricationMiddleware : AuthenticationMiddleware
    {
        public PlatformAuthoricationMiddleware(RequestDelegate next, IAuthenticationSchemeProvider schemes) : base(next, schemes)
        {
        }
    }
}
