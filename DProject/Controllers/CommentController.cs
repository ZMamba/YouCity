using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using DProject.Data;
using Microsoft.EntityFrameworkCore;
using DProject.Models;

namespace DProject.Controllers
{
    public class CommentController : Controller
    {
        private readonly DProjectContext _context;

        public CommentController(DProjectContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "user")]
        public IActionResult Create(int? propositionId)
        {
            if (propositionId == null)
            {
                return NotFound();
            }

            ViewData["proposition"] = propositionId;

            return View();
        }

        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateComment(int propositionId, [Bind("Content, PublicationDate, UserId, PropositionId")] Comment comment)
        {
            
            var user = GetUser();

            try
            {
                if (ModelState.IsValid)
                {
                    comment.PublicationDate = DateTime.Now;
                    comment.UserId = user.Id;
                    comment.PropositionId = propositionId;
                    _context.Add(comment);
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Details", "Proposition", new { id = propositionId });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }

            return View(comment);
        }

        private User GetUser()
        {
            return _context.Users.Include(p => p.Propositions).ThenInclude(e => e.City).AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
        }
    }
}