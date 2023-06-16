using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics.Metrics;
using System.Security.Claims;
using System;
using AMS202024113120.Models;
using System.IO;

namespace AMS202024113120.Controllers
{
    public class HomeController : Controller
    {
        private readonly AssetContext _context;
        private IList<Asset> asset;
        private string _path; //图片路径变项
        public HomeController(AssetContext context, IHostEnvironment environment)
        {
            _context = context;
            _path = environment.ContentRootPath + "//wwwroot//images";
        }
        public IActionResult Index()
        {
            asset = _context.Assets.OrderBy(b => b.AssetId)
                .Include(b => b.Category).AsNoTracking()
                .Include(b => b.Custodian).AsNoTracking()
                .Include(b => b.Custodian.Department).AsNoTracking()
                .ToList();
            return View(asset);
        }

        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string uid, string pwd)
        {
            //取得会员对象
            Employee member = _context.Employees.FirstOrDefault(m => m.EmployeeId == uid && m.Password == pwd);
            if (member != null)
            {
                //建立身份声明
                IList<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, member.EmployeeId),
                new Claim(ClaimTypes.Role, member.Role.Trim())
 };
                //建立身份识别对象,并指定账号与角色
                var claimsIndentity = new ClaimsIdentity(claims,
                CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };
                //进行登录动作,并带入身份识别对象
                HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIndentity),
                authProperties);
                //重定向至会员页
                TempData["Message"] = member.Role.ToString();
                return RedirectToAction("Index", TempData);

            }
            ViewBag.Message = "账号或密码错误";
            return View("Login");
        }
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        [Authorize(Roles = "Admin")]
        public IActionResult RestrictedMethod1()
        {
            // 只有Admin角色的用户才能访问此方法
            return Json(new { success = true });
        }
        [Authorize(Roles = "Admin,Member")]
        public IActionResult RestrictedMethod2()
        {
            // 只有Admin\Member角色的用户才能访问此方法
            return Json(new { success = true });
        }

    }
}
