using Aydsko.iRacingData;
using Dapper;
using System.Data;

namespace TestingiRacingAPI
{
    public class DapperResultRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDataClient _iRacingClient;

        public DapperResultRepo(IDbConnection mySqlConnection, IDataClient iRacingClient)
        {
            _connection = mySqlConnection;
            _iRacingClient = iRacingClient;
        }

        public async Task CreateQuickResult()
        {
            var quickResults = await _iRacingClient.GetMemberRecentRacesAsync();

            var results = quickResults.Data.Races[0];

            _connection.Execute("INSERT INTO quickresults (SessionId, SeriesName, SessionStartTime, WinnerName, StartPosition, Incidents, StrengthOfField, TrackName, CarId, NewiRating)" +
                " VALUES (@sessionId, @seriesName, @sessionStartTime, @winnerName, @startPosition, @incidents, @strengthOfField, @trackName, @carId, @newiRating);",
                new
                {
                    sessionId = results.SubsessionId,
                    seriesName = results.SeriesName,
                    sessionStartTime = results.SessionStartTime,
                    winnerName = results.WinnerName,
                    startPosition = results.StartPosition,
                    incidents = results.Incidents,
                    strengthOfField = results.StrengthOfField,
                    trackName = results.Track.TrackName,
                    carId = results.CarId,
                    newiRating = results.NewiRating
                });
        }

        public async Task WriteQuickResult(int sessionId)
        {
            var subSessionResults = await _iRacingClient.GetSubSessionResultAsync(sessionId, true);

            for (int i = 0; i < subSessionResults.Data.SessionResults.Length; i++)
            {
                Console.WriteLine($"{subSessionResults.Data.SubSessionId} -- {subSessionResults.Data.SessionId} -- ");
            }
        }

        public async Task CreateResult(int sessionId)
        {
            var subSessionResults = await _iRacingClient.GetSubSessionResultAsync(sessionId, true);
            var cars = await _iRacingClient.GetCarsAsync();

            var subResults = subSessionResults.Data;

            

            _connection.Execute("INSERT INTO subsession (SessionId, SeriesName, StartTime, SeasonShortName, EventTypeName, LicenseCategory, TrackName, StrengthOfField)" +
                " VALUES (@sessionId, @seriesName, @startTime, @seasonShortName, @eventTypeName, @licenseCategory, @trackName, @strengthOfField);",
                new
                {
                    sessionId = subResults.SubSessionId,
                    seriesName = subResults.SeriesShortName,
                    startTime = subResults.StartTime,
                    seasonShortName = subResults.SeasonShortName,
                    eventTypeName = subResults.EventTypeName,
                    licenseCategory = subResults.LicenseCategory,
                    trackName = subResults.Track.TrackName,
                    strengthOfField = subResults.EventStrengthOfField
                });

            for (var i = 0; i < subSessionResults.Data.SessionResults[^1].Results.Length; i++)
            {
                var results = subSessionResults.Data.SessionResults[^1].Results[i];

                _connection.Execute("INSERT INTO results (SessionId, CustId, DisplayName, CarNumber, FinishPosition, ClassPosition, StartingPosition, FinishInterval, LapsLed, LapsComplete, BestLapNum, BestLapTime, AverageLap," +
                    " Incidents, Division, ClubShortname, OldLicenseLevel, NewLicenseLevel, OldSafetyRating, NewSafetyRating, OldIRating, NewIRating, CarId)" +
                    " VALUES (@sessionId, @custId, @displayName, @carNumber, @finishPosition, @classPosition, @startingPosition, @finishInterval, @lapsLed, @lapsComplete, @bestLapNum, @bestLapTime," +
                    " @averageLap, @incidents, @division, @clubshortname, @oldLicenseLevel, @newLicenseLevel, @oldSafetyRating, @newSafetyRating, @oldIRating, @newIRating, @carId);",
                    new
                    {
                        sessionId = sessionId,
                        custId = results.CustId,
                        displayName = results.DisplayName,
                        carNumber = results.Livery.CarNumber,
                        finishPosition = results.FinishPosition + 1,
                        classPosition = results.FinishPositionInClass + 1,
                        startingPosition = results.StartingPosition + 1,
                        finishInterval = results.Interval.ConvertInterval(),
                        lapsLed = results.LapsLead,
                        lapsComplete = results.LapsComplete,
                        bestLapNum = results.BestLapNum.ConvertBestLapNum(),
                        bestLapTime = results.BestLapTime.ConvertLapTime(),
                        averageLap = results.AverageLap.ConvertLapTime(),
                        incidents = results.Incidents,
                        division = results.Division + 1,
                        clubshortname = results.ClubShortname,
                        oldLicenseLevel = results.OldLicenseLevel.ConvertLicense(),
                        newLicenseLevel = results.NewLicenseLevel.ConvertLicense(),
                        oldSafetyRating = results.OldSafetyRating,
                        newSafetyRating = results.NewSafetyRating,
                        oldIRating = results.OldIRating,
                        newIRating = results.NewIRating,
                        carId = results.CarId
                    });
            }          
        }

        public async Task GetCarNames()
        {
            var cars = await _iRacingClient.GetCarsAsync();

            for (int i = 0; i < cars.Data.Length; i++)
            {
                var carId = cars.Data[i].CarId;
                var carName = cars.Data[i].CarName;

                _connection.Execute("INSERT INTO cars (CarId, CarName) VALUES (@carId, @carName);",
                    new
                    {
                        carId = carId,
                        carName = carName
                    });
            }
        }

        public async Task<List<int>> RecentRacesChecker()
        {
            var recentRaces = await _iRacingClient.GetMemberRecentRacesAsync();
            var recentRacesList = new List<int>();

            for (int i = 0; i < recentRaces.Data.Races.Length; i++)
            {
                var getRaces = recentRaces.Data.Races[i].SubsessionId;

                recentRacesList.Add(getRaces);
            }
            return recentRacesList;
        }

        public async Task UpdateMyRatings()
        {
            var myId = new int[1] { 564678 };
            var myRatings = await _iRacingClient.GetDriverInfoAsync(myId, true);
            var ovalRatings = myRatings.Data[0].Licenses[0];
            var roadRatings = myRatings.Data[0].Licenses[1];
            var dirtOvalRatings = myRatings.Data[0].Licenses[2];
            var dirtRoadRatings = myRatings.Data[0].Licenses[3];

            _connection.Execute("UPDATE myratings SET ovallicense = @OL, ovalsafetyrating = @OSR, ovalirating = @OIR, roadlicense = @RL, roadsafetyrating = @RSR, roadirating = @RIR," +
                " dirtovallicense = @DOL, dirtovalsafetyrating = @DOSR, dirtovalirating = @DOIR, dirtroadlicense = @DRL, dirtroadsafetyrating = @DRSR, dirtroadirating = @DRIR WHERE idMyRatings = 1;",
                new
                {
                    OL = ovalRatings.LicenseLevel.ConvertLicense(),
                    OSR = ovalRatings.SafetyRating,
                    OIR = ovalRatings.IRating,
                    RL = roadRatings.LicenseLevel.ConvertLicense(),
                    RSR = roadRatings.SafetyRating,
                    RIR = roadRatings.IRating,
                    DOL = dirtOvalRatings.LicenseLevel.ConvertLicense(),
                    DOSR = dirtOvalRatings.SafetyRating,
                    DOIR = dirtOvalRatings.IRating,
                    DRL = dirtRoadRatings.LicenseLevel.ConvertLicense(),
                    DRSR = dirtRoadRatings.SafetyRating,
                    DRIR = dirtRoadRatings.IRating
                });
        }
    }
}
