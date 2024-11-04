
namespace ToDo_Api.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region [Constructor]
        private readonly ILogger<UserController> _logger;
        private readonly IUser _userRepo;

        public UserController(IUser userRepo, ILogger<UserController> logger)
        {
            _logger = logger;
            _userRepo = userRepo;
        }
        #endregion

        #region [Get Users]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetUsers(int page, int pageSize)
        {
            try
            {
                var users = await _userRepo.GetUsers(page, pageSize);

                var usersDTOs = users.Select(x => new UserDTO()
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Active = x.Active,
                }).ToList();

                return Ok(usersDTOs);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to GET USER BY ID: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Get User By Id]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                var user = await _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new { Message = "User does not exist" });
                }

                var userDTO = new UserDTO()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Active = user.Active,
                };

                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to GET USER BY ID: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Update User]
        [HttpPut("{id}")]
        public async Task<ActionResult<bool>> UpdateUser(int id, UpdateUser payload)
        {
            try
            {
                var isUpdated = await _userRepo.UpdateUser(id, payload);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update user" });
                }


                return Ok(new { Message = "User details updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to UPDATE USER: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Update User Active Status]
        [Authorize(Roles = "admin")]
        [HttpPut("update-useractivestatus")]
        public async Task<ActionResult<bool>> UpdateUserActiveStatus(UpdateUserActiveStatus payload)
        {
            try
            {
                var isUpdated = await _userRepo.UpdateUserActiveStatus(payload);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update useractive status" });
                }

                return Ok(new { Message = "User active status updated successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to UPDATE USER ACTIVE STATUS: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Update User Role]
        [Authorize(Roles = "admin")]
        [HttpPut("update-userrole")]
        public async Task<ActionResult<bool>> UpdateUserRole(UpdateUserRole payload)
        {
            try
            {
                List<string> validRoles = new List<string>() { "admin", "user" };

                if (!validRoles.Contains(payload.Role.ToLower()))
                {
                    return BadRequest(new { Message = "There is no such role" });
                }

                var isUpdated = await _userRepo.UpdateUserRole(payload);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update user role" });
                }

                return Ok(new { Message = "User role updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to UPDATE USER ROLE: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Delete User]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteUser(int id)
        {
            try
            {
                var isUpdated = await _userRepo.DeleteUser(id);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not delete user" });
                }

                return Ok(new { Message = "User deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the USER CONTROLLER while trying to DELETE USER: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        } 
        #endregion
    }
}
