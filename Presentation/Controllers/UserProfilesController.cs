using Microsoft.AspNetCore.Mvc;
using RestSharp;
using APIClasses;
using DCAssignment2A.Models;
using Newtonsoft.Json;

namespace Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : Controller
    {
        private RestClient client = new RestClient("http://localhost:5299"); //Business tier   

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserProfile>> Login([FromBody] LoginData loginData)
        {
            if (loginData == null)
            {
                Console.WriteLine("Login data is null");
                BadRequest("Error");
            }
            
            Console.WriteLine("Starting API Request to backend");
            
            RestRequest request = new RestRequest("api/userprofiles/Login", Method.Get);
            request.AddJsonBody(loginData);
            RestResponse response = await client.ExecuteAsync(request);
            
            Console.WriteLine("Received response from backend");

            if (response.IsSuccessStatusCode)//or is it isSuccess?
            {
                UserProfile u = JsonConvert.DeserializeObject<UserProfile>(response.Content);
                return Ok(u);
            } else
            {
                Error error = JsonConvert.DeserializeObject<Error>(response.Content);
                return BadRequest(error);//idk what to do sending back an error
            }    
        }
    }
}
