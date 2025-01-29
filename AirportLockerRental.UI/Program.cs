using AirportLockerRental.UI.Workflows;
using Microsoft.Extensions.Configuration;

var configBuilder = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

App.Run(configBuilder["AESKey"]);