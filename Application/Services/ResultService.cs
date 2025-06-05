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

        public async Task<BaseResponse<ResultDto>> Create(ResultDto resultDto, Guid studentId)
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
            var term = await _unitOfWork.Term.Get(s => s.CurrentTerm == true);
            var subject = teacher.TeacherSubjects.FirstOrDefault();
            var student = await _unitOfWork.Student.GetAsync(studentId);

            if (subject is null)
            {
                response.Message = "No subject found";
                return response;
            }

            if (session == null || term is null)
            {
                response.Message = "No session or term is currently set on system. Please try again later";
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
                    TermId = term.Id,
                    Term = term,
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
                getResult.SubjectScores.Add(subjectScore);
            }

            await _unitOfWork.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<StudentResultStatusDto>>> CreateBulkResults(BulkResultDto bulkResultDto)
        {
            var response = new BaseResponse<IEnumerable<StudentResultStatusDto>>();
            var studentResultStatuses = new List<StudentResultStatusDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.GetTeacher(t => t.UserId == userId);
            var session = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var term = await _unitOfWork.Term.Get(t => t.CurrentTerm == true);
            var subject = await _unitOfWork.Subject.Get(s => s.Id == bulkResultDto.SubjectId);

            if (teacher == null)
            {
                response.Message = "Teacher not found";
                return response;
            }
            if (session is null || term is null)
            {
                response.Message = "No session or term is currently set on the system. Please try again later.";
                return response;
            }
            if (subject == null)
            {
                response.Message = "Subject not found";
                return response;
            }

            if (!teacher.TeacherSubjects.Any(ts => ts.SubjectId == bulkResultDto.SubjectId))
            {
                response.Message = "Teacher is not assigned to this subject";
                return response;
            }

            foreach (var studentScore in bulkResultDto.StudentScores)
            {
                var student = await _unitOfWork.Student.GetAsync(studentScore.StudentId);
                if (student == null)
                {
                    studentResultStatuses.Add(new StudentResultStatusDto
                    {
                        StudentName = studentScore.StudentName,
                        Status = "Failed",
                        Message = "Student not found"
                    });
                    continue;
                }

                var resultExists = await _unitOfWork.Result.ExistsAsync(r =>
                    r.SessionId == session.Id && r.StudentId == student.Id && r.LevelId == bulkResultDto.LevelId);

                try
                {
                    if (!resultExists)
                    {
                        var result = new Result
                        {
                            StudentId = student.Id,
                            Student = student,
                            SessionId = session.Id,
                            Session = session,
                            LevelId = bulkResultDto.LevelId,
                            Level = student.Level,
                            TermId = term.Id,
                            Term = term,
                            CreatedBy = teacher.Id
                        };

                        var subjectScore = new SubjectScore
                        {
                            SubjectId = subject.Id,
                            Subject = subject,
                            ResultId = result.Id,
                            Result = result,
                            ContinuousAssessment = studentScore.ContinuousAssessment,
                            ExamScore = studentScore.ExamScore,
                            TotalScore = studentScore.ContinuousAssessment + studentScore.ExamScore,
                            CreatedBy = teacher.Id
                        };

                        result.SubjectScores.Add(subjectScore);
                        student.Results.Add(result);
                        await _unitOfWork.Result.Register(result);

                        studentResultStatuses.Add(new StudentResultStatusDto
                        {
                            StudentName = $"{student.FirstName} {student.LastName}",
                            Status = "Created"
                        });
                    }
                    else
                    {
                        var existingResult = await _unitOfWork.Result.Get(r =>
                            r.SessionId == session.Id && r.StudentId == student.Id && r.LevelId == bulkResultDto.LevelId);

                        var subjectScoreExists = await _context.SubjectScores.AnyAsync(ss =>
                            ss.ResultId == existingResult.Id && ss.SubjectId == subject.Id);

                        if (subjectScoreExists)
                        {
                            studentResultStatuses.Add(new StudentResultStatusDto
                            {
                                StudentName = $"{student.FirstName} {student.LastName}",
                                Status = "Skipped",
                                Message = "Scores already recorded for this subject"
                            });
                            continue;
                        }

                        var subjectScore = new SubjectScore
                        {
                            SubjectId = subject.Id,
                            Subject = subject,
                            ResultId = existingResult.Id,
                            Result = existingResult,
                            ContinuousAssessment = studentScore.ContinuousAssessment,
                            ExamScore = studentScore.ExamScore,
                            TotalScore = studentScore.ContinuousAssessment + studentScore.ExamScore,
                            CreatedBy = teacher.Id
                        };

                        existingResult.SubjectScores.Add(subjectScore);

                        studentResultStatuses.Add(new StudentResultStatusDto
                        {
                            StudentName = $"{student.FirstName} {student.LastName}",
                            Status = "Updated"
                        });
                    }
                }
                catch (Exception ex)
                {
                    studentResultStatuses.Add(new StudentResultStatusDto
                    {
                        StudentName = $"{student.FirstName} {student.LastName}",
                        Status = "Failed",
                        Message = $"Error processing result: {ex.Message}"
                    });
                }
            }

            await _unitOfWork.SaveChangesAsync();

            response.Data = studentResultStatuses;
            response.Message = "Bulk results processed successfully.";
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
                SessionName = result.Session?.SessionName,
                TermName = result.Term?.Name,
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

            if (result == null || result.Remark is null)
            {
                response.Message = "Result not released yet";
                return response;
            }

            var resultDto = new ResultDto
            {
                StudentName = $"{result.Student?.FirstName} {result.Student?.LastName}",
                Level = result.Level?.LevelName,
                TermName = result.Term?.Name,
                SessionName = result.Session?.SessionName,
                Remark = result.Remark,
                SubjectScores = [.. result.SubjectScores.Select(s => new SubjectScoreDto
                {
                    SubjectName = s.Subject?.Name,
                    ContinuousAssessment = s.ContinuousAssessment,
                    ExamScore = s.ExamScore,
                    TotalScore = s.TotalScore
                })]

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
                result => new ResultDto
                {
                    StudentName = $"{result.Student?.FirstName} {result.Student?.LastName}",
                    Level = result.Level?.LevelName,
                    TermName = result.Term?.Name,
                    Remark = result.Remark,
                    SubjectScores = [.. result.SubjectScores.Select(s => new SubjectScoreDto
                    {
                        SubjectName = s.Subject?.Name,
                        ContinuousAssessment = s.ContinuousAssessment,
                        ExamScore = s.ExamScore,
                        TotalScore = s.ContinuousAssessment + s.ExamScore
                    })]
                }).ToList();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<ResultDto>> Update(ResultDto resultDto, Guid resultId, Guid subjectId)
        {
            var response = new BaseResponse<ResultDto>();
            var resultExist = await _unitOfWork.Result.ExistsAsync(r => r.Id == resultId);
            var result = await _unitOfWork.Result.GetResultSubjectScore(r => r.Id == resultId);

            if (!resultExist || result.IsDeleted == true)
            {
                response.Message = "result not found";
                return response;
            }

            var subjectScore = result.SubjectScores.FirstOrDefault(s => s.SubjectId == subjectId);
            if (subjectScore == null)
            {
                response.Message = "Subject score not found for this subject";
                return response;
            }

            subjectScore.ContinuousAssessment = resultDto.ContinuousAssessment;
            subjectScore.ExamScore = resultDto.ExamScore;
            subjectScore.TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore;


            await _unitOfWork.Result.Update(result);
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<ResultDto>> GiveRemark(Guid resultId, ResultDto resultDto)
        {
            var response = new BaseResponse<ResultDto>();
            var result = await _unitOfWork.Result.Get(r => r.Id == resultId);

            if (result == null)
            {
                response.Message = "Result not found";
                return response;
            }

            result.Remark = resultDto.Remark;

            await _unitOfWork.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }
    }
}