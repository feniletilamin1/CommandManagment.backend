namespace CommandManagment.backend.Dtos
{
    public class TeamDto
    {
        public int? Id { get; set; }
        public string TeamName { get; set; }
        public string TeamDescription { get; set; }
        public int? ManagerId { get; set; }
        public int CreateUserId { get; set; }
    }
}
