/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET
  (c)opyright RemObjects Software, LLC. 2003-2016. All rights reserved.
---------------------------------------------------------------------------*/

namespace RemObjects.InternetPack.Ftp
{
	public class FtpListingItem
	{
		#region Constructors
		public FtpListingItem()
		{
			this.Directory = false;
			this.UserRead = true;
			this.UserWrite = true;
			this.UserExec = true;
			this.GroupRead = true;
			this.GroupWrite = true;
			this.GroupExec = true;
			this.OtherRead = true;
			this.OtherWrite = true;
			this.OtherExec = true;
			this.SubItemCount = 1;
			this.User = "user";
			this.Group = "group";
			this.Size = 0;
			#if echoes
			// TODO
			this.FileDate = System.DateTime.MinValue;
			#endif
		}

		public FtpListingItem(String item)
			: this()
		{
			this.Parse(item);
		}
		#endregion

		#region Properties
		public Boolean Directory { get; set; }

		public Int32 ImageIndex
		{
			get
			{
				if (!this.Directory)
					return 2;

				if (this.FileName == "..")
					return 0;

				return 1;
			}
		}

		public Boolean UserRead { get; set; }

		public Boolean UserWrite { get; set; }

		public Boolean UserExec { get; set; }

		public Boolean GroupRead { get; set; }

		public Boolean GroupWrite { get; set; }

		public Boolean GroupExec { get; set; }

		public Boolean OtherRead { get; set; }

		public Boolean OtherWrite { get; set; }

		public Boolean OtherExec { get; set; }

		public Int32 SubItemCount { get; set; }

		public String User { get; set; }

		public String Group { get; set; }

		public Int64 Size { get; set; }

		public DateTime FileDate { get; set; }

		public String FileName { get; set; }
		#endregion

		public static String LeadZero(String value, Int32 length)
		{
			StringBuilder lResult = new StringBuilder();
			length -= value.Length;
			while (length > 0)
			{
				lResult.Append('0');
				length--;
			}
			lResult.Append(value);

			return lResult.ToString();
		}

		public static String FtpDateToString(DateTime date)
		{
			Boolean lShowYear = (DateTime.UtcNow - date).Days > 180;
			String lResult;

			switch (date.Month)
			{

				case 1:
					lResult = "Jan";
					break;

				case 2:
					lResult = "Feb";
					break;

				case 3:
					lResult = "Mar";
					break;

				case 4:
					lResult = "Apr";
					break;

				case 5:
					lResult = "May";
					break;

				case 6:
					lResult = "Jun";
					break;

				case 7:
					lResult = "Jul";
					break;

				case 8:
					lResult = "Aug";
					break;

				case 9:
					lResult = "Sep";
					break;

				case 10:
					lResult = "Oct";
					break;

				case 11:
					lResult = "Nov";
					break;

				case 12:
					lResult = "Dec";
					break;

				default:
					return "";
			}

			lResult += date.Day > 9 ? " " + date.Day : "  " + date.Day;

			if (lShowYear)
			{
				lResult += "  " + LeadZero(date.Year.ToString(), 4);
			}
			else
			{
				lResult += " ";
				if (date.Hour < 10) { lResult += "0"; }
				lResult += date.Hour.ToString();
				lResult += ":";
				if (date.Minute < 10) { lResult += "0"; }
				lResult += date.Minute.ToString();
			}

			return lResult;
		}

		[ToString]
		public override String ToString()
		{
			Char[] lRights = new Char[] { 'd', 'r', 'w', 'x', 'r', 'w', 'x', 'r', 'w', 'x' };

			if (!this.Directory)
				lRights[0] = '-';

			if (!this.UserRead)
				lRights[1] = '-';

			if (!this.UserWrite)
				lRights[2] = '-';

			if (!this.UserExec)
				lRights[3] = '-';

			if (!this.GroupRead)
				lRights[4] = '-';

			if (!this.GroupWrite)
				lRights[5] = '-';

			if (!this.GroupExec)
				lRights[6] = '-';

			if (!this.OtherRead)
				lRights[7] = '-';

			if (!this.OtherWrite)
				lRights[8] = '-';

			if (!this.OtherExec)
				lRights[9] = '-';

			return String.Format("{0} {1,3} {2,8} {3,8} {4,7} {5} {6}", new String(lRights), this.SubItemCount, this.User,
				this.Group, this.Size, FtpListingItem.FtpDateToString(this.FileDate), this.FileName);
		}

		private static String[] ParseLine(String item, int maxItems)
		{
			var lItems = item.Split(" ");
			var lResult = new List<String>();
			var lCount = 0;
			foreach (var lItem in lItems)
			{
				if (lItem != "")
				{
					lResult.Add(lItem);
					lCount++;
					if (lCount >= maxItems) break;
				}
			}

			return lResult.ToArray();
		}

