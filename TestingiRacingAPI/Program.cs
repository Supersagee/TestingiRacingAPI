using Microsoft.Extensions.DependencyInjection;
using Aydsko.iRacingData;

Console.WriteLine("Aydsko iRacing Data API Example Console Application");

Console.WriteLine();
Console.Write("iRacing Username: ");
var username = Console.ReadLine();

Console.WriteLine();
Console.Write("iRacing Password: ");
var password = ReadPassword();

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

using var provider = services.BuildServiceProvider();
using var appScope = provider.CreateScope();

var iRacingClient = provider.GetRequiredService<IDataClient>();
var myInfo = await iRacingClient.GetMyInfoAsync();

Console.WriteLine();
Console.WriteLine("Request successful!");
Console.WriteLine($@"Driver name: {myInfo.Data.DisplayName}
Customer ID: {myInfo.Data.CustomerId}
Joined iRacing on: {myInfo.Data.MemberSince}
Suit colors are: {myInfo.Data.Suit.Color1}, {myInfo.Data.Suit.Color2}, {myInfo.Data.Suit.Color3}");

var subSessionResults = await iRacingClient.GetSubSessionResultAsync(45243121, false);

for (var i = 0; i < subSessionResults.Data.SessionResults[1].Results.Length; i++)
{
    var results = subSessionResults.Data.SessionResults[1].Results[i];

    Console.WriteLine($"{results.FinishPosition + 1} {results.CarId} {results.Position} {results.DisplayName} {results.LapsLead} {results.AverageLap} {results.BestLapTime} {results.Incidents} {results.Division + 1} {results.NewIRating} {results.NewLicenseLevel}");   
}


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