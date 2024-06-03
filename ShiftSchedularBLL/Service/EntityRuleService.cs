using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class EntityRuleService : IEntityRuleService
    {
        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IUnitOfWork _unitOfWork;

        public Task<BaseResponse<EntityRuleDTO>> AddEntityRule(AddEntityRuleDTO addEntityRuleDTO)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<EntityRuleSpecificationDTO>> AddEntityRuleSpecification(AddEntityRuleSpecificationDTO addEntityRuleSpecificationDTO)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> DeleteEntityRule(string entityId, string entityRuleId)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> DeleteEntityRuleSpecification(string entityRuleId, int specificationId)
        {
            throw new NotImplementedException();
        }

        public Task<EntityRuleDTO> GetEntityRuleById(string entityRuleId, string lcode)
        {
            throw new NotImplementedException();
        }

        public Task<EntityRuleViewModel> GetEntityRuleViewModel(BaseViewModelRequest entityRuleViewModelRequestDTO)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> UpdateEntityRule(EntityRuleDTO entityRule)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> UpdateEntityRuleSpecification(EntityRuleSpecificationDTO entityRuleSpecification)
        {
            throw new NotImplementedException();
        }
    }
}
