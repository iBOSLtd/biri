using System.Text;

namespace PeopleDesk.Helper
{
    public static class PDFTemplateGenerator
    {
        public static string GetHTMLString()
        {

#pragma warning disable CS0219 // Variable is assigned but its value is never used
            string dynamicVal = "Abu Rayhan";
#pragma warning restore CS0219 // Variable is assigned but its value is never used

            var sb = new StringBuilder();
            sb.Append(@"
                       <!DOCTYPE html>
                            <html>
                                <head>
                                    <meta charset='UTF-8'>
                                    <meta http-equiv = 'X-UA-Compatible' content = 'IE=edge'>
                                    <meta name = 'viewport' content = 'width=device-width, initial-scale=1.0'>
                                    <title > Table </ title >
                                </ head >");

            sb.Append(@"
                        <body>
                            <section class='table-design'>
                                <div class='table-wrapper'>
                                    <div class='section-heading'>
                                        <div class='heading-image'>
                                            <img src = 'https://erp.ibos.io/domain/Document/DownlloadFile?id=615159acf37288f5197e8b23' alt="">
                                        </div>
                                        <div class='heading-title'>
                                            <h3>${dynamicVal}</h3>
                                            <h6>Lalmatia, Dhaka, Bangladesh</h6>
                                            <h4>Purchase Order</h4>
                                        </div>
                                        <div></div>
                                    </div>
                                    <div class='short-details'>
                                        <p>Purchase Order No: <span>PO-DBU-AUG21-54</span></p>
                                        <p>Order Date: <span>2021-08-30</span></p>
                                        <p>Status: <span>Approved</span></p>
                                    </div>

                                    <div class='parchaseReport'>
                                        <div class='reportInfo'>
                                            <div class='reports reportInfo1'>
                                                <p>Supplier: <span>PRIME BANK LTD</span></p>
                                                <p>Email: <span>demo @ibos.io</span></p>
                                                <p>Attn: <span>PRIME BANK LTD</span></p>
                                                <p>Phone: <span>01758746556</span></p>
                                                <p>Address: <span>Dhaka</span></p>
                                            </div>
                                            <div class='reportInfo2'>
                                                <p>Ship To: <span>iBos center</span></p>
                                                <p>Bin No. <span></span></p>
                                                <p>PR No. <span>PC-DBU-JUN21-16</span></p>
                                            </div>
                                            <div class='reportInfo3'>
                                                <p>Bill To:</p>
                                                <p>Demo Business Unit</p>
                                                <p>Bin No. <span></span></p>
                                                <p>Lalmatia, Dhaka, Bangladesh</p>
                                            </div>
                                        </div>
                                    </div>");

            sb.Append(@"   
                        <div class='main - table'>
                            <table>
                                <tr>
                                    <th> SL </th>
                                    <th> ITEM </th>
                                    <th> DESCRIPTION </th>
                                    <th> UoM </th>
                                    <th> QTY.</th>
                                    <th> RATE </th>
                                    <th> VAT(%) </th>
                                    <th> VAT AMOUNT </th>
                                    <th> TOTAL </th>
                                </tr> ");

            for (int i = 0; i < 2; i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                    <td>{5}</td>
                                    <td>{6}</td>
                                    <td>{7}</td>
                                    <td>{8}</td>
                                  </tr>", "data", "data", "data", "data", "data", "data", "data", "data", "data");
            }

            sb.Append(@"
                        </table>
                            </div>
                            < div class='other-point-info'>
                                <table class='table custom-table'>
                                    <tr>
                                        <td><span>Partial Shipment</span></td>
                                        <td>No</td>
                                        <td><span>Freight</span></td>
                                        <td style = 'width: 100px;' > 0 </ td >
                                    </ tr>
                                    <tr>
                                        <td><span> No of Shipment</span></td>
                                        <td>1</td>
                                        <td><span> Others Charge</span></td>
                                        <td>0</td>
                                    </tr>
                                    <tr>
                                        <td><span> Last Shipment Date</span></td>
                                        <td>2021-09-14</td>
                                        <td><span> Gross Discount</span></td>
                                        <td>0</td>
                                    </tr>
                                    <tr>
                                        <td><span> Payment terms</span></td>
                                        <td>Credit</td>
                                        <td><span> Commission</span></td>
                                        <td>0</td>
                                    </tr>
                                    <tr>
                                        <td><span> Payment days after MRR</span></td>
                                        <td>21</td>
                                        <td><span><b>Grand Total</b></span></td>
                                        <td><b>50</b></td>
                                    </tr>
                                    <tr>
                                        <td><span> No of Installment</span></td>
                                        <td>0</td>
                                    </tr>
                                    <tr>
                                        <td><span> Installment Interval (Days)</span></td>
                                        <td>0</td>
                                    </tr>
                                </table>
                            </div>
                            <div class='others'>
                                <p class='uppercase bold'>TOTAL(IN WORD) : FIFTY TK ONLY</p>
                                <p>Other terms: <span>NA</span></p>
                                <p>Other charges: <span>NA</span></p>
                                <p>Prepared By: <span>NA</span></p>
                                <p>Approved By: <span>NA</span></p>
                            </div>");

            sb.Append(@" 
                    <div class='global-table'>
                        <table>
                            <tr>
                                <th> SL </ th>
                                <th> ITEM </ th>
                                <th> DESCRIPTION </ th>
                                <th> UoM </ th>
                                <th> QTY.</ th>
                                <th> QTY.</ th>
                            </ tr>");

            for (int i = 0; i < 2; i++)
            {
                sb.AppendFormat(@"<tr>
                                    <td>{0}</td>
                                    <td>{1}</td>
                                    <td>{2}</td>
                                    <td>{3}</td>
                                    <td>{4}</td>
                                  </tr>", "1", "HP Laptop", "N/A", "UoM", "123", "343");
            }

            sb.Append(@"  
                                    </table>
                                </div>
                            </div>
                        </section>
                    </body>
                </html>");

            return sb.ToString();
        }

        //public static string GetHTMLString(GetPurchaseOrderCommonDTObyIdtoPrint data)
        //{
        //    GetPurchaseOrderHeaderByIdToPrintDTO header = data.objHeaderDTO;
        //    string dynamicVal = "Abu Rayhan";

        //    var sb = new StringBuilder();
        //    sb.Append(@"
        //               <!DOCTYPE html>
        //                    <html>
        //                        <head>
        //                            <meta charset='UTF-8'>
        //                            <meta http-equiv = 'X-UA-Compatible' content = 'IE=edge'>
        //                            <meta name = 'viewport' content = 'width=device-width, initial-scale=1.0'>
        //                            <title > Table </ title >
        //                        </ head >");

        //    sb.Append(@"
        //                <body>
        //                    <section class='table-design'>
        //                        <div class='table-wrapper'>
        //                            <div class='section-heading'>
        //                                <div class='heading-image'>
        //                                    <img src = 'https://erp.ibos.io/domain/Document/DownlloadFile?id=615159acf37288f5197e8b23' alt="">
        //                                </div>
        //                                <div class='heading-title'>
        //                                    <h3>${dynamicVal}</h3>
        //                                    <h6>Lalmatia, Dhaka, Bangladesh</h6>
        //                                    <h4>Purchase Order</h4>
        //                                </div>
        //                                <div></div>
        //                            </div>
        //                            <div class='short-details'>
        //                                <p>Purchase Order No: <span>PO-DBU-AUG21-54</span></p>
        //                                <p>Order Date: <span>2021-08-30</span></p>
        //                                <p>Status: <span>Approved</span></p>
        //                            </div>

        //                            <div class='parchaseReport'>
        //                                <div class='reportInfo'>
        //                                    <div class='reports reportInfo1'>
        //                                        <p>Supplier: <span>PRIME BANK LTD</span></p>
        //                                        <p>Email: <span>demo @ibos.io</span></p>
        //                                        <p>Attn: <span>PRIME BANK LTD</span></p>
        //                                        <p>Phone: <span>01758746556</span></p>
        //                                        <p>Address: <span>Dhaka</span></p>
        //                                    </div>
        //                                    <div class='reportInfo2'>
        //                                        <p>Ship To: <span>iBos center</span></p>
        //                                        <p>Bin No. <span></span></p>
        //                                        <p>PR No. <span>PC-DBU-JUN21-16</span></p>
        //                                    </div>
        //                                    <div class='reportInfo3'>
        //                                        <p>Bill To:</p>
        //                                        <p>Demo Business Unit</p>
        //                                        <p>Bin No. <span></span></p>
        //                                        <p>Lalmatia, Dhaka, Bangladesh</p>
        //                                    </div>
        //                                </div>
        //                            </div>");

        //    sb.Append(@"   
        //                <div class='main - table'>
        //                    <table>
        //                        <tr>
        //                            <th> SL </th>
        //                            <th> ITEM </th>
        //                            <th> DESCRIPTION </th>
        //                            <th> UoM </th>
        //                            <th> QTY.</th>
        //                            <th> RATE </th>
        //                            <th> VAT(%) </th>
        //                            <th> VAT AMOUNT </th>
        //                            <th> TOTAL </th>
        //                        </tr> ");

        //    for (int i = 0; i < 2; i++)
        //    {
        //        sb.AppendFormat(@"<tr>
        //                            <td>{0}</td>
        //                            <td>{1}</td>
        //                            <td>{2}</td>
        //                            <td>{3}</td>
        //                            <td>{4}</td>
        //                            <td>{5}</td>
        //                            <td>{6}</td>
        //                            <td>{7}</td>
        //                            <td>{8}</td>
        //                          </tr>", "data","data", "data", "data", "data", "data", "data", "data","data");
        //    }

        //    sb.Append(@"
        //                </table>
        //                    </div>
        //                    < div class='other-point-info'>
        //                        <table class='table custom-table'>
        //                            <tr>
        //                                <td><span>Partial Shipment</span></td>
        //                                <td>No</td>
        //                                <td><span>Freight</span></td>
        //                                <td style = 'width: 100px;' > 0 </ td >
        //                            </ tr>
        //                            <tr>
        //                                <td><span> No of Shipment</span></td>
        //                                <td>1</td>
        //                                <td><span> Others Charge</span></td>
        //                                <td>0</td>
        //                            </tr>
        //                            <tr>
        //                                <td><span> Last Shipment Date</span></td>
        //                                <td>2021-09-14</td>
        //                                <td><span> Gross Discount</span></td>
        //                                <td>0</td>
        //                            </tr>
        //                            <tr>
        //                                <td><span> Payment terms</span></td>
        //                                <td>Credit</td>
        //                                <td><span> Commission</span></td>
        //                                <td>0</td>
        //                            </tr>
        //                            <tr>
        //                                <td><span> Payment days after MRR</span></td>
        //                                <td>21</td>
        //                                <td><span><b>Grand Total</b></span></td>
        //                                <td><b>50</b></td>
        //                            </tr>
        //                            <tr>
        //                                <td><span> No of Installment</span></td>
        //                                <td>0</td>
        //                            </tr>
        //                            <tr>
        //                                <td><span> Installment Interval (Days)</span></td>
        //                                <td>0</td>
        //                            </tr>
        //                        </table>
        //                    </div>
        //                    <div class='others'>
        //                        <p class='uppercase bold'>TOTAL(IN WORD) : FIFTY TK ONLY</p>
        //                        <p>Other terms: <span>NA</span></p>
        //                        <p>Other charges: <span>NA</span></p>
        //                        <p>Prepared By: <span>NA</span></p>
        //                        <p>Approved By: <span>NA</span></p>
        //                    </div>");

        //    sb.Append(@" 
        //            <div class='global-table'>
        //                <table>
        //                    <tr>
        //                        <th> SL </ th>
        //                        <th> ITEM </ th>
        //                        <th> DESCRIPTION </ th>
        //                        <th> UoM </ th>
        //                        <th> QTY.</ th>
        //                        <th> QTY.</ th>
        //                    </ tr>");

        //    for (int i = 0; i < 2; i++)
        //    {
        //        sb.AppendFormat(@"<tr>
        //                            <td>{0}</td>
        //                            <td>{1}</td>
        //                            <td>{2}</td>
        //                            <td>{3}</td>
        //                            <td>{4}</td>
        //                          </tr>", "1", "HP Laptop", "N/A", "UoM", "123", "343");
        //    }

        //    sb.Append(@"  
        //                            </table>
        //                        </div>
        //                    </div>
        //                </section>
        //            </body>
        //        </html>");

        //    return sb.ToString();
        //}
    }
}