		public void Parse(String item)
		{
			//Regex lRegEx = new Regex(@"\s+");

			// there is two modes possible Unix mode or MS-DOS mode
			if (item.StartsWith("d") || item.StartsWith("-"))
			{
				/*
				 Unix Mode
				======================================================================
				drwxr-xr-x    3 65025    100          4096 Dec 10 12:13 1 1
				drwxr-xr-x    2 65025    100          4096 Dec 10 12:13 2
				-rw-r--r--    1 65025    100            35 Dec 10 12:33 root.txt
				-rw-r--r--    1 65025    100            43 Dec 10 12:33 root2.txt


				where
				0 - access
				1 - sub item count
				2 - owner
				3 - group
				4 - size
				5 - Month
				6 - day
				7 - Time or Year
				8 - Filename
				 */
				//String[] lSplittedData = lRegEx.Split(item, 9);
				String[] lSplittedData = ParseLine(item, 9);

				// Copy splitted data to result
				// Problem is that at least one FTP server doesn;t return Group segment
				// So we have to compensate this
				String[] lSegments = new String[9];
				for (Int32 i = 0; i < 3; i++)
					lSegments[i] = lSplittedData[i];
				for (Int32 i = 1; i <= 6; i++)
					lSegments[9 - i] = lSplittedData[lSplittedData.Length - i];

				this.Directory = lSegments[0][0] != '-';
				this.UserRead = lSegments[0][1] != '-';
				this.UserWrite = lSegments[0][2] != '-';
				this.UserExec = lSegments[0][3] != '-';
				this.GroupRead = lSegments[0][4] != '-';
				this.GroupWrite = lSegments[0][5] != '-';
				this.GroupExec = lSegments[0][6] != '-';
				this.OtherRead = lSegments[0][7] != '-';
				this.OtherWrite = lSegments[0][8] != '-';
				this.OtherExec = lSegments[0][9] != '-';

				this.SubItemCount = Convert.ToInt32(lSegments[1]);
				this.User = lSegments[2];
				this.Group = lSegments[3];
				this.Size = Convert.ToInt64(lSegments[4]);

				String lMonthShortName = lSegments[5];
				String lDay = lSegments[6];
				String lTimeOrYear = lSegments[7];

				this.FileDate = FtpListingItem.StringToFtpDate(lMonthShortName, lDay, lTimeOrYear);

				this.FileName = lSegments[8];
			}
			else
			{
				/*
				 MS-DOS Mode
				======================================================================
				01-14-08  01:35PM       <DIR>          1 1
				01-14-08  01:35PM       <DIR>          2
				01-14-08  01:36PM                   35 root.txt
				01-14-08  01:36PM                   43 root2.txt

				where

				0 - date
				1 - time
				2 - Size or IsDir
				3 - Filename
				 */
				//String[] lSegments = lRegEx.Split(item, 4);
				String[] lSegments = ParseLine(item, 4);
				this.Directory = (lSegments[2] == "<DIR>");

				this.Size = this.Directory ? 0 : Convert.ToInt64(lSegments[2]);

				String lDateStr = lSegments[0];
				String lTimeStr = lSegments[1];
				this.FileDate = FtpListingItem.StringToFtpDate(lDateStr, lTimeStr);

				this.FileName = lSegments[3];
			}
		}

		public static DateTime StringToFtpDate(String value)
		{
			//String[] lParts = Regex.Split(value, @"\w+");
			String[] lParts = ParseLine(value, 3);

			return FtpListingItem.StringToFtpDate(lParts[0], lParts[1], lParts[2]);
		}

		public static DateTime StringToFtpDate(String dateString, String timeString)
		{
			Int32 lMonth = 1;
			Int32 lDay = 1;
			Int32 lYear = 1;
			Int32 lHour = 0;
			Int32 lMinutes = 0;

			//01-14-08  01:35PM
			try
			{
				#region Parsing Date (Should be MM-DD-YY)
				var lDate = dateString.Split("-");
				if (lDate.Count == 3)
				{
					lMonth = Convert.ToInt32(lDate[0]);
					lDay = Convert.ToInt32(lDate[1]);
					lYear = Convert.ToInt32(lDate[2]);

					if (lDate[2].Length == 2)
					{
						Int32 lCentury = (DateTime.UtcNow.Year / 100) * 100;
						if ((lYear + 50) > 100)
							lCentury -= 100;
						lYear = lCentury + lYear;
					}
				}
				#endregion

				#region Parsing Time (Should be HH:MMAM/PM)
				var lTime = timeString.Split(":").MutableVersion();
				if (lTime.Count == 2)
				{
					lHour = Convert.ToInt32(lTime[0]);
					if (lTime[1].Length == 4)
					{
						Boolean lPM = lTime[1].Substring(2) == "PM";
						if (lPM && lHour < 12)
							lHour += 12;
						if (!lPM && lHour == 12)
							lHour = 0;
						lTime[1] = lTime[1].Substring(0, 2);
					}
					lMinutes = Convert.ToInt32(lTime[1]);
				}
				#endregion
			}
			catch (Exception) // don't need any exception here.
			{
				return new DateTime(0);
			}

			return new DateTime(lYear, lMonth, lDay, lHour, lMinutes, 0);
		}

