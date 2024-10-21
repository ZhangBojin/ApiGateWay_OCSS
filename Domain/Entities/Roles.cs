namespace ApiGateWay_OCSS.Domain.Entities
{
    public class Roles
    {
        public int Id { get; set; }
        public required string RoleName { get; set; }
        public DateTime? CreationTime { get; set; }
    }
}
