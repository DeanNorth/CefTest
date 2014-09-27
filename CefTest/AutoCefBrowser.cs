using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefTest.Helpers;
using Xilium.CefGlue.Platform.Windows;
using Xilium.CefGlue.WPF;
using Xilium.CefGlue;

namespace CefTest
{
    public class AutoCefBrowser : IDisposable
    {
        private CefBrowser _browser;
        private CefBrowserHost _browserHost;
        private AutoCefClient _cefClient;

        private ManualResetEvent loaded = new ManualResetEvent(false);
        private bool loading = false;

        public int Width { get; set; }
        public int Height { get; set; }

        public AutoCefBrowser(int width = 1024, int height = 768)
        {
            Width = width;
            Height = height;

            var windowInfo = CefWindowInfo.Create();

            // If we have a debugger attached, then show the browser window so we can see what it's doing
            if (Debugger.IsAttached)
            {
                windowInfo.SetAsPopup(IntPtr.Zero, null);
                windowInfo.TransparentPaintingEnabled = true;
                windowInfo.Width = width;
                windowInfo.Height = height;
                windowInfo.Style = WindowStyle.WS_POPUP | WindowStyle.WS_SIZEFRAME | WindowStyle.WS_VISIBLE;
                windowInfo.StyleEx = WindowStyleEx.WS_EX_COMPOSITED;
            }
            else
            {
                windowInfo.SetAsWindowless(IntPtr.Zero, false);
            }

            var cefBrowserSettings = new CefBrowserSettings();

            _cefClient = new AutoCefClient(this);

            CefBrowserHost.CreateBrowser(windowInfo, _cefClient, cefBrowserSettings);

            // If we are debugging, then give the browser window a chance to open before the test starts
            if (Debugger.IsAttached)
            {
                System.Threading.Thread.Sleep(1000);
            }
        }

        public void HandleAfterCreated(CefBrowser browser)
        {
            if (_browser == null)
            {
                _browser = browser;
                _browserHost = _browser.GetHost();
            }
        }

        #region Loading Events

        public event LoadStartEventHandler LoadStart;
        public event LoadEndEventHandler LoadEnd;
        public event LoadingStateChangeEventHandler LoadingStateChange;
        public event LoadErrorEventHandler LoadError;

        internal void OnLoadStart(CefFrame frame)
        {
            if (frame.IsMain)
            {
                loading = true;
            }

            if (this.LoadStart != null)
            {
                var e = new LoadStartEventArgs(frame);
                this.LoadStart(this, e);
            }
        }

        internal void OnLoadEnd(CefFrame frame, int httpStatusCode)
        {
            if (frame.IsMain)
            {
                loading = false;
                loaded.Set();
            }

            if (this.LoadEnd != null)
            {
                var e = new LoadEndEventArgs(frame, httpStatusCode);
                this.LoadEnd(this, e);
            }
        }
        internal void OnLoadingStateChange(bool isLoading, bool canGoBack, bool canGoForward)
        {
            if (this.LoadingStateChange != null)
            {
                var e = new LoadingStateChangeEventArgs(isLoading, canGoBack, canGoForward);
                this.LoadingStateChange(this, e);
            }
        }
        internal void OnLoadError(CefFrame frame, CefErrorCode errorCode, string errorText, string failedUrl)
        {
            if (frame.IsMain)
            {
                loading = false;
                loaded.Set();
            }

            if (this.LoadError != null)
            {
                var e = new LoadErrorEventArgs(frame, errorCode, errorText, failedUrl);
                this.LoadError(this, e);
            }
        }

        #endregion

        #region Disposable

        ~AutoCefBrowser()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

                // TODO: What's the right way of disposing the browser instance?
                if (_browserHost != null)
                {
                    _browserHost.CloseBrowser();
                    _browserHost = null;
                }

                if (_browser != null)
                {
                    _browser.Dispose();
                    _browser = null;
                }
            }
        }

        #endregion

        public void DeleteCookies()
        {
            ThreadHelpers.PostTask(CefThreadId.IO, () =>
            {
                CefCookieManager.Global.DeleteCookies(null, null);
            });
        }

        #region Automation Helpers

        public object TryEvaluateScript(string script)
        {
            try
            {
                var result = this._cefClient.TryEval(script, _browser);

                return result;
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public AutoCefBrowser WaitForPageLoad(int timeout = 10000)
        {
            System.Threading.Thread.Sleep(100);

            loaded.Reset();
            if (!loaded.WaitOne(timeout))
            {
                throw new TimeoutException();
            }

            return this;
        }

        public AutoCefBrowser NavigateTo(string url)
        {
            url = url.TrimStart();

            if (_browser != null)
                _browser.GetMainFrame().LoadUrl(url);

            loading = true;
            return WaitForPageLoad();
        }

        public AutoCefBrowser SetValue(string cssSelector, string value)
        {
            TryEvaluateScript(string.Format("$('{0}').val('{1}').change(); void(0);", cssSelector, value));
            return this;
        }

        public string GetValue(string cssSelector)
        {
            var resultJSON = TryEvaluateScript(string.Format("$('{0}').val();", cssSelector));
            return (resultJSON ?? string.Empty).ToString();
        }

        public string GetText(string cssSelector)
        {
            var resultJSON = TryEvaluateScript(string.Format("$('{0}').text();", cssSelector));
            return (resultJSON ?? string.Empty).ToString();
        }

        public AutoCefBrowser Click(string cssSelector)
        {
            TryEvaluateScript(string.Format("$('{0}').trigger('click'); void(0);", cssSelector));
            TryEvaluateScript(string.Format("$('{0}')[0].click(); void(0);", cssSelector));

            return this;
        }

        #endregion

    }
}
