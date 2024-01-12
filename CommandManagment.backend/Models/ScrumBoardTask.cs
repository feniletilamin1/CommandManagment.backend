using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class ScrumBoardTask
    {
        public Guid? Id { get; set; }
        public int Order { get; set; }
        public string Content { get; set; }
        public int? ScrumBoardId { get; set; }
        [JsonIgnore]
        public ScrumBoard? ScrumBoard { get; set; }
        public Guid? ScrumBoardColumnId { get; set; }
        [JsonIgnore]
        public ScrumBoardColumn? ScrumBoardColumn { get; set; }
    }
}
