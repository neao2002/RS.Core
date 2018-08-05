using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RS.Lib.Demo;
using RS.Sys;
using RS.Web.MvcDemo.Models;
using Microsoft.AspNetCore.Http;
using RS.Data;

namespace RS.Web.MvcDemo.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            HttpContext.Response.Cookies.Append("test", "三百v复核评估");
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult BusView()
        {
            bool isEQ = WebHelper.Current == HttpContext;

            ViewBag.Items = App.GetAppService<IShopService>().GetShopInfos();
            ViewBag.MyShop = App.GetAppService<IUserService>().GetMyShopInfo("1680623");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult StuMain()
        {
            IAppOperator appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            appOperator = StudentUser.GetOperator();
            if (appOperator.IsLogined())
                ViewBag.LoginStatus = "已登录";
            else
                ViewBag.LoginStatus = "未登录";

            ViewBag.User = LibHelper.JSON.Serialize(appOperator);

            //这是登录用户
            return View();
        }
        public IActionResult TeaMain()
        {
            IAppOperator appOperator = TeacherUser.GetOperator();
            if (appOperator.IsLogined())
                ViewBag.LoginStatus = "已登录";
            else
                ViewBag.LoginStatus = "未登录";

            ViewBag.User = LibHelper.JSON.Serialize(appOperator);



            return View();
        }
        public IActionResult Main()
        {
            IAppOperator appOperator = SysUser.GetOperator();
            if (appOperator.IsLogined())
                ViewBag.LoginStatus = "已登录";
            else
                ViewBag.LoginStatus = "未登录";

            ViewBag.User = LibHelper.JSON.Serialize(appOperator);

            return View();
        }


        public IActionResult StuLogin()
        {
            StudentUser user = new StudentUser()
            {
                Id = Guid.NewGuid(),
                ClassName = "201821",
                IDCardNo = "36050219740922221X",
                Name = "李生",
                StudentNo = "20182110"
            };
            StudentUser.Login(user);

            return View();
        }

        public IActionResult TeaLogin()
        {
            TeacherUser user = new TeacherUser()
            {
                Id = Guid.NewGuid(),
                Name = "丁老师",
                TeacherNo = "T2201432"
            };
            TeacherUser.Login(user);
            return View();
        }

        public IActionResult Login()
        {
            SysUser user = new SysUser()
            {
                UserID = Guid.NewGuid(),
                AuditStatus = 2,
                IsGlobalAdmin = false,
                IsStop = false,
                UserName = "操作员",
                UserNo = "czy"
            };
            SysUser.Login(user);
            return View();
        }

        public IActionResult StuLogout()
        {
            StudentUser.Logout();
            return View();
        }

        public IActionResult TeaLogout()
        {
            TeacherUser.Logout();
            return View();
        }

        public IActionResult Logout()
        {
            SysUser.Logout();
            return View();
        }

        public IActionResult EduIndex()
        {
            return View();
        }

















    }
}
