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
    public class SubjectRepository : GenericRepository<Subject>, ISubjectRepository
    {
        private SchedulifyContext _context;

        public SubjectRepository(SchedulifyContext context) : base(context)
        {
            _context = context;
        }

        public List<Subject?> GetSubjectByName(string subjectName)
        {
            var subjects = _context.Subjects.Where(x => x.SubjectName == subjectName).ToList();
            return subjects;
        }
    }
}