		public static DateTime StringToFtpDate(String monthName, String day, String yearOrTime)
		{
			Int32 lYear;
			Int32 lMonth;
			Int32 lDay;
			Int32 lHour = 0;
			Int32 lMinute = 0;
			Int32 lSecond = 0;

			try
			{
				switch (monthName)
				{
					case "Jan":
						lMonth = 1;
						break;

					case "Feb":
						lMonth = 2;
						break;

					case "Mar":
						lMonth = 3;
						break;

					case "Apr":
						lMonth = 4;
						break;

					case "May":
						lMonth = 5;
						break;

					case "Jun":
						lMonth = 6;
						break;

					case "Jul":
						lMonth = 7;
						break;

					case "Aug":
						lMonth = 8;
						break;

					case "Sep":
						lMonth = 9;
						break;

					case "Oct":
						lMonth = 10;
						break;

					case "Nov":
						lMonth = 11;
						break;

					case "Dec":
						lMonth = 12;
						break;

					default:
						return new DateTime(0);
				}

				lDay = Convert.ToInt32(day);
				lYear = DateTime.UtcNow.Year;

				if (yearOrTime.IndexOf(":") == -1) // this is a year, not a time
				{
					lYear = Convert.ToInt32(yearOrTime);
				}
				else
				{
					// no year, either this year or last
					Int32 lCurrentMonth = DateTime.UtcNow.Month;
					if (lCurrentMonth < lMonth)
						lYear -= 1;

					//String[] lTimes = Regex.Split(yearOrTime, ":");
					String[] lTimes = yearOrTime.Split(":").ToArray();
					lHour = Convert.ToInt32(lTimes[0]);
					lMinute = Convert.ToInt32(lTimes[1]);
					if (lTimes.Length > 2)
						lSecond = Convert.ToInt32(lTimes[2]);
				}
			}
			catch (Exception) // don't need any exception here.
			{
				return new DateTime(0);
			}

			return new DateTime(lYear, lMonth, lDay, lHour, lMinute, lSecond);//, 999);
		}
	}

	public class FtpListing : ArrayList
	{
		public new FtpListingItem this[Int32 index]
		{
			get
			{
				return (FtpListingItem)base[index];
			}
		}

		public FtpListingItem Add()
		{
			FtpListingItem lResult = new FtpListingItem();
			this.Add(lResult);

			return lResult;
		}

		private static String[] SplitLines(String line)
		{
			var lPointer = 0;
			var lLast = 0;
			var lResult = new List<String>();

			while (lPointer < line.Length)
			{
				while ((lPointer < line.Length) && (line[lPointer] != '\n') && (line[lPointer] != '\r'))
				{
					lPointer++;
				}
				lResult.Add(line.Substring(lLast, (lPointer - lLast)));
				if ((lPointer < line.Length) && (line[lPointer] == '\r'))
					lPointer++;
				if ((lPointer < line.Length) && (line[lPointer] == '\n'))
					lPointer++;
				lLast = lPointer;
			}

			return lResult.ToArray();
		}

		public void Parse(String list, Boolean includeUpDir)
		{
			this.Clear();

			Boolean lFoundUpDir = false;

			//String[] lItems = Regex.Split(list, @"(?:\r\n|\r|\n)");
			String[] lItems = SplitLines(list);

			for (Int32 i = 0; i < lItems.Length; i++)
			{
				String lItem = lItems[i].Trim();

				if (String.IsNullOrEmpty(lItem))
					continue;

				if (lItem.ToLower().StartsWith("total"))
					continue;

                try
				{
					FtpListingItem lNewItem = new FtpListingItem();
					lNewItem.Parse(lItem);

					this.Add(lNewItem);

					if (lNewItem.Directory && lNewItem.FileName == "..")
						lFoundUpDir = true;
				}
                catch(Exception ex)
                {
                    if (ex.GetType() != typeof(FormatException))
                    {
                        if (defined("ECHOES") && (ex.GetType() != typeof(IndexOutOfRangeException)))
                            throw ex;
                        else if (!defined("ECHOES") && (ex.GetType() != typeof(RTLException)))
                            throw ex;
                    }
                }
			}

			if (includeUpDir && !lFoundUpDir)
			{
				FtpListingItem lUpItem = new FtpListingItem();
				lUpItem.Directory = true;
				lUpItem.FileName = "..";
				this.Insert(0, lUpItem);
			}
		}

		  #if !echoes
		public ISequence<FtpListingItem> GetSequence()
		{
			foreach(FtpListingItem lItem in base.fList)
			{
				yield return lItem;
			}
		}
		#endif

		[ToString]
		public override String ToString()
		{
			StringBuilder lResult = new StringBuilder();

			for (Int32 i = 0; i < this.Count; i++)
			{
				lResult.Append(this.ToString());
				lResult.Append("\r\n");
			}

			return lResult.ToString();
		}
	}
}