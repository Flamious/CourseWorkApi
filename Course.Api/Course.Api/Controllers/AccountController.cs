using Course.BLL.Interfaces;
using Course.BLL.Models;
using Course.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AccountController> _logger;
        public AccountController(BLL.Interfaces.IAuthorizationService authorizationService, ILogger<AccountController> logger)
        {
            _authorizationService = authorizationService;
            _logger = logger;
        }
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromForm] RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    ErrorResponse response = await _authorizationService.Register(model);
                    if (string.Compare(response.Message, "added") == 0) _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Добавлен пользователь {model.Username}");
                    else _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Пользователь {model.Username} не добавлен");
                    return Ok(response);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                    return Ok(new ErrorResponse("error", new List<string> { e.Message }));
                }
            }
            else
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
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
                try
                {
                    string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    LoginResponse response = await _authorizationService.RefreshToken(userId);
                    if (response == null) _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH: mm:ss")}: Токен не был обновлен.");
                    else _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Был обновлен токен для пользователя {response.Username}.");
                    return Ok(response);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{ DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                    return Ok(null);
                }
            }
            else
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH: mm:ss")}: Ошибка модели");
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
                try
                {
                    LoginResponse response = await _authorizationService.Login(model);
                    if (response == null)
                    {
                        _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Вход по {model.Email} не выполнен.");
                        return Ok(new ErrorResponse("error", new List<string>() { $"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Неверный логин или пароль" }));
                    }
                    else
                    {
                        _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Вход пользователя {response.Username} (Email: {model.Email})");
                        return Ok(response);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                    return Ok(new ErrorResponse("error", new List<string> { e.Message }));
                }
            }
            else
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH: mm:ss")}: Ошибка модели");
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

