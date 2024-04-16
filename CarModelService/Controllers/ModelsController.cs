using CarModelService.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace CarModelService.Controllers
{
    [ApiController]
    [Route("api/models")]
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
            var makeId = GetMakeId(make);
            if (makeId == null)
            {
                return NotFound($"Make '{make}' not found.");
            }

            var models = await GetModelsForMakeIdYearAsync(makeId, modelyear);

            if (models.Models.Count<0)
            {
                return Ok($"No Data Found");

            }
            return Ok(models);
        }

        #region  private
        private int? GetMakeId(string make)
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

        private async Task<CarModels> GetModelsForMakeIdYearAsync(int? makeId, int modelyear)
        {

            if (makeId == null)
            {
                return new CarModels();
            }
            try
            {
                CarModels models = new CarModels();
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://vpic.nhtsa.dot.gov/api/vehicles/GetModelsForMakeIdYear/makeId/{makeId}/modelyear/{modelyear}?format=json");
                request.Headers.Add("accept", "text/plain");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(content);

                    var listcar = new List<string>();
                    foreach (var result in data.Results)
                    {
                        listcar.Add(result.Model_Name.ToString());
                    }
                    models.Models=listcar;

                    return models;

                }
                else
                {
                    return new CarModels();
                }

            }
            catch (Exception)
            {
                return new CarModels();

            }
           

        }
        #endregion
    }
}