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
using System.Security.Claims;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [HttpGet]
        [Authorize(Roles = "RestaurantAdministrator")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            return await _context.Categories.ToListAsync();
        }

        // GET: api/Categories/5
        [HttpGet("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            if (_context.Categories == null)
            {
                return NotFound();
            }
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult PutCategory(int id, Category category)
        {
            id = category.RestaurantId;
            if (User.HasClaim("RestauranId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            _context.Entry(category).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();

        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "RestaurantAdministrator")]
        public string PostCategory(Category category)
        {

            _context.Categories.Add(category);
            _context.SaveChanges();

            return category.Name;
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult DeleteCategory(int id)
        {
            var resId = _context.Categories.Find(id);
            if (User.HasClaim("RestauranId", resId.ToString()) == false)
            {
                return Unauthorized();
            }
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var category = _context.Categories.Find(id);
            if (category != null)
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

            _context.SaveChanges();

            return NoContent();
        }
        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}