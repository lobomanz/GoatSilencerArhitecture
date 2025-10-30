
using Microsoft.AspNetCore.Mvc;
using GoatSilencerArchitecture.Data;
using GoatSilencerArchitecture.Models;
using Microsoft.EntityFrameworkCore;
using GoatSilencerArchitecture.Services;

namespace GoatSilencerArchitecture.Areas.Public.Controllers
{
    [Area("Public")]
    public class ContactController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public ContactController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<IActionResult> Index()
        {
            var contactInfo = await _context.ContactInfos.FirstOrDefaultAsync();
            return View(contactInfo);
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail(string name, string email, string question)
        {
            var contactInfo = await _context.ContactInfos.FirstOrDefaultAsync();
            if (contactInfo == null)
            {
                TempData["ErrorMessage"] = "Contact information is not configured.";
                return RedirectToAction(nameof(Index));
            }

            var subject = $"New question from {name}";
            var body = $"Name: {name}<br>Email: {email}<br>Question: {question}";

            await _emailService.SendEmailAsync(contactInfo.Email, subject, body);

            TempData["SuccessMessage"] = "Your message has been sent successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
