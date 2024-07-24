using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Praktia_WEB_API.Models;

namespace Praktia_WEB_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationSmsBillingInformationController : Controller
    {
        private readonly ShirtStoreManagementContext _context;

        public NotificationSmsBillingInformationController(ShirtStoreManagementContext context)
        {
            _context = context;
        }

        // GET: api/NotificationSmsBillingInformation defaultinis gettas
        [HttpGet]
        public async Task<IActionResult> GetNotificationSmsBillingInformations(DateTime? startDate, DateTime? endDate)
        {
            var notifications = from n in _context.NotificationSmsBillingInformations
                                select n;

            if (startDate.HasValue && endDate.HasValue)
            {
                var adjustedEndDate = endDate.Value.Date.AddDays(1).AddTicks(-1); // Sets the end date to 23:59:59.9999999 on June 30th, sito nera tai nemeta viso info
                notifications = notifications.Where(n => n.ActionDate >= startDate.Value && n.ActionDate <= adjustedEndDate);
            }


            var result = await notifications.ToListAsync();

            // Check the request type and return appropriate result
            if (Request.Headers["Accept"].ToString().Contains("text/html"))
            {
                return View("Index", result); // Return view if request is for HTML, tai cia per tinklapi
            }

            return Ok(result); // Return JSON if request is for JSON, pvz per postman
        }

        // GET: api/NotificationSmsBillingInformation/5 //gali damest id, gal irgi search datet
        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotificationSmsBillingInformation(int id)
        {
            var notification = await _context.NotificationSmsBillingInformations.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }


            if (Request.Headers["Accept"].ToString().Contains("text/html"))
            {
                return View("Details", notification); 
            }

            return Ok(notification); 
        }
    }
}
