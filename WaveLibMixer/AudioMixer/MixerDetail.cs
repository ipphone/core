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

namespace WaveLib.AudioMixer
{
	[Author("Gustavo Franco")]
	public class MixerDetail
	{
		#region Variables Declaration
		private string	mName;
		private int		mDeviceId;
		private bool	mSupportWaveIn;
		private bool	mSupportWaveOut;
		#endregion

		#region Constructors
		public MixerDetail()
		{
			mName			= "";
			mDeviceId		= -1;
			mSupportWaveIn	= false;
			mSupportWaveOut = false;
		}
		#endregion

		#region Properties
		public string MixerName
		{
			get{return mName;}
			set{mName = value;}
		}

		public int DeviceId
		{
			get{return mDeviceId;}
			set{mDeviceId = value;}
		}

		public bool SupportWaveIn
		{
			get{return mSupportWaveIn;}
			set{mSupportWaveIn = value;}
		}

		public bool SupportWaveOut
		{
			get{return mSupportWaveOut;}
			set{mSupportWaveOut = value;}
		}
		#endregion

		#region Overrides
		public override string ToString()
		{
			//return mName + ":" + mDeviceId;
			return mName;
		}
		#endregion
	}
}
