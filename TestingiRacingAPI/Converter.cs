using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingiRacingAPI
{
    public static class Converter
    {
        public static string ConvertInterval(this int interval)
        {
            if (interval >= 0 && interval < 600000)
            {
                var milliSec = interval % 10000;
                var second = (interval - milliSec) / 10000;
                var milliDec = Convert.ToDouble(milliSec * .1);
                if (milliDec < 10)
                {
                    return $"-{second}.00{Math.Truncate(milliDec)}";
                }

                if (milliDec < 100)
                {
                    return $"-{second}.0{Math.Truncate(milliDec)}";
                }

                if (second < 10)
                {
                    return $"-0{second}.{Math.Truncate(milliDec)}";
                }
                return $"-{second}.{Math.Truncate(milliDec)}";
            }

            if (interval >= 600000)
            {
                var milliSec = interval % 10000;
                var second = (interval - milliSec) / 10000;
                var minute = second / 60;
                second = second - (minute * 60);
                var milliDec = Convert.ToDouble(milliSec * .1);

                if (second < 10)
                {
                    return $"-{minute}:0{second}.{Math.Truncate(milliDec)}";
                }
                return $"-{minute}:{second}.{Math.Truncate(milliDec)}";
            }
            return "-";
        }

        public static string ConvertLapTime(this int lapTime)
        {
            if (lapTime > 999)
            {
                var milliSec = lapTime % 10000;
                var second = (lapTime - milliSec) / 10000;
                var minute = second / 60;
                second = second - (minute * 60);
                var milliDec = Convert.ToDouble(milliSec * .1);

                if (second < 10)
                {
                    return $"{minute}:0{second}.{Math.Truncate(milliDec)}";
                }
                return $"{minute}:{second}.{Math.Truncate(milliDec)}";
            }
            return "-";
        }

        public static string ConvertBestLapNum(this int lapNum)
        {
            if (lapNum < 0)
            {
                return "-";
            }
            return lapNum.ToString();
        }
    }
}
