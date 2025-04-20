using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Services
{
    public class SessionService(IUnitOfWork unitOfWork, ISessionRepository sessionRepository) : ISessionService
{
    public IUnitOfWork _unitOfWork = unitOfWork;
    public ISessionRepository _sessionRepository = sessionRepository;
    // public SessionService(IUnitOfWork unitOfWork, ISessionRepository sessionRepository)
    // {
    //     _unitOfWork = unitOfWork;
    //     _sessionRepository = sessionRepository;
    // }
    public async Task<BaseResponse<SessionDto>> Create(SessionDto sessionDto)
    {
        var response = new BaseResponse<SessionDto>();

        var ifExist = await _unitOfWork.Session.ExistsAsync(s => s.CurrentSession == true);
        if (ifExist)
        {
            response.Message = "A Session is still ongoing";
            return response;
        }

        var session = new Session
        {
            SessionName = sessionDto.SessionName,
            StartDate = sessionDto.StartDate,
            EndDate = sessionDto.EndDate,
            CurrentSession = true,
            CreatedOn = DateTime.Today
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
        
        session.CurrentSession = false;
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
            EndDate = s.EndDate,
            CurrentSession = s.CurrentSession
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