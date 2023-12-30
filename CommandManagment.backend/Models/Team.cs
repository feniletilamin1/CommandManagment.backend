using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CommandManagment.backend.Models
{
    public class Team
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public List<User> Users { get; set; }

        [ForeignKey(nameof(User))]
        public int CreateUserId { get; set; }
        [NotMapped]
        [JsonIgnore]
        public User? CreateUser { get; set; }

        public Team()
        {
            Users = new List<User>();
        }
    }
}
