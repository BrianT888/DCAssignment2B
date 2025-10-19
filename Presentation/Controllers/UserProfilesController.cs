using APIClasses;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;
using System.Text.RegularExpressions;

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
                Error error = new Error
                {
                    StatusCode = 400,
                    Message = "Login data is null"
                };
                BadRequest(error);
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

        // Update Email
        [HttpPut]
        [Route("UpdateEmail/{id}/{newEmail}")]
        public async Task<IActionResult> UpdateEmail(int id, string newEmail)
        {
            var request = new RestRequest($"api/UserProfiles/UpdateEmail/{id}/{newEmail}", Method.Put);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return Ok(response.Content);

            return StatusCode((int)response.StatusCode, response.Content);
        }

        // Update Phone Number
        [HttpPut]
        [Route("UpdatePhoneNumber/{id}/{newPhoneNumber}")]
        public async Task<IActionResult> UpdatePhoneNumber(int id, string newPhoneNumber)
        {
            var request = new RestRequest($"api/UserProfiles/UpdatePhoneNumber/{id}/{newPhoneNumber}", Method.Put);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return Ok(response.Content);

            return StatusCode((int)response.StatusCode, response.Content);
        }

        // Update Password
        [HttpPut]
        [Route("UpdatePassword/{id}/{newPassword}")]
        public async Task<IActionResult> UpdatePassword(int id, string newPassword)
        {
            var request = new RestRequest($"api/UserProfiles/UpdatePassword/{id}/{newPassword}", Method.Put);
            var response = await client.ExecuteAsync(request);

            if (response.IsSuccessful)
                return Ok(response.Content);

            return StatusCode((int)response.StatusCode, response.Content);
        }

        // Get all users
        [HttpGet]
        [Route("GetAll")]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetAllUsers()
        {
            try
            {
                var request = new RestRequest("api/UserProfiles", Method.Get);
                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var users = JsonConvert.DeserializeObject<List<UserProfile>>(response.Content);
                    return Ok(users);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.Content);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving users from business tier: {ex.Message}");
            }
        }


        [HttpPut]
        [Route("ToggleStatus/{id}/{status}")]
        public async Task<IActionResult> ToggleUserStatus(int id, bool status)
        {
            try
            {
                var request = new RestRequest($"api/UserProfiles/SetActive/{id}/{status}", Method.Put);
                RestResponse response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    return Ok(response.Content);
                }
                else
                {
                    return StatusCode((int)response.StatusCode, response.Content);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error toggling user status: {ex.Message}");
            }
        }
    }
}
