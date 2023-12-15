using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using ManagerOrder.Models.Entities;
using ManagerOrder.CommonHelper;
using System.Diagnostics;
using ManagerOrder.Models;

namespace ManagerOrder.Controllers
{
    public class HistoryOrderController : Controller
    {
        HistoryOrderRepo orderRepo = new HistoryOrderRepo();
        RegisterCustomerRepo customerRepo = new RegisterCustomerRepo();
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<User>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            //if (session.IsAdmin != 1)
            //{
            //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            //}

            return View();
        }

        public JsonResult GetAll(int isApproved, string keyword)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(0, new JsonSerializerOptions());
                }

                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword;
                var listOrder = (from order in orderRepo.GetAll()
                                 join customer in customerRepo.GetAll() on order.CustomerId equals customer.Id into t
                                 from customer in t.DefaultIfEmpty()
                                 where (order.IsApproved == isApproved || isApproved == -1) &&
                                       (order.OrderCode.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        customer.CustomerName.ToLower().Contains(keyword.ToLower().Trim()) ||
                                        keyword == "")
                                 select new
                                 {
                                     Id = order.Id,
                                     IsApproved = order.IsApproved,
                                     OrderCode = order.OrderCode,
                                     CustomerName = customer == null ? "" : customer.CustomerName,
                                     TotalIntoMoney = order.TotalIntoMoney,
                                     CustomerPayment = order.CustomerPayment,
                                     MoneyOwedCustomer = order.MoneyOwedCustomer,
                                     CreatedDate = order.CreatedDate,
                                 }).OrderByDescending(x => x.Id).ToList();
                return Json(listOrder, new JsonSerializerOptions());

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        public JsonResult Update([FromBody] HistoryOrder historyOrder)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(0, new JsonSerializerOptions());
                }

                HistoryOrder order = orderRepo.GetByID(historyOrder.Id);
                order.IsApproved = 1;
                order.MoneyOwedCustomer = order.TotalIntoMoney - historyOrder.CustomerPayment;
                return Json(orderRepo.Update(order), new JsonSerializerOptions());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
