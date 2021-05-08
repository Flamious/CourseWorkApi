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

namespace Course.Api.Controllers
{
    [EnableCors("_sideCor")]
    [Route("api/posts/{postId}/[controller]")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class CommentariesController : ControllerBase
    {
        private readonly IDbCrud dbCrud;
        public CommentariesController(IDbCrud dbCrud)
        {
            this.dbCrud = dbCrud;
        }
        [HttpGet]
        public IEnumerable<Commentary> GetCommentaries([FromRoute] int postId)
        {
            return dbCrud.GetAllCommentaries(postId);
        }

        [HttpPost]
        public async Task<IEnumerable<Commentary>> PostCommentary([FromRoute] int postId, [FromBody] BLL.Models.PostAddition.Commentary message)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return dbCrud.GetAllCommentaries(postId);
            await Task.Run(() => dbCrud.CreateCommentary(userId, postId, message.Message));
            return dbCrud.GetAllCommentaries(postId);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IEnumerable<Commentary>> UpdateCommentary([FromRoute] int postId, [FromRoute] int id, [FromBody] BLL.Models.PostAddition.Commentary message)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return dbCrud.GetAllCommentaries(postId);
            await Task.Run(() => dbCrud.UpdateCommentary(userId, postId, id, message.Message));
            return dbCrud.GetAllCommentaries(postId);
        }

        [HttpDelete]
        [Authorize(Roles = "admin")]
        [Route("{id}/admin")]
        public async Task<IEnumerable<Commentary>> DeleteCommentaryAdmin([FromRoute] int postId, [FromRoute] int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return dbCrud.GetAllCommentaries(postId);
            await dbCrud.DeleteCommentary(postId, id, isAdmin: true);
            return dbCrud.GetAllCommentaries(postId);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IEnumerable<Commentary>> DeleteCommentary([FromRoute] int postId, [FromRoute] int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return dbCrud.GetAllCommentaries(postId);
            await dbCrud.DeleteCommentary(postId, id, userId: userId);
            return dbCrud.GetAllCommentaries(postId);
        }
    }
}
