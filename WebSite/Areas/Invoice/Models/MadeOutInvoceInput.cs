using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class MadeOutInvoceInput
    {
        /// <summary>
        /// 销售单据Ticket
        /// </summary>
        [Required]
        public string ticketSn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ShopperInfo shopperInfo { get; set; }
        /// <summary>
        /// 账号密码
        /// </summary>
        [Required]
        public string account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string password { get; set; }

    }
}