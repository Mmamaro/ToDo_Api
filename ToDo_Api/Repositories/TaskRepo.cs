using Dapper;
using ToDo_Api.Data;
using ToDo_Api.Models;

namespace ToDo_Api.Repositories
{
    public interface ITask
    {
        public Task<List<ToDo_Api.Models.Task>> GetTasks(int page, int pageSize);
        public Task<List<ToDo_Api.Models.Task>> GetTasksByUserId(int id);
        public Task<ToDo_Api.Models.Task> GetTaskById(int id);
        public Task<bool> AddTask(AddTask payload);
        public Task<bool> UpdateTask(int Id, UpdateTask payload);
        public Task<bool> DeleteTask(int id);
    }
    public class TaskRepo : ITask
    {
        private readonly ILogger<TaskRepo> _logger;
        private readonly DapperContext _context;

        public TaskRepo(DapperContext context, ILogger<TaskRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> AddTask(AddTask payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Title", payload.Title.ToLower());
                parameters.Add("@Description", payload.Description != null ? payload.Description.ToLower() : null);
                parameters.Add("@DateCreated", DateTime.Now);
                parameters.Add("@DateModified", DateTime.Now);
                parameters.Add("@StatusId", payload.StatusId);
                parameters.Add("@UserId", payload.UserId);

                string query = @"INSERT INTO [dbo].[Task](Title, Description,DateCreated,DateModified,StatusId,UserId)
                                VALUES(@Title, @Description, @DateCreated, @DateModified, @StatusId, @UserId)";

                return await _context.ExecuteQuery(query, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying to ADD A TASK {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> DeleteTask(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", id);

                string query = "DELETE FROM [dbo].[Task] WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying to DELETE A TASK: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<ToDo_Api.Models.Task> GetTaskById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Id", id);

                string query = "SELECT * FROM todo_view WHERE Id = @Id";

                return await _context.QuerySingleRow<ToDo_Api.Models.Task>(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying to GET TASK BY ID: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<List<ToDo_Api.Models.Task>> GetTasks(int page, int pageSize)
        {
            try
            {
                var skip = (page - 1) * pageSize;
                var take = pageSize;

                string query = "SELECT * FROM todo_view";

                var data =  await _context.QueryMultipleRows<ToDo_Api.Models.Task>(query);

                var finalData = data.OrderBy(u => u.Id)
                                    .Skip(skip)
                                    .Take(take)
                                    .ToList();
                return finalData;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying to GET ALL TASKS: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<List<ToDo_Api.Models.Task>> GetTasksByUserId(int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", id);

                string query = @"SELECT T.Id, T.Title, T.Description, T.Description, T.DateCreated, T.DateModified, S.Name AS Status, U.Email
                                FROM [dbo].[Task] AS T
                                INNER JOIN [dbo].[Users] AS U
                                ON T.UserId = U.Id
                                INNER JOIN [dbo].[Status] AS S
                                ON T.StatusId = S.Id
                                WHERE U.Id = @Id";

                return await _context.QueryMultipleRowsWithParams<ToDo_Api.Models.Task>(query, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying GET TASKS BY USER ID: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> UpdateTask(int Id, UpdateTask payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id);
                parameters.Add("@DateModified", DateTime.Now);


                string initialQuery = "UPDATE [dbo].[Task] SET ";
                string addingQuery = string.Empty;
                string finalQuery = string.Empty;

                if (!string.IsNullOrWhiteSpace(payload.Title))
                {
                    parameters.Add("@Title", payload.Title.ToLower());
                    addingQuery += ",Title = @Title";
                }

                if (!string.IsNullOrWhiteSpace(payload.Description))
                {
                    parameters.Add("@Description", payload.Description.ToLower());
                    addingQuery += ", Description = @Description";
                }

                if (payload.StatusId.HasValue)
                {
                    parameters.Add("@StatusId", payload.StatusId);
                    addingQuery += ", StatusId = @StatusId";
                }

                finalQuery = initialQuery + addingQuery.Substring(1) + ",DateModified = @DateModified WHERE Id = @Id";

                return await _context.ExecuteQuery(finalQuery, parameters);


            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the Task REPO while trying to UPDATE A TASK {ex}", ex.Message);
                return default;
            }
        }
    }
}
