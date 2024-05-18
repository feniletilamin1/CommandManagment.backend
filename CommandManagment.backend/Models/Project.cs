using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class Project
    {
        public int Id { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(User))]
        public int CreateUserId { get; set; }
        public User? CreateUser { get; set; }
        public string Name { get; set; }
        public int TeamId { get; set; }
        public Team? Team { get; set; }
        [ForeignKey(nameof(Models.Board))]
        [JsonIgnore]
        public int BoardId { get; set; }
        public Board? Board { get; set; }
    }
}
