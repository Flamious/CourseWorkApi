using Course.DAL.Entities;

namespace Course.BLL.Interfaces
{
    public interface IJwtGenerator
    {
        string CreateToken(User user, string role);
    }
}
