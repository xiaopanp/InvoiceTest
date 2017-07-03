using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class InvoiceItemModel
    {
        /// <summary>
        /// 货物或应税劳务、服务名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 规格型号
        /// </summary>
        public virtual string Model { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        public virtual string Unit { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public virtual int Amount { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public virtual double UnitPrice { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public virtual double AmountOfMoney { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public virtual double TaxRate { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public virtual double Tax { get; set; }
    }
}