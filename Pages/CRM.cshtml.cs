using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Collections.Generic;
using System.Linq;

namespace VONetData.Pages;

[Authorize(Roles="Admin")]
public class CRMModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<IdentityUser> _userManager;

    public List<Member> RecentMembers { get; set; } = new();
    public List<IdentityUser> Users { get; set; } = new();

    [BindProperty] public int? EditMemberId { get; set; }
    [BindProperty] public string? Note { get; set; }
    [BindProperty] public string? AssignedTo { get; set; }
    [BindProperty] public string? Status { get; set; }

    // Simple in-memory dictionaries for new metadata (not persisted yet)
    public static Dictionary<int, List<string>> MemberNotes { get; } = new();
    public static Dictionary<int, string> MemberAssignments { get; } = new();
    public static Dictionary<int, string> MemberStatuses { get; } = new();

    public CRMModel(ApplicationDbContext db, UserManager<IdentityUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public void OnGet()
    {
        Load();
    }

    public IActionResult OnPostAddNote(int memberId)
    {
        if (!string.IsNullOrWhiteSpace(Note))
        {
            if (!MemberNotes.ContainsKey(memberId)) MemberNotes[memberId] = new List<string>();
            MemberNotes[memberId].Add(Note.Trim());
            Note = string.Empty;
        }
        Load();
        return Page();
    }

    public IActionResult OnPostAssign(int memberId)
    {
        if (!string.IsNullOrWhiteSpace(AssignedTo))
        {
            MemberAssignments[memberId] = AssignedTo.Trim();
        }
        Load();
        return Page();
    }

    public IActionResult OnPostStatus(int memberId)
    {
        if (!string.IsNullOrWhiteSpace(Status))
        {
            MemberStatuses[memberId] = Status.Trim();
        }
        Load();
        return Page();
    }

    public IActionResult OnPostDelete(int memberId)
    {
        var m = _db.Members.FirstOrDefault(x => x.Id == memberId);
        if (m != null)
        {
            _db.Members.Remove(m);
            _db.SaveChanges();
        }
        Load();
        return Page();
    }

    private void Load()
    {
        RecentMembers = _db.Members.OrderByDescending(m => m.Id).Take(50).ToList();
        Users = _userManager.Users.OrderBy(u => u.Email).Take(50).ToList();
    }
}