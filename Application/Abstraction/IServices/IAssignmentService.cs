using SchoolApp.Application.Models.Dto;

namespace SchoolApp.Application.Abstraction.IServices
{
    public interface IAssignmentService
    {
        Task<BaseResponse<AssignmentDto>> Create(AssignmentDto assignmentDto);
        Task<BaseResponse<AssignmentDto>> Update(AssignmentDto assignmentDto, Guid assignmentId);
        Task<BaseResponse<AssignmentDto>> Get(Guid id);
        Task<BaseResponse<AssignmentDto>> Delete(Guid assignmentId);
        Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAll();
        Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAllStudentTermAssignment();
        Task<BaseResponse<IEnumerable<AssignmentDto>>> GetAllTeacherAssignment();
    }
}