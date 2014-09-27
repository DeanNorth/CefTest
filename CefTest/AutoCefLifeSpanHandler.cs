using System;
using Xilium.CefGlue;

namespace CefTest
{
    internal sealed class AutoCefLifeSpanHandler : CefLifeSpanHandler
    {
        private readonly AutoCefBrowser _owner;

        public AutoCefLifeSpanHandler(AutoCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;
        }

        protected override void OnAfterCreated(CefBrowser browser)
        {
            _owner.HandleAfterCreated(browser);
        }
    }
}
