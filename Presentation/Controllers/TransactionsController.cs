using APIClasses;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using Newtonsoft.Json;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : Controller
    {
        private RestClient client = new RestClient("http://localhost:5299"); //Business tier   

        //get transactions for specific bank accounts
        [HttpGet]
        [Route("GetTransactionsForAccount/{accountNumber}")]
        public async Task<IActionResult> GetTransactionsByAccount(string accountNumber)
        {
            if (accountNumber == null)
            {
                Console.WriteLine("Account Number is null");
                Error error = new Error
                {
                    StatusCode = 400,
                    Message = "Account Number is null"
                };
                return BadRequest(error);
            }

            Console.WriteLine("Starting API Request to backend");

            RestRequest request = new RestRequest($"api/Transactions/GetTransactionsForAccount/{accountNumber}", Method.Get);
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

        [HttpPut]
        [Route("Transfer/{fromAccountNumber}/{toAccountNumber}/{amount}/{description}")]
        public async Task<IActionResult> Transfer(string fromAccountNumber, string toAccountNumber, decimal amount, string description)
        {
            if (fromAccountNumber == null || toAccountNumber == null)
            {
                Console.WriteLine("One or both account numbers are null");
                Error error = new Error
                {
                    StatusCode = 400,
                    Message = "One or both account numbers are null"
                };
                return BadRequest(error);
            }
            if (amount <= 0)
            {
                Console.WriteLine("Transfer amount must be positive");
                Error error = new Error
                {
                    StatusCode = 400,
                    Message = "Transfer amount must be positive"
                };
                return BadRequest(error);
            }
            Console.WriteLine("Starting API Request to backend for withdrawal");
            // First, withdraw from the source account
            RestRequest withdrawRequest = new RestRequest($"api/transactions/Withdraw/{fromAccountNumber}/{amount}/{description}", Method.Put);
            RestResponse withdrawResponse = await client.ExecuteAsync(withdrawRequest);
            if (!withdrawResponse.IsSuccessStatusCode)
            {
                Error error = JsonConvert.DeserializeObject<Error>(withdrawResponse.Content);
                return BadRequest(error);
            }
            Console.WriteLine("Withdrawal successful, starting deposit to target account");
            // Then, deposit to the target account
            RestRequest depositRequest = new RestRequest($"api/transactions/Deposit/{toAccountNumber}/{amount}/{description}", Method.Put);
            RestResponse depositResponse = await client.ExecuteAsync(depositRequest);
            if (!depositResponse.IsSuccessStatusCode)
            {
                //if deposit fails probably rerun the withdraw request backwards now to return money if it can even fail?
                Error error = JsonConvert.DeserializeObject<Error>(depositResponse.Content);
                return BadRequest(error);       
            }
            return Ok($"Successful Transfer of {amount} from {fromAccountNumber} to {toAccountNumber}");
        }
    }
}
