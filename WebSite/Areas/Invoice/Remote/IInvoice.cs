using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Areas.Invoice.Models;

namespace WebSite.Areas.Invoice.Remote
{
    public interface IInvoice
    {
        InvoiceModel GetInvoiceDetail(string ticket);
    }
}