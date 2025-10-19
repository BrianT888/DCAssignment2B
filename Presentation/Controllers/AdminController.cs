using Microsoft.AspNetCore.Mvc;
using RestSharp;
using APIClasses;
using Newtonsoft.Json;
namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private RestClient client = new RestClient("http://localhost:5299"); //Business tier   

        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IActionResult> GetAllTransactions()
        {
            Console.WriteLine("Starting API Request to backend");

            RestRequest request = new RestRequest("api/Transactions", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            if (!response.IsSuccessStatusCode)
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);
            }

            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);

            RestRequest bankAccountsRequest = new RestRequest("api/BankAccounts", Method.Get);
            RestResponse bankAccountsResponse = await client.ExecuteAsync(bankAccountsRequest);
            if (!bankAccountsResponse.IsSuccessStatusCode)
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);
            }
            var bankAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(bankAccountsResponse.Content);

            RestRequest userProfilesRequest = new RestRequest("api/UserProfiles", Method.Get);
            RestResponse userProfilesResponse = await client.ExecuteAsync(userProfilesRequest);
            if (!userProfilesResponse.IsSuccessStatusCode)
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);
            }
            var userProfiles = JsonConvert.DeserializeObject<List<UserProfile>>(userProfilesResponse.Content);

            //join the data with inner joins
            var joined = (from t in transactions
                            join b in bankAccounts on t.BankAccountId equals b.BankAccountId
                            join u in userProfiles on b.UserProfileId equals u.UserProfileId
                            select new//making a new object with selected fields for table display
                            {
                                t.TransactionId,
                                t.Date,
                                t.Description,
                                t.Amount,
                                b.BankAccountNumber,
                                u.name
                            }).ToList();

            return Ok(joined);
        }


        [HttpPost]
        [Route("CreateUser")]
        public async Task<ActionResult<UserProfile>> CreateUser([FromBody] UserProfile newUser)
        {
            if (newUser == null)
            {
                Error error = new Error{ StatusCode = 400, Message = "User data cannot be null." };
                return BadRequest(error);
            }

            RestRequest request = new RestRequest("api/UserProfiles", Method.Post);
            request.AddJsonBody(newUser);

            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine("Received response from backend");

            if (response.IsSuccessStatusCode)
            {
                var createdUser = JsonConvert.DeserializeObject<UserProfile>(response.Content);
                return Ok(createdUser);
            }
            else
            {
                try
                {
                    var error = JsonConvert.DeserializeObject<Error>(response.Content);
                    return StatusCode((int)response.StatusCode, error);
                }
                catch
                {
                    // If deserialization fails, send error
                    return StatusCode((int)response.StatusCode, new Error { StatusCode = (int)response.StatusCode, Message = response.Content ?? "Unknown error occurred." });
                }
            }
        }
    }
}
