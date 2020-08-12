using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QLHS.Data;
using QLHS.Models;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace QLHS.Controllers
{
    public class CartController : Controller
    {
        private readonly QLHSContext _context;

        public CartController(QLHSContext context)
        {
            _context = context;
        }


        public int Add()
        {
            string cusID = HttpContext.Session.GetString("CusID") ?? string.Empty;
            DateTime sDate = DateTime.ParseExact(HttpContext.Session.GetString("sDate"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime eDate = DateTime.ParseExact(HttpContext.Session.GetString("eDate"), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            if (cusID == null) return 0;

            double discount = 0;
            if (HttpContext.Session.GetInt32("Voucher") != null) discount = (double)HttpContext.Session.GetInt32("Voucher");
            double grandtotal = (double)(HttpContext.Session.GetInt32("GrandTotal") ?? 0);
            Bill bill = new Bill
            {
                Emp_ID = "admin",
                Cus_ID = cusID,
                Bill_Status = 1,
                Bill_CheckInDate = sDate,
                Bill_CheckOutDate = eDate,
                Bill_CusNum = HttpContext.Session.GetInt32("numC") ?? 0,
                Bill_BookCost = grandtotal * (double)((100 - discount) / 100),
                Bill_ExtraCost = 0,
                Bill_Total = grandtotal * ((100 - discount) / 100)
            };
            _context.Add(bill);
            _context.SaveChanges();
            int billid = bill.Bill_ID;


            string cartStr = HttpContext.Session.GetString("cart") ?? "";
            if (cartStr == "") return 0;
            JObject json = null;
            if (cartStr != "") json = JObject.Parse(cartStr);
            if (!json.HasValues) return 0;
            

            foreach (var item in json)
            {
                int roomid = (int)item.Value.SelectToken("Room");
                for (var day = sDate.Date; day < eDate.Date; day = day.AddDays(1))
                {
                    RoomBooking roomBooking = new RoomBooking
                    {
                        Bill_ID = billid,
                        Room_ID = roomid,
                        Booking_Date = day.Year * 10000 + day.Month * 100 + day.Day
                    };
                    _context.Add(roomBooking);
                }
            }
            _context.SaveChanges();



            //Mail
            var message = new MimeMessage();
            string cusEmail = HttpContext.Session.GetString("cusEmail");
            message.From.Add(new MailboxAddress("Royal HS", "royalhsbill@gmail.com"));
            message.To.Add(new MailboxAddress("Mr. Luong Do", $"{cusEmail}"));
            
            string billID = "#"+billid.ToString();
            string currDateTime = DateTime.Now.ToString(new CultureInfo("vi-vn"));


            string cusName = HttpContext.Session.GetString("cusName");
            string cusPhone = HttpContext.Session.GetString("cusPhone");
            string cusAddress = HttpContext.Session.GetString("cusAddress");

            string billCheckinDate = sDate.ToString(new CultureInfo("vi-vn"));
            string billCheckoutDate = eDate.ToString(new CultureInfo("vi-vn"));
            string grandTotal = (grandtotal * ((100 - discount) / 100)).ToString("C0", new CultureInfo("vi-vn"));

            message.Subject = $"Chi tiết hoá đơn đặt phòng {billID}";
            message.Body = new TextPart("html")
            {
                Text = $"<table align=\"center\" bgcolor=\"#dcf0f8\" border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;background-color:#f2f2f2;width:100%!important;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px\" width=\"100%\">	<tbody>		<tr>			<td align=\"center\" style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\" valign=\"top\">			<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"margin-top:15px\" width=\"600\">				<tbody>					<tr style=\"background:#fff\">						<td align=\"left\" height=\"auto\" style=\"padding:15px\" width=\"600\">						<table>							<tbody>																<tr>									<td>									<h1 style=\"font-size:17px;font-weight:bold;color:#444;padding:0 0 5px 0;margin:0\">Cảm ơn quý khách {cusName} đã đặt hàng tại Royal HS,</h1>																		<p style=\"margin:4px 0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\">Royal HS rất vui thông báo đơn hàng {billID} của quý khách đã được tiếp nhận và lưu trữ trên hệ thống.</p>																		<h3 style=\"font-size:13px;font-weight:bold;color:#02acea;text-transform:uppercase;margin:20px 0 0 0;border-bottom:1px solid #ddd\">Thông tin đơn hàng {billID} <span style=\"font-size:12px;color:#777;text-transform:none;font-weight:normal\">{currDateTime}</span></h3>									</td>								</tr>								<tr>									<td style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px\">									<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\">										<thead>											<tr>												<th align=\"left\" style=\"padding:6px 9px 0px 9px;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;font-weight:bold\" width=\"50%\">Thông tin thanh toán</th>												<th align=\"left\" style=\"padding:6px 9px 0px 9px;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;font-weight:bold\" width=\"50%\"> Thời gian nhận phòng </th>											</tr>										</thead>										<tbody>											<tr>												<td style=\"padding:3px 9px 9px 9px;border-top:0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\" valign=\"top\"><span style=\"text-transform:capitalize\">{cusName}</span><br>												<a href=\"mailto:{cusEmail}\" target=\"_blank\">{cusEmail}</a><br>												{cusPhone}<br>												{cusAddress}												</td>												<td style=\"padding:3px 9px 9px 9px;border-top:0;border-left:0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\" valign=\"top\"><span style=\"text-transform:capitalize\">{billCheckinDate}</span><br>												 <br>												{billCheckoutDate}</td>											</tr>											<tr>											</tr>																																</tbody>									</table>									</td>								</tr>								<tr>									<td>									<p style=\"margin:4px 0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\"><i>Lưu ý: Khi đến nhận phòng Homestay, nhân viên lễ tân có thể yêu cầu người nhận phòng cung cấp CMND / giấy phép lái xe để chụp ảnh hoặc ghi lại thông tin.</i></p>									</td>								</tr>																<tr>									<td>									<h2 style=\"text-align:left;margin:10px 0;border-bottom:1px solid #ddd;padding-bottom:5px;font-size:13px;color:#02acea\">CHI TIẾT ĐƠN HÀNG</h2>									<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" style=\"background:#f5f5f5\" width=\"100%\">										<thead>											<tr>												<th align=\"left\" bgcolor=\"#02acea\" style=\"padding:6px 9px;color:#fff;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:14px\">STT</th>												<th align=\"left\" bgcolor=\"#02acea\" style=\"padding:6px 9px;color:#fff;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:14px\">Nội dung</th>												<th align=\"right\" bgcolor=\"#02acea\" style=\"padding:6px 9px;color:#fff;font-family:Arial,Helvetica,sans-serif;font-size:12px;line-height:14px\">Thành tiền</th>											</tr>										</thead>										<tbody bgcolor=\"#eee\" style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px\">											<tr>												<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\"><span>1</span><br>												</td>												<td align=\"left\" style=\"padding:3px 9px\" valign=\"top\"><span>Tiền thu thanh toán đặt phòng</span></td>												<td align=\"right\" style=\"padding:3px 9px\" valign=\"top\"><span>{grandTotal}</span></td>											</tr>											 										</tbody>																						<tr bgcolor=\"#eee\">												<td align=\"right\" colspan=\"2\" style=\"padding:7px 9px\"><strong><big>Tổng tiền thu</big> </strong></td>												<td align=\"right\" style=\"padding:7px 9px\"><strong><big><span>{grandTotal}</span> </big> </strong></td>											</tr>										</tfoot>									</table>									<div style=\"margin:auto\"><a href=\"\" style=\"display:inline-block;text-decoration:none;background-color:#00b7f1!important;margin-right:30px;text-align:center;border-radius:3px;color:#fff;padding:5px 10px;font-size:12px;font-weight:bold;margin-left:35%;margin-top:5px\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=https://tiki.vn/sales/order/trackingDetail?code%3D746504874&amp;source=gmail&amp;ust=1594303783242000&amp;usg=AFQjCNGrZAnCVbnSON_VTkwd8buaWR8Okw\">Chi tiết đơn hàng tại Royal HS</a></div>									</td>								</tr>																<tr>									<td>&nbsp;									<p style=\"margin:0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\">Trường hợp quý khách có những băn khoăn về đơn hàng, có thể xem thêm mục <a href=\"\" title=\"Các câu hỏi thường gặp\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=http://hotro.Royal HS.vn/hc/vi/?utm_source%3Dtransactional%2Bemail%26utm_medium%3Demail%26utm_term%3Dlogo%26utm_campaign%3Dnew%2Border&amp;source=gmail&amp;ust=1594303783242000&amp;usg=AFQjCNGmdmxJvF88Zd8x_1a2R67nUyDO1Q\"> <strong>các câu hỏi thường gặp</strong>.</a></p>																		<p style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal;border:1px #14ade5 dashed;padding:5px;list-style-type:none\">Từ ngày 14/2/2015, Royal HS sẽ không gởi tin nhắn SMS khi đơn hàng của bạn được xác nhận thành công.</p>																		<p style=\"margin:10px 0 0 0;font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal\">Bạn cần được hỗ trợ ngay? Chỉ cần email <a href=\"mailto:hotro@Royal HS.vn\" style=\"color:#099202;text-decoration:none\" target=\"_blank\"> <strong>hotro@RoyalHS.vn</strong> </a>, hoặc gọi số điện thoại <strong style=\"color:#099202\">1900-6035</strong> (8-21h cả T7,CN). Đội ngũ Royal HS Care luôn sẵn sàng hỗ trợ bạn bất kì lúc nào.</p>									</td>								</tr>								<tr>									<td>&nbsp;									<p style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;margin:0;padding:0;line-height:18px;color:#444;font-weight:bold\">Một lần nữa Royal HS cảm ơn quý khách.</p>									<p style=\"font-family:Arial,Helvetica,sans-serif;font-size:12px;color:#444;line-height:18px;font-weight:normal;text-align:right\"><strong><a  style=\"color:#00a3dd;text-decoration:none;font-size:14px\" target=\"_blank\" data-saferedirecturl=\"https://www.google.com/url?q=http://Royal HS.vn?utm_source%3Dtransactional%2Bemail%26utm_medium%3Demail%26utm_term%3Dlogo%26utm_campaign%3Dnew%2Border&amp;source=gmail&amp;ust=1594303783242000&amp;usg=AFQjCNEVrS3Bg5SZP6XsOKPbP6hMIB_CdA\">Royal HS</a> </strong></p>									</td>								</tr>							</tbody>						</table>						</td>					</tr>				</tbody>			</table>			</td>		</tr>		<tr>			<td align=\"center\">			<table width=\"600\">				<tbody>					<tr>						<td>						<p align=\"left\" style=\"font-family:Arial,Helvetica,sans-serif;font-size:11px;line-height:18px;color:#4b8da5;padding:10px 0;margin:0px;font-weight:normal\">Quý khách nhận được email này vì đã đặt phòng tại Royal HS.<br>						Để chắc chắn luôn nhận được email thông báo, xác nhận đặt phòng từ Royal HS, quý khách vui lòng thêm địa chỉ <strong><a href=\"mailto:hotro@Royal HS.vn\" target=\"_blank\">hotro@RoyalHS.vn</a></strong> vào số địa chỉ (Address Book, Contacts) của hộp email.<br>						<b>Văn phòng Royal HS:</b> 52 Út Tịch, phường 4, quận Tân Bình, thành phố Hồ Chí Minh, Việt Nam<br>						Bạn không muốn nhận email từ Royal HS? Hủy đăng ký tại <a>đây</a>.</p>						</td>					</tr>				</tbody>			</table>			</td>		</tr>	</tbody></table>"

            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                client.Authenticate("royalhsbill@gmail.com", "**********");

                client.Send(message);
                client.Disconnect(true);
            }


            HttpContext.Session.Clear();

            return 1;
        }


        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("sDate") == "" || HttpContext.Session.GetString("eDate") == "") return RedirectToAction("Index", "Home");
            ViewData["sDate"] = HttpContext.Session.GetString("sDate");
            ViewData["eDate"] = HttpContext.Session.GetString("eDate");
            string cartStr = HttpContext.Session.GetString("cart") ?? "";

            DateTime s = DateTime.ParseExact(ViewData["sDate"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            DateTime e = DateTime.ParseExact(ViewData["eDate"].ToString(), "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
            ViewData["dd"] = e.Subtract(s).Days;

            JObject json = null;
            if (cartStr != "")
            {
                json = JObject.Parse(cartStr);
                if (json.HasValues) ViewBag.List = json;
            }
            double sum = 0;
            if (cartStr != "")
                if (json.HasValues)
                    foreach (var item in json)
                    {
                        var q = item.Value.SelectToken("Price");
                        sum += (double)q;
                    }
            HttpContext.Session.SetInt32("dd", (int)ViewData["dd"]);
            HttpContext.Session.SetInt32("GrandTotal", ((int)sum) * (int)ViewData["dd"]);
            ViewBag.Sum = sum;
            return View(await _context.Bill.ToListAsync());
        }

        public string GetVoucherValue(string id)
        {
            string newid = id ?? "";
            Voucher v = _context.Voucher.Find(newid);
            int value = 0;
            double tt = (double)HttpContext.Session.GetInt32("GrandTotal");
            double up = tt / (double)HttpContext.Session.GetInt32("dd");

            double gtt = tt;
            if (v != null)
            {
                value = v.Voucher_Value;
                HttpContext.Session.SetInt32("Voucher", value);
                tt = tt * value / 100;
                up = up * (100 - value) / 100;
                gtt -= tt;
            }
            string total = 0.ToString("C0", new CultureInfo("vi-vn"));
            string gtotal = gtotal = gtt.ToString("C0", new CultureInfo("vi-vn"));
            string uprice = uprice = up.ToString("C0", new CultureInfo("vi-vn"));
            if (value != 0)
            {
                total = tt.ToString("C0", new CultureInfo("vi-vn"));

            }


            string res = value.ToString();
            res += '/';
            res += total;
            res += '/';
            res += uprice;
            res += '/';
            res += gtotal;

            return res;

        }

        public IActionResult RemoveItem(int? id)
        {
            if (!(id is null))
            {
                int newID = id ?? 0;
                string cartStr = HttpContext.Session.GetString("cart") ?? "";
                var list = JsonConvert.DeserializeObject<Dictionary<int, Object>>(cartStr);
                if (list.ContainsKey(newID))
                {
                    list.Remove(newID);
                    HttpContext.Session.SetString("cart", JsonConvert.SerializeObject(list));
                    int qty = HttpContext.Session.GetInt32("total") ?? 1;
                    qty -= 1;
                    HttpContext.Session.SetInt32("total", qty);
                }
            }
            return RedirectToAction("Index");
        }


        public IActionResult Confirm()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(string fullname, string gender, string id, string email, string birthday, string address, string phone)
        {
            string userid = Guid.NewGuid().ToString().Substring(0, 15);
            string userpass = Guid.NewGuid().ToString().Substring(0, 15);
            UserAcc useracc = new UserAcc
            {
                User_ID = userid,
                User_Pass = userpass,
                User_Role = 3
            };
            _context.Add(useracc);
            await _context.SaveChangesAsync();
            Customer customer = new Customer
            {
                Cus_Address = address,
                Cus_Birth = DateTime.ParseExact(birthday, "yyyy-MM-dd",
                                       System.Globalization.CultureInfo.InvariantCulture),
                Cus_Email = email,
                Cus_Gender = gender,
                Cus_ID = id,
                Cus_Name = fullname,
                Cus_Phone = phone,
                User_ID = userid
            };
            var flag = await _context.Customer.Where(p => p.Cus_ID == id).CountAsync();
            if ((int)flag > 0) _context.Update(customer);
            else
                _context.Add(customer);

            HttpContext.Session.SetString("cusName", fullname);
            HttpContext.Session.SetString("cusEmail", email);
            HttpContext.Session.SetString("cusPhone", phone);
            HttpContext.Session.SetString("cusAddress", address);

            double discount = 0;
            if (HttpContext.Session.GetInt32("Voucher") != null) discount = (double)HttpContext.Session.GetInt32("Voucher");
            double grandtotal = (HttpContext.Session.GetInt32("GrandTotal") ?? 0);
            ViewBag.GrandTotal = (grandtotal * (double)((100 - discount) / 100)).ToString("C0", new CultureInfo("vi-vn"));
            ViewBag.VValue = discount;

            HttpContext.Session.SetString("CusID", customer.Cus_ID);
            await _context.SaveChangesAsync();
            return View(customer);
        }


        // GET: Cart/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Bill_ID,Emp_ID,Cus_ID,Bill_Status,Bill_CheckInDate,Bill_CheckOutDate,Bill_CusNum,Bill_BookCost,Bill_ExtraCost,Bill_Total")] Bill bill)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bill);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(bill);
        }


        // GET: Cart/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bill = await _context.Bill
                .FirstOrDefaultAsync(m => m.Bill_ID == id);
            if (bill == null)
            {
                return NotFound();
            }

            return View(bill);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bill = await _context.Bill.FindAsync(id);
            _context.Bill.Remove(bill);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BillExists(int id)
        {
            return _context.Bill.Any(e => e.Bill_ID == id);
        }
    }
}
