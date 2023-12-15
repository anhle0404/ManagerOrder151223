using ManagerOrder.CommonHelper;
using ManagerOrder.Models;
using ManagerOrder.Models.Entities;
using ManagerOrder.Repo;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ManagerOrder.Controllers
{
    public class RegisterProductController : Controller
    {
        RegisterProductRepo productRepo = new RegisterProductRepo();
        UnitRepo unitRepo = new UnitRepo();
        public IActionResult Index()
        {
            var session = HttpContext.Session.GetObject<User>("user");
            if (session.Id <= 0)
            {
                return RedirectToAction("login", "home");
            }

            if (session.IsAdmin == 3)
            {
                return RedirectToAction("Authentication", "home");
            }

            ViewBag.Units = new SelectList(unitRepo.GetAll(), "Id", "Name");
            return View();
        }

        public JsonResult GetAll(string keyword)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(0, new JsonSerializerOptions());
                }

                keyword = string.IsNullOrEmpty(keyword) ? "" : keyword.ToLower().Trim();
                var listProduct = (from p in productRepo.GetAll()
                                   join u in unitRepo.GetAll() on p.Unit equals u.Id into t
                                   from u in t.DefaultIfEmpty()
                                   where p.ProductCode.ToLower().Contains(keyword) || p.ProductName.ToLower().Contains(keyword) || keyword == ""
                                   select new
                                   {
                                       p.Id,
                                       p.ProductCode,
                                       p.ProductName,
                                       UnitName = u == null ? "" : u.Name,
                                       p.QtyInventory,
                                       p.QtyImport,
                                       p.QtyExport,
                                       p.ProductImportPrice,
                                       p.WholesalePrice
                                   }).OrderByDescending(x=>x.Id).ToList();

                return Json(listProduct, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public JsonResult GetByID(long id)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(null, new JsonSerializerOptions());
                }

                RegisterProduct product = productRepo.GetByID(id);
                return Json(product, new JsonSerializerOptions());
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }


        public JsonResult SaveData([FromBody] RegisterProduct register)
        {
            try
            {
                var session = HttpContext.Session.GetObject<User>("user");
                if (session.Id <= 0)
                {
                    return Json(new { status = 0, message = "" }, new JsonSerializerOptions());
                }

                var validate = productRepo.CheckValidate(register);
                int status = (int)validate.GetType().GetProperty("status").GetValue(validate);
                if (status == 1)
                {
                    RegisterProduct product = productRepo.GetByID(register.Id);
                    if (product == null)
                    {
                        register.QtyInventory = register.QtyImport = register.QtyExport = 0;
                        productRepo.Create(register);
                        return Json(new { status = 1, message = "Thêm thành công!" }, new JsonSerializerOptions());
                    }
                    else
                    {
                        product.ProductCode = register.ProductCode;
                        product.ProductName = register.ProductName;
                        product.Unit = register.Unit;
                        product.ProductImportPrice = register.ProductImportPrice;
                        product.WholesalePrice = register.WholesalePrice;

                        productRepo.Update(product);
                        return Json(new { status = 1, message = "Cập nhật thành công!" }, new JsonSerializerOptions());
                    }
                }
                else
                {
                    return Json(validate, new JsonSerializerOptions());
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
