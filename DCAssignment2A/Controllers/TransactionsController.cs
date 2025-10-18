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
    public class TransactionsController : ControllerBase
    {
        private readonly DBManager _context;

        public TransactionsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/Transactions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetTransactions()
        {
            try
            {
                return await _context.Transactions.ToListAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving transactions: {ex.Message}");
            }
        }

        // GET: api/Transactions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Transaction>> GetTransaction(int id)
        {
            if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

            try
            {
                var transaction = await _context.Transactions.FindAsync(id);
                if (transaction == null) return NotFound($"Transaction with ID {id} not found.");

                return transaction;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving transaction: {ex.Message}");
            }
        }

        // PUT: api/Transactions/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutTransaction(int id, Transaction transaction)
        //{
        //    if (id < 0 || id != transaction.TransactionId) return BadRequest("\"Invalid or mismatched transaction ID.");
        //    if (transaction == null) return BadRequest("Invalid transaction data.");
        //    if (!_context.BankAccounts.Any(b => b.BankAccountId == transaction.BankAccountId)) return BadRequest("Invalid BankAccountId — account not found.");
        //    if (transaction.Amount == 0) return BadRequest("Transaction amount cannot be zero.");
        //    if (string.IsNullOrWhiteSpace(transaction.Description)) return BadRequest("Transaction description cannot be empty.");

        //    try
        //    {
        //        _context.Entry(transaction).State = EntityState.Modified;
        //        await _context.SaveChangesAsync();
        //        return NoContent();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!TransactionExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error updating transaction: {ex.Message}");
        //    }

        //}

        // POST: api/Transactions
        [HttpPost]
        public async Task<ActionResult<Transaction>> PostTransaction(Transaction transaction)
        {
            if (transaction == null) return BadRequest("Invalid transaction data.");
            if (transaction.TransactionId < 0) return BadRequest("Transaction ID must be a positive integer.");
            if (!_context.BankAccounts.Any(b => b.BankAccountId == transaction.BankAccountId)) return BadRequest("Invalid BankAccountId — account not found.");
            if (transaction.Amount == 0) return BadRequest("Transaction amount cannot be zero.");
            if (string.IsNullOrWhiteSpace(transaction.Description)) transaction.Description = transaction.Amount > 0 ? "Deposit" : "Withdrawal";

            try
            {
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetTransaction", new { id = transaction.TransactionId }, transaction);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error creating transaction: {ex.Message}");

            }
        }

        // DELETE: api/Transactions/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteTransaction(int id)
        //{
        //    if (id < 0) return BadRequest("Invalid input. ID value must be a positive integer.");

        //    try
        //    {
        //        var transaction = await _context.Transactions.FindAsync(id);
        //        if (transaction == null) return NotFound($"Transaction with ID {id} not found.");

        //        _context.Transactions.Remove(transaction);
        //        await _context.SaveChangesAsync();
        //        return NoContent();
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Error deleting transaction: {ex.Message}");
        //    }
        //}

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.TransactionId == id);
        }

        //Withdaw
        [HttpPut]
        [Route("Withdraw/{accountNumber}/{amount}/{desc}")]
        public async Task<IActionResult> Withdraw(int accountNumber, decimal amount, string desc)
        {
            if (accountNumber <= 0) return BadRequest(new Error { StatusCode = 400, Message = "Invalid input. Account number must be greater than zero." });
            if (amount <= 0) return BadRequest(new Error { StatusCode = 400, Message = "Invalid input. Withdrawal amount must be positive." });
            //desc validation required
            if (!_context.BankAccounts.Any(b => b.BankAccountNumber == accountNumber)) return NotFound(new Error { StatusCode = 404, Message = "Account not found." });


            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound("Bank account not found.");
                if (bankAccount.balance < amount) return BadRequest(new Error { StatusCode = 400, Message = "Insufficient funds." });

                bankAccount.balance -= amount;
                var transaction = new Transaction
                {
                    BankAccountId = bankAccount.BankAccountId,
                    Date = DateTime.Now,
                    Amount = -amount, // Negative amount for withdrawal
                    Description = desc
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"Successful withdrawal of {amount} from account {accountNumber}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Error { StatusCode = 500, Message = $"Error performing withdrawal: {ex.Message}" });
            }
        }

        //Deposit
        [HttpPut]
        [Route("Deposit/{accountNumber}/{amount}/{desc}")]
        public async Task<IActionResult> Deposit(int accountNumber, decimal amount, string desc)
        {
            if (accountNumber <= 0) return BadRequest(new Error { StatusCode = 400, Message = "Invalid input. Account number must be greater than zero." });
            if (amount <= 0) return BadRequest(new Error { StatusCode = 400, Message = "Invalid input. Deposit amount must be positive." });
            //desc validation required
            if (!_context.BankAccounts.Any(b => b.BankAccountNumber == accountNumber)) return NotFound(new Error { StatusCode = 404, Message = "Account not found." });

            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound("Bank account not found.");
                if (amount <= 0) return BadRequest("Deposit amount must be positive value.");

                bankAccount.balance += amount;
                var transaction = new Transaction
                {
                    BankAccountId = bankAccount.BankAccountId,
                    Date = DateTime.Now,
                    Amount = amount, // Positive amount for deposit
                    Description = desc
                };
                _context.Transactions.Add(transaction);
                await _context.SaveChangesAsync();
                return Ok(new { Message = $"Successful deposit of {amount} to account {accountNumber}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new Error { StatusCode = 500, Message = $"Error performing deposit: {ex.Message}" });

            }
        }

        // Get a list of transactions for a specific bank account by account number
        [HttpGet("GetTransactionsForAccount/{accountNumber}")]
        public async Task<ActionResult<IEnumerable<Transaction>>> GetAllBankTranscationsByAccountNumber(int accountNumber)
        {
            try
            {
                var bankAccount = await _context.BankAccounts.FirstOrDefaultAsync(b => b.BankAccountNumber == accountNumber);
                if (bankAccount == null) return NotFound("Bank account not found.");

                var transactions = await _context.Transactions.Where(t => t.BankAccountId == bankAccount.BankAccountId).ToListAsync();
                if (transactions == null || transactions.Count == 0) return NotFound($"No Transactions found for bank account {accountNumber}");

                return Ok(transactions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching Transaction list for bank account: {ex.Message}");
            }
        }
    }
}
