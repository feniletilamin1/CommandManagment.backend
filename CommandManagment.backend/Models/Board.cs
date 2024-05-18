using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class Board
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        [JsonIgnore]
        public int? UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }
        public List<BoardColumn> ScrumBoardColumns { get; set; }
        public List<BoardTask> ScrumBoardTasks { get; set; }
        [NotMapped]
        public List<User> TeamUsers { get; set; }
        public Board()
        {
            ScrumBoardTasks = new List<BoardTask>();
            ScrumBoardColumns = new List<BoardColumn>();
            TeamUsers = new List<User>();
        }
    }
}
