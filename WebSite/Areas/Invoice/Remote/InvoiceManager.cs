using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Areas.Invoice.Models;
using WebSite.Common;

namespace WebSite.Areas.Invoice.Remote
{
    public class InvoiceManager:IInvoice
    { 
        /// <summary>
        /// 查询发票接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public QueryInoviceResult QueryInovice(QueryInoviceInput input)
        {
            try
            { 
                var url = "http://company.pospal.cn:18080/pospal-api2/pos/v1/invoice/queryInovice";
                QueryInoviceResult result = HttpUtil.HttpPost<QueryInoviceInput, QueryInoviceResult>(url, input);
                return result;
            }
            catch (Exception)
            {
                
                return new QueryInoviceResult()
                {
                    status = "failed"
                };
            }
        }

        /// <summary>
        /// 生成发票接口
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public QueryInoviceResult MadeOutInvoce(MadeOutInvoceInput input)
        {
            try
            { 
                var url = "http://company.pospal.cn:18080/pospal-api2/pos/v1/invoice/madeOutInvoce";
                QueryInoviceResult result = HttpUtil.HttpPost<MadeOutInvoceInput, QueryInoviceResult>(url, input);
                return result;
            }
            catch (Exception)
            {

                return new QueryInoviceResult()
                {
                    status = "failed"
                };
            }
        }

    }
}