using System;
using System.Threading;
using Xilium.CefGlue;

namespace CefTest
{
    internal sealed class AutoCefClient : CefClient
    {
        private AutoCefBrowser _owner;

        private AutoCefLifeSpanHandler _lifeSpanHandler;
        private AutoCefDisplayHandler _displayHandler;
        private AutoCefRenderHandler _renderHandler;
        private AutoCefLoadHandler _loadHandler;

        public AutoCefClient(AutoCefBrowser owner)
        {
            if (owner == null) throw new ArgumentNullException("owner");

            _owner = owner;

            _lifeSpanHandler = new AutoCefLifeSpanHandler(owner);
            _displayHandler = new AutoCefDisplayHandler(owner);
            _renderHandler = new AutoCefRenderHandler(owner);
            _loadHandler = new AutoCefLoadHandler(owner);
        }

        protected override CefLifeSpanHandler GetLifeSpanHandler()
        {
            return _lifeSpanHandler;
        }

        protected override CefDisplayHandler GetDisplayHandler()
        {
            return _displayHandler;
        }

        protected override CefRenderHandler GetRenderHandler()
        {
            return _renderHandler;
        }

        protected override CefLoadHandler GetLoadHandler()
        {
            return _loadHandler;
        }

        protected override bool OnProcessMessageReceived(CefBrowser browser, CefProcessId sourceProcess, CefProcessMessage message)
        {
            var handled = base.OnProcessMessageReceived(browser, sourceProcess, message);
            if (handled) return true;

            if (message.Name == "JavascriptResult")
            {
                var arguments = message.Arguments;
                var type = arguments.GetValueType(0);
                object value;
                switch (type)
                {
                    case CefValueType.Null: value = null; break;
                    case CefValueType.String: value = arguments.GetString(0); break;
                    case CefValueType.Int: value = arguments.GetInt(0); break;
                    case CefValueType.Double: value = arguments.GetDouble(0); break;
                    case CefValueType.Bool: value = arguments.GetBool(0); break;
                    default: value = null; break;
                }

                JavascriptResult = value;
                mrevent.Set();
                return true;
            }

            if (message.Name == "JavascriptException")
            {
                JavascriptException = message.Arguments.GetString(0);
                mrevent.Set();
                return true;
            }

            return false;
        }


        ManualResetEvent mrevent = new ManualResetEvent(false);
        string JavascriptException = null;
        object JavascriptResult = null;

        internal object TryEval(string code, CefBrowser browser)
        {
            if (browser != null)
            {
                var message = CefProcessMessage.Create("JavascriptEval");
                var arguments = message.Arguments;
                arguments.SetString(0, code);

                browser.SendProcessMessage(CefProcessId.Renderer, message);
                JavascriptException = null;
                JavascriptResult = null;
                mrevent.Reset();
                mrevent.WaitOne();
            }

            if (JavascriptException != null)
            {
                throw new Exception(JavascriptException);
            }

            return JavascriptResult;
        }
    }
}
