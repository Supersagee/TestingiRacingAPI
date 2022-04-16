namespace TestingiRacingAPI
{
    public static class Converter
    {
        public static string ConvertInterval(this int interval)
        {
            if (interval > 0)
            {
                var milliSec = interval % 10000;
                var second = (interval - milliSec) / 10000;
                var minute = second / 60;
                second = second - (minute * 60);
                var milliDec = Math.Truncate(Convert.ToDouble(milliSec * .1));

                if (second < 10)
                {
                    if (milliDec < 10)
                    {
                        return $"-{minute}:0{second}.00{milliDec}";
                    }

                    if (milliDec < 100)
                    {
                        return $"-{minute}:0{second}.0{milliDec}";
                    }
                    return $"-{minute}:0{second}.{milliDec}";
                }

                if (milliDec < 10)
                {
                    return $"-{minute}:{second}.00{milliDec}";
                }

                if (milliDec < 100)
                {
                    return $"-{minute}:{second}.0{milliDec}";
                }
                return $"-{minute}:{second}.{milliDec}";
            }

            return "-";
        }

        public static string ConvertLapTime(this int lapTime)
        {
            if (lapTime > 0)
            {
                var milliSec = lapTime % 10000;
                var second = (lapTime - milliSec) / 10000;
                var minute = second / 60;
                second = second - (minute * 60);
                var milliDec = Math.Truncate(Convert.ToDouble(milliSec * .1));

                if (second < 10)
                {
                    if (milliDec < 10)
                    {
                        return $"{minute}:0{second}.00{milliDec}";
                    }

                    if (milliDec < 100)
                    {
                        return $"{minute}:0{second}.0{milliDec}";
                    }
                    return $"{minute}:0{second}.{milliDec}";
                }

                if (milliDec < 10)
                {
                    return $"{minute}:{second}.00{milliDec}";
                }

                if (milliDec < 100)
                {
                    return $"{minute}:{second}.0{milliDec}";
                }
                return $"{minute}:{second}.{milliDec}";
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

        public static string ConvertLicense(this int licenseId)
        {
            switch (licenseId)
            {
                case < 5:
                    return "R";

                case < 9:
                    return "D";

                case < 13:
                    return "C";

                case < 17:
                    return "B";

                case < 21:
                    return "A";

                case < 25:
                    return "Pro";

                case < 29: 
                    return "PWC";

                default:
                    return "N/A";
            }
        }
    }
}
