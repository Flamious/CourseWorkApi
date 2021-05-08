using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Course.BLL.Interfaces
{
    public interface IDbInitializer
    {
        Task Initialize();
    }
}
