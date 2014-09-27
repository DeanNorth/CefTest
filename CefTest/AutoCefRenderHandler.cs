using Microsoft.Win32.SafeHandles;
using System;
using System.Windows.Input;
using Xilium.CefGlue;

namespace CefTest
{
    internal sealed class AutoCefRenderHandler : CefRenderHandler
    {
        private readonly AutoCefBrowser _owner;

        private readonly int _windowHeight = 768;
        private readonly int _windowWidth = 1024;

        public AutoCefRenderHandler(AutoCefBrowser owner)
        {
            if (owner == null)
            {
                throw new ArgumentNullException("owner");
            }

            _owner = owner;

            _windowWidth = _owner.Width;
            _windowHeight = _owner.Height;
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return GetViewRect(browser, ref rect);
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            rect.X = 0;
            rect.Y = 0;
            rect.Width = _windowWidth;
            rect.Height = _windowHeight;
            return true;
        }

        protected override bool GetScreenPoint(CefBrowser browser, int viewX, int viewY, ref int screenX, ref int screenY)
        {
            screenX = viewX;
            screenY = viewY;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnPopupShow(CefBrowser browser, bool show)
        {

        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {

        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {

        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle)
        {

        }

        protected override void OnScrollOffsetChanged(CefBrowser browser)
        {
        }
    }
}
