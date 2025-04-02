using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AngularApp2.Server.Data;
using AngularApp2.Server.Models;
using Microsoft.AspNetCore.Authorization;

namespace AngularApp2.Server.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class AddressesController : ControllerBase
    {
        private readonly AppDbContext _context;
        
        public AddressesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Addresses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddresses()
        {
            return await _context.Addresses.ToListAsync();
        }

        // GET: api/Addresses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Address>> GetAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();
            return address;
        }

        // GET: api/Addresses/ByStudent/5
        [HttpGet("ByStudent/{studentId}")]
        public async Task<ActionResult<IEnumerable<Address>>> GetAddressesByStudent(int studentId)
        {
            return await _context.Addresses.Where(a => a.StudentId == studentId).ToListAsync();
        }

        // POST: api/Addresses
        [HttpPost]
        public async Task<ActionResult<Address>> PostAddress(Address address)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentExists = await _context.Students.AnyAsync(s => s.Id == address.StudentId);
            if (!studentExists)
                return BadRequest("Invalid StudentId");

            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetAddress), new { id = address.AddressID }, address);
        }

        // PUT: api/Addresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAddress(int id, Address address)
        {
            if (id != address.AddressID)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            _context.Entry(address).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Addresses.Any(e => e.AddressID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAddress(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
                return NotFound();

            _context.Addresses.Remove(address);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
