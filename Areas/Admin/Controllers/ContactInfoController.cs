
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email,PhoneNumber,CompanyName,Address")] ContactInfo contactInfo)
        {
            if (ModelState.IsValid)
            {
                if (id != contactInfo.Id)
                {
                    return NotFound();
                }

                if (id == 0)
                {
                    _context.Add(contactInfo);
                }
                else
                {
                    _context.Update(contactInfo);
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Edit));
            }
            return View(contactInfo);
        }
    }
}
