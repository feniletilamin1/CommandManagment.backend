namespace CommandManagment.backend.Models
{
    public class ResetPasswordToken
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Token {  get; set; }
    }
}
