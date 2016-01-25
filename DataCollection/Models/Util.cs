using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DataCollection.Models
{
    public class Util
    {
        public string convDateIntToStr(int? dateNum)
        {
            string Datestr = dateNum.ToString();
            return Datestr.Substring(6, 2) + "/" + Datestr.Substring(4, 2) + "/" + Datestr.Substring(0, 4);
        }

        public string convDateIntToStr(string dateNum)
        {
            if (!string.IsNullOrEmpty(dateNum.Trim()))
                return dateNum.Substring(6, 2) + "/" + dateNum.Substring(4, 2) + "/" + dateNum.Substring(0, 4);
            else
                return string.Empty;
        }

        public int convDateStrToInt(string strDate, string time = "")
        {
            var dateArr = strDate.Split('/');
            return int.Parse(dateArr[2] + dateArr[1] + dateArr[0] + time.Trim());
        }

        public string convDateStrToDBFormatStr(string strDate)
        {
            var split = strDate.Split(' ');
            var dateArr = split[0].Split('/');
            var time = split.Length > 1 ? split[1].Replace(":", "") : string.Empty;
            return dateArr[2] + dateArr[1] + dateArr[0] + time;
        }

        public string convTimeStrFormat(string strTime)
        {
            if (!string.IsNullOrEmpty(strTime.Trim()))
                return strTime.Substring(0, 2) + ":" + strTime.Substring(2, 2) + ":" + strTime.Substring(4, 2);
            else
                return string.Empty;
        }

        public string convDateTimeStrToDBFormatStr(string strDate)
        {
            var datetimeArr = strDate.Split(' ');
            return this.convDateStrToDBFormatStr(datetimeArr[0]) + this.convTimeSQLFormat(datetimeArr[1]);
        }

        public string convUserNameFormat(string uname)
        {
            if (!string.IsNullOrEmpty(uname.Trim()))
            {
                var uarr = uname.Trim().Split(' ');
                if (uarr.Length > 1)
                    return uarr[0] + " " + (uarr[1].Trim().Length > 1 ? uarr[1].Substring(0, 1) : uarr[1]) + ".";
                else
                    return uarr[0];
            }
            else
                return string.Empty;
        }

        public string convTimeSQLFormat(string inputTime)
        {
            return inputTime.Replace(":", "") + "00";
        }

        public string convInputTimeDispFormat(string inputTime)
        {
            return inputTime  + ":00";
        }
    }
}