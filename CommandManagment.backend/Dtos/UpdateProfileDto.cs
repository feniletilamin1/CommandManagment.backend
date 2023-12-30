namespace CommandManagment.backend.Dtos
{
    public class UpdateProfileDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? MiddleName { get; set; }
        public IFormFile? Photo { get; set; }
        public string Specialization { get; set; }
    }
}
