using Course.BLL.Interfaces;
using Course.BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Course.Api.Controllers
{
    [EnableCors("_sideCor")]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IDbCrud dbCrud;
        private readonly ILogger<PostsController> _logger;
        public PostsController(IDbCrud dbCrud, ILogger<PostsController> logger)
        {
            this.dbCrud = dbCrud;
            _logger = logger;
        }

        #region Post
        [HttpGet]
        public IEnumerable<Post> GetAll()
        {
            return dbCrud.GetAllPosts();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPost([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var post = await Task.Run(() => dbCrud.GetPost(id));
            if (post == null)
            {
                return NotFound();
            }
            return Ok(post);

        }


        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] string songName, [FromForm] string songPath, [FromForm] string imagePath)
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
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка добавления поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                if (songPath == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка добавления поста. Нет файла аудио.");
                    return BadRequest("No song");
                }
                await dbCrud.CreatePost(userId, songName, songPath, imagePath);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Добавлен пост. Id пользователя: {userId}. Название песни: {songName}. Файл аудио: {songPath}. Файл картинки: {imagePath}.");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}/admin")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDelete([FromRoute] int id)
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
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                await dbCrud.DeletePost(id, isAdmin: true);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Удален пост администратором. Id администратора: {userId}. Id поста: {id}.");
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
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
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                await dbCrud.DeletePost(id, userId: userId);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Удален пост. Id пользователя: {userId}. Id поста: {id}.");
                return NoContent();
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        #endregion

        #region Files
        [HttpPost]
        [Route("files/{isSong}")]
        public async Task<IActionResult> UploadFile(IFormFile uploadedFile, [FromRoute] string isSong)
        {

            bool isMusic;
            if (string.Compare(isSong, "music") == 0)
            {
                isMusic = true;
            }
            else
            if (string.Compare(isSong, "image") == 0)
            {
                isMusic = false;
            }
            else
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка загрузки файла. Пользователь не найден.");
                    return BadRequest("No user");
                }
                string fileName = await dbCrud.UploadFile(uploadedFile, userId, isMusic);
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Загружен файл. Id пользователя: {userId}. Файл: {uploadedFile.FileName}.");
                return Ok(fileName);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete]
        [Route("files/{isSong}/{file}")]
        public async Task<IActionResult> DeleteFile([FromRoute] string file, [FromRoute] string isSong)
        {
            bool isMusic;
            if (string.Compare(isSong, "music") == 0)
            {
                isMusic = true;
            }
            else
            if (string.Compare(isSong, "image") == 0)
            {
                isMusic = false;
            }
            else
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления файла. Пользователь не найден.");
                    return BadRequest("No user");
                }
                if (file == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления файла. Нет пути к файлу.");
                    return BadRequest("No file");
                }
                await Task.Run(() => dbCrud.DeleteFile(userId, file, isMusic));
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Удален файл. Id пользователя: {userId}. Файл: {file}.");
                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        #endregion
        #region Assess
        [HttpGet]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentGet([FromRoute] int postId)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Получение оценок поста. Id поста: {postId}. Ошибка модели");
                var errorMsg = new
                {
                    message = "error",
                    error = ModelState.Values.SelectMany(e =>
                    e.Errors.Select(er => er.ErrorMessage))
                };
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка получение оценок поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                (int number, bool? userAssess) response = await Task.Run(() => dbCrud.GetAssessments(userId, postId));
                var _response = new
                {
                    Number = response.number,
                    UserAssess = response.userAssess.ToString()
                };
                return Ok(_response);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        [HttpPost]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentPost([FromRoute] int postId, [FromBody] BLL.Models.PostAddition.Assessment assessment)
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
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка оценки поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                (int number, bool? userAssess) response = await Task.Run(() => dbCrud.CreateUpdateAssessment(userId, postId, assessment.Like));
                var _response = new
                {
                    Number = response.number,
                    UserAssess = response.userAssess.ToString()
                };
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Поставлена оценка посту. Id пользователя: {userId}. Id поста: {postId}.");
                return Ok(_response);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }


        [HttpDelete]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentDelete([FromRoute] int postId)
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
                return BadRequest(errorMsg);
            }
            try
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null)
                {
                    _logger.LogWarning($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Попытка удаления оценки поста. Пользователь не найден.");
                    return BadRequest("No user");
                }
                int response = await Task.Run(() => dbCrud.DeleteAssessment(userId, postId));
                _logger.LogInformation($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: Удалена оценка поста. Id пользователя: {userId}. Id поста: {postId}.");
                return Ok(response);
            }
            catch (Exception e)
            {
                _logger.LogError($"{DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")}: {e.Message}");
                return BadRequest(e.Message);
            }
        }
        #endregion


    }
}

