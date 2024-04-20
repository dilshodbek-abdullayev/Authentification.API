using Auth.DTOs;
using Auth.MyModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Auth.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<ActionResult<string>> Regsiter(RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var user = new AppUser
            { 
                 FullName= registerDTO.FullName,
                 UserName= registerDTO.Email,
                 Email= registerDTO.Email,
                 Age = registerDTO.Age,

             
            
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            foreach (var role in registerDTO.Roles)
            {
                await _userManager.AddToRoleAsync(user, role);
            }
            return Ok(result);
        }
        [HttpPost]
        public async Task<ActionResult<string>> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return Unauthorized("User not found with this Email");
            }

            var test = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

            if (!test)
            {
                return Unauthorized("Password invalid");
            }
            return Ok("Welcome");
        }
        [HttpGet]
        public async Task<ActionResult<string>> GetAllUsers() 
        {
            var result = await _userManager.Users.ToListAsync();
            return Ok(result);
         
        }
        [HttpGet]
        public async Task<ActionResult<string>> GetByIdUser(string id)
        {
            var result = await _userManager.FindByIdAsync(id);
            return Ok(result);
        }
    }
}
