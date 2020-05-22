using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Passwords
{
    public class ForgotPasswordEmailViewModel : JiggswapEmailViewModelBase
    {
        public string ResetUrl { get; set; }

        public override string RazorViewPath => "/Views/Emails/Passwords/ForgotPasswordEmail.cshtml";

        public override string GetPlainContent()
        {
            return @$"Hello, Please use the following link to reset your password:
[{ResetUrl}]
If you have any questions, please get in touch with us by replying to this email.

- JiggSwap";
        }
    }
}