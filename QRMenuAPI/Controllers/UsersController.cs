using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QRMenuAPI.Data;
using QRMenuAPI.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace QRMenuAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDBContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDBContext context, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: api/Users
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _signInManager.UserManager.Users.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public ActionResult PutApplicationUser(string id, ApplicationUser applicationUser/*, string? password = null, string? currentPassword = null*/)
        {
            var existingApplicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;

            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;

            _signInManager.UserManager.UpdateAsync(existingApplicationUser).Wait();

            //if(password != null)
            //{
            //    _userManager.ChangePasswordAsync(existingApplicationUser, currentPassword, password);

            //}

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Roles = "Administrator, CompanyAdministrator")]
        public async Task<ActionResult<ApplicationUser>> PostApplicationUser(ApplicationUser applicationUser, string password)
        {
            await _signInManager.UserManager.CreateAsync(applicationUser, password);
            _context.Users.Add(applicationUser);

            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteApplicationUser(string id)
        {
            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;

            await _signInManager.UserManager.UpdateAsync(applicationUser);

            return NoContent();
        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        [HttpPost("Login")]
        public bool Login(string username, string password)
        {
            Microsoft.AspNetCore.Identity.SignInResult signInResult;
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            Claim claim;

            if (applicationUser == null)
            {
                return false;
            }
            signInResult = _signInManager.PasswordSignInAsync(applicationUser, password, false, false).Result;
            if (signInResult.Succeeded == true)
            {
                claim = new Claim("CompanyId", applicationUser.CompanyId.ToString());
                _signInManager.UserManager.AddClaimAsync(applicationUser, claim).Wait();
            }
            return signInResult.Succeeded;
        }
        [HttpPost("ResetPassword")]
        public void ResetPassword(string username, string password)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            if (applicationUser == null)
            {
                return;
            }
            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser, password);

        }
        [HttpPost("PasswordReset")]
        public string? PasswordReset(string username)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            if (applicationUser == null)
            {
                return null;
            }
            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;


        }

        [HttpPost("ValideToken")]
        public ActionResult<string?> ValidateToken(string username, string token, string newPassword)
        {
            ApplicationUser? applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            if (applicationUser == null)
            {
                return NotFound();
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassword).Result;
            if (identityResult.Succeeded == false)
            {
                return identityResult.Errors.First().Description;
            }

            return Ok();
        }
        [HttpPost("AssignRole")]
        [Authorize(Roles = "Administrator")]
        public void AssignRole(string username, string roleName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(username).Result;
            IdentityRole identityRole = _roleManager.FindByNameAsync(roleName).Result;

            _signInManager.UserManager.AddToRoleAsync(applicationUser, identityRole.Name).Wait();
        }
    }
}