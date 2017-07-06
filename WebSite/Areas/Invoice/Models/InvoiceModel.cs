using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class InvoiceModel
    {
        /// <summary>
        /// 机器编号
        /// </summary>
        public virtual string MachineCode { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        public virtual string InvoiceHeader { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        public virtual string InvoiceCode { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        public virtual string InvoiceNumber { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        public virtual DateTime BillingDate { get; set; }
        /// <summary>
        /// 机器编码
        /// </summary>
        public virtual string MachineNumber { get; set; }

        /// <summary>
        /// 校验码
        /// </summary>
        public virtual string CheckCode { get; set; }

        /// <summary>
        /// 买方
        /// </summary>
        public virtual TaxpayerModel Buyer { get; set; }
        /// <summary>
        /// 项目列表
        /// </summary>
        public virtual IList<InvoiceItemModel> Items { get; set; }
        /// <summary>
        /// 卖方
        /// </summary>
        public virtual TaxpayerModel Seller { get; set; }
        /// <summary>
        /// 印章图片路径
        /// </summary>
        public virtual string Seal { get; set; }
        /// <summary>
        /// 二维码，路径
        /// </summary>
        public virtual string QcCode { get; set; }
        /// <summary>
        /// 收款人
        /// </summary>
        public virtual string Payee { get; set; }
        /// <summary>
        /// 复核
        /// </summary>
        public virtual string Review { get; set; }
        /// <summary>
        /// 开票人
        /// </summary>
        public virtual string Printor { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// 密码区
        /// </summary>
        public virtual string Password { get; set; }

        /// <summary>
        /// 下载链接
        /// </summary>
        public virtual string DownLoadUrl { get; set; }
        /// <summary>
        /// 价税合计
        /// </summary>
        /// <returns></returns>
        public int SumAmount()
        {
            if (Items == null)
                return 0;
            return Items.Sum(a => a.Amount);
        }
        /// <summary>
        /// 价税合计
        /// </summary>
        /// <returns></returns>
        public double SumAmountOfMoney()
        {
            if (Items == null)
                return 0.0;
            return Items.Sum(a => a.AmountOfMoney);
        }
        /// <summary>
        /// 价格人民币大写
        /// </summary>
        /// <returns></returns>
        public string SumAmountOfMoneyUper()
        {
            var sum = SumAmountOfMoney();
            return RmbCapitalization.RmbAmount(sum);
        }

        /// <summary>
        /// 税率
        /// </summary>
        /// <returns></returns>
        public double SumTax()
        {
            if (Items == null)
                return 0;
            return Items.Sum(a => a.Tax);
        }
    }
}