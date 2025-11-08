using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;

namespace GoatSilencerArchitecture.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactInfoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContactInfoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/ContactInfo/Edit
        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var contactInfo = await _context.ContactInfos.FirstOrDefaultAsync();
            if (contactInfo == null)
            {
                contactInfo = new ContactInfo();
            }
            return View(contactInfo);
        }

        // POST: Admin/ContactInfo/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("Id,Email,PhoneNumber,CompanyName,Address")] ContactInfo contactInfo)
        {
            if (!ModelState.IsValid)
            {
                // Return with validation errors
                return View(contactInfo);
            }

            // Check if a record exists
            var existing = await _context.ContactInfos.FirstOrDefaultAsync();

            if (existing == null)
            {
                _context.Add(contactInfo);
            }
            else
            {
                existing.CompanyName = contactInfo.CompanyName;
                existing.Email = contactInfo.Email;
                existing.PhoneNumber = contactInfo.PhoneNumber;
                existing.Address = contactInfo.Address;
                _context.Update(existing);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit));
        }
    }
}
