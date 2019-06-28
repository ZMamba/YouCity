using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DProject.Models;
using DProject.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DProject.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly DProjectContext _context;

        public HomeController(DProjectContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(
            string sortOrder,
            string currentFilter,
            string searchString,
            int? pageNumber)
        {
            //ViewData["CurrentSort"] = sortOrder;
            //ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            //ViewData["DateSortParm"] = sortOrder == "Date" ? "date_desc" : "Date";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var propositions = from p in _context.Propositions.Include(s => s.City).Include(a => a.User).Include(l => l.Likes)
                               .Include(c => c.Comments)
                               select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                propositions = propositions.Where(p => p.Title.Contains(searchString)
                                                        || p.Content.Contains(searchString));
            }

            //switch (sortOrder)
            //{
            //    case "name_desc":
            //        propositions = propositions.OrderByDescending(p => p.Title);
            //        break;
            //    case "Date":
            //        propositions = propositions.OrderBy(p => p.Date);
            //        break;
            //    case "date_desc":
            //        propositions = propositions.OrderByDescending(p => p.Date);
            //        break;
            //    default:
            //        propositions = propositions.OrderBy(p => p.Title);
            //        break;
            //}

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


            int pageSize = 8;

            return View(await PaginatedList<Proposition>.CreateAsync(propositions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        //public IActionResult Index()
        //{
        //    string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
        //    return Content($"ваша роль: {role}");
        //}



        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
