using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
using SchoolApp.Infrastructure.Context;
using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;

namespace SchoolApp.Application.Services
{
    public class StudentService(IUnitOfWork unitOfWork, ApplicationContext context, IHttpContextAccessor httpContextAccessor) : IStudentService
    {
        private readonly ApplicationContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<BaseResponse<StudentDto>> Delete(Guid id)
        {
            var response = new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.Id == id && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }
            var student = await _unitOfWork.Student.Get(a => a.Id == id);
            if (student == null)
            {
                
                response.Message = "not found or is already deleted";
                response.Status = false;
                return response;
            }
            student.IsDeleted = true;
            await _unitOfWork.Student.Update(student);

            response.Message = "Student deleted";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Get(Guid id)
        {
            var response =  new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.Id == id && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }

            var student = await _unitOfWork.Student.GetStudent(s => s.Id == id);
            if (student == null)
            {
                response.Message = "not found";
                response.Status = false;
                return response;
            }
            var studentDto = new StudentDto
            {
                Id = id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                LevelName = student.Level?.LevelName,
                StudentId = student.StudentId,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                Address = student.Address,
                CreatedOn = student.CreatedOn
            };
           
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Get(string email)
        {
            var response = new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.Email == email && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }

            var student = await _unitOfWork.Student.GetStudent(s =>s.Email == email && !s.IsDeleted);
            if (student == null)
            {
                response.Message = "not found";
                response.Status = false;
                return response;
            }
            
            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                StudentId = student.StudentId,
                LevelName = student.Level?.LevelName,
            };
            
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> GetByCurrentUserId()
        {
            var response = new BaseResponse<StudentDto>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.UserId == userId && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }

