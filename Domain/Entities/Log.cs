namespace ApiGateWay_OCSS.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public DateTime CreatTime { get; set; }
        public required string Level { get; set; }
        public required string Msg { get; set; }
        public required string Controller { get; set; }
        public required string Action { get; set; }
        public int UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
    }
}
