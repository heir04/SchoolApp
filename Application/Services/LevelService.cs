using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Services
{
    public class LevelService : ILevelService
    {
        private readonly ILevelRepository _levelRepository;
        public LevelService(ILevelRepository levelRepository) 
        {
            _levelRepository = levelRepository;
        }
        public async Task<BaseResponse<LevelDto>> Create(LevelDto levelDto)
        {
            var response = new BaseResponse<LevelDto>();
            var getLevel = await _levelRepository.Get(l => l.LevelName == levelDto.LevelName);
            if (getLevel != null)
            {
                response.Message = "already exist";   
                return response;
            }

            var level = new Level
            {
                LevelName = levelDto.LevelName,
                //TeacherId = levelDto.LevelTeacherId,
                //Teacher = levelDto.LevelTeacher
            };
            await _levelRepository.Register(level);

            var levelDTO = new LevelDto 
            { 
                LevelName = level.LevelName
            };

            response.Message = "created succesfully";
            response.Status = true;
            response.Data = levelDTO;
            return response;
        }

        public async Task<BaseResponse<LevelDto>> Delete(Guid levelId)
        {
            var response = new BaseResponse<LevelDto>();
            var level = await _levelRepository.Get(l => l.Id == levelId);

            if (level is null)
            {
                response.Message = "Not found";
                return response;
            }

            if (level.IsDeleted == true)
            {
                response.Message = "level already deleted";
                return response;
            }

            level.IsDeleted = true;
            await _levelRepository.Update(level);
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<LevelDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<LevelDto>>();
            var levels = await _levelRepository.GetAll();

            if (levels is null)
            {
                response.Message = "No level found";
                return response;
            }

            var levelDtos = levels.Select(l => new LevelDto{
                Id = l.Id,
                LevelName = l.LevelName
            }).ToList();

            response.Data = levelDtos;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<LevelDto>> Update(LevelDto levelDto, Guid levelId)
        {
            var response = new BaseResponse<LevelDto>();
            var level = await _levelRepository.Get(l => l.Id == levelId);
            if (level is null)
            {
                response.Message = "Not found";
                return response;
            }

            level.LevelName = levelDto.LevelName;
            await _levelRepository.Update(level);
            response.Message = "Success";
            response.Status = true;
            return response;
        }

    }
}
