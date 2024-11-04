using Dapper;
using System.Linq.Expressions;
using ToDo_Api.Data;
using ToDo_Api.Models;

namespace ToDo_Api.Repositories
{
    #region [Interface]
    public interface IStatus
    {
        public Task<bool> AddStatus(string name);
        public Task<List<Status>> GetStatuses();
        public Task<Status> GetStatusById(int id);
        public Task<bool> UpdateStatus(Status payload);
        public Task<bool> DeleteStatus(int id);
    } 
    #endregion
    public class StatusRepo : IStatus
    {
        #region [Constructor]
        private readonly ILogger<StatusRepo> _logger;
        private readonly DapperContext _context;

        public StatusRepo(DapperContext context, ILogger<StatusRepo> logger)
        {
            _context = context;
            _logger = logger;
        }
        #endregion


        #region [Add Status]
        public async Task<bool> AddStatus(string name)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", name.ToLower());

                string query = "INSERT INTO [dbo].[Status](Name) VALUES(@Name)";

                return await _context.ExecuteQuery(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS REPO while trying to ADD A STATUS: {ex}", ex.Message);
                return default;
            }
        }
        #endregion

        #region [Delete Status]
        public async Task<bool> DeleteStatus(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", id);

                string query = "DELETE FROM [dbo].[Status] WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS REPO while trying to DELETE A STATUS: {ex}", ex.Message);
                return default;
            }
        }
        #endregion

        #region [Get Status By Id]
        public async Task<Status> GetStatusById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", id);

                string query = "SELECT * FROM [dbo].[Status] WHERE Id = @Id";

                return await _context.QuerySingleRow<Status>(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS REPO while trying to GET STATUS BY ID: {ex}", ex.Message);
                return default;
            }
        }
        #endregion

        #region [Get Statuses]
        public async Task<List<Status>> GetStatuses()
        {
            try
            {
                string query = "SELECT * FROM [dbo].[Status]";

                return await _context.QueryMultipleRows<Status>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS REPO while trying to GET ALL STATUSES: {ex}", ex.Message);
                return default;
            }
        }
        #endregion

        #region [Update Status]
        public async Task<bool> UpdateStatus(Status payload)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", payload.Id);
                parameter.Add("@Name", payload.Name.ToLower());

                string query = "UPDATE [dbo].[Status] SET Name = @Name WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameter);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the STATUS REPO while trying to UPDATE STATUS: {ex}", ex.Message);
                return default;
            }
        } 
        #endregion
    }
}
