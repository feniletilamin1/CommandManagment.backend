using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class ScrumBoardColumn
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int ScrumBoardId { get; set; }
        [JsonIgnore]
        public ScrumBoard? ScrumBoard { get; set; }
        [JsonIgnore]
        public List<ScrumBoardTask>? ScrumBoardTasks { get; set; }

        public ScrumBoardColumn()
        {
            ScrumBoardTasks = new List<ScrumBoardTask>();
        }
    }
}
