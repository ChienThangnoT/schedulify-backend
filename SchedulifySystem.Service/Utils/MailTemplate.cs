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
    }
}
