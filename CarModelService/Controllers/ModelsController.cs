using CarModelService.Model;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace CarModelService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ModelsController : ControllerBase
    {

        private readonly ILogger<ModelsController> _logger;

        public ModelsController(ILogger<ModelsController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<string>>> GetModels(int modelyear, string make)
        {
            var makeId = await GetMakeIdAsync(make);
            if (makeId == null)
            {
                return NotFound($"Make '{make}' not found.");
            }

            return Ok();
        }

        #region  private
        private async Task<int?> GetMakeIdAsync(string make)
        {
            try
            {
                string connString = AppSettingsModel.ConnectionStrings;

                using (var connection = new SqlConnection(connString))
                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "dbo.GetMakeId";

                    command.Parameters.AddWithValue("@Make", make);

                    SqlParameter outputParameter = new SqlParameter();
                    outputParameter.ParameterName = "@MakeId";
                    outputParameter.SqlDbType = SqlDbType.Int;
                    outputParameter.Direction = ParameterDirection.Output;
                    command.Parameters.Add(outputParameter);


                    connection.Open();
                    command.ExecuteNonQuery();

                    int makeId = Convert.ToInt32(outputParameter.Value);

                    return makeId;
                }

            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion
    }
}