using Dapper;
using ToDo_Api.Data;
using ToDo_Api.Models;

namespace ToDo_Api.Repositories
{
    public interface IUser
    {
        public Task<bool> RegisterUser(User payload);
        public Task<List<User>> GetUsers();
        public Task<User> GetUserById(int id);
        public Task<User> UserExists(string email);
        public Task<User> LogIn(LogInModel payload);
        public Task<bool> UpdateUser(int Id, UpdateUser payload);
        public Task<bool> UpdateUserActiveStatus(UpdateUserActiveStatus payload);
        public Task<bool> UpdateUserRole(UpdateUserRole payload);
        public Task<bool> DeleteUser(int id);

    }
    public class UserRepo : IUser
    {
        private readonly ILogger<UserRepo> _logger;
        private readonly DapperContext _context;

        public UserRepo(DapperContext context, ILogger<UserRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> DeleteUser(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);

                string query = "DELETE FROM [dbo].[Users] WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to DELETE A USER: {ex}", ex.Message);
                return default;
            }
        }

        public Task<User> GetUserById(int id)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Id", id);

                string query = "SELECT * FROM [dbo].[Users] WHERE Id = @Id";

                return _context.QuerySingleRow<User>(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to GET A USER BY ID: {ex}", ex.Message);
                return default;
            }
        }

        public Task<List<User>> GetUsers()
        {
            try
            {
                string query = "SELECT * FROM [dbo].[Users]";

                return _context.QueryMultipleRows<User>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to GET ALL USERS: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<User> LogIn(LogInModel payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Email", payload.Email.ToLower());

                string query = "SELECT * FROM [dbo].[Users] WHERE Email = @Email AND Active = 'true' ";

                var user = await _context.QuerySingleRow<User>(query, parameters);

                if (user == null)
                {
                    return null;
                }

                bool isValid = BCrypt.Net.BCrypt.Verify(payload.Password, user.PasswordHash);

                if (!isValid)
                {
                    return null;
                }

                return user;

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to LOG IN: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> RegisterUser(User payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Email", payload.Email.ToLower());
                parameters.Add("FirstName", payload.FirstName.ToLower());
                parameters.Add("LastName", payload.LastName.ToLower());
                parameters.Add("Role", payload.Role.ToLower());
                parameters.Add("PasswordHash", BCrypt.Net.BCrypt.HashPassword(payload.PasswordHash));
                parameters.Add("Active", payload.Active);

                string query = @"INSERT INTO [dbo].[Users](FirstName, LastName,Email, Role, PassWordHash, Active)
                                VALUES(@FirstName, @LastName, @Email, @Role, @PassWordHash, @Active)";

                return await _context.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to REGISTER A USER: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> UpdateUser(int Id, UpdateUser payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Id", Id);
               
                string initialQuery = "UPDATE [dbo].[Users] SET ";
                string addingQuery = string.Empty;
                string finalQuery = string.Empty;

                if (!string.IsNullOrWhiteSpace(payload.FirstName))
                {
                    parameters.Add("FirstName", payload.FirstName.ToLower());
                    addingQuery = ", FirstName = @FirstName";
                }

                if (!string.IsNullOrWhiteSpace(payload.LastName))
                {
                    parameters.Add("LastName", payload.LastName.ToLower());
                    addingQuery = ", LastName = @LastName";
                }

                if (!string.IsNullOrWhiteSpace(payload.Email))
                {
                    parameters.Add("Email", payload.Email.ToLower());
                    addingQuery = ", Email = @Email";
                }

                finalQuery = initialQuery + addingQuery.Substring(1) + " WHERE Id = @Id";

                return await _context.ExecuteQuery(finalQuery, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to UPDATE A USER: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> UpdateUserActiveStatus(UpdateUserActiveStatus payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", payload.Id);
                parameters.Add("Active", payload.Active);

                string query = "UPDATE [dbo].[Users] SET Active = @Active WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameters);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to UPDATE USER ACTIVE STATUS: {ex}", ex.Message);
                return default;
            }
        }

        public async Task<bool> UpdateUserRole(UpdateUserRole payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("Id", payload.Id);
                parameters.Add("Role", payload.Role.ToLower());

                string query = "UPDATE [dbo].[Users] SET Role = @Role WHERE Id = @Id";

                return await _context.ExecuteQuery(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to UPDATE USER ROLE STATUS: {ex}", ex.Message);
                return default;
            }
        }

        public Task<User> UserExists(string email)
        {
            try
            {
                var parameter = new DynamicParameters();
                parameter.Add("Email", email.ToLower());

                string query = "SELECT * FROM [dbo].[Users] WHERE Email = @Email";

                return _context.QuerySingleRow<User>(query, parameter);

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in the User Repo while trying to CHECK IF USER EXISTS: {ex}", ex.Message);
                return default;
            }
        }
    }
}
