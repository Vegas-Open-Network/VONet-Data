using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using VONetData.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace VONetData.Pages
{
    [Authorize]
    public class UserPanelModel : PageModel
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        public List<Member> JoinRequests { get; set; } = new();

        public UserPanelModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public void OnGet()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                JoinRequests = _db.Members.Where(m => m.UserId == userId).ToList();
            }
        }
    }
}
