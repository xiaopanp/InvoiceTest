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
            model.MachineCode = "22312313";
            model.InvoiceHeader = "深圳市品道餐饮管理有限公司";
            model.InvoiceCode = "035001600111";
            model.InvoiceNumber = "21801705";
            model.BillingDate= new DateTime(2017,01,06);
            model.CheckCode = "69210 74904 21035 77321";
            model.QcCode = "";
            model.Seal = "";

            model.Buyer = new TaxpayerModel();
            model.Buyer.Name = "购买方名称";
            model.Buyer.TaxpayerNumber = "购买方纳税人识别号";
            model.Buyer.Address = "购买方地址";
            model.Buyer.Phone = "0592-223222";
            model.Buyer.BankAndAccountInfo = "购买方开户行及账号";
            model.Password = @"026>**>-46*+*-5<09>7422457//*/-192+1<>64*61<-*55>-*1/355><6>**>-46*+*-5<09332719/8001315544<<101>12932280-1251*+";

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
            model.Seller.Address = "购买方地址";
            model.Seller.Phone = "0592-2232223";
            model.Seller.BankAndAccountInfo = "销售方开户行及账号";
            model.Seal = "";

            model.Payee = "收款人";
            model.Printor = "开票人";
            model.Review = "复核";
            model.Remark = "备注备注备注备注备注备注备注备注";
            return model;
        }
    }
}