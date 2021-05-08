using Course.BLL.Interfaces;
using Course.BLL.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Course.BLL.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly UserManager<DAL.Entities.User> _userManager;
        private readonly SignInManager<DAL.Entities.User> _signInManager;
        private readonly IJwtGenerator _jwtGenerator;
        public AuthorizationService(UserManager<DAL.Entities.User> userManager,
        SignInManager<DAL.Entities.User> signInManager, IJwtGenerator jwtGenerator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<LoginResponse> Login(LoginModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return null;
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (result.Succeeded)
            {
                var roles = await _userManager.GetRolesAsync(user);
                string role = roles.ToList().FirstOrDefault();
                return new LoginResponse()
                {
                    Username = user.UserName,
                    IsCreatingPost = user.IsCreatingPost,
                    Role = role,
                    Token = _jwtGenerator.CreateToken(user, role)
                };
            }

            return null;
        }
        public async Task<LoginResponse> RefreshToken(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            string role = roles.ToList().FirstOrDefault();
            return new LoginResponse()
            {
                Username = user.UserName,
                IsCreatingPost = user.IsCreatingPost,
                Role = role,
                Token = _jwtGenerator.CreateToken(user, role)
            };
        }

        public async Task<ErrorResponse> Register(RegistrationModel model)
        {
            DAL.Entities.User user = new User
            {
                Email = model.Email,
                UserName = model.Username,
                IsCreatingPost = false
            };
            // Добавление нового пользователя
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "user");
                return new ErrorResponse("added");
            }
            else
            {
                List<string> errorList = new List<string>();
                foreach (var error in result.Errors)
                {
                    errorList.Add(error.Description);
                }
                return new ErrorResponse("error", errorList);
            }
        }
    }
}
