using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using System.Security.Claims;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public RestaurantsController(ApplicationDBContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/Restaurants
        [HttpGet]
        [Authorize(Roles = "CompanyAdministrator,RestaurantAdministrator")]
        public async Task<ActionResult<IEnumerable<Restaurant>>> GetRestaurants()
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            return await _context.Restaurants.ToListAsync();
        }
        [HttpGet("Menu/{id}")]
        public ActionResult<Restaurant> GetMenu(int id)
        {
            List<object> categoryList = new List<object>();
            List<object> foodList = new List<object>();

            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = _context.Restaurants.Include(r => r.Categories).ThenInclude(c => c.Foods).FirstOrDefault(r => r.Id == id);
            if (restaurant == null)
            {
                return NotFound();
            }
            return restaurant;
        }

        // GET: api/Restaurants/5
        [HttpGet("{id}")]
        [Authorize(Roles = "CompanyAdministrator,RestaurantAdministrator")]
        public async Task<ActionResult<Restaurant>> GetRestaurant(int id)
        {
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound();
            }

            return restaurant;
        }

        // PUT: api/Restaurants/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "RestaurantAdministrator")]
        public ActionResult PutRestaurant(int id, Restaurant restaurant)
        {

            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Content("Wrong User");
            }
            _context.Entry(restaurant).State = EntityState.Modified;
            _context.SaveChanges();
            return Ok();
        }

        // POST: api/Restaurants
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "CompanyAdministrator")]
        public int PostRestaurant(Restaurant restaurant)
        {
            ApplicationUser applicationUser = new ApplicationUser();
            Claim claim;

            _context.Restaurants.Add(restaurant);
            _context.SaveChanges();
            applicationUser.CompanyId = restaurant.CompanyId;
            applicationUser.Email = restaurant.EMail;
            applicationUser.Name = restaurant.Name;
            applicationUser.PhoneNumber = restaurant.Phone;
            applicationUser.RegisterDate = DateTime.Today;
            applicationUser.StateId = restaurant.StateId;
            applicationUser.UserName = restaurant.Name + "123";
            _userManager.CreateAsync(applicationUser, "Admin1234!").Wait();
            claim = new Claim("RestaurantId", restaurant.Id.ToString());
            _userManager.AddClaimAsync(applicationUser, claim).Wait();
            _userManager.AddToRoleAsync(applicationUser, "RestaurantAdministrator").Wait();
            return restaurant.Id;
        }

        // DELETE: api/Restaurants/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "CompanyAdministrator,RestaurantAdministrator")]
        public ActionResult DeleteRestaurant(int id)
        {
            if (User.HasClaim("RestaurantId", id.ToString()) == false)
            {
                return Unauthorized();
            }
            if (_context.Restaurants == null)
            {
                return NotFound();
            }
            var restaurant = _context.Restaurants.Find(id);
            if (restaurant == null)
            {
                return NotFound();
            }
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
            _context.SaveChanges();

            return NoContent();
        }

        private bool RestaurantExists(int id)
        {
            return (_context.Restaurants?.Any(e => e.Id == id)).GetValueOrDefault();
        }


    }
}