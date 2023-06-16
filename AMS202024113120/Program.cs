using Microsoft.AspNetCore.Authentication.Cookies;
using AMS202024113120.Models;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMvc();
var constr = $"Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename={builder.Environment.ContentRootPath}App_Data\\AssetsDb.mdf;Integrated Security=True;Trusted_Connection=True;";


builder.Services.AddDbContext<AssetContext>
 (options => options.UseSqlServer(constr));
builder.Services.AddDbContext<AssetContext>();
//添加认证方法,使用cookie认证 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(options =>
 {
     //只能透过http(s)来访问 
     options.Cookie.HttpOnly = true;
     //未登录时重定向至index 
     options.LoginPath = new PathString("/Home/Index");
     //拒绝访问时重定向至index 
     options.AccessDeniedPath = new PathString("/Home/Index");
 });

var app = builder.Build();
app.UseStaticFiles();
//启用cookie策略 
app.UseCookiePolicy();
//启用身份认证 
app.UseAuthentication();
//启用授权功能 
app.UseAuthorization();
app.MapControllerRoute(name: "default",
 pattern: "{Controller=Home}/{Action=Index}/{Id?}");
app.Run();