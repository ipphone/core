//
//  THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
//  KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
//  IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
//  PURPOSE. IT CAN BE DISTRIBUTED FREE OF CHARGE AS LONG AS THIS HEADER 
//  REMAINS UNCHANGED.
//
//  Email:  gustavo_franco@hotmail.com
//
//  Copyright (C) 2005 Franco, Gustavo 
//
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace WaveLib.AudioMixer
{
	[Author("Gustavo Franco")]
	public class CallbackWindow : NativeWindow, IDisposable
	{
		#region Constants Declaration
		private const int WS_CHILD = 0x40000000, WS_VISIBLE = 0x10000000, WM_ACTIVATEAPP = 0x001C;
		#endregion

		#region Variables Declaration
		private CallbackWindowControlChangeHandler	mPtrMixerControlChange;
		private CallbackWindowLineChangeHandler		mPtrMixerLineChange;
		#endregion

		#region Constructors
		internal CallbackWindow(CallbackWindowControlChangeHandler ptrMixerControlChange, CallbackWindowLineChangeHandler ptrMixerLineChange)
		{
			CreateParams cp = new CreateParams();

			mPtrMixerControlChange	= ptrMixerControlChange;
			mPtrMixerLineChange		= ptrMixerLineChange;

			this.CreateHandle(cp);
		}
		#endregion

		#region Overrides
		[DebuggerNonUserCode(), System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name="FullTrust")]
		protected override void WndProc(ref Message m)
		{
			switch (m.Msg)
			{
				case MixerNative.MM_MIXM_LINE_CHANGE:
					mPtrMixerLineChange(m.WParam, (uint) m.LParam);
					break;

				case MixerNative.MM_MIXM_CONTROL_CHANGE:
					mPtrMixerControlChange(m.WParam, (uint) m.LParam);
					break;
				default:
					break;

			}
			base.WndProc (ref m);
		}
		#endregion

		#region IDisposable Members
		public void Dispose()
		{
			if (this.Handle != IntPtr.Zero)
			{
				this.DestroyHandle();
			}
		}
		#endregion
	}
}
