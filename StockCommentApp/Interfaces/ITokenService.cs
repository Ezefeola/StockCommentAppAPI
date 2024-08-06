using StockCommentApp.Models;

namespace StockCommentApp.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
