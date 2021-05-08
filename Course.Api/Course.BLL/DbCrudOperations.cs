using Course.BLL.Interfaces;
using Course.BLL.Models;
using Course.DAL.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.BLL
{
    public class DbCrudOperations : IDbCrud
    {
        private readonly IDbRepository db;
        private readonly UserManager<DAL.Entities.User> _userManager;

        private string pathMusic = @"wwwroot\Music\";
        private string pathImages = @"wwwroot\Images\";
        private string temporaryDesignation = "temp-";


        public DbCrudOperations(IDbRepository repository, UserManager<DAL.Entities.User> userManager)
        {
            db = repository;
            _userManager = userManager;
        }
        public void CreateCommentary(string userId, int postId, string message)
        {
            db.Commentaries.Create(new DAL.Entities.Commentary()
            {
                Message = message,
                PostId = postId,
                UserId = userId,
                CreatingDate = DateTime.Now
            });
            Save();
        }

        public async Task CreatePost(string userId, string songName, string songPath, string imagePath)
        {
            string finalSongFile = RemoveSubstringAtStart(songPath, temporaryDesignation);
            using(Stream source = File.Open(pathMusic + songPath, FileMode.Open))
            {
                using(Stream destination = File.Open(pathMusic + finalSongFile, FileMode.Create))
                {
                    await source.CopyToAsync(destination);
                }
            }
            File.Delete(pathMusic + songPath);

            string finalImageFile;
            if (string.IsNullOrEmpty(imagePath))
            {
                finalImageFile = null;
            }
            else
            {
                finalImageFile = RemoveSubstringAtStart(imagePath, temporaryDesignation);
                using (Stream source = File.Open(pathImages + imagePath, FileMode.Open))
                {
                    using (Stream destination = File.Open(pathImages + finalImageFile, FileMode.Create))
                    {
                        await source.CopyToAsync(destination);
                    }
                }
                File.Delete(pathImages + imagePath);
            }
            db.Posts.Create(new DAL.Entities.Post()
            {
                Content = songName,
                UserId = userId,
                FileName = finalSongFile,
                ImageName = finalImageFile,
                CreatingDate = DateTime.Now
            });
            Save();
        }

        public async Task<string> UploadFile(IFormFile uploadedFile, string userId, bool isMusic)
        {
            string fileName = $"{temporaryDesignation}{userId} - {DateTime.Now.ToString("dd.MM.yyyy.HH.mm.ss")}-{uploadedFile.FileName}";
            string path = (isMusic ? pathMusic : pathImages) + fileName;

            using (var fileStream = new FileStream(path, FileMode.CreateNew))
            {
                await uploadedFile.CopyToAsync(fileStream);
            }
            return fileName;
        }
        public void DeleteFile(string userId, string path, bool isMusic)
        {
            if (!path.Contains(userId)) return;
            string fullPath = (isMusic ? pathMusic : pathImages) + path;
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }

        public async Task DeleteCommentary(int postId, int id, string userId = "", bool isAdmin = false)
        {
            var c = await Task.Run(()=>db.Commentaries.Get(id));
            if (c != null)
            {
                if ((string.Compare(userId, c.UserId) != 0 && !isAdmin) || postId != c.PostId) return;
                db.Commentaries.Delete(id);
                Save();
            }
        }

        public async Task DeletePost(int id, string userId = "", bool isAdmin = false)
        {
            var post = await Task.Run(() => db.Posts.Get(id));
            if (post != null)
            {
                if (string.Compare(userId, post.UserId) != 0 && !isAdmin) return;
                var assessments = db.Assessments.GetAll().Where(a => a.PostId == id).ToArray();
                foreach (var a in assessments)
                {
                    db.Assessments.Delete(a.AssessmentId);
                }
                var commentaries = db.Commentaries.GetAll().Where(c => c.PostId == id).ToArray();
                foreach (var c in commentaries)
                {
                    db.Commentaries.Delete(c.CommentaryId);
                }
                string pathMusic = this.pathMusic + post.FileName;
                string pathImages = this.pathImages + post.ImageName;
                if (File.Exists(pathMusic)) File.Delete(pathMusic);
                if (File.Exists(pathImages)) File.Delete(pathImages);
                db.Posts.Delete(id);
                Save();
            }
        }


        public List<Commentary> GetAllCommentaries(int postId)
        {
            return db.Commentaries.GetAll().Where(i => i.PostId == postId).Select(i => new Commentary(i)).Reverse().ToList();
        }

        public List<Post> GetAllPosts()
        {
            return db.Posts.GetAll().Select(i => new Post(i)).Reverse().ToList();
        }


        public Commentary GetCommentary(int id)
        {
            var commentary = db.Commentaries.Get(id);
            return commentary == null ? null : new Commentary(db.Commentaries.Get(id));
        }

        public Post GetPost(int id)
        {
            var post = db.Posts.Get(id);
            return post == null ? null : new Post(db.Posts.Get(id));
        }


        public void UpdateCommentary(string userId, int postId, int id, string message)
        {
            var c = db.Commentaries.Get(id);
            if (c != null)
            {
                if (string.Compare(userId, c.UserId) != 0 || postId != c.PostId) return;
                c.Message = message;
                db.Commentaries.Update(c);
                Save();
            }
        }

        public void UpdatePost(int id, Post post)
        {
            var item = db.Posts.Get(id);
            if (item != null)
            {
                item.Content = post.Content;
                db.Posts.Update(item);
                Save();
            }
        }

        public bool Save()
        {
            if (db.Save() > 0) return true;
            return false;
        }

        public (int, bool?) CreateUpdateAssessment(string userId, int postId, bool assessment)
        {
            DAL.Entities.Assessment a = db.Assessments.GetAll().FirstOrDefault(a => a.PostId == postId && string.Compare(a.UserId, userId) == 0);
            if (a != null)
            {
                if (a.Like != assessment)
                {
                    a.Like = assessment;
                    db.Assessments.Update(a);
                    Save();
                }
            }
            else
            {
                db.Assessments.Create(new DAL.Entities.Assessment()
                {
                    UserId = userId,
                    PostId = postId,
                    Like = assessment
                });
                Save();
            }
            int allAssessments = GetTotalAssessment(postId);
            return (allAssessments, assessment);
        }

        public int DeleteAssessment(string userId, int postId)
        {
            var a = db.Assessments.GetAll().FirstOrDefault(a => a.PostId == postId && string.Compare(a.UserId, userId) == 0);
            if (a != null)
            {
                db.Assessments.Delete(a.AssessmentId);
                Save();
            }
            int allAssessments = GetTotalAssessment(postId);
            return allAssessments;
        }

        public (int, bool?) GetAssessments(string userId, int postId)
        {
            int allAssessments = GetTotalAssessment(postId);
            bool? a = db.Assessments.GetAll().FirstOrDefault(a => a.PostId == postId && string.Compare(a.UserId, userId) == 0)?.Like;
            return (allAssessments, a);
        }


        private int GetTotalAssessment(int postId)
        {
            int allGoodAssessments = db.Assessments.GetAll().Where(a => a.PostId == postId && a.Like).Count();
            int allBadAssessments = db.Assessments.GetAll().Where(a => a.PostId == postId && !a.Like).Count();

            return allGoodAssessments - allBadAssessments;
        }

        private string RemoveSubstringAtStart(string sourceString, string removeString)
        {
            int index = sourceString.IndexOf(removeString);
            return index != 0 ? sourceString : sourceString.Remove(index, removeString.Length);
        }
    }
}
