// Public Domain OpenPOP.NET < http://hpop.sourceforge.net > library MIME decoder code portions
//
// Author of OpenPOP.NET library is Kasper Foens ( http://foens.users.sourceforge.net )
// Full copy of OpenPOP.NET can be obtained from http://hpop.sourceforge.net
//
#if toffee
using RemObjects.Elements.RTL;
#endif

namespace RemObjects.InternetPack.Messages.Mime.Decode
{
	/// <summary>
	/// Class used to decode RFC 2822 Date header fields.
	/// </summary>
	static class Rfc2822DateTime
	{
		/// <summary>
		/// Converts a String in RFC 2822 format into a <see cref="DateTime"/> object
		/// </summary>
		/// <param name="inputDate">The date to convert</param>
		/// <returns>A valid <see cref="DateTime"/> object, which represents the same time as the String that was converted</returns>
		/// <exception cref="ArgumentNullException"><exception cref="ArgumentNullException">If <paramref name="inputDate"/> is <see langword="null"/></exception></exception>
		/// <exception cref="ArgumentException">If the <paramref name="inputDate"/> could not be parsed into a <see cref="DateTime"/> object</exception>
		public static DateTime StringToDate(String inputDate)
		{
			if (inputDate == null)
				throw new ArgumentNullException("inputDate");

			// Old date specification allows comments and a lot of whitespace
			inputDate = StripCommentsAndExcessWhitespace(inputDate);

			try
			{
				// Extract the date
				String[] date = ExtractDate(inputDate);

				// Convert the date String into a DateTime
				//DateTime dateTime = System.Convert.ToDateTime(date, CultureInfo.InvariantCulture);
                DateTime dateTime = ToDateTime(date);

				// Convert the date into UTC
				//dateTime = new System.DateTime(dateTime.Ticks, DateTimeKind.Utc);
                var lOffset = TimeZone.Local.OffsetToUTC;
                dateTime = dateTime + lOffset;

				// Adjust according to the time zone
				dateTime = AdjustTimezone(dateTime, inputDate);

				// Return the parsed date
				return dateTime;
			}
			catch (ArgumentException e)
			{
				throw new ArgumentException("Could not parse date: " + e.Message + ". Input was: \"" + inputDate + "\"", e);
			}
		}

		/// <summary>
		/// Adjust the <paramref name="dateTime"/> object given according to the timezone specified in the <paramref name="dateInput"/>.
		/// </summary>
		/// <param name="dateTime">The date to alter</param>
		/// <param name="dateInput">The input date, in which the timezone can be found</param>
		/// <returns>An date altered according to the timezone</returns>
		/// <exception cref="ArgumentException">If no timezone was found in <paramref name="dateInput"/></exception>
		private static DateTime AdjustTimezone(DateTime dateTime, String dateInput)
		{
			// We know that the timezones are always in the last part of the date input
			var parts = dateInput.Split(" ");
			String lastPart = parts[parts.Count - 1];

			// Convert timezones in older formats to [+-]dddd format.
			//lastPart = Regex.Replace(lastPart, @"UT|GMT|EST|EDT|CST|CDT|MST|MDT|PST|PDT|[A-I]|[K-Y]|Z", MatchEvaluator);
            lastPart = ReplaceOldTimezone(lastPart);

			// Find the timezone specification
			// Example: Fri, 21 Nov 1997 09:55:06 -0600
			// finds -0600
			//Match match = Regex.Match(lastPart, @"[\+-](?<hours>\d\d)(?<minutes>\d\d)");
			//if (match.Success)
            if (length(lastPart) == 5)
			{
				// We have found that the timezone is in +dddd or -dddd format
				// Add the number of hours and minutes to our found date
				//Int32 hours = Int32.Parse(match.Groups["hours"].Value);
				//Int32 minutes = Int32.Parse(match.Groups["minutes"].Value);
                Int32 hours = Convert.ToInt32(lastPart.Substring(1, 2));
                Int32 minutes = Convert.ToInt32(lastPart.Substring(3, 2));

				//Int32 factor = match.Value[0] == '+' ? -1 : 1;
                Int32 factor = lastPart[0] == '+' ? -1 : 1;

				dateTime = dateTime.AddHours(factor * hours);
				dateTime = dateTime.AddMinutes(factor * minutes);

				return dateTime;
			}

			// A timezone of -0000 is the same as doing nothing
			return dateTime;
		}

		/// <summary>
		/// Convert timezones in older formats to [+-]dddd format.
		/// </summary>
		/// <param name="match">The match that was found</param>
		/// <returns>The String to replace the matched String with</returns>
		private static String ReplaceOldTimezone(String timeZone)
		{
			switch (timeZone)
			{
				// "A" through "I"
				// are equivalent to "+0100" through "+0900" respectively
				case "A": return "+0100";
				case "B": return "+0200";
				case "C": return "+0300";
				case "D": return "+0400";
				case "E": return "+0500";
				case "F": return "+0600";
				case "G": return "+0700";
				case "H": return "+0800";
				case "I": return "+0900";

				// "K", "L", and "M"
				// are equivalent to "+1000", "+1100", and "+1200" respectively
				case "K": return "+1000";
				case "L": return "+1100";
				case "M": return "+1200";

				// "N" through "Y"
				// are equivalent to "-0100" through "-1200" respectively
				case "N": return "-0100";
				case "O": return "-0200";
				case "P": return "-0300";
				case "Q": return "-0400";
				case "R": return "-0500";
				case "S": return "-0600";
				case "T": return "-0700";
				case "U": return "-0800";
				case "V": return "-0900";
				case "W": return "-1000";
				case "X": return "-1100";
				case "Y": return "-1200";

				// "Z", "UT" and "GMT"
				// is equivalent to "+0000"
				case "Z":
				case "UT":
				case "GMT":
					return "+0000";

				// US time zones
				case "EDT": return "-0400"; // EDT is semantically equivalent to -0400
				case "EST": return "-0500"; // EST is semantically equivalent to -0500
				case "CDT": return "-0500"; // CDT is semantically equivalent to -0500
				case "CST": return "-0600"; // CST is semantically equivalent to -0600
				case "MDT": return "-0600"; // MDT is semantically equivalent to -0600
				case "MST": return "-0700"; // MST is semantically equivalent to -0700
				case "PDT": return "-0700"; // PDT is semantically equivalent to -0700
				case "PST": return "-0800"; // PST is semantically equivalent to -0800

				default:
					return timeZone;
			}
		}

