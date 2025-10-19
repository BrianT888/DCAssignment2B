using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DCAssignment2A.Data;
using APIClasses;

namespace DCAssignment2A.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BankAccountsController : ControllerBase
    {
        private readonly DBManager _context;

        public BankAccountsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/BankAccounts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetBankAccounts()
        {
            try
            {
                return await _context.BankAccounts.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving bank accounts: {ex.Message}");
            }
        }

        // GET: api/BankAccounts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BankAccount>> GetBankAccount(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var bankAccount = await _context.BankAccounts.FindAsync(id);

                if (bankAccount == null) return NotFound($"Bank account with ID {id} not found.");

                return bankAccount;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving bank account: {ex.Message}");
            }
        }

        // PUT: api/BankAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount(int id, BankAccount bankAccount)
        {
            if (id < 0 || id != bankAccount.BankAccountId) return BadRequest("Invalid or mismatched bank account ID.");
            if (bankAccount == null) return BadRequest("Invalid bank account data.");
            if (string.IsNullOrWhiteSpace(bankAccount.BankName)) return BadRequest("Bank name cannot be empty.");
            if (string.IsNullOrWhiteSpace(bankAccount.BankAccountHolderName)) return BadRequest("Holder name cannot be empty.");
            if (bankAccount.balance < 0) return BadRequest("Balance cannot be negative.");
            if (_context.BankAccounts.Any(b => b.BankAccountNumber == bankAccount.BankAccountNumber)) return BadRequest("Bank account number already exists.");

            // Check Duplicates
            if (_context.BankAccounts.Any(b => b.BankAccountId == bankAccount.BankAccountId && b.BankAccountId != id)) return Conflict("Bank account ID already exists.");
            if (_context.BankAccounts.Any(b => b.BankAccountNumber == bankAccount.BankAccountNumber && b.BankAccountId != id)) return Conflict("Bank account number already exists.");

            try
            {
                _context.Entry(bankAccount).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating bank account: {ex.Message}");
            }
        }

        // POST: api/BankAccounts
        [HttpPost]
        public async Task<ActionResult<BankAccount>> PostBankAccount(BankAccount bankAccount)
        {
            if (bankAccount == null) return BadRequest("Invalid bank account data.");
            if (bankAccount.UserProfileId < 0) return BadRequest("ID must be a positive integer.");
            if (bankAccount.balance < 0) return BadRequest("Invalid input. Balance cannot be negative.");
            if (bankAccount.BankAccountNumber <= 0) return BadRequest("Invalid input. Bank account number must be greater than zero.");
            if (string.IsNullOrWhiteSpace(bankAccount.BankName)) return BadRequest("Bank name cannot be empty.");
            if (string.IsNullOrWhiteSpace(bankAccount.BankAccountHolderName)) return BadRequest("Holder name cannot be empty.");

            // Check Duplicates
            if (_context.BankAccounts.Any(b => b.BankAccountId == bankAccount.BankAccountId)) return Conflict("Bank account ID already exists.");
            if (_context.BankAccounts.Any(b => b.BankAccountNumber == bankAccount.BankAccountNumber)) return Conflict("Bank account number already exists.");

            try
            {
                _context.BankAccounts.Add(bankAccount);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetBankAccount", new { id = bankAccount.BankAccountId }, bankAccount);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating bank account: {ex.Message}");
            }
        }

        // DELETE: api/BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var bankAccount = await _context.BankAccounts.FindAsync(id);
                if (bankAccount == null) return NotFound($"Bank account with ID {id} not found.");

                _context.BankAccounts.Remove(bankAccount);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting bank account: {ex.Message}");
            }
        }

        // Checks if bank account with given id exists in the database
        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.BankAccountId == id);
        }

        // GET: Account Number = api/BankAccounts/Number/{number}
        [HttpGet("Number/{number}")]
        public async Task<ActionResult<BankAccount>> GetBankAccountByAccountNumber(int number)
        {
            if (number <= 0) return BadRequest("Invalid input. Bank account number must be greater than zero.");

            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == number);

                return bankAccount;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error finding bank account: {ex.Message}");
            }
        }

        //GET: All Bank Accounts by User ID = api/BankAccounts/UserID/{id}
        [HttpGet("UserID/{id}")]
        public async Task<ActionResult<IEnumerable<BankAccount>>> GetAllBankAccountsByUser(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                //checks bank accounts fk if it matches user id, if it does add to list
                var bankAccounts = await _context.BankAccounts.Where(b => b.UserProfileId == id).ToListAsync();

                return Ok(bankAccounts);
            }         
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching bank accounts: {ex.Message}");
            }
        }

        // Update Bank Account Holder Name
        [HttpPut]
        [Route("UpdateHolderName/{accountNumber}/{holderName}")]
        public async Task<IActionResult> UpdateHolderName(int accountNumber, string holderName)
        {
            if (accountNumber <= 0 || string.IsNullOrWhiteSpace(holderName)) return BadRequest("Invalid input. Account number must be a positive integer and holder name cannot be empty.");

            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound($"No bank accounts found with account number: {accountNumber}.");

                bankAccount.BankAccountHolderName = holderName.Trim();
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated holder name to {holderName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating holder name: {ex.Message}");
            }
        }

        // update Bank Name
        [HttpPut]
        [Route("UpdateBankName/{accountNumber}/{bankName}")]
        public async Task<IActionResult> UpdateBankName(int accountNumber, string bankName)
        {
            if (accountNumber <= 0 || string.IsNullOrWhiteSpace(bankName)) return BadRequest("Invalid input. Account number must be a positive integer and bank name cannot be empty.");

            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound($"No bank accounts found with account number: {accountNumber}.");

                bankAccount.BankName = bankName.Trim();
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated bank name to {bankName}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating bank name: {ex.Message}");
            }
            
        }

        // Update Balance
        [HttpPut]
        [Route("UpdateBalance/{accountNumber}/{balance}")]
        public async Task<IActionResult> UpdateBalance(int accountNumber, decimal balance)
        {
            if (accountNumber <= 0) return BadRequest("Invalid input. Account number must be a positive integer.");
            if (balance < 0) return BadRequest("Invalid input. Balance cannot be negative.");

            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound($"No bank accounts found with account number: {accountNumber}.");

                bankAccount.balance = balance;
                await _context.SaveChangesAsync();
                return Ok($"Successfully updated balance to {balance}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating balance: {ex.Message}");
            }
        }
    }
}
