using System;

namespace Tizen.Application {
    class Window : IDisposable {
        private IntPtr _native_window = IntPtr.Zero;

        public bool Visible {
            get {
                return _native_window != IntPtr.Zero ? Interop.Window.evas_object_visible_get(_native_window) : false;
            }
        }
        public Window() {
            _native_window = Interop.Window.elm_win_add(IntPtr.Zero, "Window", 0);
        }
        ~Window() {
            Dispose();
        }
        public void Show() {
            Interop.Window.evas_object_show(_native_window);
        }
        public void Hide() {
            Interop.Window.evas_object_hide(_native_window);
        }
        public void Active() {
            Interop.Window.elm_win_activate(_native_window);
        }
        public void InActive() {
            Interop.Window.elm_win_lower(_native_window);
        }

        public void Dispose() {
            if (_native_window != IntPtr.Zero) {
                Interop.Window.evas_object_unref(_native_window);
                _native_window = IntPtr.Zero;
            }
        }
    }
}
