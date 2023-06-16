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
//�����֤����,ʹ��cookie��֤ 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
 .AddCookie(options =>
 {
     //ֻ��͸��http(s)������ 
     options.Cookie.HttpOnly = true;
     //δ��¼ʱ�ض�����index 
     options.LoginPath = new PathString("/Home/Index");
     //�ܾ�����ʱ�ض�����index 
     options.AccessDeniedPath = new PathString("/Home/Index");
 });

var app = builder.Build();
app.UseStaticFiles();
//����cookie���� 
app.UseCookiePolicy();
//���������֤ 
app.UseAuthentication();
//������Ȩ���� 
app.UseAuthorization();
app.MapControllerRoute(name: "default",
 pattern: "{Controller=Home}/{Action=Index}/{Id?}");
app.Run();