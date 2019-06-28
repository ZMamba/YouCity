using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DProject.Data;
using DProject.Models;
using Microsoft.EntityFrameworkCore;

namespace DProject.Controllers
{
    public class LikeController : Controller
    {
        private readonly DProjectContext _context;

        public LikeController(DProjectContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult Index(int propositionId, Like like)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);

            var isLiked = _context.Likes.FirstOrDefault(c => c.PropositionId == propositionId && c.UserId == user.Id);

            if (isLiked == null)
            {
                like.PropositionId = propositionId;
                like.UserId = user.Id;

                _context.Add(like);
                _context.SaveChanges();

                return RedirectToAction("Details", "Proposition", new { id = propositionId });
            }

            return RedirectToAction("Details", "Proposition", new { id = propositionId });
        }
    }
}