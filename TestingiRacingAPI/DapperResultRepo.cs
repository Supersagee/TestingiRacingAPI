using Aydsko.iRacingData;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingiRacingAPI
{
    public class DapperResultRepo
    {
        private readonly IDbConnection _connection;
        private readonly IDataClient _iRacingClient;

        public DapperResultRepo(IDbConnection connection, IDataClient iRacingClient)
        {
            _connection = connection;
            _iRacingClient = iRacingClient;
        }

        public async Task CreateResult(int sessionId)
        {
            var subSessionResults = await _iRacingClient.GetSubSessionResultAsync(sessionId, true);

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

            for (var i = 0; i < subSessionResults.Data.SessionResults[1].Results.Length; i++)
            {
                var results = subSessionResults.Data.SessionResults[1].Results[i];

                _connection.Execute("INSERT INTO results (SessionId, CustId, DisplayName, CarNumber, FinishPosition, StartingPosition, FinishInterval, LapsLed, BestLapNum, BestLapTime, AverageLap," +
                    " Incidents, Division, ClubShortname, OldLicenseLevel, NewLicenseLevel, OldSafetyRating, NewSafetyRating, OldIRating, NewIRating, CarId)" +
                    " VALUES (@sessionId, @custId, @displayName, @carNumber, @finishPosition, @startingPosition, @finishInterval, @lapsLed, @bestLapNum, @bestLapTime," +
                    " @averageLap, @incidents, @division, @clubshortname, @oldLicenseLevel, @newLicenseLevel, @oldSafetyRating, @newSafetyRating, @oldIRating, @newIRating, @carId);",
                    new
                    {
                        sessionId = sessionId,
                        custId = results.CustId,
                        displayName = results.DisplayName,
                        carNumber = results.Livery.CarNumber,
                        finishPosition = results.FinishPosition + 1,
                        startingPosition = results.StartingPosition + 1,
                        finishInterval = results.Interval,
                        lapsLed = results.LapsLead,
                        bestLapNum = results.BestLapNum,
                        bestLapTime = results.BestLapTime,
                        averageLap = results.AverageLap,
                        incidents = results.Incidents,
                        division = results.Division + 1,
                        clubshortname = results.ClubShortname,
                        oldLicenseLevel = results.OldLicenseLevel,
                        newLicenseLevel = results.NewLicenseLevel,
                        oldSafetyRating = results.OldSafetyRating,
                        newSafetyRating = results.NewSafetyRating,
                        oldIRating = results.OldIRating,
                        newIRating = results.NewIRating,
                        carId = results.CarId
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
                " dirtovallicense = @DOL, dirtovalsafetyrating = @DOSR, dirtovalirating = @DOIR, dirtroadlicense = @DRL, dirtroadsafetyrating = @DRSR, dirtroadirating = @DRIR WHERE idMyRatings = 4;",
                new
                {
                    OL = ovalRatings.LicenseLevel,
                    OSR = ovalRatings.SafetyRating,
                    OIR = ovalRatings.IRating,
                    RL = roadRatings.LicenseLevel,
                    RSR = roadRatings.SafetyRating,
                    RIR = roadRatings.IRating,
                    DOL = dirtOvalRatings.LicenseLevel,
                    DOSR = dirtOvalRatings.SafetyRating,
                    DOIR = dirtOvalRatings.IRating,
                    DRL = dirtRoadRatings.LicenseLevel,
                    DRSR = dirtRoadRatings.SafetyRating,
                    DRIR = dirtRoadRatings.IRating
                });
        }
    }
}
