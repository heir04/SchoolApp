using System.Security.Cryptography.X509Certificates;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Application.Services
{
    public class TeacherService(IUnitOfWork unitOfWork, ApplicationContext context) : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ApplicationContext _context = context;

        public async Task<BaseResponse<TeacherDto>> Register(TeacherDto teacherDto)
        {
            var response = new BaseResponse<TeacherDto>();
            var defaultPassword = $"{teacherDto.FirstName}";
            string saltString = HashingHelper.GenerateSalt();
            string hashedPassword = HashingHelper.HashPassword(defaultPassword, saltString);

            var emailExist = await _unitOfWork.Teacher.ExistsAsync(t => t.Email == teacherDto.Email);
            if (emailExist)
            {
                response.Message = "Email already in use";
                return response;
            }

            var user = new User
            {
                UserName = $"{teacherDto.FirstName}{teacherDto.LastName}",
                HashSalt = saltString,
                PasswordHash = hashedPassword,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Email = teacherDto.Email?.ToLower()
            };

            var role = await _unitOfWork.Role.Get(r => r.Name == "Teacher");
            if (role is null)
            {
                response.Message = "Role not found";
                response.Status = false;
            }

            var userRole = new UserRole
            {
                User = user,
                UserId = user.Id,
                Role = role,
                RoleId = role.Id
            };
           _context.UserRoles.Add(userRole);
           user.UserRoles.Add(userRole);
            
            var teacher = new Teacher
            { 
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email?.ToLower(),
                UserId = user.Id,
                User = user,
                CreatedOn = DateTime.Today
            };
            
           var subjects = await _unitOfWork.Subject.GetAllByIdsAsync(teacherDto.SubjectIds);

           var teacherSubjects = new HashSet<TeacherSubject>();
           foreach (var subject in subjects)
           {
                var teacherSubject = new TeacherSubject
                {
                    TeacherId = teacherDto.Id,
                    SubjectId = subject.Id,
                    Teacher = teacher,
                    Subject = subject,
                    CreatedOn = DateTime.Today
                };
                teacherSubjects.Add(teacherSubject);
           }

           teacher.TeacherSubjects = teacherSubjects;
            
            await _unitOfWork.Teacher.Register(teacher);
            await _unitOfWork.User.Register(user);
            await _unitOfWork.SaveChangesAsync();
            response.Message = "Created successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<TeacherDto>> Get(Guid id)
        {
            var response = new BaseResponse<TeacherDto>();
            var teacher = await _unitOfWork.Teacher.Get(t => t.Id == id);

            if (teacher == null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var teacherDto = new TeacherDto
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email
            };

            response.Data =teacherDto;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<TeacherDto>> Get(string email)
        {
            var response = new BaseResponse<TeacherDto>();
            var teacher = await _unitOfWork.Teacher.Get(t => t.Email == email);

            if (teacher == null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            var teacherDto = new TeacherDto
            {
                FirstName = teacher.FirstName,
                LastName = teacher.LastName,
                Email = teacher.Email
            };

            response.Data =teacherDto;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<TeacherDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<TeacherDto>>();
            var teachers = await _unitOfWork.Teacher.GetAllTeachers();
            if (teachers == null || teachers.Count == 0)
            {
                response.Message = "No teachers found";
                return response;
            }
            
            var teacherDto = teachers.Select(t => new TeacherDto
            {
                Id = t.Id,
                FirstName = t.FirstName,
                LastName = t.LastName,
                Email = t.Email,
                Subjects = t.TeacherSubjects.Select(ts => ts.Subject.Name).ToList()
            }).ToList();
            
            response.Message = "Success";
            response.Status = true;
            response.Data = teacherDto;
            return response;
        }

        public async Task<BaseResponse<TeacherDto>> Update(TeacherDto teacherDto, Guid id)
        {
            var response = new BaseResponse<TeacherDto>();
            var teacher = await _unitOfWork.Teacher.Get(t => t.Id == id);
            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            teacher.FirstName = teacherDto.FirstName;
            teacher.LastName = teacherDto.LastName;
            teacher.Email = teacherDto.Email;
            await _unitOfWork.Teacher.Update(teacher);
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<TeacherDto>> Delete(Guid id)
        {
            var response = new BaseResponse<TeacherDto>();
            var teacher = await _unitOfWork.Teacher.Get(t => t.Id == id);

            if (teacher is null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            if (teacher.IsDeleted == true)
            {
                response.Message = "Subject already deleted";
                return response;
            }

            teacher.IsDeleted = true;
            await _unitOfWork.Teacher.Update(teacher);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }
    }
}