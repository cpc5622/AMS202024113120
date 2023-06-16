using Microsoft.AspNetCore.Mvc;
using AMS202024113120.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace AMS202024113120.Controllers
{
    public class MemberController : Controller
    {
        private readonly AssetContext _context;
        private IList<Asset> asset;
        private IList<AssetCategory> categories;
        private IList<Department> departments;
        private IList<Employee> employees;

        public MemberController(AssetContext context, IHostEnvironment environment)
        {
            //从构造方法注入数据库对象context
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        //查询资产类别
        public IActionResult QueryAssetCategory(string Name)
        {
            if (!string.IsNullOrEmpty(Name))
            {
                categories = _context.AssetCategories.OrderBy(b => b.CategoryId)
               .Where(b => b.CategoryName.Contains(Name))
               .ToList();
                return View(categories);
            }
            categories = _context.AssetCategories.OrderBy(b => b.CategoryId).ToList();
            return View(categories);
        }
        //查询部门
        public IActionResult QueryDepartment(string DepartmentName,string Manager)
        {

            var query = _context.Departments.Include(b => b.Manager).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(DepartmentName))
            {
                query = query.Where(b => b.DepartmentName.Contains(DepartmentName));
            }
            if (!string.IsNullOrEmpty(Manager))
            {
                query = query.Where(b => b.Manager.Name.Contains(Manager));
            }
            var departments = query.OrderBy(b => b.DepartmentId)
                .Include(b => b.Manager).AsNoTracking()
                .ToList();

            return View(departments);
        }
        //查询员工
        public IActionResult QueryEmployee(string Name, string Role, string DepartmentName)
        {
            var query = _context.Employees.Include(b => b.Department).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(Name))
            {
                query = query.Where(b => b.Name.Contains(Name));
            }
            if (!string.IsNullOrEmpty(Role))
            {
                query = query.Where(b => b.Role.Contains(Role));
            }
            if (!string.IsNullOrEmpty(DepartmentName))
            {
                query = query.Where(b => b.Department.DepartmentName.Contains(DepartmentName));
            }
            var employees = query.OrderBy(b => b.DepartmentId)
                .Include(b => b.Department).AsNoTracking()
                .ToList();
            return View(employees);
        }
        //查询资产
        public IActionResult QueryAsset(string Id, string assetName, string CategoryName, string purchaseYear, string location, string CustodianName, string DepartmentName)
        {
            // 使用LINQ扩充方法
            var query = _context.Assets.Include(b => b.Category).Include(b => b.Custodian.Departments).AsNoTracking().AsQueryable();
            if (!string.IsNullOrEmpty(Id))
            {
                query = query.Where(b => b.AssetId.ToString().Contains(Id));
            }
            if (!string.IsNullOrEmpty(assetName))
            {
                query = query.Where(b => b.AssetName.Contains(assetName));
            }
            if (!string.IsNullOrEmpty(CategoryName))
            {
                query = query.Where(b => b.Category.CategoryName.Contains(CategoryName));
            }
            if (!string.IsNullOrEmpty(purchaseYear))
            {
                int.TryParse(purchaseYear, out int year);
                query = query.Where(b => b.PurchaseDate.Year == year);
            }
            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(b => b.Location.Contains(location));
            }
            if (!string.IsNullOrEmpty(CustodianName))
            {
                query = query.Where(b => b.Custodian.Name.Contains(CustodianName));
            }
            //未能实现
            if (!string.IsNullOrEmpty(DepartmentName))
            {
                query = query.Where(b => b.Custodian.Department.DepartmentName.Contains(DepartmentName));
            }
            var asset = query.OrderBy(b => b.AssetId)
                .Include(b => b.Custodian).AsNoTracking()
                .Include(b=>b.Custodian.Department).AsNoTracking()
                .ToList();
            return View(asset);
        }

        


    }
}
