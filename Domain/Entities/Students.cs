namespace ApiGateWay_OCSS.Domain.Entities;

public class Students
{
    public required int StudentId { get; set; }
    public required int UserId { get; set; }
    public string? Gender { get; set; }
    public long? PhoneNumber { get; set; }
    public string? Address { get; set; }
    //专业
    public string? Major { get; set; }
    //学籍（全/非全日制）
    public required string EnrollmentStatus { get; set; }
    //所属院系
    public string? Department { get; set; }
    //入学时间
    public DateTime? Joining { get; set; }
    //状态（在读，休学）
    public required string Status { get; set; }
}