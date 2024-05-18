using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class BoardTask
    {
        public Guid? Id { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }
        public bool IsDone { get; set; }
        public int? ScrumBoardId { get; set; }
        [JsonIgnore]
        public Board? ScrumBoard { get; set; }
        public Guid? ScrumBoardColumnId { get; set; }
        [JsonIgnore]
        public BoardColumn? ScrumBoardColumn { get; set; }
        [ForeignKey(nameof(User))]
        public int ResponsibleUserId { get; set; }
        public User? ResponsibleUser { get; set; }
        public DateTimeOffset DateTimeCreated { get; set; }
        public int PriorityIndex { get; set; }
        public DateTimeOffset DateTimeEnd { get; set; }
        public bool IsArchived { get; set; }
    }
}
