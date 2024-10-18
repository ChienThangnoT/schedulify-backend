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
    public class TeachableSubjectRepository : GenericRepository<TeachableSubject>, ITeachableSubjectRepository
    {
        public TeachableSubjectRepository(SchedulifyContext context) : base(context)
        {
        }
    }
}
