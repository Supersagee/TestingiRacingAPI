using Microsoft.Extensions.DependencyInjection;
using Aydsko.iRacingData;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using TestingiRacingAPI;
using System.IO;

var username = "username";
var password = "password";

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

var repo = new DapperResultRepo(conn, iRacingClient);

var resultsList = await repo.RecentRacesChecker();

try
{
    if (resultsList.Count > 0)
    {
        for (var i = 0; i < resultsList.Count; i++)
        {
            await repo.CreateResult(resultsList[i]);
        }
    }
}
catch (MySqlException ex)
{
    
}

await repo.UpdateMyRatings();
