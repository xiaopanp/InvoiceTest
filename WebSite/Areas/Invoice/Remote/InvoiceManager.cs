using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Areas.Invoice.Models;

namespace WebSite.Areas.Invoice.Remote
{
    public class InvoiceManager:IInvoice
    {
        public InvoiceModel GetInvoiceDetail(string ticket)
        {
            //TODO:调用接口
            throw new NotImplementedException();
        }
    }
}