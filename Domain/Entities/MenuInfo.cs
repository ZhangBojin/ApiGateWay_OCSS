namespace ApiGateWay_OCSS.Domain.Entities
{
    public class MenuInfo
    {
        public int Id { get; set; }
        public required string Href { get; set; }
        public required string Name { get; set; }
        public required string Icon { get; set; }
        //序列
        public required int Sequence { get; set; }
        public required int RoleId { get; set; }
    }
}
