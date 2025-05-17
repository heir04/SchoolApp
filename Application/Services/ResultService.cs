using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace SchoolApp.Application.Services
{
    public class ResultService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ApplicationContext context) : IResultService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ApplicationContext _context = context;

        public async Task<BaseResponse<ResultDto>> Create(
            ResultDto resultDto, Guid studentId)
        {
            var response = new BaseResponse<ResultDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.GetTeacher(t => t.UserId == userId);
            var session = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var subject = teacher.TeacherSubjects.FirstOrDefault();
            var student = await _unitOfWork.Student.GetAsync(studentId);

            if (subject is null)
            {
                response.Message = "No subject found";
                return response;
            }
            
            if (session == null)
            {
                response.Message = "No session is currently set on system. Please try again later";
                return response;
            }
            if (teacher == null)
            {
                response.Message = "Teacher not found";
                return response;
            }

            if (student == null)
            {
                response.Message = "Student not found";
                return response;
            }

            var resultExists = await _unitOfWork.Result
                .ExistsAsync(r => r.SessionId == session.Id && r.StudentId == studentId && 
                                  r.LevelId == student.LevelId);

            

            if (!resultExists)
            {
                var result = new Result
                {
                    StudentId = studentId,
                    Student = student,
                    SessionId = session.Id, 
                    Session = session,
                    LevelId = student.LevelId,
                    Level = student.Level,
                    Terms = resultDto.Terms,
                    Remark = resultDto.Remark,
                    CreatedBy = teacher.Id
                };
                var subjectScore = new SubjectScore
                {
                    SubjectId = subject.SubjectId,
                    Subject = subject.Subject,
                    ResultId = result.Id,
                    Result = result,
                    ContinuousAssessment = resultDto.ContinuousAssessment,
                    ExamScore = resultDto.ExamScore,
                    TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore,
                    CreatedBy = teacher.Id
                };
                _context.SubjectScores.Add(subjectScore);
                result.SubjectScores.Add(subjectScore);
                student.Results.Add(result);
                await _unitOfWork.Result.Register(result);
            }
            else
            {
                var getResult = await _unitOfWork.Result
                .Get(r => r.SessionId == session.Id && r.StudentId == studentId && 
                                  r.LevelId == student.LevelId);
                
                var subjectScoreExists = await _context.SubjectScores.AnyAsync(ss => ss.ResultId == getResult.Id && ss.SubjectId == subject.Id);
                if (subjectScoreExists)
                {
                    response.Message = "Scores already recorded";
                    return response;
                }
                
                var subjectScore = new SubjectScore
                {
                    SubjectId = subject.SubjectId,
                    Subject = subject.Subject,
                    ResultId = getResult.Id,
                    Result = getResult,
                    ContinuousAssessment = resultDto.ContinuousAssessment,
                    ExamScore = resultDto.ExamScore,
                    TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore,
                    CreatedBy = teacher.Id
                };
                _context.SubjectScores.Add(subjectScore);
                getResult.SubjectScores.Add(subjectScore);
            }

           await _unitOfWork.SaveChangesAsync();
           response.Message = "Success";
           response.Status = true;
           return response;
        }

        public async Task<BaseResponse<ResultDto>> Delete(Guid resultId)
        {
            var response = new BaseResponse<ResultDto>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
    
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }
            var result = await _unitOfWork.Result.Get(s => s.Id == resultId);

            if (result is null)
            {
                response.Message = "result not found";
                return response;
            }

            if (result.IsDeleted == true)
            {
                response.Message = "Result already deleted";
                return response;
            }

            result.IsDeleted = true;
            result.IsDeleteBy = userId;
            result.IsDeleteOn = DateTime.UtcNow;
            await _unitOfWork.Result.Update(result);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<ResultDto>> Get(Guid id)
        {
            var response = new BaseResponse<ResultDto>();

            var selectSession = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var result = await _unitOfWork.Result.GetResult(r => r.Id == id && r.SessionId == selectSession.Id);

            if (result == null)
            {
                response.Message = "Result not found";
                return response;
            }

            var resultDto = new ResultDto
            {
                StudentName = $"{result.Student?.FirstName} {result.Student?.LastName}",
                Level = result.Level?.LevelName,
                Terms = result.Terms,
                Remark = result.Remark,
                SubjectScores = result.SubjectScores.Select(s => new SubjectScoreDto
                {
                    SubjectName = s.Subject?.Name,
                    ContinuousAssessment = s.ContinuousAssessment,
                    ExamScore = s.ExamScore,
                    TotalScore = s.ContinuousAssessment + s.ExamScore
                }).ToList()
                
            };

            response.Data = resultDto;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<ResultDto>> CheckResult()
        {
            var response = new BaseResponse<ResultDto>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }
            var checkStudent = await _unitOfWork.Student.Get(s => s.UserId == userId);
            var selectSession = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var result = await _unitOfWork.Result.GetResult(r => r.StudentId == checkStudent.Id && r.SessionId == selectSession.Id);

            if (result == null)
            {
                response.Message = "Result not found";
                return response;
            }

            var resultDto = new ResultDto
            {
                StudentName = $"{result.Student?.FirstName} {result.Student?.LastName}",
                Level = result.Level?.LevelName,
                Terms = result.Terms,
                Remark = result.Remark,
                SubjectScores = result.SubjectScores.Select(s => new SubjectScoreDto
                {
                    SubjectName = s.Subject?.Name,
                    ContinuousAssessment = s.ContinuousAssessment,
                    ExamScore = s.ExamScore,
                    TotalScore = s.TotalScore
                }).ToList()
                
            };

            response.Data = resultDto;
            response.Message = "Success";
            response.Status = true;
            return response;
        }
        public async Task<BaseResponse<IEnumerable<ResultDto>>> GetAllResult(Guid subjectId)
        {
            var response = new BaseResponse<IEnumerable<ResultDto>>();
            var session = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var result = await _unitOfWork.Result.GetAllResult(r => r.Session == session);
            
            if (result is null)
            {
                response.Message = "Result is not available currently";
                return response;
            }
            
            response.Data = result.Select(
                result => new ResultDto{
                    StudentName = $"{result.Student?.FirstName} {result.Student?.LastName}",
                    Level = result.Level?.LevelName,
                    Terms = result.Terms,
                    Remark = result.Remark,
                    SubjectScores = result.SubjectScores.Select(s => new SubjectScoreDto
                    {
                        SubjectName = s.Subject?.Name,
                        ContinuousAssessment = s.ContinuousAssessment,
                        ExamScore = s.ExamScore,
                        TotalScore = s.ContinuousAssessment + s.ExamScore
                    }).ToList()
            }).ToList();
            response.Message = "Success";
            response.Status = true;
            return response;
        }
        public async Task<BaseResponse<ResultDto>> Update(ResultDto resultDto, Guid resultId, Guid subjectId)
        {
            var response = new BaseResponse<ResultDto>();
            var resultExist = await _unitOfWork.Result.ExistsAsync(r => r.Id == resultId);
            var result = await _unitOfWork.Result.Get(r => r.Id == resultId);

            if (!resultExist)
            {
                response.Message = "result not found";
                return response;
            }
            
            var subjectScore = result.SubjectScores.FirstOrDefault();
            if (subjectScore != null)
            {
                subjectScore.ContinuousAssessment = resultDto.ContinuousAssessment;
                subjectScore.ExamScore = resultDto.ExamScore;
                subjectScore.TotalScore = resultDto.TotalScore;
            }

            await _unitOfWork.Result.Update(result);
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<StudentResultStatusDto>>> CreateResultsForLevel(
            ResultDto resultDto, Guid levelId)
        {
            var response = new BaseResponse<IEnumerable<StudentResultStatusDto>>();
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.GetTeacher(t => t.UserId == userId);
            var session = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var subject = teacher.TeacherSubjects.FirstOrDefault();

            if (session == null)
            {
                response.Message = "No session is currently set on the system. Please try again later.";
                return response;
            }

            if (subject is null)
            {
                response.Message = "No subject found";
                return response;
            }

            if (teacher == null)
            {
                response.Message = "Teacher not found.";
                return response;
            }

            var students = await _unitOfWork.Student.GetByExpression(s => s.LevelId == levelId);

            if (students == null || !students.Any())
            {
                response.Message = "No students found for the specified level.";
                return response;
            }

            var studentResultStatuses = new List<StudentResultStatusDto>();

            foreach (var student in students)
            {
                var resultExists = await _unitOfWork.Result.ExistsAsync(r =>
                    r.SessionId == session.Id && r.StudentId == student.Id && r.LevelId == levelId);

                
                if (!resultExists)
                {
                    var result = new Result
                    {
                        StudentId = student.Id,
                        Student = student,
                        SessionId = session.Id,
                        Session = session,
                        LevelId = levelId,
                        Level = student.Level,
                        Terms = resultDto.Terms,
                        Remark = resultDto.Remark,
                        CreatedBy = teacher.Id
                    };

                    var subjectScore = new SubjectScore
                    {
                        SubjectId = subject.SubjectId,
                        Subject = subject.Subject,
                        ResultId = result.Id,
                        Result = result,
                        ContinuousAssessment = resultDto.ContinuousAssessment,
                        ExamScore = resultDto.ExamScore,
                        TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore,
                        CreatedBy = teacher.Id
                    };

                    _context.SubjectScores.Add(subjectScore);
                    result.SubjectScores.Add(subjectScore);
                    student.Results.Add(result);
                    await _unitOfWork.Result.Register(result);
                    
                    studentResultStatuses.Add(new StudentResultStatusDto
                    {
                        StudentName = $"{student.FirstName} {student.LastName}",
                        Status = "Result Created"
                    });
                }
                else
                {
                    var getResult = await _unitOfWork.Result
                    .Get(r => r.SessionId == session.Id && r.StudentId == student.Id && 
                                    r.LevelId == student.LevelId);
                    
                    var subjectScoreExists = await _context.SubjectScores.AnyAsync(ss => ss.ResultId == getResult.Id && ss.SubjectId == subject.Id);
                    
                    if (subjectScoreExists)
                    {
                        studentResultStatuses.Add(new StudentResultStatusDto
                        {
                            StudentName = $"{student.FirstName} {student.LastName}",
                            Status = "Result Already Exists"
                        });
                    }
                    
                    var subjectScore = new SubjectScore
                    {
                        SubjectId = subject.SubjectId,
                        Subject = subject.Subject,
                        ResultId = getResult.Id,
                        Result = getResult,
                        ContinuousAssessment = resultDto.ContinuousAssessment,
                        ExamScore = resultDto.ExamScore,
                        TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore,
                        CreatedBy = teacher.Id
                    };
                    _context.SubjectScores.Add(subjectScore);
                    getResult.SubjectScores.Add(subjectScore);  
                }
            }

            await _unitOfWork.SaveChangesAsync();

            response.Data = studentResultStatuses;
            response.Message = "Results processed successfully.";
            response.Status = true;
            return response;
        }
    }
}