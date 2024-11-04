using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ToDo_Api.Models;
using ToDo_Api.Repositories;

namespace ToDo_Api.Controllers
{
    [Authorize]
    [Route("api/tasks")]
    [ApiController]
    public class TaskController : ControllerBase
    {
        private readonly ITask _taskRepo;
        private readonly ILogger<TaskController> _logger;
        private readonly IUser _userRepo;
        private readonly IStatus _statusRepo;

        public TaskController(ITask taskRepo, ILogger<TaskController> logger, IUser userRepo, IStatus statusRepo)
        {
            _logger = logger;
            _taskRepo = taskRepo;
            _userRepo = userRepo;
            _statusRepo = statusRepo;
        }

        [HttpGet]
        public async Task<ActionResult<List<ToDo_Api.Models.Task>>> GetTasks(int page, int pageSize)
        {
            try
            {
                var tasks = await _taskRepo.GetTasks(page, pageSize);

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to GET ALL TASKS: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }

        [HttpGet("userid/{id}")]
        public async Task<ActionResult<List<ToDo_Api.Models.Task>>> GetTasksByUserId(int id)
        {
            try
            {

                var user = await _userRepo.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new {Message = "User does not exist"});
                }

                var tasks = await _taskRepo.GetTasksByUserId(id);

                if (tasks == null)
                {
                    return NotFound(new { Message = "No tasks for this user" });
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to GET TASKS BY USER ID: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ToDo_Api.Models.Task>> GetTaskById(int id)
        {
            try
            {

                var task = await _taskRepo.GetTaskById(id);

                if (task == null)
                {
                    return NotFound(new { Message = "Task does not exist" });
                }

                return Ok(task);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to GET TASK BY ID : {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<bool>> AddTask(AddTask payload)
        {
            try
            {
                var status = await _statusRepo.GetStatusById(payload.StatusId);

                if (status == null)
                {
                    return NotFound(new {Message = "Status Id does not exist"});
                }

                var user = await _userRepo.GetUserById(payload.UserId);

                if (user == null)
                {
                    return NotFound(new { Message = "User Id does not exist" });
                }

                var isAdded = await _taskRepo.AddTask(payload);

                if (!isAdded)
                {
                    return BadRequest(new {Message = "Could not add task"});
                }

                return Ok(new {Message = "Task added successfully"});
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to ADD A TASK: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }

        [HttpPut("id")]
        public async Task<ActionResult<bool>> UpdateTask(int id, UpdateTask payload)
        {
            try
            {
                var currentlyLoggedUser = User.FindFirstValue(ClaimTypes.Email);

                var task = await _taskRepo.GetTaskById(id);

                if (task == null)
                {
                   return NotFound(new { Message = "Task does not exist" });
                }

                if (task.Email != currentlyLoggedUser)
                {
                    return Unauthorized( new {Message = "Attemping to update a Task that dos not belong to you"});
                }

                var isUpdated = await _taskRepo.UpdateTask(id, payload);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update task" });
                }

                return Ok(new { Message = "Task updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to UPDATE A TASK: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteTask(int id)
        {
            try
            {
                var currentlyLoggedUser = User.FindFirstValue(ClaimTypes.Email);

                var task = await _taskRepo.GetTaskById(id);

                if (task == null)
                {
                    return NotFound(new { Message = "Task does not exist" });
                }

                if (task.Email != currentlyLoggedUser)
                {
                    return Unauthorized(new { Message = "Attemping to delete a Task that dos not belong to you" });
                }

                var isDeleted = await _taskRepo.DeleteTask(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete task" });
                }

                return Ok(new { Message = "Task deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the TASK CONTROLLER while trying to DELETE A TASK: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
    }
}
//To do
//Add ON DELETE CASCADE ON THE TASK TABLE