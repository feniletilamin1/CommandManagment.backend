using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class ScrumBoard
    {
        public int Id { get; set; }
        [ForeignKey(nameof(User))]
        [JsonIgnore]
        public int UserId { get; set; }
        [JsonIgnore]
        public User? User { get; set; }
        [JsonIgnore]
        public int ProjectId { get; set; }
        [JsonIgnore]
        public Project? Project { get; set; }
        public List<ScrumBoardColumn> ScrumBoardColumns { get; set; }
        public List<ScrumBoardTask> ScrumBoardTasks { get; set; }

        public ScrumBoard()
        {
            ScrumBoardTasks = new List<ScrumBoardTask>();
            ScrumBoardColumns = new List<ScrumBoardColumn>();
        }
    }
}
