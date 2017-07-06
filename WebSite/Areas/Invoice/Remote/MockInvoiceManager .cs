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
            model.MachineCode = "661616299601";
            model.InvoiceHeader = "深圳市品道餐饮管理有限公司";
            model.InvoiceCode = "035001600111";
            model.InvoiceNumber = "32901344";
            model.BillingDate= new DateTime(2017,01,06);
            model.CheckCode = "82597 65174 05087 24836";
            model.QcCode = "";
            model.Seal = "";

            model.Buyer = new TaxpayerModel();
            model.Buyer.Name = "陈瑞庆";
            model.Buyer.TaxpayerNumber = "购买方纳税人识别号";
            model.Buyer.Address = "购买方地址";
            model.Buyer.Phone = "0592-223222";
            model.Buyer.BankAndAccountInfo = "购买方开户行及账号";
            model.Password = @"026>**>-46*+*-5<09>7422457//*/-192+1<>64*61<-*55>-*1/355><6>**>-46*+*-5<09332719/8001315544<<101>12932280-1251*+";

            model.Items = new List<InvoiceItemModel>();
            model.Items.Add(new InvoiceItemModel()
            {
                Name= "通信服务费",
                Model="",
                Unit="元",
                Amount=1,
                UnitPrice = 55.77,
                AmountOfMoney = 55.77,
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
            model.Seller.Name = "中国移动通信集团福建有限公司泉州分公司";
            model.Seller.TaxpayerNumber = "91350500705388519D";
            model.Seller.Address = "泉州市刺桐东路移动通信大厦 ";
            model.Seller.Phone = "0595-22118588";
            model.Seller.BankAndAccountInfo = "中国建设银行泉州市分行营业部,35001652490050011699";
            model.Seal = "";

            model.Payee = "收款人";
            model.Printor = "福建移动";
            model.Review = "复核";
            model.Remark = "13599935081起止时间:20170401-20170430;购买充值卡时请索取发票。“折扣折让”指您获赠的话费中当月实际使用金额。“充值卡已出具发票金额”指当月实际使用的充值卡金额。受理流水: (436380516752)";
            return model;
        }
    }
}