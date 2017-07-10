using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class QueryInoviceResult
    {
        public string status { get; set; }
        public QueryInoviceData data { get; set; }
    }
}