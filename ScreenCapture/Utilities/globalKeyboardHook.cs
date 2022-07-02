using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Utilities {
	/// <summary>
	/// A class that manages a global low level keyboard hook
	/// </summary>
	class globalKeyboardHook {
		#region Constant, Structure and Delegate Definitions
		/// <summary>
		/// defines the callback type for the hook
		/// </summary>
		public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

		public delegate bool HookedKeyEventHandler(object sender, Key key);

		public struct keyboardHookStruct {
			public int vkCode;
			public int scanCode;
			public int flags;
			public int time;
			public int dwExtraInfo;
		}

		const int WH_KEYBOARD_LL = 13;
		const int WM_KEYDOWN = 0x100;
		const int WM_KEYUP = 0x101;
		const int WM_SYSKEYDOWN = 0x104;
		const int WM_SYSKEYUP = 0x105;
		#endregion

		#region Instance Variables
		/// <summary>
		/// The collections of virtual keys to watch for
		/// </summary>
		private List<int> HookedKeys = new List<int>();
		/// <summary>
		/// Handle to the hook, need this to unhook and call the next hook
		/// </summary>
		IntPtr hhook = IntPtr.Zero;

		/// <summary>
		/// Handle to hook proc, keep it alive until the hook uninstalled to make sure other process can invoke the proc
		/// </summary>
		private keyboardHookProc hHookProc;

		private static globalKeyboardHook Instance;
		#endregion

		#region Events
		/// <summary>
		/// Occurs when one of the hooked keys is pressed
		/// </summary>
		public event HookedKeyEventHandler KeyDown;
		/// <summary>
		/// Occurs when one of the hooked keys is released
		/// </summary>
		public event HookedKeyEventHandler KeyUp;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="globalKeyboardHook"/> class and installs the keyboard hook.
		/// </summary>
		public globalKeyboardHook() {
			Instance = this;
			hHookProc = hookProc;
			//hook();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="globalKeyboardHook"/> is reclaimed by garbage collection and uninstalls the keyboard hook.
		/// </summary>
		~globalKeyboardHook() {
			//unhook();
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Installs the global hook
		/// </summary>
		public void hook() {
			IntPtr hInstance = LoadLibrary("User32");
			hhook = SetWindowsHookEx(WH_KEYBOARD_LL, hHookProc, hInstance, 0);
		}

		/// <summary>
		/// Uninstalls the global hook
		/// </summary>
		public void unhook() {
			UnhookWindowsHookEx(hhook);
		}

		public void addKeyToWatch(Key key)
        {
			HookedKeys.Add(KeyInterop.VirtualKeyFromKey(key));
        }

		/// <summary>
		/// The callback for the keyboard hook
		/// </summary>
		/// <param name="code">The hook code, if it isn't >= 0, the function shouldn't do anyting</param>
		/// <param name="wParam">The event type</param>
		/// <param name="lParam">The keyhook event information</param>
		/// <returns></returns>
		static public int hookProc(int code, int wParam, ref keyboardHookStruct lParam) {
			if (code >= 0) {
				if (Instance.HookedKeys.Contains(lParam.vkCode)) {
					Key key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
					bool handled = false;
					//KeyEventArgs kea = new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource, 0, key);
					if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (Instance.KeyDown != null)) {
						handled = Instance.KeyDown(Keyboard.PrimaryDevice, key) ;
					} else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (Instance.KeyUp != null)) {
						handled = Instance.KeyUp(Keyboard.PrimaryDevice, key);
					}
					if (handled)
						return 1;
				}
			}
			return CallNextHookEx(Instance.hhook, code, wParam, ref lParam);
		}
		#endregion

		#region DLL imports
		/// <summary>
		/// Sets the windows hook, do the desired event, one of hInstance or threadId must be non-null
		/// </summary>
		/// <param name="idHook">The id of the event you want to hook</param>
		/// <param name="callback">The callback.</param>
		/// <param name="hInstance">The handle you want to attach the event to, can be null</param>
		/// <param name="threadId">The thread you want to attach the event to, can be null</param>
		/// <returns>a handle to the desired hook</returns>
		[DllImport("user32.dll")]
		static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

		/// <summary>
		/// Unhooks the windows hook.
		/// </summary>
		/// <param name="hInstance">The hook handle that was returned from SetWindowsHookEx</param>
		/// <returns>True if successful, false otherwise</returns>
		[DllImport("user32.dll")]
		static extern bool UnhookWindowsHookEx(IntPtr hInstance);

		/// <summary>
		/// Calls the next hook.
		/// </summary>
		/// <param name="idHook">The hook id</param>
		/// <param name="nCode">The hook code</param>
		/// <param name="wParam">The wparam.</param>
		/// <param name="lParam">The lparam.</param>
		/// <returns></returns>
		[DllImport("user32.dll")]
		static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

		/// <summary>
		/// Loads the library.
		/// </summary>
		/// <param name="lpFileName">Name of the library</param>
		/// <returns>A handle to the library</returns>
		[DllImport("kernel32.dll")]
		static extern IntPtr LoadLibrary(string lpFileName);
		#endregion
	}
}
