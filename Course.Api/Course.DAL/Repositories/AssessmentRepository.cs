using Course.DAL.Entities;
using Course.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Repositories
{
    public class AssessmentRepository : IRepository<Assessment>
    {
        private readonly PostContext _context;

        public AssessmentRepository(PostContext context)
        {
            _context = context;
        }

        public void Create(Assessment item)
        {
            _context.Assessment.Add(item);
        }

        public void Delete(int id)
        {
            var item = _context.Assessment.Find(id);
            if (item != null)
                _context.Assessment.Remove(item);
        }

        public Assessment Get(int id)
        {
            return _context.Assessment.Find(id);
        }

        public IEnumerable<Assessment> GetAll()
        {
            return _context.Assessment.Include(c => c.Post).Include(c => c.User);
        }

        public void Update(Assessment item)
        {
            _context.Entry(item).State = EntityState.Modified;
        }
    }
}
