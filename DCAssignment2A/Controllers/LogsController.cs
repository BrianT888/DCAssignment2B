using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIClasses;
using DCAssignment2A.Data;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DCAssignment2A.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly DBManager _context;

        public LogsController(DBManager context)
        {
            _context = context;
        }

        // GET: api/Logs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Log>>> GetLogs()
        {
            return await _context.Logs.ToListAsync();
        }

        // GET: api/Logs/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<Log>> GetLog(int id)
        //{
        //    var log = await _context.Logs.FindAsync(id);

        //    if (log == null)
        //    {
        //        return NotFound();
        //    }

        //    return log;
        //}

        // PUT: api/Logs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutLog(int id, Log log)
        //{
        //    if (id != log.logId)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(log).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!LogExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/Logs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Log>> PostLog(Log log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLog", new { id = log.logId }, log);
        }

        // DELETE: api/Logs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLog(int id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log == null)
            {
                return NotFound();
            }

            _context.Logs.Remove(log);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogExists(int id)
        {
            return _context.Logs.Any(e => e.logId == id);
        }

        [HttpPost]
        [Route("CreateLog/{userId}/{logMsg}")]
        public async Task<ActionResult<Log>> CreateLog(int userId, string logMsg)
        {
            Log log = new Log();
            log.logMsg = $"Admin {userId} {logMsg}";
            log.Date = DateTime.Now;
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetLog", new { id = log.logId }, log);
        }
    }
}
