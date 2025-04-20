using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Helper;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Application.Services
{
    public class StudentService(IUnitOfWork unitOfWork, ApplicationContext context) : IStudentService
    {
        private readonly ApplicationContext _context = context;
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        // public StudentService(IUnitOfWork unitOfWork, ApplicationContext context)
        // {
        //     _context = context;
        //     _unitOfWork = unitOfWork;
        // }

        public async Task<BaseResponse<StudentDto>> Delete(Guid id)
        {
            var response = new BaseResponse<StudentDto>();
            var student = await _unitOfWork.Student.Get(a => a.Id == id);
            if (student == null || student.IsDeleted == true)
            {
                
                response.Message = "not found or is already deleted";
                response.Status = false;
                return response;
            }
            student.IsDeleted = true;

            response.Message = "Student deleted";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Get(Guid id)
        {
            var response =  new BaseResponse<StudentDto>();
            var student = await _unitOfWork.Student.Get(s => s.Id == id);
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
                //LevelId = student.LevelId,
            };
           
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Get(string email)
        {
            var response = new BaseResponse<StudentDto>();
            var student = await _unitOfWork.Student.Get(s =>s.Email == email);
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
            };
            
            response.Data = studentDto;
            response.Message = "getStudent gotten";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<StudentDto>>();
            var students = await _unitOfWork.Student.GetAll();
            if (students == null || students.Count == 0)
            {
                response.Message = "no getStudent in this level";
                response.Status = false;
                return response;
            }

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                LevelId = s.LevelId,
                LevelName = s.Level?.LevelName,
                Email = s.Email,
            }).ToList();
            
            response.Message = "list of students";
            response.Status = true;
            response.Data = studentDtos;
            return response;  
        }

        public async Task<BaseResponse<IEnumerable<StudentDto>>> GetAll(Guid levelId)
        {
            var response = new BaseResponse<IEnumerable<StudentDto>>();
            var students = await _unitOfWork.Student.GetByExpression(s => s.LevelId == levelId);
            if (students == null || students.Count == 0)
            {
                response.Message = "no Student registered in this level";
                response.Status = false;
                return response;
            }

            var studentDtos = students.Select(s => new StudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Email = s.Email,
                LevelId = s.LevelId,
                LevelName = s.Level?.LevelName
            }).ToList();

            response.Message = "list of students";
            response.Status = true;
            response.Data = studentDtos;
            return response;
        }

        public async Task<BaseResponse<StudentDto>> GetByStudentId(string studentId)
        {
            var response = new BaseResponse<StudentDto>();
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
                LevelName = student.LastName  
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
                response.Status = false;
            }

            var user = new User
            {
                UserName = $"{studentDto.FirstName}{studentDto.LastName}",
                HashSalt = saltString,
                PasswordHash = hashedPassword,
                // PasswordHash = BCrypt.Net.BCrypt.HashPassword(defaultPassword),
                Email = studentDto.Email?.ToLower()
            };

            var role = await _unitOfWork.Role.Get(r => r.Name == "Student");
            if (role == null)
            {
                response.Message = "Role not found";
                response.Status = false;
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
                response.Status = false;
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
                CreatedOn = DateTime.Today
            };
            var addStudent = await _unitOfWork.Student.Register(student);
            await _unitOfWork.User.Register(user);
            await _unitOfWork.SaveChangesAsync();
            // student.CreatedBy = addStudent.Id;
            // student.LastModifiedBy = addStudent.Id;
            // student.IsDeleted = false;
            // await _unitOfWork.Student.Update(student);
        

            // var studentDTo = new StudentDto
            // {
            //     Id = addStudent.Id,
            //     StudentId = addStudent.StudentId,
            //     FirstName = addStudent.FirstName,
            //     LastName = addStudent.LastName,
            //     Email = addStudent.Email,
            //     LevelName = addStudent.Level.LevelName
            //     //LevelId = addStudent.LevelId
            // };

          
            response.Message = "Student registered succesfully";
            response.Status = true;
            // Data = studentDTo
            return response;
        }

        public async Task<BaseResponse<StudentDto>> Update(StudentDto studentDto, Guid id)
        {
            var response = new BaseResponse<StudentDto>();
            var student = await _unitOfWork.Student.Get(s => s.Id == id);
            if (student == null)
            {
                response.Message = "not found";
                response.Status = false;
            }

            var getUser = await _unitOfWork.User.Get(u => u.Email == studentDto.Email);
            if (getUser == null)
            {
                response.Message = "User not found";
                response.Status = false;
            }
            getUser.Email = studentDto.Email;
            await _unitOfWork.User.Update(getUser);

            student.User.Email = studentDto.Email ?? student.User.Email;
            student.FirstName = studentDto.FirstName ?? student.FirstName;
            student.LastName = studentDto.LastName ?? student.LastName;
            //student.LevelId = studentDto.LevelId;
            await _unitOfWork.Student.Update(student);
            await _unitOfWork.SaveChangesAsync();

            response.Message = "updated succesfully";
            response.Status = true;
            return response;
        }
    }
}
