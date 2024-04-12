using er_transformer_huawei_int.Models;
using er_transformer_huawei_int.Models.Dto;

namespace er_transformer_huawei_int.Data
{
    public interface IMongoRepository
    {
        Task<TokenDto> SetToken(string token, string user);
        Task<List<TokenDto>> GetToken(string user);
    }
}
