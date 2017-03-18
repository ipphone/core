using AudioLibrary.WMME.Native;

namespace AudioLibrary.WMME
{
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
