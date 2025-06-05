using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Application.Services
{
    public class SessionService(IUnitOfWork unitOfWork, ApplicationContext context) : ISessionService
{
    public IUnitOfWork _unitOfWork = unitOfWork;
    public ApplicationContext _context = context;

    public async Task<BaseResponse<SessionDto>> Create(SessionDto sessionDto)
    {
        var response = new BaseResponse<SessionDto>();

        var sessionExist = await _unitOfWork.Session.ExistsAsync(s => s.CurrentSession == true);
        if (sessionExist)
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
            Terms =
            [
                new() { Name = "First Term", CurrentTerm = true},
                new() { Name = "Second Term", CurrentTerm = false},
                new() { Name = "Third Term", CurrentTerm = false}
            ]
        };

       await _unitOfWork.Session.Register(session);
       await _unitOfWork.SaveChangesAsync();
       response.Message = "Success";
       response.Status = true;
       return response;
    }

    public async Task<BaseResponse<SessionDto>> UpdateTerm(Guid termId)
    {
        var response = new BaseResponse<SessionDto>();
        var session = await _unitOfWork.Session.GetCurrentSession();
        var currentTerm = session.Terms.FirstOrDefault(t => t.CurrentTerm == true);
        var term = await _unitOfWork.Term.Get(t => t.Id == termId);
        
        if (session is null || currentTerm is null)
        {
            response.Message = "No current session or term found";
            return response;
        }

        currentTerm.CurrentTerm = false;
        term.CurrentTerm = true;

        await _unitOfWork.SaveChangesAsync();
        response.Message = "Term updated";
        response.Status = true;
        return response;
    }

    public async Task<BaseResponse<SessionDto>> EndSession()
    {
        var response = new BaseResponse<SessionDto>();
        var getSession = await _unitOfWork.Session.Get(s => s.CurrentSession == true && s.IsDeleted == false);
        if (getSession is null)
        {
            response.Message = "No ongoing session found";
            return response;
        }

        getSession.CurrentSession = false;

        await _unitOfWork.Session.Update(getSession);
        response.Message = "Session ended successfully";
        response.Status = true;
        return response;
    }

    public async Task<BaseResponse<SessionDto>> Delete(Guid sessionId)
    {
        var response = new BaseResponse<SessionDto>();
        var sessionExist = await _unitOfWork.Session.ExistsAsync(s => s.Id == sessionId && s.IsDeleted == false);
        var session = await _unitOfWork.Session.Get(t => t.Id == sessionId);

        if (!sessionExist)
        {
            response.Message = "Session not found";
            return response;
        }

        if(session.CurrentSession == true)
        {
            response.Message = "Cannot delete an ongoing session. End session before deleting.";
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
        var sessionExist = await _unitOfWork.Session.ExistsAsync(s => s.Id == id && s.IsDeleted == false);
        if (!sessionExist)
        {
            response.Message = "session not found";
            return response;
        }

        var session = await _unitOfWork.Session.Get(s => s.Id == id);
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

        if (sessions is null || sessions.Count() == 0)
        {
            response.Message = "No session registered";
            return response;
        }

        var sessionDtos = sessions
        .Where(s => s.IsDeleted == false)
        .Select(s => new SessionDto{
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
        var sessionExist = await _unitOfWork.Session.ExistsAsync(s => s.Id == sessionId && s.IsDeleted == false);
        if (!sessionExist)
        {
            response.Message = "session not found";
            return response;
        }
        
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