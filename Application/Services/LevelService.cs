using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Application.Abstraction.IServices;
using SchoolApp.Application.Models.Dto;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Helper;

namespace SchoolApp.Application.Services
{
    public class LevelService(IUnitOfWork unitOfWork, ValidatorHelper validator) : ILevelService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly ValidatorHelper _validator = validator;

        public async Task<BaseResponse<LevelDto>> Create(LevelDto levelDto)
        {
            var response = new BaseResponse<LevelDto>();
            var getLevel = await _unitOfWork.Level.ExistsAsync(l => l.LevelName == levelDto.LevelName && l.IsDeleted == false);
            if (getLevel)
            {
                response.Message = "already exist";   
                return response;
            }

            var ValidateCategory = _validator.ValidateCategory(levelDto.Category);
            if (!ValidateCategory)
            {
                response.Message = "Invalid Category";
                return response;
            }

            var level = new Level
            {
                LevelName = levelDto.LevelName,
                Category = levelDto.Category,
                CreatedOn = DateTime.Today
                //TeacherId = levelDto.LevelTeacherId,
                //Teacher = levelDto.LevelTeacher
            };
            await _unitOfWork.Level.Register(level);
            await _unitOfWork.SaveChangesAsync();


            response.Message = "created succesfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<LevelDto>> Delete(Guid levelId)
        {
            var response = new BaseResponse<LevelDto>();
            var levelExist = await _unitOfWork.Level.ExistsAsync(l => l.Id == levelId && l.IsDeleted == false);
            if (!levelExist)
            {
                response.Message = "Not found";
                return response;
            }
            var level = await _unitOfWork.Level.Get(l => l.Id == levelId);

            if (level is null )
            {
                response.Message = "Not found";
                return response;
            }

            level.IsDeleted = true;
            await _unitOfWork.Level.SaveChangesAsync();
            response.Message = "Deleted Successfully";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<IEnumerable<LevelDto>>> GetAll()
        {
            var response = new BaseResponse<IEnumerable<LevelDto>>();
            var levels = await _unitOfWork.Level.GetAll();

            if (levels is null || levels.Count() == 0)
            {
                response.Message = "No level found";
                return response;
            }

            var levelDtos = levels
            .Where(l => l.IsDeleted == false)
            .Select(l => new LevelDto
            {
                Id = l.Id,
                LevelName = l.LevelName,
                Category = l.Category ?? string.Empty
            }).ToList();

            response.Data = levelDtos;
            response.Message = "Success";
            response.Status = true;
            return response;
        }

        public async Task<BaseResponse<LevelDto>> Update(LevelDto levelDto, Guid levelId)
        {
            var response = new BaseResponse<LevelDto>();
            var levelExist = await _unitOfWork.Level.ExistsAsync(l => l.Id == levelId && l.IsDeleted == false);
            if (!levelExist)
            {
                response.Message = "Not found";
                return response;
            }

            var level = await _unitOfWork.Level.Get(l => l.Id == levelId);
            if (level is null)
            {
                response.Message = "Not found";
                return response;
            }

            var ValidateCategory = _validator.ValidateCategory(levelDto.Category);
            if (!ValidateCategory)
            {
                response.Message = "Invalid Category";
                return response;
            }

            level.LevelName = levelDto.LevelName;
            level.Category = levelDto.Category;
            await _unitOfWork.Level.SaveChangesAsync();
            response.Message = "Success";
            response.Status = true;
            return response;
        }

    }
}
