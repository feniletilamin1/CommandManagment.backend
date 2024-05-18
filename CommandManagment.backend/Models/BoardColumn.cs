using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class BoardColumn
    {
        public Guid? Id { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int ScrumBoardId { get; set; }
        [JsonIgnore]
        public Board? ScrumBoard { get; set; }
        [JsonIgnore]
        public List<BoardTask>? ScrumBoardTasks { get; set; }

        public BoardColumn()
        {
            ScrumBoardTasks = new List<BoardTask>();
        }
    }
}
