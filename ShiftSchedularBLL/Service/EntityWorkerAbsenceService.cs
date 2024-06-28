using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularRL.Resources.AbsenceManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class EntityWorkerAbsenceService : IEntityWorkerAbsenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAbsenceTypeLocalizationRepository _absenceTypeLocalizationRepository;
        private readonly IGenericRepository<EntityWorkerAbsence> _entityWorkerAbsenceRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IMapper _mapper;

        #region Constructor

        public EntityWorkerAbsenceService(IUnitOfWork unitOfWork,
           IAbsenceTypeLocalizationRepository absenceTypeLocalizationRepository,
           IGenericRepository<EntityWorkerAbsence> entityWorkerAbsenceRepository,
           ILocalizationRepository localizationRepository,
           IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _absenceTypeLocalizationRepository = absenceTypeLocalizationRepository;
            _entityWorkerAbsenceRepository = entityWorkerAbsenceRepository;
            _localizationRepository = localizationRepository;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        public async Task<BaseResponse<bool>> AbsenceApprovalDecision(AbsenceApprovalDecisionDTO absenceApprovalDecisionDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            return response;
        }

        public Task<BaseResponse<EntityWorkerAbsenceDTO>> AddEntityWorkerAbsence(AddEntityWorkerAbsenceDTO addEntityWorkerAbsence)
        {
            throw new NotImplementedException();
        }

        #region Delete Entity Worker Absence

        public async Task<BaseResponse<bool>> DeleteEntityWorkerAbsence(string absenceId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.DeleteEntityWorkerAbsenceUnexpectedError;

            if(string.IsNullOrWhiteSpace(absenceId))
            {
                response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                return response;
            }

            EntityWorkerAbsence entityWorkerAbsence = await _entityWorkerAbsenceRepository.GetById(absenceId);
            if(entityWorkerAbsence == null)
            {
                response.Message = AbsenceRelatedMessages.AbsenceNotFound;
                return response;
            }
            else
            {
                await _entityWorkerAbsenceRepository.Delete(entityWorkerAbsence.EntityWorkerAbsenceId);
                response.Success = true;
                response.Message = AbsenceRelatedMessages.DeleteEntityWorkerAbsenceSuccessMessage;
            }

            return response;
        }

        #endregion

        public Task<EntityWorkerAbsenceDTO> GetEntityWorkerAbsenceById(string id, string lcode)
        {
            throw new NotImplementedException();
        }

        public Task<EntityWorkerAbsenceViewModel> GetEntityWorkerAbsenceViewModel(BaseViewModelRequest viewModelRequestDTO)
        {
            throw new NotImplementedException();
        }

        public async Task<BaseResponse<bool>> UpdateEntityWorkerAbsence(EntityWorkerAbsenceDTO entityWorkerAbsenceDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            return response;
        }

        #endregion
    }
}
