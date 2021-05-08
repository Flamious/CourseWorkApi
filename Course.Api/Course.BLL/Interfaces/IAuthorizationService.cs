using Course.BLL.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Course.BLL.Interfaces
{
    public interface IAuthorizationService
    {
        Task<ErrorResponse> Register(RegistrationModel model);
        Task<LoginResponse> Login(LoginModel model);
        Task<LoginResponse> RefreshToken(string userId);
    }
}
