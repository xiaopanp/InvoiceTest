using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    /// <summary>
    /// 销售方
    /// </summary>
    public class TaxpayerModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public virtual string Name { get; set; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public virtual string TaxpayerNumber { get; set; }
        /// <summary>
        /// 地 址、电 话
        /// </summary>
        public virtual string Contcat { get; set; }
        /// <summary>
        /// 开户行及账号
        /// </summary>
        public virtual string BankAndAccountInfo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// 密码区
        /// </summary>
        public virtual string Password { get; set; }
        /// <summary>
        /// 印章图片路径
        /// </summary>
        public virtual string Seal { get; set; }
    }
}