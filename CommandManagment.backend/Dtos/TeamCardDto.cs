using CommandManagment.backend.Models;

namespace CommandManagment.backend.Dtos
{
    public class TeamCardDto
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public List<User> users { get; set; }
    }


}
