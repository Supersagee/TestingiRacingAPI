using Microsoft.Extensions.DependencyInjection;
using Aydsko.iRacingData;
using Microsoft.Extensions.Configuration;
using System.Data;
using MySql.Data.MySqlClient;
using TestingiRacingAPI;

var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

string connString = config.GetConnectionString("AzureConnection");
IDbConnection mySqlConnection = new MySqlConnection(connString);

var username = "";
var password = "";

var services = new ServiceCollection();
services.AddIRacingDataApi(options =>
{
    options.Username = username;
    options.Password = password;
});

using var provider = services.BuildServiceProvider();
using var appScope = provider.CreateScope();
var iRacingClient = provider.GetRequiredService<IDataClient>();

var repo = new DapperResultRepo(mySqlConnection, iRacingClient);

await repo.UpdateMyRatings();

//await repo.GetCarNames();

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
