namespace ApiGateWay_OCSS.Domain.Entities
{
    public class UserRole
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int RoleId { get; set; }
        public DateTime? CreationTime { get; set; }

    }
}
