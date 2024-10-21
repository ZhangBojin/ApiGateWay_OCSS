namespace ApiGateWay_OCSS.Domain.Entities
{
    public class Users
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
