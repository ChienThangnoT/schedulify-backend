using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.Utils
{
    internal class MailTemplate
    {
        public static string ConfirmTemplate(string school)
        {
            string body = $@"
                <!DOCTYPE html>
                <head>
                    <meta charset=""utf-8"" />
                    <title>Đăng ký thành công</title>
                </head>
                <body>
                    <div style=""container"">
                        <div style=""header"">
                            <img src=""https://firebasestorage.googleapis.com/v0/b/schedulify-70161.appspot.com/o/logo.jpg?alt=media&token=3a838eda-3add-46cf-bbdc-40adc1d5fb9a""alt=""Logo"" style=""width:120px;"">
                        </div>
                        <div style=""content"">
                            <h3>Đăng ký tài khoản thành công</h3>
                            <p style=""font-size: 15px;"">Bạn đã đăng ký trở thành Quản lý trường học của {school}.</p>
                            <p style=""font-size: 15px;"">Hiện tại bạn đã có thể đăng nhập vào hệ thống để thực hiện quản lý cho trường của bạn.</p>
                            <p style=""font-size: 15px; font-style: italic;"">Chúc bạn có trải nghiệm tuyệt vời với Schedulify.</p>
                            <div style=""note"">
                                <p style=""font-size: 15px; padding-top: 40px;font-style: italic;"">* Lưu ý: Tài khoản chỉ có thể đăng nhập được khi đã được chứng thực.</p>
                            </div>
                        </div>
                        <div style=""footer"">
                            <div style=""info"">
                                <p>Liên lạc đến schedulifyse078@gmail.com để hiểu rõ hơn.</p>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";
            return body;
        }

        public static string ResetPasswordTemplate(string email, int code)
        {
            string body = $@"
                <!DOCTYPE html>
				<html lang=""en"">
					<head>
						<meta charset=""UTF-8"" />
						<meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
						<title>Schedulify</title>
						<script src=""https://cdn.tailwindcss.com""></script>
						<style></style>
					</head>
					<body>
						<div class=""container"">
							<div class=""content py-3"">
								<div>
									<h4 class=""w-full text-center font-bold text-2xl"">
										Đặt lại mật khẩu tài khoản
									</h4>
									<p class="""">
										Bạn vừa thực hiện yêu cầu đặt lại mật khẩu cho tài khoản
										<span class=""font-semibold"">{email}</span>
										tại Schedulify.
									</p>
									<h2 class=""text-lg"">Đây là mã OTP của bạn:</h2>
									<h1 class=""w-full text-center text-4xl font-bold text-[#003e6d] tracking-widest"">
										{code}
									</h1>
								</div>
								<div class=""otp""></div>
								<div class=""note"">
									<p class=""italic"">
										* Lưu ý: Nếu bạn không thực hiện yêu cầu này, vui lòng bỏ qua.
									</p>
								</div>
							</div>
							<div class=""footer"">
								<img
									class=""w-full h-fit object-contain object-bottom""
									src=""https://firebasestorage.googleapis.com/v0/b/schedulify-70161.appspot.com/o/mail-footer.png?alt=media&token=78e20378-0c99-4c20-8c8e-06102b8c824f""
									alt=""Logo""
								/>
								<div class=""info py-3"">
									<p>
										Liên lạc đến
										<a
											href=""mailto:schedulifyse078@gmail.com""
											class=""text-[#004e89] underline""
											>schedulifyse078@gmail.com</a
										>
										để biết thêm thông tin chi tiết.
									</p>
									<p class=""text-justify"">
										<strong>Địa chỉ</strong>:
										<a
											href=""https://maps.app.goo.gl/E85zatFSnrFjRkJA9""
											class=""underline""
										>
											Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức,
											Hồ Chí Minh 700000, Việt Nam
										</a>
									</p>
								</div>
							</div>
						</div>
					</body>
				</html>
            ";
            return body;
        }

        public static string SendPasswordTemplate(string schoolName, string accountName,string email, string password)
        {
            string body = $@"
                    <!DOCTYPE html>
                    <html lang=""vi"">
                      <head>
                        <meta charset=""UTF-8"" />
                        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"" />
                        <title>Xác Thực Tài Khoản Fricks</title>
                      </head>
                      <body
                        style=""
                          font-family: 'Inter', sans-serif;
                          background-color: #f4f4f4;
                          padding: 20px;
                          margin: 0;
                        ""
                      >
                        <div
                          style=""
                            width: 600px;
                            background-color: #24547c;
                            margin: 0 auto;
                            overflow: hidden;
                            box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
                          ""
                        >
                          <div
                            style=""
                              background-color: #003e6d;
                              text-align: left;
                              width: 600px;
                              height: 80px;
                              padding: 0 20px;
                              display: table;
                            ""
                          >
                            <div style=""display: table-cell; vertical-align: middle; color: #ffffff;"">
                              <h2>Schedulify</h2>
                            </div>
                          </div>

                          <div
                            style=""
                              padding: 30px 50px;
                              text-align: left;
                              background-color: #fff;
                              margin-top: 5px;
                            ""
                          >
                            <h2 style=""color: #333"">
                              Thông tin tài khoản
                              <span style=""color: #f4a02a; font-weight: 700"">Schedulify</span>
                            </h2>
                            <p>Xin chào, <strong>{accountName}</strong></p>
                            <p>
                              Bạn đã được tạo tài khoản {email} thuộc trường THPT {schoolName}. Đây là thông tin đăng nhập
                              của bạn.
                            </p>
                            <div style=""margin-top: 35px;"">
                              <p>
                                Tài khoản: <span style=""font-style: italic; font-weight: 700; color: #24547c""
                                >{email}</span>
                              </p>
                              <p>
                                Mật khẩu: <span style=""font-style: italic; font-weight: 700; color: #24547c""
                                >{password}</span>
                              </p>
                            </div>
                            <p style=""font-size: 16px; font-style: italic; line-height: 22px; margin-top: 35px; color: red"">
                              <strong style=""color: red"">*Lưu ý:</strong> Không cung cấp thông tin tài khoản cho người khác để tránh những thiệt hại không đáng có!
                            </p>
                          </div>

                          <div
                            style=""
                              background-color: #24547c;
                              color: #fff;
                              padding: 20px;
                              text-align: center;
                              line-height: 25px;
                            ""
                          >
                            <p
                              style=""
                                margin: 0 0 10px 0;
                                color: orange;
                                font-weight: 700;
                                font-size: 17px;
                              ""
                            >
                            Schedulify - Thời gian hài hòa, tri thức vươn xa
                            </p>
                            <a
                              href=""mailto:fricks.customerservice@gmail.com?subject=Contact&body=Dear shop,%0D%0A%0D%0ATôi có vấn đề này...""
                              style=""color: #ffffff; text-decoration: none; font-size: small""
                            >
                            schedulifyse078@gmail.com
                            </a>

                            <p style=""font-size: small; margin: 5px 0"">(+84) 977 54 54 50</p>
                            <p style=""font-size: small; margin: 0"">
                              Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức,
							                    Hồ Chí Minh
                            </p>
                          </div>
                        </div>
                      </body>
                    </html>

                ";
            return body;
        }
    }
}
