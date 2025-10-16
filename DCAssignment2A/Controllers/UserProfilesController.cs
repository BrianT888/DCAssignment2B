using Azure.Core;
using DCAssignment2A.Data;
using DCAssignment2A.Models;
using APIClasses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace DCAssignment2A.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserProfilesController : ControllerBase
    {
        private readonly DBManager _context;

        public UserProfilesController(DBManager context)
        {
            _context = context;
        }

        // GET: api/UserProfiles
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfiles()
        {
            try
            {
                return await _context.UserProfiles.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user profiles: {ex.Message}");
            }
        }

        // GET: api/UserProfiles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfile>> GetUserProfile(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var userProfile = await _context.UserProfiles.FindAsync(id);
                if (userProfile == null) return NotFound($"User with ID {id} not found.");
                return userProfile;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user profile: {ex.Message}");
            }
        }

        // PUT: api/UserProfiles/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserProfile(int id, UserProfile userProfile)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (id != userProfile.UserProfileId) return BadRequest("ID mismatch.");
            if (userProfile == null) return BadRequest("User profile cannot be null.");
            if (string.IsNullOrWhiteSpace(userProfile.name)) return BadRequest("Name is required.");
            if (string.IsNullOrWhiteSpace(userProfile.email)) return BadRequest("Email is required.");
            if (string.IsNullOrWhiteSpace(userProfile.address)) return BadRequest("Address is required.");
            if (string.IsNullOrWhiteSpace(userProfile.phoneNumber)) return BadRequest("Phone number is required.");
            if (string.IsNullOrWhiteSpace(userProfile.password)) return BadRequest("Password is required.");
            if (!Regex.IsMatch(userProfile.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return BadRequest("Invalid email format.");
            if (!Regex.IsMatch(userProfile.phoneNumber, @"^04\d{8}$")) return BadRequest("Phone number must be in format 04XXXXXXXX.");

            // Check duplicates
            if (_context.UserProfiles.Any(u => u.UserProfileId == userProfile.UserProfileId && u.UserProfileId != id)) return Conflict("User profile ID already exists.");
            if (_context.UserProfiles.Any(u => u.email == userProfile.email && u.UserProfileId != id)) return Conflict("Email already exists.");
            if (_context.UserProfiles.Any(u => u.phoneNumber == userProfile.phoneNumber && u.UserProfileId != id)) return Conflict("Phone number already exists.");
            if (_context.UserProfiles.Any(u => u.name.ToLower() == userProfile.name.ToLower() && u.UserProfileId != id)) return Conflict("Username already exists.");
            userProfile.profilePicture = ProfilePicReader.GetDefaultPFP();

            _context.Entry(userProfile).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserProfileExists(id))
                {
                    return NotFound(    );
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating user profile: {ex.Message}");
            }

            return NoContent();
        }

        // POST: api/UserProfiles
        [HttpPost]
        public async Task<ActionResult<UserProfile>> PostUserProfile(UserProfile userProfile)
        {
            // Input validation
            if (userProfile == null) return BadRequest("UserProfile cannot be null.");
            if (userProfile.UserProfileId < 0) return BadRequest("ID must be a positive integer.");
            if (string.IsNullOrWhiteSpace(userProfile.name)) return BadRequest("Name is required.");
            if (string.IsNullOrWhiteSpace(userProfile.email)) return BadRequest("Email is required.");
            if (string.IsNullOrWhiteSpace(userProfile.address)) return BadRequest("Address is required.");
            if (string.IsNullOrWhiteSpace(userProfile.phoneNumber)) return BadRequest("Phone number is required.");
            if (string.IsNullOrWhiteSpace(userProfile.password)) return BadRequest("Password is required.");
            if (!Regex.IsMatch(userProfile.email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return BadRequest("Invalid email format.");         
            if (!Regex.IsMatch(userProfile.phoneNumber, @"^04\d{8}$")) return BadRequest("Phone number must be in format 04XXXXXXXX.");

            // Check duplicates
            if (_context.UserProfiles.Any(u => u.UserProfileId == userProfile.UserProfileId)) return Conflict("User profile ID already exists.");
            if (_context.UserProfiles.Any(u => u.email == userProfile.email)) return Conflict("Email already exists.");
            if (_context.UserProfiles.Any(u => u.phoneNumber == userProfile.phoneNumber)) return Conflict("Phone number already exists.");
            if (_context.UserProfiles.Any(u => u.name.ToLower() == userProfile.name.ToLower())) return Conflict("Username already exists.");

            try
            {
                //set the user profile to have the default profile picture
                userProfile.profilePicture = ProfilePicReader.GetDefaultPFP();
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetUserProfile", new { id = userProfile.UserProfileId }, userProfile);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating user profile: {ex.Message}");
            }
        }

        // DELETE: api/UserProfiles/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserProfile(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var user = await _context.UserProfiles.FindAsync(id);
                if (user == null) return NotFound($"User with ID {id} not found.");

                _context.UserProfiles.Remove(user);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting user profile: {ex.Message}");
            }
        }

        private bool UserProfileExists(int id)
        {
            return _context.UserProfiles.Any(e => e.UserProfileId == id);
        }

        // GET: Username = api/UserProfiles/Name/{name}
        [HttpGet("Name/{name}")]
        public async Task<ActionResult<UserProfile>> GetUserProfileByUsername(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return BadRequest("Name cannot be empty.");

            try
            {
                var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.name == name);
                if (user == null) return NotFound($"User with name {name} not found.");
                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }

        // GET: Email = api/UserProfiles/Email/{email}
        [HttpGet("Email/{email}")]
        public async Task<ActionResult<UserProfile>> GetUserProfileByEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return BadRequest("Email cannot be empty.");
            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return BadRequest("Invalid email format.");

            try
            {
                var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.email == email);
                if (user == null) return NotFound($"User with email {email} not found.");
                return user;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving user: {ex.Message}");
            }
        }

        // Upload a profile picture to a user profile
        [HttpPost("UploadUserPFP/{id}")]
        public async Task<IActionResult> UploadUserPFP(int id, IFormFile file)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

            // Validate file type
            var allowedFiles = new[] { ".jpg", ".jpeg", ".png" };
            var fileType = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedFiles.Contains(fileType)) return BadRequest("Invalid file type. Use .jpg, .jpeg, or .png");


            try
            {
                var userProfile = await _context.UserProfiles.FindAsync(id);
                if (userProfile == null) return NotFound("User not found.");

                // Load image and resize to 360x360
                using var stream = file.OpenReadStream();
                using var original = System.Drawing.Image.FromStream(stream);
                using var resized = new Bitmap(360, 360);

                using (var g = Graphics.FromImage(resized))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.Clear(Color.White);
                    g.DrawImage(original, 0, 0, 360, 360);
                }

                using (var ms = new MemoryStream())
                {
                    resized.Save(ms, ImageFormat.Jpeg);
                    userProfile.profilePicture = ms.ToArray();
                }

                await _context.SaveChangesAsync();
                return Ok("Profile picture uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error uploading profile picture: {ex.Message}");
            }
        }

        //get the profile picture for a user
        [HttpGet("ProfilePic/{id}")]
        public async Task<IActionResult> GetUserPFP(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var user = await _context.UserProfiles.FindAsync(id);
                if (user == null) return NotFound($"User with ID {id} not found.");

                return File(user.profilePicture, "image/jpeg");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving profile picture: {ex.Message}");
            }
        }

        // Update Name
        [HttpPut]
        [Route("UpdateName/{id}/{newName}")]
        public async Task<IActionResult> UpdateName(int id, string newName)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (string.IsNullOrWhiteSpace(newName)) return BadRequest("New name cannot be empty.");

            if (_context.UserProfiles.Any(u => u.name.ToLower() == newName.ToLower())) return Conflict("Username already exists.");

            try
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserProfileId == id);
                if (userProfile == null) return NotFound($"User profile with ID {id} not found.");

                userProfile.name = newName;
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated name to {newName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating name: {ex.Message}");
            }
        }

        // Update Email
        [HttpPut]
        [Route("UpdateEmail/{id}/{newEmail}")]
        public async Task<IActionResult> UpdateEmail(int id, string newEmail)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (string.IsNullOrWhiteSpace(newEmail)) return BadRequest("Email cannot be empty.");
            if (!Regex.IsMatch(newEmail, @"^[^@\s]+@[^@\s]+\.[^@\s]+$")) return BadRequest("Invalid email format.");
            if (_context.UserProfiles.Any(u => u.email == newEmail)) return Conflict("Email already exists.");

            try
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserProfileId == id);
                if (userProfile == null) return NotFound($"User profile with ID {id} not found.");

                userProfile.email = newEmail;
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated email to {newEmail}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating email: {ex.Message}");
            }

        }

        // Update Address
        [HttpPut]
        [Route("UpdateAddress/{id}/{newAddress}")]
        public async Task<IActionResult> UpdateAddress(int id, string newAddress)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (string.IsNullOrWhiteSpace(newAddress)) return BadRequest("New address cannot be empty.");

            try
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserProfileId == id);
                if (userProfile == null) return NotFound($"User profile with ID {id} not found.");

                userProfile.address = newAddress;
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated address to {newAddress}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating address: {ex.Message}");
            }
        }

        // Update Phone Number
        [HttpPut]
        [Route("UpdatePhoneNumber/{id}/{newPhoneNumber}")]
        public async Task<IActionResult> UpdatePhoneNumber(int id, string newPhoneNumber)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (!Regex.IsMatch(newPhoneNumber, @"^04\d{8}$")) return BadRequest("Phone number must be in format 04XXXXXXXX.");
            if (_context.UserProfiles.Any(u => u.phoneNumber == newPhoneNumber)) return Conflict("Phone number already exists.");

            try
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserProfileId == id);
                if (userProfile == null) return NotFound("User profile not found.");

                userProfile.phoneNumber = newPhoneNumber;
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated phone number to {newPhoneNumber}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating phone number: {ex.Message}");
            }

        }

        // Update Password
        [HttpPut]
        [Route("UpdatePassword/{id}/{newPassword}")]
        public async Task<IActionResult> UpdatePassword(int id, string newPassword)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");
            if (string.IsNullOrWhiteSpace(newPassword)) return BadRequest("New password cannot be empty.");

            try
            {
                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(u => u.UserProfileId == id);
                if (userProfile == null) return NotFound("User profile not found.");

                userProfile.password = newPassword;
                await _context.SaveChangesAsync();

                return Ok($"Successfully updated password to {newPassword}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating phone number: {ex.Message}");
            }
        }


        // Attempt to login
        //[HttpGet]
        //[Route("Login")]
        //public async Task<IActionResult> Login([FromBody] LoginData loginData)
        //{
        //    if (string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password)) return BadRequest("Username and password are required.");

        //    var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.name == loginData.Username && u.password == loginData.Password);

        //    if (user == null)
        //    {
        //        return Unauthorized("Invalid username or password.");
        //    }

        //    // Return user info (not password or address)
        //    return Ok(new
        //    {
        //        user.UserProfileId,
        //        user.name,
        //        user.profilePicture,
        //        user.phoneNumber,
        //        user.email,
        //    });
        //}

        //brians attempt
        //feel free to change it to above but gimme reason why so i can understand lol
        [HttpGet]
        [Route("Login")]
        public async Task<ActionResult<UserProfile>> Login([FromBody] LoginData loginData)
        {
            Console.WriteLine("Login attempt received.");
            if (string.IsNullOrWhiteSpace(loginData.Username) || string.IsNullOrWhiteSpace(loginData.Password)) return BadRequest("Username and password are required.");
            Console.WriteLine("login data is: " + loginData.Username + " : " + loginData.Password);
            var user = await _context.UserProfiles.FirstOrDefaultAsync(u => u.name == loginData.Username && u.password == loginData.Password);
            Console.WriteLine("Found user in database.");
            if (user == null)
            {
                Error error = new Error
                {
                    StatusCode = 401,
                    Message = "Invalid username or password."
                };
                return Unauthorized(error);
            }

            return Ok(user);
        }
    }
}
