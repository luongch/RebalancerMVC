using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RebalancerMVC.Data;
using RebalancerMVC.Models;

namespace RebalancerMVC.Controllers
{
    public class EtfsController : Controller
    {
        private readonly EtfContext _context;

        public EtfsController(EtfContext context)
        {
            _context = context;
        }

        // GET: Etfs
        public async Task<IActionResult> Index()
        {
            var etfs = await _context.Etf.ToListAsync();
            var etfViewModel = new EtfViewModel
            {
                Etfs = etfs
            };
            ViewBag.isValid = IsPortfolioBalanceValid(etfs);

            return View(etfViewModel);
        }

        // GET: Etfs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Etfs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,Percentage")] Etf etf)
        {
            if (ModelState.IsValid)
            {
                _context.Add(etf);
                await _context.SaveChangesAsync();
                TempData["Success"] = "The ETF has been created";
                return RedirectToAction(nameof(Index));
            }
            return View(etf);
        }

        // GET: Etfs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etf = await _context.Etf.FindAsync(id);
            if (etf == null)
            {
                return NotFound();
            }
            return View(etf);
        }

        // POST: Etfs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,Percentage")] Etf etf)
        {
            if (id != etf.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(etf);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EtfExists(etf.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["Success"] = "The ETF has been edited";
                return RedirectToAction(nameof(Index));
            }
            return View(etf);
        }

        // GET: Etfs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var etf = await _context.Etf
                .FirstOrDefaultAsync(m => m.Id == id);
            if (etf == null)
            {
                return NotFound();
            }

            return View(etf);
        }

        // POST: Etfs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var etf = await _context.Etf.FindAsync(id);
            _context.Etf.Remove(etf);
            await _context.SaveChangesAsync();
            TempData["Success"] = "The ETF has been deleted";
            return RedirectToAction(nameof(Index));
        }

        
        [HttpGet]
        public async Task<IActionResult> Rebalance(decimal cash)
        {
            var etfs = await _context.Etf.ToListAsync();

            var portfolioTotal = GetPortfolioTotal(etfs, cash);
            var results = RebalancePortfolio(etfs, portfolioTotal);

            var resultsVM = new ResultViewModel
            {
                Results = results
            };            

            return View(resultsVM);
        }

        private bool EtfExists(int id)
        {
            return _context.Etf.Any(e => e.Id == id);
        }

        private decimal GetPortfolioTotal(List<Etf> etfs, decimal cash)
        {
            decimal portfolioTotal = 0;
            portfolioTotal += cash;

            foreach (var etf in etfs)
            {
                portfolioTotal += (etf.Price * etf.Quantity);
            };

            return portfolioTotal;
        }

        private List<Result> RebalancePortfolio(List<Etf> etfs, decimal portfolioTotal)
        {
            List<Result> results = new List<Result>();
            foreach (var etf in etfs)
            {
                var expectedTotal = portfolioTotal * (etf.Percentage / 100);
                var expectedQuantity = Math.Floor(expectedTotal / etf.Price);
                var changeInQuantity = expectedQuantity - etf.Quantity;
                var action = changeInQuantity < 0 ? "Sell" : "Buy";

                
                var result = new Result
                {
                    Name = etf.Name,
                    Quantity = changeInQuantity < 0 ? Math.Abs(changeInQuantity) : changeInQuantity,
                    Action = action
                };
                results.Add(result);
                
            }

            return results;
        }

        private bool IsPortfolioBalanceValid(List<Etf> etfs)
        {
            decimal total = 0;
            foreach (var etf in etfs)
            {
                total += etf.Percentage;
            }

            return total <= 100;
        }
    }
}
