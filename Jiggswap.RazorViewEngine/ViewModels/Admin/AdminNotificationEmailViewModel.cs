using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels.Admin
{
    public class AdminNotificationEmailViewModel : JiggswapEmailViewModelBase
    {
        public string Message { get; set; }

        public override string RazorViewPath => "/Views/Emails/Admin/AdminNotificationEmail.cshtml";

        public override string GetPlainContent()
        {
            return Message;
        }
    }
}