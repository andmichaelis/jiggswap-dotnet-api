using System;
using System.Collections.Generic;
using System.Text;

namespace Jiggswap.RazorViewEngine.ViewModels
{
    public abstract class JiggswapEmailViewModelBase
    {
        public abstract string RazorViewPath { get; }

        public abstract string GetPlainContent();
    }
}