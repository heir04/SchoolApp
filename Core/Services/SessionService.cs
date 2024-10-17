using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Core.Dto;
using SchoolApp.Core.IServices;

namespace SchoolApp.Core.Services
{
    public class SessionService : ISessionService
{
    public IUnitOfWork _unitOfWork;
    public ISessionRepository _sessionRepository;
    public SessionService(IUnitOfWork unitOfWork, ISessionRepository sessionRepository)
    {
        _unitOfWork = unitOfWork;
        _sessionRepository = sessionRepository;
    }
    public async Task<BaseResponse<SessionDto>> Create(SessionDto sessionDto)
    {
        var response = new BaseResponse<SessionDto>();

        var ifExist = await _unitOfWork.Session.Get(s => s.SessionName == sessionDto.SessionName);
        if (ifExist != null)
        {
            response.Message = "Session already exist";
            return response;
        }

        var session = new Session
        {
            SessionName = sessionDto.SessionName,
            StartDate = sessionDto.StartDate,
            EndDate = sessionDto.EndDate,
            CurrentSession = true
        };

       await _unitOfWork.Session.Register(session);
       response.Message = "Success";
       response.Status = true;
       return response;
    }

    public async Task<BaseResponse<SessionDto>> Delete(Guid sessionId)
    {
        var response = new BaseResponse<SessionDto>();
        var session = await _unitOfWork.Session.Get(t => t.Id == sessionId);

        if (session is null)
        {
            response.Message = "Session not found";
            return response;
        }

        if (session.IsDeleted == true)
        {
            response.Message = "Session already deleted";
            return response;
        }

        session.IsDeleted = true;
        await _unitOfWork.Session.Update(session);
        response.Message = "Deleted Successfully";
        response.Status = true;
        return response;
    }

    public async Task<BaseResponse<SessionDto>> Get(Guid id)
    {
        var response = new BaseResponse<SessionDto>();
        var session = await _sessionRepository.Get(s => s.Id == id);
        if (session is null)
        {
            response.Message = "Not found";
            return response;
        }

        var sessionDto = new SessionDto
        {
            SessionName = session.SessionName,
            StartDate = session.StartDate,
            EndDate = session.EndDate
        };
        response.Data = sessionDto;
        response.Message = "Success";
        response.Status = true;
        return response;
    }
    public async Task<BaseResponse<IEnumerable<SessionDto>>> GetAll()
    {
        var response = new BaseResponse<IEnumerable<SessionDto>>();
        var sessions = await _unitOfWork.Session.GetAll();

        if (sessions is null)
        {
            response.Message = "No session registered";
            return response;
        }

        var sessionDtos = sessions.Select(s => new SessionDto{
            Id = s.Id,
            SessionName = s.SessionName,
            StartDate = s.StartDate,
            EndDate = s.EndDate
        }).ToList();

        response.Data = sessionDtos;
        response.Message = "Success";
        response.Status = true;
        return response;
    }

    public async Task<BaseResponse<SessionDto>> Update(SessionDto sessionDto, Guid sessionId)
    {
        var response = new BaseResponse<SessionDto>();
        var session = await _unitOfWork.Session.Get(l => l.Id == sessionId);
        if (session is null)
        {
            response.Message = "Session not found";
            return response;
        }

        session.SessionName = sessionDto.SessionName;
        session.StartDate = sessionDto.StartDate;
        session.EndDate = sessionDto.EndDate;
        await _unitOfWork.Session.Update(session);
        response.Message = "Success";
        response.Status = true;
        return response;
    }
}
}