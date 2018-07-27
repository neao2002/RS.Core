using System;
using System.Collections.Generic;
using System.Text;
using RS;
using RS.Sys;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RSSystemExtension
    {
        public static IAppRegister RegAppSystemID(this IAppRegister register,string SystemID)
        {
            AppUser.InitAppID(SystemID);
            return register;
        }
    }
}
