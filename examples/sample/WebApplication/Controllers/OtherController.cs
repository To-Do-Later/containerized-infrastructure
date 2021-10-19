using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace WebApplication.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class OtherController : ControllerBase
    {
        private readonly ILogger<OtherController> logger;

        private readonly IConfiguration configuration;

        public OtherController(ILogger<OtherController> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById([FromRoute] string id)
        {
            // This is just for example
            var parameters = new Dictionary<string, object>
            {
                { "@Id", id }
            };
            string sql = "SELECT * FROM WeatherForecasts where id = @Id";

            using (var connection = new SqlConnection(this.configuration.GetConnectionString("DefaultConnection")))
            {
                return Ok(await connection.QuerySingleAsync<WeatherForecast>(sql, parameters).ConfigureAwait(false));
            }
        }
    }
}
