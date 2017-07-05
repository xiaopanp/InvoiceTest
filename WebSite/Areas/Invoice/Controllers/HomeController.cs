﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebSite.Areas.Invoice.Remote;

namespace WebSite.Areas.Invoice.Controllers
{
    public class HomeController : Controller
    {
        // GET: Invoice/Home
        public ActionResult Index()
        {
            ViewBag.Title = "Invoice ";
            ViewBag.Message = "Invoice description page.";
            return View();
        }

        // GET: Invoice/Home
        public ActionResult Detail(string invoiceToket)
        {
            //调用查询接口
            IInvoice invoice = new MockInvoiceManager();
            var detail = invoice.GetInvoiceDetail(invoiceToket);
            //生成pdf

            //自动发送邮件
             

            return View(detail);
        }
    }
}