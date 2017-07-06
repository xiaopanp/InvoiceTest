using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Aspose.Pdf;
using HtmlAgilityPack;
using Svg;
using WebSite.Areas.Invoice.Remote;

namespace WebSite.Areas.Invoice.Controllers
{
    public class HomeController : Controller
    {
        // GET: Invoice/Home
        public ActionResult Index()
        {
            ViewBag.Title = "Invoice ";
            ViewBag.Message = "Invoice description page.";
            return View();
        }

        // GET: Invoice/Home
        public ActionResult Detail(string invoiceToket)
        {
            //调用查询接口
            IInvoice invoice = new MockInvoiceManager();
            var detail = invoice.GetInvoiceDetail(invoiceToket);
            //生成pdf
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.Load(System.AppDomain.CurrentDomain.BaseDirectory+"template\\einvoice.svg",Encoding.UTF8);
            //自动发送邮件

            HtmlNode machineCode = doc.DocumentNode.SelectSingleNode("//*[@id='MachineCode']");
            machineCode.InnerHtml = detail.MachineCode ?? "";

            HtmlNode invoiceCode = doc.DocumentNode.SelectSingleNode("//*[@id='InvoiceCode']");
            invoiceCode.InnerHtml = detail.InvoiceCode ?? "";

            HtmlNode invoiceNumber = doc.DocumentNode.SelectSingleNode("//*[@id='InvoiceNumber']");
            invoiceNumber.InnerHtml = detail.InvoiceNumber ?? "";

            HtmlNode billingDate = doc.DocumentNode.SelectSingleNode("//*[@id='BillingDate']");
            billingDate.InnerHtml = detail.BillingDate==null ? "": detail.BillingDate.ToString("yyyy年MM月dd日");

            HtmlNode checkCode = doc.DocumentNode.SelectSingleNode("//*[@id='CheckCode']");
            checkCode.InnerHtml = detail.CheckCode ?? "";

            HtmlNode buyerName = doc.DocumentNode.SelectSingleNode("//*[@id='Buyer.Name']");
            buyerName.InnerHtml = detail.Buyer.Name ?? "";

            HtmlNode buyerTaxpayerNumber = doc.DocumentNode.SelectSingleNode("//*[@id='Buyer.TaxpayerNumber']");
            buyerTaxpayerNumber.InnerHtml = detail.Buyer.TaxpayerNumber ?? "";

            HtmlNode buyerAddress = doc.DocumentNode.SelectSingleNode("//*[@id='Buyer.Address']");
            buyerAddress.InnerHtml = detail.Buyer.Address ?? "";
            buyerAddress.InnerHtml += detail.Buyer.Phone ?? "";

            HtmlNode buyerBankAndAccountInfo = doc.DocumentNode.SelectSingleNode("//*[@id='Buyer.BankAndAccountInfo']");
            buyerBankAndAccountInfo.InnerHtml = detail.Buyer.BankAndAccountInfo ?? "";

            MulLine(doc, detail.Password, "PasswordLine", 28);

            int yAd = 16;
            HtmlNode password = doc.DocumentNode.SelectSingleNode("//*[@id='Password']");
            if(detail.Items != null && detail.Items.Count > 0)
            {
                int index = 0;
                foreach(var item in detail.Items)
                {
                    HtmlNode node = doc.DocumentNode.SelectSingleNode(string.Format("//*[@id='item{0}']",index==0?0:index-1));
                    int preY = int.Parse(node.Attributes["y"].Value);
                    var parent = node.ParentNode;
                    var newNode = index > 0 ? node.Clone() : node;
                    var child = newNode.SelectNodes("tspan");
                    child[0].InnerHtml = item.Name ?? string.Empty;
                    child[1].InnerHtml = item.Model ?? string.Empty;
                    child[2].InnerHtml = item.Unit ?? string.Empty;
                    child[3].InnerHtml = item.Amount <= 0 ? string.Empty : item.Amount.ToString();
                    child[4].InnerHtml = item.UnitPrice <= 0 ? string.Empty : item.UnitPrice.ToString();
                    child[5].InnerHtml = item.AmountOfMoney <= 0 ? string.Empty : item.AmountOfMoney.ToString();
                    child[6].InnerHtml = item.TaxRate <= 0 ? string.Empty : item.TaxRate.ToString();
                    child[7].InnerHtml = item.Tax <= 0 ? string.Empty : item.Tax.ToString();
                    if(index != 0)
                    {
                        newNode.Attributes["id"].Value = string.Format("item{0}", index);
                        newNode.Attributes["y"].Value = (preY + yAd).ToString();
                        parent.InsertAfter(newNode, node);
                    }

                    index++;
                }
            }
            HtmlNode sumAmountOfMoney = doc.DocumentNode.SelectSingleNode("//*[@id='SumAmountOfMoney']");
            sumAmountOfMoney.InnerHtml = detail.SumAmountOfMoney().ToString();

            HtmlNode sumTax = doc.DocumentNode.SelectSingleNode("//*[@id='SumTax']");
            sumTax.InnerHtml = detail.SumTax().ToString();

            HtmlNode sumAmountOfMoneyUper = doc.DocumentNode.SelectSingleNode("//*[@id='SumAmountOfMoneyUper']");
            sumAmountOfMoneyUper.InnerHtml = detail.SumAmountOfMoneyUper().ToString();
            
            HtmlNode sellerName = doc.DocumentNode.SelectSingleNode("//*[@id='Seller.Name']");
            sellerName.InnerHtml = detail.Seller.Name ?? "";

            HtmlNode sellerTaxpayerNumber = doc.DocumentNode.SelectSingleNode("//*[@id='Seller.TaxpayerNumber']");
            sellerTaxpayerNumber.InnerHtml = detail.Seller.TaxpayerNumber ?? "";

            HtmlNode sellerAddress = doc.DocumentNode.SelectSingleNode("//*[@id='Seller.Address']");
            sellerAddress.InnerHtml = detail.Seller.Address ?? "";
            sellerAddress.InnerHtml +=detail.Seller.Phone ?? "";
            HtmlNode sellerBankAndAccountInfo = doc.DocumentNode.SelectSingleNode("//*[@id='Seller.BankAndAccountInfo']");
            sellerBankAndAccountInfo.InnerHtml = detail.Seller.BankAndAccountInfo ?? "" ;

            MulLine(doc, detail.Remark, "RemarkLine", 28);

            HtmlNode payee = doc.DocumentNode.SelectSingleNode("//*[@id='Payee']");
            payee.InnerHtml = detail.Payee ?? "";

            HtmlNode review = doc.DocumentNode.SelectSingleNode("//*[@id='Review']");
            review.InnerHtml = detail.Review ?? "";

            HtmlNode printor = doc.DocumentNode.SelectSingleNode("//*[@id='Printor']");
            printor.InnerHtml = detail.Printor ?? "";
            //需要优化，直接保存 viewBag属性会变成小写，导致错误
            var docstring = doc.DocumentNode.InnerHtml.Replace("viewbox", "viewBox");
            //doc.Save(string.Format("{0}download\\{1}.svg",System.AppDomain.CurrentDomain.BaseDirectory,detail.InvoiceNumber),Encoding.UTF8 );
            var filename = string.Format("{0}download\\{1}.svg", System.AppDomain.CurrentDomain.BaseDirectory,detail.InvoiceNumber);
            System.IO.File.WriteAllText(filename, docstring);
            SvgDocument svgDoc = SvgDocument.Open(filename);
            var svgImage = svgDoc.Draw();
            svgImage.Save(filename.Replace(".svg", ".png"));
            detail.DownLoadUrl = string.Format("{0}download/{1}.svg", "../../", detail.InvoiceNumber);
            return View(detail);
        }

        private void MulLine(HtmlDocument doc,string content,string nodeid,int maxlength)
        { 
            if (!string.IsNullOrEmpty(content))
            {
                var lineCount = content.Length / maxlength ;
                for (var i = 0; i <= lineCount; i++)
                {
                    HtmlNode node = doc.DocumentNode.SelectSingleNode(string.Format("//*[@id='{0}{1}']", nodeid, i));
                    if (node != null)
                    {
                        if ((i*maxlength + maxlength) < content.Length)
                        {
                            node.InnerHtml = HtmlStringTranc(content.Substring(i*maxlength, maxlength));
                        }
                        else
                        {
                            node.InnerHtml = HtmlStringTranc(content.Substring(i*maxlength));
                        }
                    }
                }
            }
        }

        public string HtmlStringTranc(string target)
        {
            return target.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}