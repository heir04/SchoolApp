using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Application.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationContext _context;


        public TeacherService(IUnitOfWork unitOfWork, ApplicationContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }
        public async Task<BaseResponse<TeacherDto>> Register(TeacherDto teacherDto)
        {
            var response = new BaseResponse<TeacherDto>();
            var emailExist = await _unitOfWork.Teacher.Get(t => t.Email == teacherDto.Email);
            if (emailExist is not null)
            {
                response.Message = "Email already in use";
                return response;
            }

            var user = new User
            {
                UserName = $"{teacherDto.FirstName}{teacherDto.LastName}",
                Password = BCrypt.Net.BCrypt.HashPassword(teacherDto.Password),
                Email = teacherDto.Email
            };
            await _unitOfWork.User.Register(user);

            var role = await _unitOfWork.Role.Get(r => r.Name == "Teacher");
            if (role == null)
            {
                
                response.Message = "Role not found";
                response.Status = false;
            }

            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            };
           _context.UserRoles.Add(userRole);
            
            var teacher = new Teacher
            { 
                FirstName = teacherDto.FirstName,
                LastName = teacherDto.LastName,
                Email = teacherDto.Email,
                UserId = user.Id,
                User = user
            
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
                    Subject = subject
                };
                teacherSubjects.Add(teacherSubject);
           }

           teacher.TeacherSubjects = teacherSubjects;
            
            await _unitOfWork.Teacher.Register(teacher);
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
            var teachers = await _unitOfWork.Teacher.GetAll();
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