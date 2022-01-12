using System.Security.Claims;
using System.Threading.Tasks;
using API.DTOs;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API.Controllers
{
    // Controller API and end points for User authentication
    [AllowAnonymous]
    [ApiController]
    [Route("api/[controller]")]
    // Scaffolding the class with ControllerBase
    public class AccountController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        // Constructor for managing users, sign ins, and tokens
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService)
        {
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Get route for login returns an Action Result, expected in the format of a UserDto.
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            // Grab email provided by client for login and see if exists
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            // If it doesn't exist in the database the return Unauthorized response
            if (user == null) return Unauthorized();
            // If it does exist in the database then check password provided against hash and salted password in database, 
            // False flag is so the user doesn't get locked out on failure
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            // If password check passes return a UserDto with DisplayName, Image, and auth token
            if (result.Succeeded)
            {
                return CreateUserObject(user);
            }
            // If password fails return Unauthorized response
            return Unauthorized();
        }

        // New User Register Endpoint, returns an Action Result, expected in the format of a UserDto.
        // A POST request
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            // Look for any matching email
            if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            {
                ModelState.AddModelError("email", "Email taken");
                return ValidationProblem();
            }
            // Look for any matching username
            if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            {
                ModelState.AddModelError("username", "Username taken");
                return ValidationProblem();
            }

            // If none build out user
            var user = new AppUser
            {
                DisplayName = registerDto.DisplayName,
                Email = registerDto.Email,
                UserName = registerDto.Username
            };
            // Added new user to the database
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                // Everything went well return newly created user
                return CreateUserObject(user);
            }
            // If something went wrong send BadRequest response
            return BadRequest("Problem registering user");
        }

        // Authorized Endpoint, returns an Action Result, expected in the format of a UserDto.
        // A GET Request
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<UserDto>> GetCurrentUser()
        {
            var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

            return CreateUserObject(user);
        }

        private UserDto CreateUserObject(AppUser user)
        {
            return new UserDto
            {
                DisplayName = user.DisplayName,
                Image = null,
                Token = _tokenService.CreateToken(user),
                Username = user.UserName
            };
        }
    }
}