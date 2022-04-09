using Aydsko.iRacingData;
using Aydsko.iRacingData.Member;
using Aydsko.iRacingData.Results;
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

        public async void CreateResult(int sessionId, IDataClient iRacingClient)
        {
            var subSessionResults = await iRacingClient.GetSubSessionResultAsync(sessionId, true);

            for (var i = 0; i < subSessionResults.Data.SessionResults[1].Results.Length; i++)
            {
                var results = subSessionResults.Data.SessionResults[1].Results[i];

                _connection.Execute("INSERT INTO results (SessionId, CustId, DisplayName, CarNumber, FinishPosition, StartingPosition, LapsLed, BestLapNum, BestLapTime, AverageLap," +
                    " Incidents, Division, ClubShortname, OldLicenseLevel, NewLicenseLevel, OldSafetyRating, NewSafetyRating, OldIRating, NewIRating, CarId)" +
                    " VALUES (@sessionId, @custId, @displayName, @carNumber, @finishPosition, @startingPosition, @lapsLed, @bestLapNum, @bestLapTime," +
                    " @averageLap, @incidents, @division, @clubshortname, @oldLicenseLevel, @newLicenseLevel, @oldSafetyRating, @newSafetyRating, @oldIRating, @newIRating, @carId);",
                    new
                    {
                        sessionId = sessionId,
                        custId = results.CustId,
                        displayName = results.DisplayName,
                        carNumber = results.Livery.CarNumber,
                        finishPosition = results.FinishPosition + 1,
                        startingPosition = results.StartingPosition + 1,
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

        public async void UpdateMyRatings(int[] myId)
        {
            var myRatings = await _iRacingClient.GetDriverInfoAsync(myId, true);
            var ovalRatings = myRatings.Data[0].Licenses[0];
            var roadRatings = myRatings.Data[0].Licenses[1];
            var dirtOvalRatings = myRatings.Data[0].Licenses[2];
            var dirtRoadRatings = myRatings.Data[0].Licenses[3];

            _connection.Execute("UPDATE myratings SET ovallicense = @OL, ovalsafetyrating = @OSR, ovalirating = @OIR, roadlicense = @RL, roadsafetyrating = @RSR, roadirating = @RIR, dirtovallicense = @DOL, dirtovalsafetyrating = @DOSR, dirtovalirating = @DOIR, dirtroadlicense = @DRL, dirtroadsafetyrating = @DRSR, dirtroadirating = @DRIR WHERE idMyRatings = 4;",
                new { OL = ovalRatings.LicenseLevel, OSR = ovalRatings.SafetyRating, OIR = ovalRatings.IRating, RL = roadRatings.LicenseLevel, RSR = roadRatings.SafetyRating, RIR = roadRatings.IRating, DOL = dirtOvalRatings.LicenseLevel, DOSR = dirtOvalRatings.SafetyRating, DOIR = dirtOvalRatings.IRating, DRL = dirtRoadRatings.LicenseLevel, DRSR = dirtRoadRatings.SafetyRating, DRIR = dirtRoadRatings.IRating });

            Console.WriteLine(ovalRatings.IRating);
            Console.WriteLine(roadRatings.SafetyRating);
            Console.WriteLine(dirtOvalRatings.SafetyRating);
            Console.WriteLine(dirtRoadRatings.IRating);
        }
    }
}
