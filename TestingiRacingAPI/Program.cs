using Microsoft.Extensions.DependencyInjection;
using Aydsko.iRacingData;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using TestingiRacingAPI;
using Dapper;

Console.WriteLine("Aydsko iRacing Data API Example Console Application");

Console.WriteLine();
Console.Write("iRacing Username: ");
var username = Console.ReadLine();

Console.WriteLine();
Console.Write("iRacing Password: ");
var password = Console.ReadLine();

if (username is null || password is null)
{
    Console.WriteLine("Username or password was not supplied. Exiting...");
    return;
}

var services = new ServiceCollection();
services.AddIRacingDataApi(options =>
{
    options.Username = username;
    options.Password = password;
});

var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

string connString = config.GetConnectionString("DefaultConnection");
IDbConnection conn = new MySqlConnection(connString);

using var provider = services.BuildServiceProvider();
using var appScope = provider.CreateScope();

var iRacingClient = provider.GetRequiredService<IDataClient>();
var myInfo = await iRacingClient.GetMyInfoAsync();

var repo = new DapperResultRepo(conn, iRacingClient);

var sessionId = 45243121;
await repo.CreateResult(sessionId);

var myId = new int[1] { 564678 };
await repo.UpdateMyRatings(myId);

var recentResults = repo.ReturnRecentResults();

foreach (var recentResult in await recentResults)
{
    Console.WriteLine(recentResult);
}


//repo.UpdateMyRatings(myId);

/*var myRatings = await iRacingClient.GetDriverInfoAsync(myId, true);
var ovalRatings = myRatings.Data[0].Licenses[0];
var roadRatings = myRatings.Data[0].Licenses[1];
var dirtOvalRatings = myRatings.Data[0].Licenses[2];
var dirtRoadRatings = myRatings.Data[0].Licenses[3];

conn.Execute("UPDATE myratings SET ovallicense = @OL, ovalsafetyrating = @OSR, ovalirating = @OIR, roadlicense = @RL, roadsafetyrating = @RSR, roadirating = @RIR," +
    " dirtovallicense = @DOL, dirtovalsafetyrating = @DOSR, dirtovalirating = @DOIR, dirtroadlicense = @DRL, dirtroadsafetyrating = @DRSR, dirtroadirating = @DRIR WHERE idMyRatings = 4;",
    new { OL = ovalRatings.LicenseLevel,
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
        DRIR = dirtRoadRatings.IRating });

var sessionId = 45243121;
var subSessionResults = await iRacingClient.GetSubSessionResultAsync(sessionId, true);

for (var i = 0; i < subSessionResults.Data.SessionResults[1].Results.Length; i++)
{
    var results = subSessionResults.Data.SessionResults[1].Results[i];

    conn.Execute("INSERT INTO results (SessionId, CustId, DisplayName, CarNumber, FinishPosition, StartingPosition, FinishInterval, LapsLed, BestLapNum, BestLapTime, AverageLap," +
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
*/
//Password Concealer
static string ReadPassword()
{
    string password = "";
    ConsoleKeyInfo info = Console.ReadKey(true);
    while (info.Key != ConsoleKey.Enter)
    {
        if (info.Key != ConsoleKey.Backspace)
        {
            Console.Write("*");
            password += info.KeyChar;
        }
        else if (info.Key == ConsoleKey.Backspace)
        {
            if (!string.IsNullOrEmpty(password))
            {
                
                password = password.Substring(0, password.Length - 1);
                
                int pos = Console.CursorLeft;
                
                Console.SetCursorPosition(pos - 1, Console.CursorTop);
                
                Console.Write(" ");
                
                Console.SetCursorPosition(pos - 1, Console.CursorTop);
            }
        }
        info = Console.ReadKey(true);
    }
    Console.WriteLine();
    return password;
}