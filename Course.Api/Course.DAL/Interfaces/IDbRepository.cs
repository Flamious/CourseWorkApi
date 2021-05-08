using Course.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Course.DAL.Interfaces
{
    public interface IDbRepository
    {
        IRepository<Post> Posts { get; }
        IRepository<User> Users { get; }
        IRepository<Commentary> Commentaries { get; }
        IRepository<Assessment> Assessments { get; }
        int Save();
    }
}
