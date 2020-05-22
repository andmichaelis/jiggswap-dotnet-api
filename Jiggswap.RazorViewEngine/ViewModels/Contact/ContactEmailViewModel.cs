using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Jiggswap.RazorViewEngine.ViewModels.Feedback
{
    public class ContactEmailViewModel : JiggswapEmailViewModelBase
    {
        public override string RazorViewPath => "/Views/Emails/Contact/ContactEmail.cshtml";

        public override string GetPlainContent()
        {
            return @$"{Name} <{Email}> has submitted the following comment:
---
{Comment}
---
- JiggSwap";
        }

        public string Email { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
    }
}