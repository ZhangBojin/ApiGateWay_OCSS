namespace ApiGateWay_OCSS.Domain.Entities
{
    public class Teachers
    {
        public required int TeacherId { get; set; }
        public required int UserId { get; set; }
        public string? Gender { get; set; }
        public long? PhoneNumber {  get; set; }
        //职称
        public string? Title {  get; set; }
        //所属院系
        public string? Department {  get; set; }
        //入职时间
        public DateTime? Joining { get; set; }
    }
}
