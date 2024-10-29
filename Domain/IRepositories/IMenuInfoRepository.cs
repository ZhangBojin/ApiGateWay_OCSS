using ApiGateWay_OCSS.Domain.Entities;

namespace ApiGateWay_OCSS.Domain.IRepositories
{
    public interface IMenuInfoRepository
    {
        Task<List<MenuInfo>> GetMenu(int roleId);
    }
}
