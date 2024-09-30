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
                            <img src=""data:image/jpeg;base64,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"">
                        </div>
                        <div style=""content"">
                            <h3>Đăng ký tài khoản thành công</h3>
                            <p style=""font-size: 15px;"">Bạn đã đăng ký trở thành Quản lý trường học của trường {school}.</p>
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
