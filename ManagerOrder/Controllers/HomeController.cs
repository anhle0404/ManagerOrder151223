using ManagerOrder.CommonHelper;
using ManagerOrder.Models;
using ManagerOrder.Models.Entities;
using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagerOrder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        UserRepo userRepo = new UserRepo();
        RegisterProductRepo productRepo = new RegisterProductRepo();
        HistoryOrderRepo orderRepo = new HistoryOrderRepo();
        HistoryOrderDetailRepo orderDetailRepo = new HistoryOrderDetailRepo();
        UnitRepo unitRepo = new UnitRepo();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<User>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            if (session.IsAdmin != 1)
            {
                return RedirectToAction("Authentication", "home");
            }


            return View();
        }

        public IActionResult Authentication()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        #region Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login([FromForm] User login)
        {
            try
            {
                User user = userRepo.GetAll().Where(x => x.UserName == login.UserName && x.Password == login.Password).FirstOrDefault();
                if (user != null)
                {
                    HttpContext.Session.SetObject<User>("user", user);

                    if (user.IsAdmin == 1)
                    {
                        return RedirectToAction("index");

                    }
                    else if (user.IsAdmin == 2)
                    {
                        return RedirectToAction("index", "RegisterProduct");
                    }
                    else
                    {
                        return RedirectToAction("index", "HistoryOrder");
                    }

                }
                else
                {
                    ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
                }

                return View(user);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        [HttpGet]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login");
        }
        #endregion


        public JsonResult GetAllData(int type)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(0, new JsonSerializerOptions());
                }

                DateTime dateStart = new DateTime(2023, 11, 20, 0, 0, 0);
                DateTime dateEnd = new DateTime(2023, 11, 30, 23, 59, 59);

                //Get danh sách sản phẩm
                var listProducts = (from p in productRepo.GetAll()
                                    join u in unitRepo.GetAll() on p.Unit equals u.Id into t
                                    from u in t.DefaultIfEmpty()
                                    select new
                                    {
                                        Id = p.Id,
                                        p.ProductCode,
                                        p.ProductName,
                                        UnitName = u == null ? "" : t.First().Name,
                                        p.QtyInventory,
                                        p.QtyImport,
                                        p.QtyExport,
                                        p.ProductImportPrice,
                                        p.WholesalePrice
                                    }).ToList();
                var products = new
                {
                    data = listProducts,
                    totalProduct = listProducts.Count(),
                    totalMoneyImport = listProducts.Sum(x => x.ProductImportPrice)
                };

                //Get doanh thu
                var listOrders = orderRepo.GetAll().Where(x => x.IsApproved == 1 &&
                                                            (Convert.ToDateTime(x.CreatedDate) >= dateStart && Convert.ToDateTime(x.CreatedDate) <= dateEnd)).ToList();
                var orders = new
                {
                    data = listOrders,
                    totalRevenue = listOrders.Sum(x => x.TotalIntoMoney)
                };

                //Get data report
                var reports = orderRepo.GetDataReport(dateStart, dateEnd);

                var tuple = new Tuple<object, object, object>(products, orders, reports);

                return Json(tuple, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
