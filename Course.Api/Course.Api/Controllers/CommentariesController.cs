using Course.BLL.Interfaces;
using Course.BLL.Models;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace Course.Api.Controllers
{
    [EnableCors("_sideCor")]
    [Route("api/posts/{postId}/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class CommentariesController : ControllerBase
    {
        private readonly IDbCrud dbCrud;
        private readonly ILogger<CommentariesController> _logger;
        public CommentariesController(IDbCrud dbCrud, ILogger<CommentariesController> logger)
        {
            this.dbCrud = dbCrud;
            _logger = logger;
        }
        [HttpGet]
        public IEnumerable<Commentary> GetCommentaries([FromRoute] int postId)
        {
            return dbCrud.GetAllCommentaries(postId);
        }

        [HttpPost]
        public async Task<IEnumerable<Commentary>> PostCommentary([FromRoute] int postId, [FromBody] BLL.Models.PostAddition.Commentary message)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return dbCrud.GetAllCommentaries(postId);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка добавления комментария. Пользователь не найден.");
                    return dbCrud.GetAllCommentaries(postId);
                }
                await Task.Run(() => dbCrud.CreateCommentary(userId, postId, message.Message));
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Добавлен комментарий \"{message.Message}\". Id пользователя: {userId}. Id поста: {postId}");
                return dbCrud.GetAllCommentaries(postId);
            }
            catch(Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return dbCrud.GetAllCommentaries(postId);
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IEnumerable<Commentary>> UpdateCommentary([FromRoute] int postId, [FromRoute] int id, [FromBody] BLL.Models.PostAddition.Commentary message)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return dbCrud.GetAllCommentaries(postId);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка редактирования комментария. Пользователь не найден.");
                    return dbCrud.GetAllCommentaries(postId);
                }
                await Task.Run(() => dbCrud.UpdateCommentary(userId, postId, id, message.Message));
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Комментарий изменен на \"{message.Message}\". Id пользователя: {userId}. Id поста: {postId}. Id комментария: {id}");
                return dbCrud.GetAllCommentaries(postId);
            }
            catch(Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return dbCrud.GetAllCommentaries(postId);
            }
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("{id}/admin")]
        public async Task<IEnumerable<Commentary>> DeleteCommentaryAdmin([FromRoute] int postId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return dbCrud.GetAllCommentaries(postId);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления комментария. Пользователь не найден.");
                    return dbCrud.GetAllCommentaries(postId);
                }
                await dbCrud.DeleteCommentary(postId, id, isAdmin: true);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Комментарий удален администратором. Id администратора: {userId}. Id поста: {postId}. Id комментария: {id}");
                return dbCrud.GetAllCommentaries(postId);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return dbCrud.GetAllCommentaries(postId);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IEnumerable<Commentary>> DeleteCommentary([FromRoute] int postId, [FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return dbCrud.GetAllCommentaries(postId);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления комментария. Пользователь не найден.");
                    return dbCrud.GetAllCommentaries(postId);
                }
                await dbCrud.DeleteCommentary(postId, id, userId: userId);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Комментарий удален. Id пользователя: {userId}. Id поста: {postId}. Id комментария: {id}");
                return dbCrud.GetAllCommentaries(postId);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return dbCrud.GetAllCommentaries(postId);
            }
        }
    }
}
