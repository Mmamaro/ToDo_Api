

namespace ToDo_Api.Models.DTOs
{
    public class TaskDTO
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int StatusId { get; set; }
        public int UserId { get; set; }
    }
}
