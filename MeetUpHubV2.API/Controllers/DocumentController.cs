using MeetUpHubV2.DataAccess;
using Microsoft.AspNetCore.Mvc;

namespace MeetUpHubV2.API.Controllers
{
    public class DocumentController : Controller
    {

        private readonly MeetUpHubV2DbContext _context;

        public DocumentController(MeetUpHubV2DbContext context)
        {
            _context = context;
        }

        //Bu iki işlemi hem admin hemde çalışan tablosundaki herkes yapabilme yetkisine sahip olacak.
        // Onayla butonu bu action'ı tetikleyecek.
        public async Task<IActionResult> ApproveDocument(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (user.Document != null)
                {
                    user.AccountStatus = "Onaylandı";
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok("Belge onaylandı.");
        }
        // Reddet butonu bu action'ı tetikleyecek.
        public async Task<IActionResult> RejectDocument(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                if (user.Document != null)
                {
                    user.AccountStatus = "Reddedildi";
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            return Ok("Belge Reddedildi.");
        }

    }
}
