using Course.DAL.Entities;
using Course.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Repositories
{
    public class DbRepository : IDbRepository
    {
        private readonly PostContext _context;

        public DbRepository(PostContext context)
        {
            _context = context;
        }

        private PostRepository post;
        private UserRepository user;
        private CommentaryRepository commentary;
        private AssessmentRepository assessment;
        public IRepository<Post> Posts
        {
            get
            {
                if (post == null)
                    post = new PostRepository(_context);
                return post;
            }
        }

        public IRepository<User> Users
        {
            get
            {
                if (user == null)
                    user = new UserRepository(_context);
                return user;
            }
        }

        public IRepository<Commentary> Commentaries
        {
            get
            {
                if (commentary == null)
                    commentary = new CommentaryRepository(_context);
                return commentary;
            }
        }

        public IRepository<Assessment> Assessments
        {
            get
            {
                if (assessment == null)
                    assessment = new AssessmentRepository(_context);
                return assessment;
            }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
