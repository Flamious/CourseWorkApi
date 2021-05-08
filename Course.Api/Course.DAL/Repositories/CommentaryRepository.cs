using Course.DAL.Entities;
using Course.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Repositories
{
    public class CommentaryRepository : IRepository<Commentary>
    {
        private readonly PostContext _context;

        public CommentaryRepository(PostContext context)
        {
            _context = context;
        }

        public void Create(Commentary item)
        {
            _context.Commentary.Add(item);
        }

        public void Delete(int id)
        {
            var item = _context.Commentary.Find(id);
            if (item != null)
                _context.Commentary.Remove(item);
        }

        public Commentary Get(int id)
        {
            return _context.Commentary.Find(id);
        }

        public IEnumerable<Commentary> GetAll()
        {
            return _context.Commentary.Include(c=>c.Post).Include(c=>c.User);
        }

        public void Update(Commentary item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
