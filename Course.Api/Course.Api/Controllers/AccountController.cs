using Course.BLL.Interfaces;
using Course.BLL.Models;
using Course.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
namespace Course.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly BLL.Interfaces.IAuthorizationService _authorizationService;
        public AccountController(BLL.Interfaces.IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                ErrorResponse response = await _authorizationService.Register(model);
                return Ok(response);
            }
            else
            {
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return Ok(errorMsg);
            }
        }
        [HttpPost]
        [Authorize(AuthenticationSchemes = "Bearer")]
        [Route("check")]
        public async Task<IActionResult> Check()
        {
            if (ModelState.IsValid)
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                LoginResponse response = await _authorizationService.RefreshToken(userId);
                return Ok(response);
            }
            else
            {
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return Ok(errorMsg);
            }
        }
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromForm] LoginModel model)
        {
            if (ModelState.IsValid)
            {
                LoginResponse response = await _authorizationService.Login(model);
                if (response == null) return Ok(new ErrorResponse("error", new List<string>() { "Неверный логин или пароль" }));
                else return Ok(response);
            }
            else
            {
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return Ok(errorMsg);
            }
        }
    }
}

