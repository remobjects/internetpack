/*---------------------------------------------------------------------------
  RemObjects Internet Pack for .NET - Core Library
  (c)opyright RemObjects Software, LLC. 2003-2012. All rights reserved.

  Using this code requires a valid license of the RemObjects Internet Pack
  which can be obtained at http://www.remobjects.com?ip.
---------------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

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
            this.FileDate = DateTime.MinValue;
        }

        public FtpListingItem(String item)
            : this()
        {
            this.Parse(item);
        }
        #endregion

        #region Properties
        public Boolean Directory
        {
            get
            {
                return this.fDirectory;
            }
            set
            {
                this.fDirectory = value;
            }
        }
        private Boolean fDirectory;

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

        public Boolean UserRead
        {
            get
            {
                return fUserRead;
            }
            set
            {
                fUserRead = value;
            }
        }
        private Boolean fUserRead;

        public Boolean UserWrite
        {
            get
            {
                return fUserWrite;
            }
            set
            {
                fUserWrite = value;
            }
        }
        private Boolean fUserWrite;

        public Boolean UserExec
        {
            get
            {
                return fUserExec;
            }
            set
            {
                fUserExec = value;
            }
        }
        private Boolean fUserExec;

        public Boolean GroupRead
        {
            get
            {
                return fGroupRead;
            }
            set
            {
                fGroupRead = value;
            }
        }
        private Boolean fGroupRead;

        public Boolean GroupWrite
        {
            get
            {
                return fGroupWrite;
            }
            set
            {
                fGroupWrite = value;
            }
        }
        private Boolean fGroupWrite;

        public Boolean GroupExec
        {
            get
            {
                return fGroupExec;
            }
            set
            {
                fGroupExec = value;
            }
        }
        private Boolean fGroupExec;

        public Boolean OtherRead
        {
            get
            {
                return fOtherRead;
            }
            set
            {
                fOtherRead = value;
            }
        }
        private Boolean fOtherRead;

        public Boolean OtherWrite
        {
            get
            {
                return fOtherWrite;
            }
            set
            {
                fOtherWrite = value;
            }
        }
        private Boolean fOtherWrite;

        public Boolean OtherExec
        {
            get
            {
                return fOtherExec;
            }
            set
            {
                fOtherExec = value;
            }
        }
        private Boolean fOtherExec;

        public Int32 SubItemCount
        {
            get
            {
                return fSubItemCount;
            }
            set
            {
                fSubItemCount = value;
            }
        }
        private Int32 fSubItemCount;

        public String User
        {
            get
            {
                return fUser;
            }
            set
            {
                fUser = value;
            }
        }
        private String fUser;

        public String Group
        {
            get
            {
                return fGroup;
            }
            set
            {
                fGroup = value;
            }
        }
        private String fGroup;

        private Int64 fSize = 0;
        public Int64 Size
        {
            get
            {
                return fSize;
            }
            set
            {
                fSize = value;
            }
        }

        public DateTime FileDate
        {
            get
            {
                return fFileDate;
            }
            set
            {
                fFileDate = value;
            }
        }
        private DateTime fFileDate;

        public String FileName
        {
            get
            {
                return fFileName;
            }
            set
            {
                fFileName = value;
            }
        }
        private String fFileName;
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
            Boolean lShowYear = (DateTime.Now - date).Days > 180;
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

            if (date.Day > 9)
                lResult += " " + date.Day;
            else
                lResult += "  " + date.Day;

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
                this.Group, this.fSize, FtpListingItem.FtpDateToString(this.FileDate), this.FileName);
        }

        public void Parse(String item)
        {
            Regex lRegEx = new Regex(@"\s+");

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
                String[] lSplittedData = lRegEx.Split(item, 9);

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

                this.SubItemCount = Int32.Parse(lSegments[1]);
                this.User = lSegments[2];
                this.Group = lSegments[3];
                this.Size = Int64.Parse(lSegments[4]);

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
                String[] lSegments = lRegEx.Split(item, 4);
                this.Directory = (lSegments[2] == "<DIR>");

                this.Size = this.Directory ? 0 : Int64.Parse(lSegments[2]);

                String lDateStr = lSegments[0];
                String lTimeStr = lSegments[1];
                this.FileDate = FtpListingItem.StringToFtpDate(lDateStr, lTimeStr);

                this.FileName = lSegments[3];
            }
        }

        public static DateTime StringToFtpDate(String value)
        {
            String[] lParts = Regex.Split(value, @"\w+");

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
                String[] lDate = dateString.Split(new Char[] { '-' });
                if (lDate.Length == 3)
                {
                    lMonth = Convert.ToInt32(lDate[0]);
                    lDay = Convert.ToInt32(lDate[1]);
                    lYear = Convert.ToInt32(lDate[2]);

                    if (lDate[2].Length == 2)
                    {
                        Int32 lCentury = (DateTime.Now.Year / 100) * 100;
                        if ((lYear + 50) > 100)
                            lCentury -= 100;
                        lYear = lCentury + lYear;
                    }
                }
                #endregion

                #region Parsing Time (Should be HH:MMAM/PM)
                String[] lTime = timeString.Split(new Char[] { ':' });
                if (lTime.Length == 2)
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

                lDay = Int32.Parse(day);
                lYear = DateTime.Now.Year;

                if (yearOrTime.IndexOf(":") == -1) // this is a year, not a time
                {
                    lYear = Int32.Parse(yearOrTime);
                }
                else
                {
                    // no year, either this year or last
                    Int32 lCurrentMonth = DateTime.Now.Month;
                    if (lCurrentMonth < lMonth)
                        lYear -= 1;

                    String[] lTimes = Regex.Split(yearOrTime, ":");
                    lHour = Int32.Parse(lTimes[0]);
                    lMinute = Int32.Parse(lTimes[1]);
                    if (lTimes.Length > 2)
                        lSecond = Int32.Parse(lTimes[2]);
                }
            }
            catch (Exception) // don't need any exception here.
            {
                return new DateTime(0);
            }

            return new DateTime(lYear, lMonth, lDay, lHour, lMinute, lSecond, 999);
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

        public void Parse(String list, Boolean includeUpDir)
        {
            this.Clear();

            Boolean lFoundUpDir = false;

            String[] lItems = Regex.Split(list, @"(?:\r\n|\r|\n)");

            for (Int32 i = 0; i < lItems.Length; i++)
            {
                String lItem = lItems[i].Trim();

                if (String.IsNullOrEmpty(lItem))
                    continue;

                if (lItem.StartsWith("total", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    FtpListingItem lNewItem = new FtpListingItem();
                    lNewItem.Parse(lItem);

                    this.Add(lNewItem);

                    if (lNewItem.Directory && lNewItem.FileName == "..")
                        lFoundUpDir = true;
                }
                catch (IndexOutOfRangeException)
                {
                }
                catch (FormatException)
                {
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
