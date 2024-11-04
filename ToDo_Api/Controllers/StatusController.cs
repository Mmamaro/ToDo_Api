using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ToDo_Api.Models;
using ToDo_Api.Repositories;

namespace ToDo_Api.Controllers
{
    [Authorize]
    [Route("api/statuses")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        #region [Constructor]
        private readonly IStatus _statusRepo;
        private readonly ILogger<StatusController> _logger;

        public StatusController(IStatus statusRepo, ILogger<StatusController> logger)
        {
            _statusRepo = statusRepo;
            _logger = logger;
        }
        #endregion

        #region [Add Status]
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<bool>> AddStatus(string name)
        {
            try
            {

                var isAdded = await _statusRepo.AddStatus(name);

                if (!isAdded)
                {
                    return BadRequest(new { Message = "Could not add status" });
                }

                return Ok(new { Message = "Status added successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS CONTROLLER while trying to ADD A STATUS: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Get Statuses]
        [HttpGet]
        public async Task<ActionResult<List<Status>>> GetStatuses()
        {
            try
            {
                var statuses = await _statusRepo.GetStatuses();

                return statuses;
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS CONTROLLER while trying to Get all statuses: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Get Status By Id]
        [HttpGet("{id}")]
        public async Task<ActionResult<Status>> GetStatusById(int id)
        {
            try
            {
                var status = await _statusRepo.GetStatusById(id);

                if (status == null)
                {
                    return NotFound(new { Message = "Status id does not exist" });
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS CONTROLLER while trying to GET STATUS BY ID: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Update Status]
        [Authorize(Roles = "admin")]
        [HttpPut]
        public async Task<ActionResult<Status>> UpdateStatus(Status payload)
        {
            try
            {
                var status = await _statusRepo.GetStatusById(payload.Id);

                if (status == null)
                {
                    return NotFound(new { Message = "Status id does not exist" });
                }

                var isUpdated = await _statusRepo.UpdateStatus(payload);

                if (!isUpdated)
                {
                    return BadRequest(new { Message = "Could not update status" });
                }

                return Ok(new { Message = "Status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS CONTROLLER while trying to update status: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        }
        #endregion

        #region [Delete Status]
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<bool>> DeleteStatus(int id)
        {
            try
            {
                var status = await _statusRepo.GetStatusById(id);

                if (status == null)
                {
                    return NotFound(new { Message = "Status id does not exist" });
                }

                var isDeleted = await _statusRepo.DeleteStatus(id);

                if (!isDeleted)
                {
                    return BadRequest(new { Message = "Could not delete status" });
                }

                return Ok(new { Message = "Status deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS CONTROLLER while trying to delete a status: {ex}", ex.Message);
                return BadRequest(new { Message = "Encountered an error" });
            }
        } 
        #endregion
    }
}
