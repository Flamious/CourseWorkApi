using Course.DAL.Entities;
using Course.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Repositories
{
    public class PostRepository : IRepository<Post>
    {
        private readonly PostContext _context;

        public PostRepository(PostContext context)
        {
            _context = context;
        }

        public void Create(Post item)
        {
            _context.Post.Add(item);
        }

        public void Delete(int id)
        {
            var item = _context.Post.Find(id);
            if (item != null)
            {
                _context.Post.Remove(item);
            }
            _context.SaveChanges();
        }

        public Post Get(int id)
        {
            return _context.Post.Find(id);
        }

        public IEnumerable<Post> GetAll()
        {
            return _context.Post.Include(c => c.Commentary).Include(c => c.User).Include(a=>a.Assessment);
        }

        public void Update(Post item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
