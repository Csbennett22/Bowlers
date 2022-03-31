using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
            return RedirectToAction("ViewBowlers");
        }

        public IActionResult ViewBowlers(int teamid, string sqlstr)
        {
            ViewBag.Teams = _context.Teams.ToList();

            if (teamid == 0) //pass in the correct team name and information to be displayed
            {
                var stuff = new Team { TeamName = "", TeamID = 0 };
                ViewBag.TeamName = stuff;

                sqlstr = "SELECT * from  Bowlers "; //WHERE BowlerFirstName LIKE 'a%'
            }
            else
            {
                var stuff = _context.Teams.Single(x => x.TeamID == teamid);
                ViewBag.TeamName = stuff;
                sqlstr = "SELECT * from  Bowlers WHERE TeamID = " + teamid.ToString(); //

            }

            var blah = _context.Bowlers
                .FromSqlRaw(sqlstr) //
                .Include(x => x.Team)
                .ToList();

            return View(blah);
        }
        [HttpGet]
        public IActionResult AddBowler()
        {
            ViewBag.Teams = _context.Teams.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult AddBowler(Bowler bowler)
        {
            if (ModelState.IsValid)
            {
                var stuff = _context.Teams.Single(x => x.TeamID == bowler.TeamID);

                var blah = _context.Bowlers
                    .FromSqlRaw("SELECT * from  Bowlers ") //WHERE BowlerFirstName LIKE 'a%'
                    .Include(x => x.Team)
                    .ToList();

                _context.Update(stuff); //updates the timeslot table

                _context.Add(bowler);
                _context.SaveChanges();

                return RedirectToAction("ViewBowlers");
            }
            else //if invalid, send back to the form and see error messages
            {
                //ViewBag.Category = blahContext.Categories.ToList();
                return View(bowler);
            }
        }

        [HttpGet]
        public IActionResult EditBowler(int bowlerid) //we might have to make it so when an appointment is changed, the time slot is freed up and the new appointment is not available
        {
            ViewBag.Teams = _context.Teams.ToList();

            var stuff = _context.Bowlers.Single(x => x.BowlerID == bowlerid);

            return View("AddBowler", stuff);
        }

        [HttpPost]
        public IActionResult EditBowler(Bowler bowlerstuff)
        {

            if (ModelState.IsValid)
            {
                _context.Update(bowlerstuff);
                _context.SaveChanges();

                return RedirectToAction("ViewBowlers");
            }
            else
            {
                //ModelState.AddModelError("", "Make sure all fields are correctly filled out");
                ViewBag.Teams = _context.Teams.ToList();
                return View("AddBowler", bowlerstuff);
            }
        }


        [HttpGet]
        public IActionResult DeleteBowler(int bowlerid)
        {
            var to_delete = _context.Bowlers.Single(x => x.BowlerID == bowlerid);

            return View(to_delete);
        }

        [HttpPost]
        public IActionResult DeleteBowler(Bowler bowler, int bowlerid)
        {
            var appointment1 = _context.Bowlers.Single(x => x.BowlerID == bowlerid);

            _context.Bowlers.Remove(appointment1);
            _context.SaveChanges();

            return RedirectToAction("ViewBowlers");
        }
    }
}