            var student = await _unitOfWork.Student.GetStudent(s => s.UserId == userId && !s.IsDeleted);
            if (student == null)
            {
                response.Message = "not found";
                response.Status = false;
                return response;
            }
            
            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                StudentId = student.StudentId,
                LevelName = student.Level?.LevelName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                Address = student.Address,
                CreatedOn = student.CreatedOn
            };
            
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<StudentDto>>();
            var students = await _unitOfWork.Student.GetAllStudents();
            if (students == null || students.Count == 0)
            {
                response.Message = "no getStudent in this level";
                response.Status = false;
                return response;
            }

            var studentDtos = students
            .Where(s => s.IsDeleted == false)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                LevelId = s.LevelId,
                LevelName = s.Level?.LevelName,
                Email = s.Email,
                StudentId = s.StudentId,
                DateOfBirth = s.DateOfBirth,
                PhoneNumber = s.PhoneNumber,
                Gender = s.Gender,
                Address = s.Address,
                CreatedOn = s.CreatedOn
            }).ToList();
            
            response.Message = "list of students";
            response.Status = true;
            response.Data = studentDtos;
            return response;  
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll(Guid levelId)
        {
            var response = new BaseResponse<IEnumerable<StudentDto>>();
            var students = await _unitOfWork.Student.GetAllStudents(s => s.LevelId == levelId);
            if (students == null || students.Count == 0)
            {
                response.Message = "no Student registered in this level";
                response.Status = false;
                return response;
            }

            var studentDtos = students
            .Where(s => s.IsDeleted == false)
            .Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                LevelId = s.LevelId,
                LevelName = s.Level?.LevelName,
                StudentId = s.StudentId,
                DateOfBirth = s.DateOfBirth,
                PhoneNumber = s.PhoneNumber,
                Gender = s.Gender,
                Address = s.Address,
                CreatedOn = s.CreatedOn
            }).ToList();

            response.Message = "list of students";
            response.Status = true;
            response.Data = studentDtos;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> GetByStudentId(string studentId)
        {
            var response = new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.StudentId == studentId && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }
            
            var student = await _unitOfWork.Student.Get(s => s.StudentId == studentId);
            if (student == null)
            {
               
                response.Message = "not found";
                response.Status = false;
            }
            var studentDto = new StudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Email = student.Email,
                LevelName = student.LastName,
                DateOfBirth = student.DateOfBirth,
                PhoneNumber = student.PhoneNumber,
                Gender = student.Gender,
                Address = student.Address,
                CreatedOn = student.CreatedOn
            };
           
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Register(StudentDto studentDto)
        {
            var response = new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.Email == studentDto.Email);
            var defaultPassword = $"{studentDto.FirstName}";
            string saltString = HashingHelper.GenerateSalt();
            string hashedPassword = HashingHelper.HashPassword(defaultPassword, saltString);

            if (studentExist)
            {
                response.Message = $"Student with email: {studentDto.Email} already exist";
                return response;
            }

            var user = new User
            {
                UserName = $"{studentDto.FirstName}{studentDto.LastName}",
                HashSalt = saltString,
                PasswordHash = hashedPassword,
                Email = studentDto.Email?.ToLower()
            };

            var role = await _unitOfWork.Role.Get(r => r.Name == "Student");
            if (role == null)
            {
                response.Message = "Role not found";
                return response;
            }

            var userRole = new UserRole
            {
                UserId = user.Id,
                User = user,
                RoleId = role.Id,
                Role = role
            };
            _context.UserRoles.Add(userRole);
            user.UserRoles.Add(userRole);

            var getlevel = await _unitOfWork.Level.Get(l => l.LevelName == studentDto.LevelName);
            if (getlevel == null)
            {
                response.Message = "level not found";
                return response;
            }
            
            studentDto.StudentId = $"STU{Guid.NewGuid().ToString().Replace("-", "")[..5].ToUpper()}";

            var student = new Student
            {
                StudentId = studentDto.StudentId,
                FirstName = studentDto.FirstName,
                LastName = studentDto.LastName,
                Email = studentDto.Email?.ToLower(),
                LevelId = getlevel.Id,
                Level = getlevel,
                UserId = user.Id,
                User = user,
                DateOfBirth = studentDto.DateOfBirth,
                PhoneNumber = studentDto.PhoneNumber,
                Gender = studentDto.Gender,
                Address = studentDto.Address
            };

            var addStudent = await _unitOfWork.Student.Register(student);
            await _unitOfWork.User.Register(user);
            await _unitOfWork.SaveChangesAsync();
          
            response.Message = "Student registered succesfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Update(StudentDto studentDto, Guid id)
        {
            var response = new BaseResponse<StudentDto>();
            var studentExist = await _unitOfWork.Student.ExistsAsync(s => s.Id == id && s.IsDeleted == false);
            if (!studentExist)
            {
                response.Message = "Not found";
                return response;
            }
            var student = await _unitOfWork.Student.Get(s => s.Id == id);
            if (student == null)
            {
                response.Message = "not found";
                response.Status = false;
            }

            var getUser = await _unitOfWork.User.Get(u => u.Id == student.UserId);
            if (getUser == null)
            {
                response.Message = "User not found";
                response.Status = false;
            }
            getUser.Email = studentDto.Email;
            // await _unitOfWork.User.Update(getUser);

            var getlevel = await _unitOfWork.Level.Get(l => l.LevelName == studentDto.LevelName);
            if (getlevel == null)
            {
                response.Message = "level not found";
                return response;
            }

            student.Email = studentDto.Email ?? student.Email;
            student.FirstName = studentDto.FirstName ?? student.FirstName;
            student.LastName = studentDto.LastName ?? student.LastName;
            student.DateOfBirth = studentDto.DateOfBirth;
            student.PhoneNumber = studentDto.PhoneNumber ?? student.PhoneNumber;
            student.Gender = studentDto.Gender;
            student.Address = studentDto.Address;
            student.LastModifiedOn = DateTime.UtcNow;
            student.Level = getlevel;
            student.LevelId = getlevel.Id;

            await _unitOfWork.SaveChangesAsync();

            response.Message = "updated succesfully";
            response.Status = true;
            return response;
        }
    }
}
