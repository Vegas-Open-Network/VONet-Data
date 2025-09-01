using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Collections.Generic;
using System.Linq;

namespace VONetData.Pages;

[Authorize(Roles="Admin")]
public class CRMUserModel : PageModel
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ApplicationDbContext _db;
    public IdentityUser? User { get; set; }
    public List<Member> AssignedMembers { get; set; } = new();

    public CRMUserModel(UserManager<IdentityUser> userManager, ApplicationDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public IActionResult OnGet(string id)
    {
        User = _userManager.Users.FirstOrDefault(u => u.Id == id);
        if (User == null) return NotFound();
        // Map assignments (string comparison on MemberAssignments values)
        var targetAssignments = CRMModel.MemberAssignments
            .Where(kv => kv.Value.Equals(User.Email, System.StringComparison.OrdinalIgnoreCase))
            .Select(kv => kv.Key)
            .ToHashSet();
        if (targetAssignments.Count > 0)
        {
            AssignedMembers = _db.Members.Where(m => targetAssignments.Contains(m.Id)).ToList();
        }
        return Page();
    }
}