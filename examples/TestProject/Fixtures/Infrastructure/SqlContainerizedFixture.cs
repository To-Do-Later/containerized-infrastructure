using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ToDoLater.Containerized.Infrastructure.Docker;
using ToDoLater.Containerized.Infrastructure.Docker.Models;

namespace TestProject.Fixtures.Infrastructure
{
    public class SqlContainerizedFixture : DockerContainerizedService
    {
        public SqlContainerizedFixture(IDockerClient dockerClient, DockerServiceSettings settings)
           : base(dockerClient, settings)
        {
        }

        public override async Task<bool> IsReadyAsync(CancellationToken cancellationToken)
        {
            try
            {
                var port = this.settings.PortMappings.First().Key;
                using (var conn = new SqlConnection($"Server=localhost,{port};User=sa;Password=!MyMagicPasswOrd;Timeout=5;TrustServerCertificate=true"))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> PrepareAsync(CancellationToken cancellationToken)
        {
            var port = this.settings.PortMappings.First().Key;
            using (var conn = new SqlConnection($"Server=localhost,{port};User=sa;Password=!MyMagicPasswOrd;Timeout=5;TrustServerCertificate=true"))
            {
                await conn.OpenAsync();

                foreach (var query in Queries)
                {
                    using (var command = new SqlCommand(query, conn))
                    {
                         command.ExecuteNonQuery();
                         
                    }
                }

                return true;
            }
        }

        private static List<string> Queries = new List<string>
        {
            @"IF DB_ID('WeatherForecastdb') IS NULL 
                BEGIN CREATE DATABASE WeatherForecastdb; 
            END",

            @"IF SUSER_ID('WeatherForecastUser') IS NULL 
                CREATE LOGIN[WeatherForecastUser] WITH PASSWORD = 'MyPassword', CHECK_POLICY = OFF, CHECK_EXPIRATION = OFF; 
            ",

             @"use [WeatherForecastdb]
                                IF USER_ID('WeatherForecastUser') IS NULL
                                BEGIN
                                 Alter login [WeatherForecastUser] with default_database = [WeatherForecastdb];
                                 CREATE user [WeatherForecastUser] for login [WeatherForecastUser] 
                                 EXEC sp_addrolemember 'db_datareader', 'WeatherForecastUser'
                                 EXEC sp_addrolemember 'db_datawriter', 'WeatherForecastUser'
                                END 
            ",

             @"CREATE TABLE [WeatherForecasts] (
                            [Id] varchar(250) NOT NULL,
                            [Date] datetimeoffset(7) NOT NULL,
                            [TemperatureC] int NOT NULL,
                            [Summary] varchar(250) NOT NULL,
                            CONSTRAINT [PK_WeatherForecasts] PRIMARY KEY ([Id])
                        );
             ",

            @"Insert into [dbo].[WeatherForecasts]
            ([Id],[Date],[TemperatureC],[Summary])
            values ('1A2B3C','2021-01-14 21:13:35.230',35,'It´s hot')"
        };
    }
}
