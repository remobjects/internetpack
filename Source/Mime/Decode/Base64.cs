// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//

namespace RemObjects.InternetPack.Messages.Mime.Decode
{
	/// <summary>
	/// Utility class for dealing with Base64 encoded strings
	/// </summary>
	static class Base64
	{
		/// <summary>
		/// Decodes a base64 encoded String into the bytes it describes
		/// </summary>
		/// <param name="base64Encoded">The String to decode</param>
		/// <returns>A Byte array that the base64 String described</returns>
		public static Byte[] Decode(String base64Encoded)
		{
			return Convert.Base64StringToByteArray(base64Encoded);
		}

		/// <summary>
		/// Decodes a Base64 encoded String using a specified <see cref="System.Text.Encoding"/>
		/// </summary>
		/// <param name="base64Encoded">Source String to decode</param>
		/// <param name="encoding">The encoding to use for the decoded Byte array that <paramref name="base64Encoded"/> describes</param>
		/// <returns>A decoded String</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="base64Encoded"/> or <paramref name="encoding"/> is <see langword="null"/></exception>
		/// <exception cref="FormatException">If <paramref name="base64Encoded"/> is not a valid base64 encoded String</exception>
		public static String Decode(String base64Encoded, Encoding encoding)
		{
			if (base64Encoded == null)
				throw new ArgumentNullException("base64Encoded");

			if (encoding == null)
				throw new ArgumentNullException("encoding");

			return encoding.GetString(Decode(base64Encoded));
		}
	}
}