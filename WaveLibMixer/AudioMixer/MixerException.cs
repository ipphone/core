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
	public class MixerException : System.Exception
	{
		#region Variables Declaration
		private readonly MMErrors	mErrorCode;
		#endregion

		#region Constructors
		public MixerException(MMErrors errorCode, string errorMessage) : base(errorMessage)
		{
			mErrorCode = errorCode;
		}
		#endregion

		#region Properties
		public MMErrors ErrorCode
		{
			get{return mErrorCode;}
		}
		#endregion
	}
}
