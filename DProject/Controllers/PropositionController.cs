using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DProject.Models;
using DProject.Data;
using Microsoft.EntityFrameworkCore;
using DProject.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace DProject.Controllers
{
    public class PropositionController : Controller
    {
        private readonly DProjectContext _context;

        public PropositionController(DProjectContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "user")]
        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;


            var propositions = from p in _context.Propositions.Include(s => s.City).Include(a => a.User)
                               where p.User.Email == User.Identity.Name
                               select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                propositions = propositions.Where(p => p.Title.Contains(searchString)
                                                        || p.Content.Contains(searchString));
            }

            ViewData["CurrentSort"] = sortOrder;
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "Date" : "";
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";

            switch (sortOrder)
            {
                case "name_desc":
                    propositions = propositions.OrderByDescending(p => p.Title);
                    break;
                case "Date":
                    propositions = propositions.OrderBy(p => p.Date);
                    break;
                case "Name":
                    propositions = propositions.OrderBy(p => p.Title);
                    break;
                default:
                    propositions = propositions.OrderByDescending(p => p.Date);
                    break;
            }

            int pageSize = 6;

            return View(await PaginatedList<Proposition>.CreateAsync(propositions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var proposition = _context.Propositions.Include(o => o.User).Include(c => c.Comments).ThenInclude(x => x.User).Include(l => l.Likes).
                                Include(c => c.City).FirstOrDefault(p => p.Id == id);

            if (proposition == null)
            {
                return NotFound();
            }


            var isLiked = "";
            if (User.Identity.IsAuthenticated)
            {
                //проверяем, лайкнул ли пользователь запись, если да - значит присваевам свойству display значение "none" и форма не отображается
                var user = _context.Users.FirstOrDefault(u => u.Email == User.Identity.Name);
                var like = _context.Likes.FirstOrDefault(c => c.PropositionId == id && c.UserId == user.Id);
                if (like != null)
                {
                    isLiked = "none";
                }
            }
            ViewData["isLiked"] = isLiked;

            return View(proposition);
        }

        [Authorize(Roles = "user")]
        public IActionResult Create()
        {
            var cities = _context.Cities.ToList();
            ViewData["Cities"] = cities;

            return View();
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title, Content, UserId, CityId, Date")] Proposition proposition)
        {
            var cities = await _context.Cities.ToListAsync();
            ViewData["Cities"] = cities;

            var user = GetUser();

            try
            {
                if (ModelState.IsValid)
                {
                    proposition.UserId = user.Id;
                    proposition.Date = DateTime.Now;
                    _context.Add(proposition);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Proposition");
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                    "Try again, and if the problem persists " +
                    "see your system administrator.");
            }
            return View(proposition);
        }

        //GET
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cities = _context.Cities.ToList();
            ViewData["Cities"] = cities;

            var proposition = await _context.Propositions.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);


            if (proposition == null)
            {
                return NotFound();
            }
            return View(proposition);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propositionToUpdate = await _context.Propositions.FirstOrDefaultAsync(p => p.Id == id);

            if (await TryUpdateModelAsync<Proposition>(
                propositionToUpdate, 
                "",
                p => p.Title, p => p.Content, p => p.Date, p => p.CityId))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. " +
                        "Try again, and if the problem persists, " +
                        "see your system administrator.");
                }
            }
            return View(propositionToUpdate);
        }

        public IActionResult Delete(int? id, bool? saveChangesError = false)
        {
            if (id == null)
            {
                return NotFound();
            }
            var user = GetUser();

            var proposition = user.Propositions.FirstOrDefault(p => p.Id == id);

            if (proposition == null)
            {
                return NotFound();
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] = "Delete failed. Try again, and if the problem persists " +
                                            "see your system administrator.";
            }

            return View(proposition);
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var user = GetUser();
            var proposition = user.Propositions.FirstOrDefault(p => p.Id == id);

            if (proposition == null)
            {
                return RedirectToAction(nameof(Index));
            }

            try
            {
                _context.Propositions.Remove(proposition);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }
        }

        private User GetUser()
        {
            return _context.Users.Include(p => p.Propositions).ThenInclude(e => e.City).AsNoTracking().FirstOrDefault(u => u.Email == User.Identity.Name);
        }

        private Proposition GetPropositionById(int? id)
        {
            var user = GetUser();

            return user.Propositions.FirstOrDefault(p => p.Id == id);
        }
    }
}