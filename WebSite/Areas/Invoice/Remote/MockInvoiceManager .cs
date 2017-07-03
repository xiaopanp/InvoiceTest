using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSite.Areas.Invoice.Models;

namespace WebSite.Areas.Invoice.Remote
{
    public class MockInvoiceManager: IInvoice
    {
        public InvoiceModel GetInvoiceDetail(string ticket)
        {
            InvoiceModel model = new InvoiceModel();
            model.InvoiceCode = "035001600111";
            model.InvoiceNumber = "21801705";
            model.BillingDate= new DateTime(2017,01,06);
            model.CheckCode = "69210 74904 21035 77321";
            model.QcCode = "";
            model.Seal = "";

            model.Buyer = new TaxpayerModel();
            model.Buyer.Name = "购买方名称";
            model.Buyer.TaxpayerNumber = "购买方纳税人识别号";
            model.Buyer.Contcat = "购买方地址电话";
            model.Buyer.BankAndAccountInfo = "购买方开户行及账号";
            model.Buyer.Password = "+2/4<18>3++4>612>9+<9+/<666001 - +80 >/< 56744 * 8 > *99 - 2 >/ 0 +0 + 64868//4<<*++9>615+9+2928* 62292 < -+80 >/< 56744 * 8 > *> 036";

            model.Items = new List<InvoiceItemModel>();
            model.Items.Add(new InvoiceItemModel()
            {
                Name="项目名称1",
                Model="",
                Unit="元",
                Amount=1,
                UnitPrice =100,
                AmountOfMoney = 100,
                TaxRate=0.01,
                Tax = 100*0.01
            });
            model.Items.Add(new InvoiceItemModel()
            {
                Name = "项目名称2",
                Model = "",
                Unit = "元",
                Amount = 2,
                UnitPrice = 200,
                AmountOfMoney = 400,
                TaxRate = 0.02,
                Tax = 400 * 0.01
            });

            model.Seller = new TaxpayerModel();
            model.Seller.Name = "销售方名称";
            model.Seller.TaxpayerNumber = "销售方纳税人识别号";
            model.Seller.Contcat = "销售方地址电话";
            model.Seller.BankAndAccountInfo = "销售方开户行及账号";
            model.Seal = "";

            model.Payee = "收款人";
            model.Review = "复核";
            return model;
        }
    }
}