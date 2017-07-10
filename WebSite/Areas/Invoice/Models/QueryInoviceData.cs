using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class QueryInoviceData
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual int id { get; set; }
        /// <summary>
        /// 单据信息
        /// </summary>
        public virtual int userId { get; set; }
        /// <summary>
        /// 单据信息
        /// </summary>
        public virtual string ticketSn { get; set; }

        //开票方信息
        /// <summary>
        /// 税号
        /// </summary>
        public virtual string taxpayerId { get; set; }
        /// <summary>
        ///开票公司
        /// </summary>
        public virtual string taxpayerCompany { get; set; }
        /// <summary>
        ///开票公司地址
        /// </summary>
        public virtual string taxpayerCompanyAddress { get; set; }
        /// <summary>
        /// 开票公司电话
        /// </summary>
        public virtual string taxpayerCompanyTel { get; set; }
        /// <summary>
        /// 购货方省份--->非必须
        /// </summary>
        public virtual string province { get; set; }
        /// <summary>
        /// 购货方地址--->非必须
        /// </summary>
        public virtual string address { get; set; }
        /// <summary>
        /// 购货方邮箱--->非必须
        /// </summary>
        public virtual string email { get; set; }
        /// <summary>
        /// 购货方固定电话 -->非必填
        /// </summary>
        public virtual string telePhone { get; set; }
        /// <summary>
        /// 购货方手机号>必填
        /// </summary>
        public virtual string mobilePhone { get; set; }
        /// <summary>
        /// 购货方企业类型  -->必填-->1：企业  2：机关事业单位  3：个人  4：其它
        /// </summary>
        public virtual string industryType { get; set; }
        /// <summary>
        /// 购货方识别号 -- 非必须--> 企业消费，识别号为必填项
        /// </summary>
        public virtual string taxpayerIdentificationNumber { get; set; }
        /// <summary>
        /// 发票抬头
        /// </summary>
        public virtual string invoiceTitle { get; set; }
        /// <summary>
        /// 发票号码
        /// </summary>
        public virtual string invoiceNumber { get; set; }
        /// <summary>
        /// 发票代码
        /// </summary>
        public virtual string invoceCode { get; set; }
        /// <summary>
        /// 税率
        /// </summary>
        public virtual string taxRate { get; set; }
        /// <summary>
        /// 下载电子发票地址
        /// </summary>
        public virtual string downloadUrl { get; set; }
        /// <summary>
        /// 开票日期
        /// </summary>
        public virtual string dateTime { get; set; }
        /// <summary>
        /// 需要开票的金额
        /// </summary>
        public virtual string amountForTax { get; set; }
        /// <summary>
        /// 税额
        /// </summary>
        public virtual string amountOfTax { get; set; }
        /// <summary>
        /// 价税总额-- amountForTax + amountOfTax
        /// </summary>
        public virtual string totalAmount { get; set; }
    }
}