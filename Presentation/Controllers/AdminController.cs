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

        //change as see fit, im making a start as a guess

        [HttpGet]
        [Route("GetAllTransactions")]
        public async Task<IActionResult> GetAllTransactions()//gets all transactions indiscriminately
        {
            Console.WriteLine("Starting API Request to backend");
            RestRequest request = new RestRequest("api/Transactions", Method.Get);
            RestResponse response = await client.ExecuteAsync(request);
            Console.WriteLine("Received response from backend");
            if (response.IsSuccessStatusCode)
            {
                List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(response.Content);
                return Ok(transactions);
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);
            }
        }

        [HttpGet]
        [Route("GetAllUsersTransactions")]
        //For each user, get their bank accounts, then get transactions for each account add to dictionary of Users and their Transactions
        //Unsure if this method is necessary 
        public async Task<ActionResult<Dictionary<UserProfile, List<Transaction>>>> GetAllClientsTransactions()
        {
            Console.WriteLine("Starting API Request to backend");

            RestRequest request = new RestRequest("api/UserProfiles", Method.Get);//get all user profiles first
            RestResponse response = await client.ExecuteAsync(request);
            Dictionary<UserProfile, List<Transaction>> userTransactions = new Dictionary<UserProfile, List<Transaction>>();//to get transactions for each specific user?

            if (response.IsSuccessStatusCode)
            {
                List<UserProfile> users = JsonConvert.DeserializeObject<List<UserProfile>>(response.Content);//get back the list of users

                foreach (var user in users)
                {
                    RestRequest bankAccountRequest = new RestRequest($"api/BankAccounts/UserID/{user.UserProfileId}", Method.Get);
                    RestResponse bankAccountResponse = await client.ExecuteAsync(bankAccountRequest);
                    if (bankAccountResponse.IsSuccessStatusCode)
                    {
                        List<BankAccount> bankAccounts = JsonConvert.DeserializeObject<List<BankAccount>>(bankAccountResponse.Content);
                        foreach (var account in bankAccounts)
                        {
                            RestRequest transactionsRequest = new RestRequest($"api/Transactions/GetTransactionsForAccount/{account.BankAccountNumber}", Method.Get);
                            RestResponse transactionsResponse = await client.ExecuteAsync(transactionsRequest);
                            if (transactionsResponse.IsSuccessStatusCode)
                            {
                                List<Transaction> transactions = JsonConvert.DeserializeObject<List<Transaction>>(transactionsResponse.Content);

                                userTransactions.Add(user, transactions);
                            }
                        }
                    }
                }
                return Ok(userTransactions);
            }
            else
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);
            }
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
