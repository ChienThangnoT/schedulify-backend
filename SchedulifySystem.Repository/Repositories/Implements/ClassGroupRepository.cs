using SchedulifySystem.Repository.DBContext;
using SchedulifySystem.Repository.EntityModels;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Repository.Repositories.Implements
{
    public class ClassGroupRepository : GenericRepository<ClassGroup>, IClassGroupRepository
    {
        public ClassGroupRepository(SchedulifyContext context) : base(context)
        {
        }
    }
}
