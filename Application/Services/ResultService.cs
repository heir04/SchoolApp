using System.Security.Claims;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Services
{
    public class ResultService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor) : IResultService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public async Task<BaseResponse<ResultDto>> Create(
            ResultDto resultDto, Guid studentId)
        {
            var response = new BaseResponse<ResultDto>();

            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
            {
                response.Message = "Invalid user ID";
                return response;
            }

            var teacher = await _unitOfWork.Teacher.Get(t => t.UserId == userId);
            var selectSession = await _unitOfWork.Session.Get(s => s.CurrentSession == true);
            var subject = teacher.TeacherSubjects.SingleOrDefault();
            var student = await _unitOfWork.Student.GetAsync(studentId);
            var checkResult = await _unitOfWork.Result
                .ExistsAsync(r => r.SessionId == selectSession.Id && r.StudentId == studentId && 
                                  r.LevelId == student.LevelId);
            
            if (selectSession == null)
            {
                response.Message = "No session is currently set on system. Please try again later";
                return response;
            }

            if (checkResult)
            { 
                response.Message = "Result already added";
                return response;
            }

            var result = new Result
            {
                StudentId = studentId,
                Student = student,
                SessionId = selectSession.Id, 
                Session = selectSession,
                LevelId = student.LevelId,
                Level = student.Level,
                Terms = resultDto.Terms,
                Remark = resultDto.Remark
            };

            var subjectScore = new SubjectScore
            {
                SubjectId = subject.SubjectId,
                Subject = subject.Subject,
                ContinuousAssessment = resultDto.ContinuousAssessment,
                ExamScore = resultDto.ExamScore,
                TotalScore = resultDto.ContinuousAssessment + resultDto.ExamScore,
            };
            result.SubjectScores.Add(subjectScore);
            
           student.Results.Add(result);

           await _unitOfWork.Result.Register(result);
           await _unitOfWork.SaveChangesAsync();
           response.Message = "Success";
           response.Status = true;
           return response;
        }

        public async Task<BaseResponse<ResultDto>> Delete(Guid resultId)
        {
            var response = new BaseResponse<ResultDto>();
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
            await _unitOfWork.Result.Update(result);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;;
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
            var userIdClaim = _httpContextAccessor.HttpContext?.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var checkStudent = await _unitOfWork.Student.Get(s => s.UserId.ToString() == userIdClaim);
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
                    TotalScore = s.ContinuousAssessment + s.ExamScore
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
                response.Message = "Result is not availlable currently";
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
    }
}