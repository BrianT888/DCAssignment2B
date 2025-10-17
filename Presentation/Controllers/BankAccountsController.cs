 using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using APIClasses;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private RestClient client = new RestClient("http://localhost:5299"); //Business tier   

        [HttpGet]
        [Route("UserID/{id}")]
        public async Task<ActionResult<BankAccount>> GetAllBankAccountsByID(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var request = new RestRequest($"api/BankAccounts/UserID/{id}", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var bankAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(response.Content);

                    if (bankAccounts == null || bankAccounts.Count == 0) return NotFound($"No bank accounts found for user with ID: {id}.");

                    return Ok(bankAccounts);
                }
                else
                {
                    Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                    return BadRequest(error);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching bank accounts from business tier: {ex.Message}");
            }
        }
    }
}
