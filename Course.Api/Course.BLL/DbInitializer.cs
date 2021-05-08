using Course.BLL.Interfaces;
using Course.BLL.Models;
using Course.DAL.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Course.BLL
{
    public class DbInitializer : IDbInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<DAL.Entities.User> _userManager;

        public DbInitializer(RoleManager<IdentityRole> roleManager, UserManager<DAL.Entities.User> userManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task Initialize()
        {
            if (await _roleManager.FindByNameAsync("admin") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("admin"));
            }
            if (await _roleManager.FindByNameAsync("user") == null)
            {
                await _roleManager.CreateAsync(new IdentityRole("user"));
            }

            string adminEmail = "admin@mail.com";
            string adminName = "admin";
            string adminPassword = "Aa123456!";
            if (await _userManager.FindByNameAsync(adminEmail) == null)
            {
                User admin = new User
                {
                    Email = adminEmail,
                    UserName = adminName
                };
                IdentityResult result = await
                _userManager.CreateAsync(admin, adminPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(admin, "admin");
                }
            }
            // Создание Пользователя
            string userEmail = "user@mail.com";
            string userName = "user";
            string userPassword = "Aa123456!";
            if (await _userManager.FindByNameAsync(userEmail) == null)
            {
                User user = new User
                {
                    Email = userEmail,
                    UserName = userName 
                };
                IdentityResult result = await
                _userManager.CreateAsync(user, userPassword);
                if (result.Succeeded)
                {

                    await _userManager.AddToRoleAsync(user, "user");
                }
                #region
                //if (dbCrud.GetAllPosts().Count > 0)
                //{
                //    return;
                //}
                //var users = new User[]
                //{
                //    new User{ Login="login1", Password="password1", Nickname="Kolya" },
                //    new User{ Login="login2", Password="password2", Nickname="Petya" },
                //    new User{ Login="login3", Password="password3", Nickname="Vasya" }
                //};
                //foreach (User u in users)
                //{
                //    dbCrud.CreateUser(u);
                //}

                //var posts = new Post[]
                //{
                //    new Post { UserId=dbCrud.GetAllUsers()[0].UserId,Content="Post1" },
                //    new Post { UserId=dbCrud.GetAllUsers()[0].UserId,Content="Post2" }

                //};
                //foreach (Post p in posts)
                //{
                //    dbCrud.CreatePost(p);
                //}

                //var commentaries = new Commentary[]
                //{
                //    new Commentary { PostId=dbCrud.GetAllPosts()[0].PostId, UserId=dbCrud.GetAllUsers()[1].UserId,Message="commentary1" },
                //    new Commentary { PostId=dbCrud.GetAllPosts()[1].PostId, UserId=dbCrud.GetAllUsers()[1].UserId,Message="commentary2" }

                //};
                //foreach (Commentary c in commentaries)
                //{
                //    dbCrud.CreateCommentary(c);
                //}
                #endregion
            }
        }
    }
}
