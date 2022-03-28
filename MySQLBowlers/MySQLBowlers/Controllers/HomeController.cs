using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySQLBowlers.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MySQLBowlers.Controllers
{
    public class HomeController : Controller
    {
        //private IBowlersRepository _repo {get; set;}
        private BowlersDbContext _context { get; set; }
        public HomeController(BowlersDbContext temp)
        {
            //_repo
            _context = temp;
        }

        public IActionResult Index()
        {
                        //_repo
            var blah = _context.Bowlers
                //.Include(x => Teams)
                .FromSqlRaw("SELECT * from  Bowlers WHERE BowlerFirstName LIKE 'a%'")
                .ToList();
            return View(blah);
        }
    }
}
