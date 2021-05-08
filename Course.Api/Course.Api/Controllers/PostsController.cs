using Course.BLL.Interfaces;
using Course.BLL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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
        public PostsController(IDbCrud dbCrud)
        {
            this.dbCrud = dbCrud;
        }

        #region Post
        [Route("admincheck")]
        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult Check()
        {
            return Ok("You are admin");
        }
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
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return BadRequest("No user");
                if (songPath == null) return BadRequest("No song");
                await dbCrud.CreatePost(userId, songName, songPath, imagePath);
                return Ok();
        }

        [HttpDelete]
        [Route("{id}/admin")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> AdminDelete([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            await dbCrud.DeletePost(id, isAdmin: true);
            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            await dbCrud.DeletePost(id, userId: userId);
            return NoContent();
        }

        #endregion

        #region Files
        [HttpPost]
        [Route("files/{isSong}")]
        public async Task<IActionResult> UploadFile(IFormFile uploadedFile, [FromRoute] string isSong)
        {
            bool isMusic;
            if(string.Compare(isSong, "music") == 0)
            {
                isMusic = true;
            }
            else
            if(string.Compare(isSong, "image") == 0)
            {
                isMusic = false;
            }
            else
            {
                return NotFound();
            }
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            string fileName = await dbCrud.UploadFile(uploadedFile, userId, isMusic);
            return Ok(fileName);
        }

        [HttpDelete]
        [Route("files/{isSong}/{file}")]
        public async Task<IActionResult> DeleteFile([FromRoute]string file, [FromRoute] string isSong)
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
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            if (file == null) return BadRequest("No file");
            await Task.Run(() => dbCrud.DeleteFile(userId, file, isMusic));
            return Ok();
        }
        #endregion
        #region Assess
        [HttpGet]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentGet([FromRoute] int postId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            (int number, bool? userAssess) response = await Task.Run(() => dbCrud.GetAssessments(userId, postId));
            var _response = new
            {
                Number = response.number,
                UserAssess = response.userAssess.ToString()
            };
            return Ok(_response);
        }
        [HttpPost]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentPost([FromRoute] int postId, [FromBody] BLL.Models.PostAddition.Assessment assessment)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            (int number, bool? userAssess) response = await Task.Run(() => dbCrud.CreateUpdateAssessment(userId, postId, assessment.Like));
            var _response = new
            {
                Number = response.number, UserAssess = response.userAssess.ToString()
            };
            return Ok(_response);
        }

        [HttpDelete]
        [Route("{postId}/assess")]
        public async Task<IActionResult> AssessmentDelete([FromRoute] int postId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return BadRequest("No user");
            int response = await Task.Run(() => dbCrud.DeleteAssessment(userId, postId));
            return Ok(response);
        }
        #endregion
        

    }
}

