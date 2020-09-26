﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using WebAppProject.Data;
using WebAppProject.Models;

namespace WebAppProject.Controllers
{
    public class ManagerOverviewController : Controller
    {
        private readonly MvcProjectContext _context;

        public ManagerOverviewController(MvcProjectContext context)
        {
            _context = context;
        }

        // GET: ManagerOverview
        public async Task<IActionResult> Index()
        {
            // Delete When Finish Debugging !!!
            HttpContext.Session.SetString("IsAdmin", "true");
            // ------------------------------------------------------------

            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            ViewModel viewModel = new ViewModel
            {
                Users = await _context.User.ToListAsync(),
                WaterTransactions = await _context.WaterTransactions.ToListAsync(),
                ElectricityTransaction = await _context.ElectricityTransactions.ToListAsync(),
                PropertyTaxTransactions = await _context.PropertyTaxTransactions.ToListAsync()
            };
            return View(viewModel);
        }

        // GET: ManagerOverview/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // GET: ManagerOverview/Create
        public IActionResult Create()
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            return View();
        }

        // POST: ManagerOverview/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID")] ViewModel viewModel)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (ModelState.IsValid)
            {
                _context.Add(viewModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: ManagerOverview/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel.FindAsync(id);
            if (viewModel == null)
            {
                return NotFound();
            }
            return View(viewModel);
        }

        // POST: ManagerOverview/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID")] ViewModel viewModel)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id != viewModel.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(viewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ViewModelExists(viewModel.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(viewModel);
        }

        // GET: ManagerOverview/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            if (id == null)
            {
                return NotFound();
            }

            var viewModel = await _context.ViewModel
                .FirstOrDefaultAsync(m => m.ID == id);
            if (viewModel == null)
            {
                return NotFound();
            }

            return View(viewModel);
        }

        // POST: ManagerOverview/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            var viewModel = await _context.ViewModel.FindAsync(id);
            _context.ViewModel.Remove(viewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SearchAndResult(string SearchDB, string SearchCatagory, Config.TransactionStatus wantedStatus, string? SearchKeyWord)
        {
            ViewData["AfterSearch"] = true;

            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            ViewModel model = new ViewModel();

            // Init:
            model.Users = null;
            model.WaterTransactions = null;
            model.ElectricityTransaction = null;
            model.PropertyTaxTransactions = null;

            switch (SearchDB)
            {
                case "User":
                    if (SearchCatagory.Equals("UserID"))
                    {
                       model.Users = await _context.User.Where(user => user.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }   
                    else if (SearchCatagory.Equals("FirstName"))
                    {
                        model.Users = await _context.User.Where(user => user.FirstName.Contains(SearchKeyWord)).ToListAsync();
                    } 
                    else if (SearchCatagory.Equals("LastName"))
                    {
                        model.Users = await _context.User.Where(user => user.LastName.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("Email"))
                    {
                        model.Users = await _context.User.Where(user => user.Email.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("City"))
                    {
                        model.Users = await _context.User.Where(user => user.PropertyCity.Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("EnteranceDate")) {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.Users = await _context.User.Where(user => user.EnteranceDate.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("IsAdmin"))
                    {
                        model.Users = await _context.User.Where(user => user.IsAdmin.Equals(true)).ToListAsync();
                    }

                    break;

                case "WaterTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    } 
                    else if (SearchCatagory.Equals("WaterMeterID"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.WaterMeterID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterLastReadDate"))
                    {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.WaterMeterLastReadDate.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterTransactionStatus"))
                    {
                        model.WaterTransactions = await _context.WaterTransactions.Where(water => water.Status.Equals(wantedStatus)).ToListAsync();
                    }

                    break;

                case "ElectricityTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterID"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.ElectricityMeterID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterLastReadDate"))
                    {
                        var ParsedDate = DateTime.Parse(SearchKeyWord);
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.ElectricityMeterLastRead.Equals(ParsedDate)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ElectricityTransactionStatus"))
                    {
                        model.ElectricityTransaction = await _context.ElectricityTransactions.Where(electricity => electricity.Status.Equals(wantedStatus)).ToListAsync();
                    }
                    
                    break;

                case "PropertyTaxTransaction":
                    if (SearchCatagory.Equals("UserID"))
                    {
                       model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.UserID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("WaterMeterID"))
                    {
                        model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.PropertyID.ToString().Contains(SearchKeyWord)).ToListAsync();
                    }
                    else if (SearchCatagory.Equals("ElectricityTransactionStatus"))
                    {
                        model.PropertyTaxTransactions = await _context.PropertyTaxTransactions.Where(property => property.Status.Equals(wantedStatus)).ToListAsync();
                    }

                    break;

                default:
                    break;
            }


            return View(model);
        }

        public IActionResult Stats()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SearchAndResult()
        {
            ViewData["AfterSearch"] = false;

            var NotAdminRedirection = ValidateAdmin(this.HttpContext);
            if (NotAdminRedirection != null)
            {
                return NotAdminRedirection; // Redirect to Home/Index
            }

            return View();
        }

        private bool ViewModelExists(int id)
        {
            return _context.ViewModel.Any(e => e.ID == id);
        }

        public ActionResult ValidateAdmin (HttpContext context)
        {
            return context.Session.GetString("IsAdmin") == "true" ? null : RedirectToAction("Index", "Home");
        }
    }
}
