using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CompaniesController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Companies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Company>>> GetCompanies()
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            return await _context.Companies.ToListAsync();
        }

        // GET: api/Companies/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Company>> GetCompany(int id)
        {
            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = await _context.Companies.FindAsync(id);

            if (company == null)
            {
                return NotFound();
            }

            return company;
        }

        // PUT: api/Companies/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "CompanyAdministrator")]
        public async Task<IActionResult> PutCompany(int id, Company company)
        {
            if (User.HasClaim("CompanyId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            if (id != company.Id)
            {
                return BadRequest();
            }

            _context.Entry(company).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public int PostCompany(Company company)
        {

            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;
            _context.Companies.Add(company);
            _context.SaveChanges();
            applicationUser.CompanyId = company.Id;
            applicationUser.Email = company.EMail;
            applicationUser.Name = company.Name;
            applicationUser.PhoneNumber = company.Phone;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = 1;
            applicationUser.UserName = company.Name + company.Id.ToString();
            _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
            claim = new Claim("CompanyId", company.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "CompanyAdministrator").Wait();
            return company.Id;

        }

        // DELETE: api/Companies/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public ActionResult DeleteCompany(int id)
        {

            if (User.HasClaim("CompanyId", id.ToString()) == false && User.IsInRole("Administrator") == false)
            {
                return Unauthorized();
            }

            if (_context.Companies == null)
            {
                return NotFound();
            }
            var company = _context.Companies.Find(id);
            if (company != null)
            {
                company.StateId = 0;
                _context.Companies.Update(company);
                IQueryable<Restaurant> restaurants = _context.Restaurants.Where(r => r.CompanyId == id);
                foreach (Restaurant restaurant in restaurants)
                {
                    restaurant.StateId = 0;
                    _context.Restaurants.Update(restaurant);
                    IQueryable<Category> categories = _context.Categories.Where(c => c.RestaurantId == restaurant.Id);
                    foreach (Category category in categories)
                    {
                        category.StateId = 0;
                        _context.Categories.Update(category);
                        IQueryable<Food> foods = _context.Foods.Where(f => f.CategoryId == category.Id);
                        foreach (Food food in foods)
                        {
                            food.StateId = 0;
                            _context.Foods.Update(food);
                        }
                    }
                }
                IQueryable<ApplicationUser> users = _context.Users.Where(u => u.CompanyId == id);
                foreach (ApplicationUser user in users)
                {
                    user.StateId = 0;
                    _context.Users.Update(user);
                }
            }

            _context.SaveChanges();
            return Ok();
        }
        private bool CompanyExists(int id)
        {
            return (_context.Companies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}