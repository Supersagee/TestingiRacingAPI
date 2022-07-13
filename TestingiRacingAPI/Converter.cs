namespace TestingiRacingAPI

{
    public static class Converter
    {
        public static string ConvertInterval(this TimeSpan? time)
        {
            var format = @"mm\:ss\.fff";

            if (time.HasValue)
            {
                if (time < TimeSpan.Zero)
                {
                    return "-";
                }
                
                TimeSpan convertedTime = time.Value;
                return $"-{convertedTime.ToString(format)}";
            }

            return "-";
        }

        public static string ConvertLapTime(this TimeSpan? time)
        {
            var format = @"mm\:ss\.fff";
            
            if (time.HasValue)
            {
                if (time < TimeSpan.Zero)
                {
                    return "-";
                }

                TimeSpan convertedTime = time.Value;
                return convertedTime.ToString(format);
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