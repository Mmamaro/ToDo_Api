using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo_Api.Data;
using ToDo_Api.Models;
using ToDo_Api.Models.DTOs;
using ToDo_Api.Repositories;

namespace ToDo_Api.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        #region [Constructor]
        private readonly ILogger<AuthController> _logger;
        private readonly IUser _userRepo;
        private readonly TokenService _tokenService;

        public AuthController(IUser userRepo, ILogger<AuthController> logger, TokenService tokenService)
        {
            _logger = logger;
            _userRepo = userRepo;
            _tokenService = tokenService;
        }
        #endregion


        #region [Register User]
        [HttpPost("register")]
        public async Task<ActionResult<bool>> RegisterUser(RegisterUser payload)
        {
            try
            {

                if (payload.Password != payload.ConfirmPassword)
                {
                    return BadRequest(new { Message = "Passwords don't match" });
                }

                var userxists = await _userRepo.UserExists(payload.Email);

                if (userxists != null)
                {
                    return BadRequest(new { Message = "User email already exists" });
                }

                var user = new User()
                {
                    FirstName = payload.FirstName,
                    LastName = payload.LastName,
                    Email = payload.Email,
                    Role = "user",
                    Active = true,
                    PasswordHash = payload.Password
                };

                var isAdded = await _userRepo.RegisterUser(user);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not rgister user" });
                }

                return Ok(new { Message = "User registered successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to REGISTER USER: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Log in]
        [HttpPost("login")]
        public async Task<ActionResult<User>> LogIn(LogInModel payload)
        {
            try
            {
                var user = await _userRepo.LogIn(payload);

                if (user == null)
                {
                    return BadRequest(new { Message = "Incorrect Credentials" });
                }

                string token = await _tokenService.GenerateToken(user);

                var Credentials = new Credentials()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Role = user.Role,
                    Active = user.Active,
                    Token = token
                };


                return Ok(Credentials);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to LOG IN: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        } 
        #endregion
    }
}
