using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using VONetData.Models;
using System.Collections.Generic;
using System.Linq;

namespace VONetData.Pages;

[Authorize(Roles="Admin")]
public class CRMMemberModel : PageModel
{
    private readonly ApplicationDbContext _db;
    public Member? Member { get; set; }
    public List<string> Notes { get; set; } = new();
    public string? Status { get; set; }
    public string? AssignedTo { get; set; }

    public CRMMemberModel(ApplicationDbContext db) => _db = db;

    public IActionResult OnGet(int id)
    {
        Load(id);
        if (Member == null) return NotFound();
        return Page();
    }

    public IActionResult OnPostAddNote(int id, string? note)
    {
        Load(id);
        if (Member == null) return NotFound();
        if (!string.IsNullOrWhiteSpace(note))
        {
            if (!CRMModel.MemberNotes.ContainsKey(id)) CRMModel.MemberNotes[id] = new List<string>();
            CRMModel.MemberNotes[id].Add(note.Trim());
        }
        return RedirectToPage("/CRM/Member", new { id });
    }

    public IActionResult OnPostUpdateMeta(int id, string? status, string? assignedTo)
    {
        Load(id);
        if (Member == null) return NotFound();
        if (!string.IsNullOrWhiteSpace(status)) CRMModel.MemberStatuses[id] = status.Trim();
        if (!string.IsNullOrWhiteSpace(assignedTo)) CRMModel.MemberAssignments[id] = assignedTo.Trim();
        return RedirectToPage("/CRM/Member", new { id });
    }

    public IActionResult OnPostDelete(int id)
    {
        var m = _db.Members.FirstOrDefault(x => x.Id == id);
        if (m != null)
        {
            _db.Members.Remove(m);
            _db.SaveChanges();
            CRMModel.MemberNotes.Remove(id);
            CRMModel.MemberAssignments.Remove(id);
            CRMModel.MemberStatuses.Remove(id);
        }
        return RedirectToPage("/CRM");
    }

    private void Load(int id)
    {
        Member = _db.Members.FirstOrDefault(m => m.Id == id);
        if (Member != null)
        {
            if (CRMModel.MemberNotes.ContainsKey(id)) Notes = CRMModel.MemberNotes[id];
            Status = CRMModel.MemberStatuses.ContainsKey(id) ? CRMModel.MemberStatuses[id] : null;
            AssignedTo = CRMModel.MemberAssignments.ContainsKey(id) ? CRMModel.MemberAssignments[id] : null;
        }
    }
}