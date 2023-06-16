using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using AMS202024113120.Models;
using Microsoft.EntityFrameworkCore;

namespace AMS202024113120.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AssetContext _context;
        private IList<Asset> asset;
        private IList<AssetCategory> categories;
        private IList<Department> department;
        private IList<Employee> employees;
        private string _path; //  图片的路径
        public IActionResult Index()
        {
            return View();
        }
        public AdminController(AssetContext context, IHostEnvironment environment)
        {
            _context = context;
            //设置相片的文件夹路径 --> 通过构造方法获取
            _path = environment.ContentRootPath + "//wwwroot//images";
        }
        //  添加资产
        public IActionResult AddAsset()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> AddAsset(Asset asset, IFormFile imgFile)
        {
            if (ModelState.IsValid)
            {
                if (imgFile != null)
                {
                    if (imgFile.Length > 0)
                    {
                        //相片提交
                        string fileName =
                       $"{Guid.NewGuid().ToString()}.{Path.GetExtension(imgFile.FileName).Substring(1)}";
                        string savePath = $"{_path}\\{fileName}";
                        using (var steam = new FileStream(savePath, FileMode.Create))
                        {
                            await imgFile.CopyToAsync(steam);
                        }
                        asset.ImgName = fileName;
                        asset.PurchaseDate = DateTime.Now;
                        //相片信息写入记录
                        _context.Assets.Add(asset);
                        _context.SaveChanges();
                        return RedirectToAction("Index", "Home"); //重定向到Home/Index
                    }
                }
            }
            return View(asset);
        }
        //添加资产类别
        public IActionResult AddCategory()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddCategory(AssetCategory category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.AssetCategories.Add(category);
                    _context.SaveChanges();
                    TempData["Result"] = "资产类别添加成功!";
                    //添加成功则重定向到Index动作方法,显示首页
                    return RedirectToAction("QueryAssetCategory", "Member");
                }
                catch (Exception ex)
                {
                    TempData["Result"] = "资产类别添加失败!";
                }
            }
            return View(category);
        }

        //添加部门
        public IActionResult AddDepartment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Departments.Add(department);
                    _context.SaveChanges();
                    TempData["Result"] = "资产类别添加成功!";
                    //添加成功则重定向到Index动作方法,显示首页
                    return RedirectToAction("QueryDepartment", "Member");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return RedirectToAction("QueryDepartment", "Member");
                }
            }
            Console.WriteLine("faild");
            return View(department);
        }

        //添加员工
        public IActionResult AddEmployee()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Employees.Add(employee);
                    _context.SaveChanges();
                    TempData["Result"] = "员工添加成功!";
                    //添加成功则重定向到Index动作方法,显示首页
                    return RedirectToAction("QueryEmployee", "Member");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    // return RedirectToAction("QueryDepartment", "Member");
                }
            }
            return View(employee);
        }
        //资产类别管理页面
        public IActionResult ManageCategory()
        {
            categories = _context.AssetCategories.OrderBy(b => b.CategoryId)
                .ToList();
            return View(categories);
        }
        //删除类别
        public IActionResult DeleteCategory(int id)
        {
            //isReferenced是一个bool类型的变量，表示查询的结果。如果查询到了参照行，则isReferenced的值为true；
            var isReferenced = _context.Assets.Any(t => t.CategoryId == id);
            if (isReferenced)
            {
                
                return RedirectToAction("ManageCategory", "Admin");
            }
            else
            {
                var category = _context.AssetCategories.FirstOrDefault(b => b.CategoryId == id);
                _context.AssetCategories.Remove(category);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageCategory", "Admin");
        }
        //修改资产类别
        public IActionResult EditCategory(int id)
        {
            var category = _context.AssetCategories.FirstOrDefault(b => b.CategoryId == id);
            return View(category);
        }
        [HttpPost]
        public IActionResult EditCategory(AssetCategory category)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int categoryId = category.CategoryId;
                    var tmp = _context.AssetCategories.FirstOrDefault(b => b.CategoryId == categoryId);
                    tmp.CategoryName = category.CategoryName;
                    tmp.Description = category.Description;
                    _context.SaveChanges();
                    TempData["Result"] = "类别信息修改成功!";
                    return RedirectToAction("ManageCategory", "Admin");
                }
                catch (Exception ex)
                {
                    TempData["Result"] = "类别信息修改失败!";
                }
            }
            return View(category);
        }

        //资产管理页面
        public IActionResult ManageAsset()
        {
            asset = _context.Assets.OrderBy(b => b.AssetId)
                .Include(b => b.Category).AsNoTracking()
                .Include(b => b.Custodian).AsNoTracking()
                .ToList();
            return View(asset);
        }
        //删除资产
        public IActionResult DeleteAsset(int id)
        {
            var asset = _context.Assets.FirstOrDefault(b => b.AssetId == id);
            _context.Assets.Remove(asset);
            _context.SaveChanges();
            return RedirectToAction("ManageAsset", "Admin");
        }
        //修该资产
        public IActionResult EditAsset(int id)
        {
            var asset = _context.Assets.FirstOrDefault(b => b.AssetId == id);
            return View(asset);
        }
        //IFormFile? imgFile允许没提交照片时执行ModelState.IsValid的内容
        [HttpPost]
        public async Task<IActionResult> EditAsset(Asset asset,IFormFile? imgFile)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    int id = asset.AssetId;
                    var tmp = _context.Assets.FirstOrDefault(b => b.AssetId == id);
                    //判断是否要更新照片
                    if (imgFile != null)
                    {
                        if (imgFile.Length > 0)
                        {
                            //相片提交
                            string fileName =
                           $"{Guid.NewGuid().ToString()}.{Path.GetExtension(imgFile.FileName).Substring(1)}";
                            string savePath = $"{_path}\\{fileName}";
                            using (var steam = new FileStream(savePath, FileMode.Create))
                            {
                                await imgFile.CopyToAsync(steam);
                            }
                            tmp.ImgName = fileName;
                        }
                    }
                    else
                    {
                        tmp.ImgName = tmp.ImgName;
                    }
                    tmp.ImgName = tmp.ImgName;
                    tmp.AssetName = asset.AssetName;
                    tmp.AssetSpec = asset.AssetSpec;
                    tmp.Price = asset.Price;
                    tmp.PurchaseDate = asset.PurchaseDate;
                    tmp.Location = asset.Location;
                    tmp.CategoryId = asset.CategoryId;
                    tmp.CustodianId = asset.CustodianId;
                    _context.SaveChanges();
                    TempData["Result"] = "资产信息修改成功!";
                    return RedirectToAction("ManageAsset", "Admin");
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Login", "Home");
                    TempData["Result"] = "资产信息修改失败!";
                }
            }
            return View(asset);
        }

        //管理部门页面
        public IActionResult ManageDepartment()
        {
            department = _context.Departments.OrderBy(b => b.DepartmentId)
                .Include(b => b.Manager).AsNoTracking()
                .ToList();
            return View(department);
        }
        //修改部门信息
        public IActionResult EditDepartment(int id)
        {
            var department = _context.Departments.FirstOrDefault(b => b.DepartmentId == id);

            return View(department);
        }
        //删除部门
        public IActionResult DeleteDepartment(int id)
        {
            var isReferenced = _context.Employees.Any(t => t.DepartmentId == id);
            if (isReferenced)
            {
                return RedirectToAction("ManageDepartment", "Admin");
            }
            else
            {
                var department  = _context.Departments.FirstOrDefault(b => b.DepartmentId == id);
                _context.Departments.Remove(department);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageDepartment", "Admin");
        }
        [HttpPost]
        public IActionResult EditDepartment(Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    int id = department.DepartmentId;
                    var tmp = _context.Departments.FirstOrDefault(b => b.DepartmentId == id);
                    tmp.DepartmentName = department.DepartmentName;
                    tmp.ManagerId = department.ManagerId;
                    _context.SaveChanges();
                    return RedirectToAction("ManageDepartment", "Admin");
                }
                catch (Exception ex)
                {

                }
            }
            return View(department);
        }
        //管理员工页面
        public IActionResult ManageEmployee()
        {
            employees = _context.Employees.OrderBy(b => b.EmployeeId)
                .Include(b => b.Department).AsNoTracking()
                .ToList();
            return View(employees);
        }
        //删除员工信息
        public IActionResult DeleteEmployee(string id)
        {
            var isReferenced1 = _context.Departments.Any(t => t.ManagerId == id);
            var isReferenced2 = _context.Assets.Any(t => t.CustodianId == id);
            if(isReferenced1==false && isReferenced2==false)
            {
                var employee = _context.Employees.FirstOrDefault(b => b.EmployeeId == id);
                _context.Employees.Remove(employee);
                _context.SaveChanges();
            }
            return RedirectToAction("ManageEmployee", "Admin");
        }
        //修改员工信息
        public IActionResult EditEmployee(string id)
        {
            var employee = _context.Employees.FirstOrDefault(b => b.EmployeeId == id);
            return View(employee);
        }
        [HttpPost]
        public IActionResult EditEmployee(Employee employee)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string id = employee.EmployeeId;
                    var tmp = _context.Employees.FirstOrDefault(b => b.EmployeeId == id);
                    tmp.Password = employee.Password;
                    tmp.Name = employee.Name;
                    tmp.Phone = employee.Phone;
                    tmp.Role = employee.Role;
                    tmp.DepartmentId = employee.DepartmentId;
                    _context.SaveChanges();
                    return RedirectToAction("ManageEmployee","Admin");
                }
                catch (Exception ex)
                {

                }
            }
            return View(employee);
        }
    }
}
