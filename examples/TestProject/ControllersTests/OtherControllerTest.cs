using Xunit;
using TestProject.Fixtures;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Shouldly;

namespace TestProject.ControllersTests
{
    // WeatherForecastControllerTest, OtherControllerTest and AndOtherControllerTest will use the same instance of the Infrastructure Fixture, both test will run the test on parallel
    [Collection("InfrastructureWithExistingRecordsCollection")]
    public class OtherControllerTest
    {
        private readonly InfrastructureWithExistingRecords infrastructureWithExistingRecords;

        public OtherControllerTest(InfrastructureWithExistingRecords infrastructureWithExistingRecords)
        {
            this.infrastructureWithExistingRecords = infrastructureWithExistingRecords;
        }

        [Fact]
        public async Task GetOtherByIdAsync_WithValidId_ReturnsOkWithForecast()
        {
            // arrange
            var id = "1A2B3C";

            // act
            using (HttpResponseMessage response = await this.infrastructureWithExistingRecords.httpClient.GetAsync($"/Other/{id}"))
            {
                var result = await response.Content.ReadAsAsync<WebApplication.WeatherForecast>( );

                // assert
                response.StatusCode.ShouldBe(HttpStatusCode.OK);
                result.Id.ShouldBe(id);
            }
        }
    }
}