		/// <summary>
		/// Extracts the date part from the <paramref name="dateInput"/>
		/// </summary>
		/// <param name="dateInput">The date input String, from which to extract the date part</param>
		/// <returns>The extracted date part</returns>
		/// <exception cref="ArgumentException">If a date part could not be extracted from <paramref name="dateInput"/></exception>
		private static String[] ExtractDate(String dateInput)
		{
			// Matches the date and time part of a String
			// Example: Fri, 21 Nov 1997 09:55:06 -0600
			// Finds: 21 Nov 1997 09:55:06
			// Seconds does not need to be specified
			// Even though it is illigal, sometimes hours, minutes or seconds are only specified with one digit
			/*Match match = Regex.Match(dateInput, @"\d\d? .+ (\d\d\d\d|\d\d) \d?\d:\d?\d(:\d?\d)?");
			if (match.Success)
			{
				return match.Value;
			}*/
            var lParts = dateInput.Split(" ");
            if (lParts.Count >= 4)
            {
                String[] lReturn = new String[4];
                for (var i = 0; i < 4; i++)
                    lReturn[i] = lParts[i + 1];
                return lReturn;
            }

			throw new ArgumentException("No date part found");
		}

		/// <summary>
		/// Strips and removes all comments and excessive whitespace from the String
		/// </summary>
		/// <param name="input">The input to strip from</param>
		/// <returns>The stripped String</returns>
		private static String StripCommentsAndExcessWhitespace(String input)
		{
			// Strip out comments
			// Also strips out nested comments
			//input = Regex.Replace(input, @"(\((?>\((?<C>)|\)(?<-C>)|.?)*(?(C)(?!))\))", "");
            var lInput = new RemObjects.Elements.RTL.StringBuilder();
            var lNoCopy = 0;
            for (int i = 0; i < length(input); i++)
            {
                var lChar = input[i];
                if (lChar == '(')
                    lNoCopy++;
                else
                    if (lChar == ')')
                        lNoCopy--;
                if ((lNoCopy == 0) && (lChar != ')'))
                    lInput.Append(lChar);
            }
            RemObjects.Elements.RTL.String lToReturn;
            lToReturn = lInput.ToString();

			// Reduce any whitespace character to one space only
			//input = Regex.Replace(input, @"\s+", " ");
            lInput.Clear();
            lNoCopy = 0;
            for (int i = 0; i < length(lToReturn); i++)
            {
                var lChar = lToReturn[i];
                if (lChar == " ")
                    lNoCopy++;
                else
                    lNoCopy = 0;
                
                if (lNoCopy <= 1)
                    lInput.Append(lChar);             
            }
            lToReturn = lInput.ToString();

			// Remove all initial whitespace
			//input = Regex.Replace(input, @"^\s+", "");

			// Remove all ending whitespace
			//input = Regex.Replace(input, @"\s+$", "");
            lToReturn = lToReturn.Trim();

			// Remove spaces at colons
			// Example: 22: 33 : 44 => 22:33:44
			//input = Regex.Replace(input, @" ?: ?", ":");
            lToReturn = lToReturn.Replace(" : ", ":");
            lToReturn = lToReturn.Replace(" :", ":");
            lToReturn = lToReturn.Replace(": ", ":");

			return lToReturn;
		}

        private static DateTime ToDateTime(String[] inputDate)
        {
            // We now we have a date like: 21 Nov 1997 09:55:06, from ExtractDatetime func
            // [0] -> day  [1] -> month  [2] -> year  [3] -> time			
            var lMonth = 1;            
            switch (inputDate[1].ToLower())
			{
				case "jan": lMonth = 1; break;
				case "feb": lMonth = 2; break;
				case "mar": lMonth = 3; break;
				case "apr": lMonth = 4; break;
				case "may": lMonth = 5; break;
				case "jun": lMonth = 6; break;
				case "jul": lMonth = 7; break;
				case "aug": lMonth = 8; break;
				case "sep": lMonth = 9; break;
				case "oct": lMonth = 10; break;
				case "nov": lMonth = 11; break;
				case "dec": lMonth = 12; break;
            }

            var lTime = inputDate[3].Split(':');
            return new DateTime(Convert.ToInt32(inputDate[2]), lMonth, Convert.ToInt32(inputDate[0]),  
                Convert.ToInt32(lTime[0]), Convert.ToInt32(lTime[1]), Convert.ToInt32(lTime[2]));
        }
	}
}