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
        private readonly IStudentClassInGroupRepository _studentClassInGroupRepository;
        private readonly IClassGroupRepository _classGroupRepository;
        private readonly ISubjectRepository _subjectRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IProvinceRepository _provinceRepository;
        private readonly IOtpRepository _otpRepository;
        private readonly ISubjectGroupRepository _subjectGroupRepository;
        private readonly IDistrictRepository _districtRepository;
        private readonly ISchoolYearRepository _schoolYearRepository;

        public UnitOfWork(SchedulifyContext context, 
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRoleAssignmentRepository roleAssignmentRepository,
            ITeacherRepository teacherRepository,
            ISchoolRepository schoolRepository,
            IStudentClassesRepository studentClassesRepository,
            IStudentClassInGroupRepository studentClassInGroupRepository,
            IClassGroupRepository classGroupRepository,
            ISubjectRepository subjectRepository,
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository,
            IRoomTypeRepository roomTypeRepository,
            ISubjectGroupRepository subjectGroupRepository,
            IDistrictRepository districtRepository,
            IProvinceRepository provinceRepository,
            IOtpRepository otpRepository,
            ISchoolYearRepository schoolYearRepository)
        {
            _context = context;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _roleAssignmentRepository = roleAssignmentRepository;
            _teacherRepository = teacherRepository;
            _schoolRepository = schoolRepository;
            _studentClassesRepository = studentClassesRepository;
            _studentClassInGroupRepository = studentClassInGroupRepository;
            _classGroupRepository = classGroupRepository;
            _subjectRepository = subjectRepository;
            _buildingRepository = buildingRepository;
            _subjectGroupRepository = subjectGroupRepository;
            _districtRepository = districtRepository;
            _roomRepository = roomRepository;
            _roomTypeRepository = roomTypeRepository;
            _provinceRepository = provinceRepository;
            _otpRepository = otpRepository;
            _schoolYearRepository = schoolYearRepository;
        }

        public IUserRepository UserRepo => _userRepository;
        public IRoleRepository RoleRepo => _roleRepository;
        public IRoleAssignmentRepository RoleAssignmentRepo => _roleAssignmentRepository;
        public ITeacherRepository TeacherRepo => _teacherRepository;
        public ISchoolRepository SchoolRepo => _schoolRepository;
        public IStudentClassesRepository StudentClassesRepo => _studentClassesRepository;
        public ISubjectRepository SubjectRepo => _subjectRepository;
        public IStudentClassInGroupRepository StudentClassInGroupRepo => _studentClassInGroupRepository;
        public IClassGroupRepository ClassGroupRepo => _classGroupRepository;
        public IBuildingRepository BuildingRepo => _buildingRepository;
        public ISubjectGroupRepository SubjectGroupRepo => _subjectGroupRepository;
        public IDistrictRepository DistrictRepo=> _districtRepository;
        public IProvinceRepository ProvinceRepo => _provinceRepository;
        public IRoomRepository RoomRepo => _roomRepository;
        public IRoomTypeRepository RoomTypeRepo => _roomTypeRepository;
        public IOtpRepository OTPRepo => _otpRepository;

        public ISchoolYearRepository SchoolYearRepo => _schoolYearRepository;

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
