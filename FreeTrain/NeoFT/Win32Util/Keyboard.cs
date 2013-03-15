using System;
using System.Runtime.InteropServices;

namespace nft.win32util
{
	/// <summary>
	/// Keyboard-related utility functions
	/// </summary>
	public class Keyboard
	{
		[DllImport("User32.dll")]
		private static extern short GetAsyncKeyState( int vKey );

		// virtual key code (see Platform SDK)
		private const int VK_LEFT = 0x25;
		private const int VK_UP = 0x26;
		private const int VK_RIGHT = 0x27;
		private const int VK_DOWN = 0x28;
		private const int VK_SHIFT = 0x10;
		private const int VK_CONTROL = 0x11;

		public static bool isShiftKeyPressed {
			get {
				return GetAsyncKeyState(VK_SHIFT)<0;
			}
		}

		public static bool isControlKeyPressed {
			get {
				return GetAsyncKeyState(VK_CONTROL)<0;
			}
		}
	}
}
