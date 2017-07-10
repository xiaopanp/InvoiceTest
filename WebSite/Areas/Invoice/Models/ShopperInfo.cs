using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebSite.Areas.Invoice.Models
{
    public class ShopperInfo
    {
        /// <summary>
        /// 购货方省份--->非必须
        /// </summary>
        public string province { get; set; }            
        /// <summary>
        /// 购货方地址--->非必须
        /// </summary>
        public string address { get; set; }           
        /// <summary> 
        /// 购货方邮箱--->非必须
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// 购货方固定电话 -->非必填
        /// </summary>
        public string telePhone { get; set; }
        /// <summary> 
        /// 购货方手机号>必填
        /// </summary>
        [Required]
        public string mobilePhone { get; set; }
        /// <summary> 
        /// 发票抬头--购货方名称
        /// </summary>
        [Required]
        public string invoiceTitle { get; set; }
        /// <summary> 
        /// 购货方企业类型  -->必填-->1：企业  2：机关事业单位  3：个人  4：其它
        /// </summary>
        [Required]
        public int industryType { get; set; }
        /// <summary>
        /// 购货方识别号 -- 非必须--> 企业消费，识别号为必填项
        /// </summary>
        public string taxpayerIdentificationNumber { get; set; }
        /// <summary>
        /// 购货方银行账号--->非必须
        /// </summary>
        public string bankAccount { get; set; }           
    }
}