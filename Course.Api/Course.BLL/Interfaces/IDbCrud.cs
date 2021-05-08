using Course.BLL.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Course.BLL.Interfaces
{
    public interface IDbCrud
    {
        List<Post> GetAllPosts();
        //List<User> GetAllUsers();
        List<Commentary> GetAllCommentaries(int postId);
        (int, bool?) GetAssessments(string userId, int postId);
        Task CreatePost(string userId, string songName, string songPath, string imagePath);

        Task<string> UploadFile(IFormFile uploadedFile, string userId, bool isMusic);
        void DeleteFile(string userId, string path, bool isMusic);
        //void CreateUser(User user);
        (int, bool?) CreateUpdateAssessment(string userId, int postId, bool assessment);
        void CreateCommentary(string userId, int postId, string message);
        void UpdatePost(int id, Post post);
        //void UpdateUser(int id, User user);
        void UpdateCommentary(string userId, int postId, int id, string message);
        Task DeletePost(int id, string userId = "", bool isAdmin = false);
        //void DeleteUser(int id);
        Task DeleteCommentary(int postId, int id, string userId = "", bool isAdmin = false);
        int DeleteAssessment(string userId, int PostId);
        Post GetPost(int id);
        //User GetUser(int id);
        Commentary GetCommentary(int id);
        bool Save();
    }
}
