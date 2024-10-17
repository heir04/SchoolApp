using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Core.Dto;
using SchoolApp.Core.IServices;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Core.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationContext _context;
        private readonly IStudentRepository _studentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILevelRepository _levelRepository;
        public StudentService(IStudentRepository studentRepository,
        IUserRepository userRepository, IRoleRepository roleRepository,
        ApplicationContext context, ILevelRepository levelRepository)
        {
            _context = context;
            _studentRepository = studentRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _levelRepository = levelRepository;
        }

        public async Task<BaseResponse<StudentDto>> Delete(Guid id)
        {
            var student = await _studentRepository.Get(a => a.Id == id);
            if (student == null || student.IsDeleted == true)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "not found or is already deleted",
                    Status = false
                };
            }
            student.IsDeleted = true;
            return new BaseResponse<StudentDto>
            {
                Message = "Student deleted",
                Status = true
            };
        }

        public async Task<BaseResponse<StudentDto>> Get(Guid id)
        {
            var student = await _studentRepository.Get(s => s.Id == id);
            if (student == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "not found",
                    Status = false
                };
            }
            var studentDto = new StudentDto
            {
                Id = id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                //LevelId = student.LevelId,
            };
            return new BaseResponse<StudentDto>
            {
                Data = studentDto,
                Message = "getStudent gotten",
                Status = true
            };
        }

        public async Task<BaseResponse<StudentDto>> Get(string email)
        {
            var student = await _studentRepository.Get(s =>s.Email == email);
            if (student == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "not found",
                    Status = false
                };
            }
            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
            };
            return new BaseResponse<StudentDto>
            {
                Data = studentDto,
                Message = "getStudent gotten",
                Status = true
            };
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll()
        {
            var students = await _studentRepository.GetAll();
            if (students == null)
            {
                return new BaseResponse<IEnumerable<StudentDto>>
                {
                    Message = "no getStudent in this level",
                    Status = false
                };
            }

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                LevelName = s.LastName,
                Email = s.Email,
            }).ToList();
            return new BaseResponse<IEnumerable<StudentDto>>
            {
                Message = "list of students",
                Status = true,
                Data = studentDtos
            };
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll(Guid levelId)
        {
            var students = await _studentRepository.GetByExpression(s => s.LevelId == levelId);
            if (students == null)
            {
                return new BaseResponse<IEnumerable<StudentDto>>
                {
                    Message = "no Student registered in this level",
                    Status = false
                };
            }

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
            }).ToList();
            return new BaseResponse<IEnumerable<StudentDto>>
            {
                Message = "list of students",
                Status = true,
                Data = studentDtos
            };
        }

        public async Task<BaseResponse<StudentDto>> GetByStudentId(string studentId)
        {
            var student = await _studentRepository.Get(s => s.StudentId == studentId);
            if (student == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "not found",
                    Status = false
                };
            }
            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,  
            };
            return new BaseResponse<StudentDto>
            {
                Data = studentDto,
                Message = "getStudent gotten",
                Status = true
            };
        }

        public async Task<BaseResponse<StudentDto>> Register(StudentDto studentDto)
        {
            var getStudent = await _studentRepository.Get(s => s.Email == studentDto.Email);
            if (getStudent != null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "already ex ist",
                    Status = false
                };
            }
            var user = new User
            {
                UserName = $"{studentDto.FirstName} {studentDto.LastName}",
                Password = BCrypt.Net.BCrypt.HashPassword(studentDto.Password),
                Email = studentDto.Email
            };
            await _userRepository.Register(user);
            var role = await _roleRepository.Get(r => r.Name == "Student");
            if (role == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "Role not found",
                    Status = false
                };
            }
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
            _context.UserRoles.Add(userRole);
            var getlevel = await _levelRepository.Get(l => l.LevelName == studentDto.LevelName);
            if (getlevel == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "level not found",
                    Status = false
                };
            }
            studentDto.StudentId =$"Stu{Guid.NewGuid().ToString().Replace("-", "").Substring(0, 5).ToUpper()}";
            var student = new Student
            {
                StudentId = studentDto.StudentId,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email,
                LevelId = getlevel.Id,
                UserId = user.Id
            };
            var addStudent = await _studentRepository.Register(student);
            student.CreatedBy = addStudent.Id;
            student.LastModifiedBy = addStudent.Id;
            student.IsDeleted = false;
            await _studentRepository.Update(student);

            var studentDTo = new StudentDto
            {
                Id = addStudent.Id,
                StudentId = addStudent.StudentId,
                FirstName = addStudent.FirstName,
                LastName = addStudent.LastName,
                Email = addStudent.Email,
                LevelName = addStudent.Level.LevelName
                //LevelId = addStudent.LevelId
            };

            return new BaseResponse<StudentDto>
            {
                Message = "Studennt registered succesfully",
                Status = true,
                Data = studentDTo
            };
        }

        public async Task<BaseResponse<StudentDto>> Update(StudentDto studentDto, Guid id)
        {
            var student = await _studentRepository.Get(s => s.Id == id);
            if (student == null)
            {
                return new BaseResponse<StudentDto> 
                {
                    Message = "not found",
                    Status = false
                };
            }

            var getUser = await _userRepository.Get(u => u.Email == studentDto.Email);
            if (getUser == null)
            {
                return new BaseResponse<StudentDto>
                {
                    Message = "User not found",
                    Status = false
                };
            }
            getUser.Email = studentDto.Email;
            getUser.Password = studentDto.Password;
            await _userRepository.Update(getUser);

            student.User.Email = studentDto.Email ?? student.User.Email;
            student.User.Password = studentDto.Password ?? student.User.Password;
            student.FirstName = studentDto.FirstName ?? student.FirstName;
            student.LastName = studentDto.LastName ?? student.LastName;
            //student.LevelId = studentDto.LevelId;
            await _studentRepository.Update(student);

            return new BaseResponse<StudentDto> 
            {
                Message = "udated succesfully",
                Status = true
            };

        }
    }
}
