using AngularApp2.Server.Data;
using AngularApp2.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace AngularApp2.Server.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Students
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            try
            {
                var students = await _context.Students
                    .Include(s => s.Addresses)
                    .ToListAsync();

                // Create a simplified result without circular references
                var result = students.Select(s => new
                {
                    Id = s.Id,
                    Name = s.Name,
                    Age = s.Age,
                    ProfilePicture = s.ProfilePicture,
                    Addresses = s.Addresses?.Select(a => new
                    {
                        AddressID = a.AddressID,
                        AddressLine = a.AddressLine,
                        City = a.City,
                        State = a.State,
                        PinCode = a.PinCode,
                        StudentId = a.StudentId
                        // Omitting the "Student" navigation property
                    }).ToList()
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error retrieving students: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                // Return a more detailed error response for debugging
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Students/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.
                Include(s => s.Addresses)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
                return NotFound();
            return student;
        }
        //[HttpPost]
        //public async Task<ActionResult<Student>> PostStudent([FromForm] string student, IFormFile? ProfilePicture)
        //{
        //    try
        //    {
        //        // Deserialize student from JSON string
        //        var studentObj = System.Text.Json.JsonSerializer.Deserialize<Student>(student);

        //        if (studentObj == null)
        //        {
        //            Console.WriteLine("Received null student object after deserialization");
        //            return BadRequest("Invalid student data");
        //        }

        //        // Debug logging
        //        Console.WriteLine($"Received student: Name={studentObj.Name}, Age={studentObj.Age}, HasAddresses={studentObj.Addresses?.Count ?? 0}");

        //        // Handle profile picture if provided
        //        if (ProfilePicture != null && ProfilePicture.Length > 0)
        //        {
        //            Console.WriteLine($"Received profile picture: {ProfilePicture.FileName}, {ProfilePicture.Length} bytes");
        //            using var ms = new MemoryStream();
        //            await ProfilePicture.CopyToAsync(ms);
        //            studentObj.ProfilePicture = ms.ToArray();
        //        }

        //        studentObj.Addresses ??= new List<Address>();

        //        // First add just the student without addresses
        //        var addresses = studentObj.Addresses?.ToList();
        //        studentObj.Addresses = null;
        //        _context.Students.Add(studentObj);

        //        // Save to generate the student ID
        //        await _context.SaveChangesAsync();

        //        // Now add the addresses with the correct StudentId
        //        if (addresses != null && addresses.Any())
        //        {
        //            foreach (var address in addresses)
        //            {
        //                address.StudentId = studentObj.Id;
        //            }
        //            studentObj.Addresses = addresses;
        //            await _context.SaveChangesAsync();
        //        }

        //        Console.WriteLine($"Student created with ID: {studentObj.Id}");
        //        return CreatedAtAction(nameof(GetStudent), new { id = studentObj.Id }, studentObj);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error creating student: {ex.Message}");
        //        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> PostStudent([FromBody] Student student)
        {
            
            if (student == null)
            {
                Console.WriteLine("Received null student object");
                return BadRequest("Invalid student data");
            }
            // Debug logging
            Console.WriteLine($"Received student: Name={student.Name}, Age={student.Age}, HasAddresses={student.Addresses?.Count ?? 0}");

            // Handle profile picture if it's a base64 string
            if (student.ProfilePicture != null)
            {
                try
                {
                    // The ProfilePicture property is already a byte[] at this point,
                    // so we shouldn't need any additional conversion
                    Console.WriteLine($"Profile picture received with {student.ProfilePicture.Length} bytes");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing profile picture: {ex.Message}");
                    student.ProfilePicture = null;
                }
            }
            //// Handle profile picture if it's a base64 string
            //if (student.ProfilePicture != null && student.ProfilePicture.Length > 0)
            //{
            //    try
            //    {
            //        // Convert base64 string to byte array if it's coming as a string
            //        string base64String = System.Text.Encoding.UTF8.GetString(student.ProfilePicture);
            //        if (base64String.StartsWith("data:"))
            //        {
            //            // Extract base64 data from data URL
            //            string base64Data = base64String.Substring(base64String.IndexOf(',') + 1);
            //            student.ProfilePicture = Convert.FromBase64String(base64Data);
            //            Console.WriteLine("Converted data URL to byte array");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error processing profile picture: {ex.Message}");
            //        // Continue without the profile picture if there's an error
            //    }
            //}

            student.Addresses ??= new List<Address>();

            // Clear model state for StudentId in addresses
            if (student.Addresses != null && student.Addresses.Any())
            {
                for (int i = 0; i < student.Addresses.Count; i++)
                {
                    ModelState.Remove($"Addresses[{i}].StudentId");
                }
            }

            if (!ModelState.IsValid)
            {
                // Log model state errors
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }

            // Additional checks
            if (string.IsNullOrEmpty(student.Name) || student.Age == 0)
            {
                Console.WriteLine("Student name or age is null or empty");
                return BadRequest("Student name or age cannot be empty");
            }

            try
            {
                
                //if (ProfilePicture != null)
                //{

                //    using var ms = new MemoryStream();
                //    await ProfilePicture.CopyToAsync(ms);
                //    student.ProfilePicture = ms.ToArray();
                //}

                // First add just the student without addresses
                var addresses = student.Addresses?.ToList();
                student.Addresses = null;
                _context.Students.Add(student);

                // Save to generate the student ID
                await _context.SaveChangesAsync();

                // Now add the addresses with the correct StudentId
                if (addresses != null && addresses.Any())
                {
                    foreach (var address in addresses)
                    {
                        address.StudentId = student.Id;
                    }
                    student.Addresses = addresses;
                    await _context.SaveChangesAsync();
                }
                Console.WriteLine($"Student created with ID: {student.Id}");
                return CreatedAtAction(nameof(GetStudent), new { id = student.Id }, student);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating student: {ex.Message}");
                return StatusCode(500, "Internal server error while creating student");
            }
        }


        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutStudent(int id, [FromBody] Student student)
        {
            

            // Add detailed logging for debugging
            Console.WriteLine($"Received PUT request for student ID: {id}");

            if (student == null)
            {
                return BadRequest("Student object is null");
            }

            Console.WriteLine($"Student data: Name={student.Name}, Age={student.Age}, HasAddresses={student.Addresses?.Count ?? 0}");
            
            //// Handle profile picture if it's a base64 string
            //if (student.ProfilePicture != null && student.ProfilePicture.Length > 0)
            //{
            //    try
            //    {
            //        // Convert base64 string to byte array if it's coming as a string
            //        string base64String = System.Text.Encoding.UTF8.GetString(student.ProfilePicture);
            //        if (base64String.StartsWith("data:"))
            //        {
            //            // Extract base64 data from data URL
            //            string base64Data = base64String.Substring(base64String.IndexOf(',') + 1);
            //            student.ProfilePicture = Convert.FromBase64String(base64Data);
            //            Console.WriteLine("Converted data URL to byte array");
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        Console.WriteLine($"Error processing profile picture: {ex.Message}");
            //        // Continue without the profile picture if there's an error
            //    }
            //}

            if (id != student.Id)
            {
                Console.WriteLine($"ID mismatch: Path ID={id}, Student ID={student.Id}");
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Model error: {error.ErrorMessage}");
                    }
                }
                return BadRequest(ModelState);
            }
            try
            {

                
                // First, get the existing student with addresses
                var existingStudent = await _context.Students
                    .Include(s => s.Addresses)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (existingStudent == null)
                {
                    return NotFound($"Student with ID {id} not found");
                }

                // Update the student properties
                existingStudent.Name = student.Name;
                existingStudent.Age = student.Age;
                // Handle profile picture if it's a base64 string
                if (student.ProfilePicture != null)
                {
                    try
                    {
                        existingStudent.ProfilePicture = student.ProfilePicture;
                        Console.WriteLine("Updated profile picture");
                        Console.WriteLine($"Profile picture received with {existingStudent.ProfilePicture.Length} bytes");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error processing profile picture: {ex.Message}");
                        existingStudent.ProfilePicture = null;
                    }
                }
                //if (student.ProfilePicture != null)
                //{
                //    existingStudent.ProfilePicture = student.ProfilePicture;
                //}

                // Handle addresses: first remove addresses that are no longer present
                if (existingStudent.Addresses != null)
                {
                    // Get IDs of addresses in the incoming student object
                    var updatedAddressIds = student.Addresses?.Select(a => a.AddressID).ToList() ?? new List<int>();

                    // Find addresses to remove (those in existing but not in updated)
                    var addressesToRemove = existingStudent.Addresses
                        .Where(a => a.AddressID > 0 && !updatedAddressIds.Contains(a.AddressID))
                        .ToList();

                    foreach (var addressToRemove in addressesToRemove)
                    {
                        _context.Addresses.Remove(addressToRemove);
                    }
                }

                // Now handle new or updated addresses
                if (student.Addresses != null)
                {
                    foreach (var address in student.Addresses)
                    {
                        // Make sure address has the correct student ID
                        address.StudentId = id;

                        if (address.AddressID > 0)
                        {
                            // This is an existing address, update it
                            _context.Entry(address).State = EntityState.Modified;
                        }
                        else
                        {
                            // This is a new address, add it to the student
                            existingStudent.Addresses ??= new List<Address>();
                            existingStudent.Addresses.Add(address);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating student: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

            //_context.Entry(student).State = EntityState.Modified;

            //    try
            //    {
            //        await _context.SaveChangesAsync();
            //    }
            //    catch (DbUpdateConcurrencyException)
            //    {
            //        if (!_context.Students.Any(e => e.Id == id))
            //            return NotFound();
            //        else
            //            throw;
            //    }

            //    return NoContent();
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {

            var student = await _context.Students.Include(s => s.Addresses)
                                                 .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null)
                return NotFound();

            if (student.Addresses != null && student.Addresses.Any())
                _context.Addresses.RemoveRange(student.Addresses);

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
