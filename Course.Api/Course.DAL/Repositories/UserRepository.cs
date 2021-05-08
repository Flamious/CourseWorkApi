using Course.DAL.Entities;
using Course.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Repositories
{
    public class UserRepository : IRepository<User>
    {
        private readonly PostContext _context;

        public UserRepository(PostContext context)
        {
            _context = context;
        }

        public void Create(User item)
        {
            _context.User.Add(item);
        }

        public void Delete(int id)
        {
            var item = _context.User.Find(id);
            if (item != null)
                _context.User.Remove(item);
        }

        public User Get(int id)
        {
            return _context.User.Find(id);
        }

        public IEnumerable<User> GetAll()
        {
            return _context.User.Include(c => c.Post).Include(c => c.Commentary);
        }

        public void Update(User item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
