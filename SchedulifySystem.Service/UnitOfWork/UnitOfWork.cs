using Microsoft.EntityFrameworkCore.Storage;
using SchedulifySystem.Repository.DBContext;
using SchedulifySystem.Repository.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulifySystem.Service.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SchedulifyContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IRoleAssignmentRepository _roleAssignmentRepository;
        private readonly ITeacherRepository _teacherRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IStudentClassesRepository _studentClassesRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly ISubjectGroupRepository _subjectGroupRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly ISchoolYearRepository _schoolYearRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly ITeacherAssignmentRepository _teacherAssignmentRepository;
        private readonly ITeachableSubjectRepository _teachableSubjectRepository;
        private readonly ITermRepository _termRepository;
        private readonly ISubjectInGroupRepository _subjectInGroupRepository;
        private readonly IRoomSubjectRepository _roomSubjectRepository;

        public UnitOfWork(SchedulifyContext context, 
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRoleAssignmentRepository roleAssignmentRepository,
            ITeacherRepository teacherRepository,
            ISchoolRepository schoolRepository,
            IStudentClassesRepository studentClassesRepository,
            ISubjectRepository subjectRepository,
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository,
            ISubjectGroupRepository subjectGroupRepository,
            IDistrictRepository districtRepository,
            IProvinceRepository provinceRepository,
            IOtpRepository otpRepository,
            ISchoolYearRepository schoolYearRepository,
            IDepartmentRepository departmentRepository,
            ITeacherAssignmentRepository teacherAssignmentRepository,
            ITeachableSubjectRepository teachableSubjectRepository,
            ITermRepository termRepository,
            ISubjectInGroupRepository subjectInGroupRepository,
            IRoomSubjectRepository roomSubjectRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleAssignmentRepository = roleAssignmentRepository;
            _teacherRepository = teacherRepository;
            _schoolRepository = schoolRepository;
            _studentClassesRepository = studentClassesRepository;
            _subjectRepository = subjectRepository;
            _buildingRepository = buildingRepository;
            _subjectGroupRepository = subjectGroupRepository;
            _districtRepository = districtRepository;
            _roomRepository = roomRepository;
            _provinceRepository = provinceRepository;
            _otpRepository = otpRepository;
            _schoolYearRepository = schoolYearRepository;
            _departmentRepository = departmentRepository;
            _teacherAssignmentRepository = teacherAssignmentRepository;
            _teachableSubjectRepository = teachableSubjectRepository;
            _termRepository = termRepository;
            _subjectInGroupRepository = subjectInGroupRepository;
            _roomSubjectRepository = roomSubjectRepository;
        }

        public IUserRepository UserRepo => _userRepository;
        public IRoleRepository RoleRepo => _roleRepository;
        public IRoleAssignmentRepository RoleAssignmentRepo => _roleAssignmentRepository;
        public ITeacherRepository TeacherRepo => _teacherRepository;
        public ISchoolRepository SchoolRepo => _schoolRepository;
        public IStudentClassesRepository StudentClassesRepo => _studentClassesRepository;
        public ISubjectRepository SubjectRepo => _subjectRepository;
        public IBuildingRepository BuildingRepo => _buildingRepository;
        public ISubjectGroupRepository SubjectGroupRepo => _subjectGroupRepository;
        public IDistrictRepository DistrictRepo=> _districtRepository;
        public IProvinceRepository ProvinceRepo => _provinceRepository;
        public IRoomRepository RoomRepo => _roomRepository;
        public IOtpRepository OTPRepo => _otpRepository;
        public ISchoolYearRepository SchoolYearRepo => _schoolYearRepository;
        public IDepartmentRepository DepartmentRepo => _departmentRepository;
        public ITeacherAssignmentRepository TeacherAssigntRepo=> _teacherAssignmentRepository;
        public ITeacherAssignmentRepository TeacherAssignmentRepo => _teacherAssignmentRepository;
        public ITeachableSubjectRepository TeachableSubjectRepo => _teachableSubjectRepository;
        public ITermRepository TermRepo => _termRepository;
        public ISubjectInGroupRepository SubjectInGroupRepo => _subjectInGroupRepository;
        public IRoomSubjectRepository RoomSubjectRepo => _roomSubjectRepository;

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
