using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.ShiftManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class ShiftTemplateService : IShiftTemplateService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeneralService _generalService;
        private readonly IGenericRepository<ShiftBreakTemplate> _shiftBreakTemplateRepository;
        private readonly IGenericRepository<ShiftTemplate> _shiftTemplateRepository;
        private readonly IShiftTemplateBreaksRepository _shiftTemplateBreaksRepository;
        private readonly IShiftBreakTypeLocalizationRepository _shiftBreakTypeLocalizationRepository;
        private readonly ILocalizationRepository _localizationRepository;

        #region Constructor

        public ShiftTemplateService(IMapper mapper, 
            IUnitOfWork unitOfWork,
            IGeneralService generalService, 
            IGenericRepository<ShiftBreakTemplate> shiftBreakTemplateRepository, 
            IGenericRepository<ShiftTemplate> shiftTemplateRepository,
            IShiftTemplateBreaksRepository shiftTemplateBreaksRepository,
            IShiftBreakTypeLocalizationRepository shiftBreakTypeLocalizationRepository,
            ILocalizationRepository localizationRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _generalService = generalService;
            _shiftBreakTemplateRepository = shiftBreakTemplateRepository;
            _shiftTemplateRepository = shiftTemplateRepository;
            _shiftTemplateBreaksRepository = shiftTemplateBreaksRepository;
            _shiftBreakTypeLocalizationRepository = shiftBreakTypeLocalizationRepository;
            _localizationRepository = localizationRepository;
        }

        #endregion

        #region Add Shift Break Template

        public async Task<BaseResponse<int>> AddShiftBreakTemplate(ShiftBreakTemplateSubmissionModel submissionModel)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(submissionModel != null)
            {
                if (string.IsNullOrEmpty(submissionModel.ShiftBreakTemplateName))
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateEmptyName;
                    return response;
                }

                else if(submissionModel.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTypeIdentifierIsZero;
                    return response;
                }

                else if(submissionModel.ShiftBreakDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftDurationIsNotZero;
                    return response;
                }

                ShiftBreakTemplate shiftBreakTemplate = _mapper.Map<ShiftBreakTemplate>(submissionModel);
                shiftBreakTemplate = await _shiftBreakTemplateRepository.Add(shiftBreakTemplate);

                if (shiftBreakTemplate.ShiftBreakTemplateId != 0)
                {
                    response.Result = shiftBreakTemplate.ShiftBreakTemplateId;
                    response.Success = true;
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateAddSuccesful;
                }

            }
            return response;
        }

        #endregion

        #region Add Shift Template

        public async Task<BaseResponse<int>> AddShiftTemplate(ShiftTemplateSubmissionModel submissionModel)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(submissionModel != null)
            {
                if(string.IsNullOrEmpty(submissionModel.ShiftName))
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftTemplateEmptyName;
                    return response;
                }

                else if(submissionModel.ShiftDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftDurationIsNotZero;
                    return response;
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    ShiftTemplate shiftTemplate = _mapper.Map<ShiftTemplate>(submissionModel);
                    shiftTemplate = await _shiftTemplateRepository.Add(shiftTemplate);

                    // If there are shift break associations
                    if (submissionModel.ShiftBreakTemplateIdAssociations.Count() != 0 && shiftTemplate.ShiftTemplateId != 0)
                    {
                        IEnumerable<ShiftBreakTemplate> shiftBreakTemplates = await _shiftBreakTemplateRepository.GetAll();

                        // Check if association exists, and if so, add it.
                        foreach (int shiftBreakTemplateId in submissionModel.ShiftBreakTemplateIdAssociations)
                        {
                            if (shiftBreakTemplates.Any(x => x.ShiftBreakTemplateId == shiftBreakTemplateId))
                            {
                                ShiftTemplateBreaks shiftTemplateBreaks = new ShiftTemplateBreaks
                                {
                                    ShiftBreakTemplateId = shiftBreakTemplateId,
                                    ShiftTemplateId = shiftTemplate.ShiftTemplateId
                                };

                                await _shiftTemplateBreaksRepository.Add(shiftTemplateBreaks);
                            }
                        }
                    }

                    if (shiftTemplate.ShiftTemplateId != 0)
                    {
                        await _unitOfWork.CommitAsync();
                        response.Result = shiftTemplate.ShiftTemplateId;
                        response.Success = true;
                        response.Message = ShiftTemplateRelatedMessages.ShiftTemplateAddSuccesful;
                    }
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
                finally
                {
                    _unitOfWork.Dispose();
                }
            }
            return response;
        }

        #endregion

        #region Delete Shift Break Template

        public async Task<BaseResponse<bool>> DeleteShiftBreakTemplate(int shiftBreakTemplateId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if (shiftBreakTemplateId == 0)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateIdentifierIsZero;
                return response;
            }

            ShiftBreakTemplate shiftBreakTemplate = await _shiftBreakTemplateRepository.GetById(shiftBreakTemplateId);
            if(shiftBreakTemplate == null)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateNotFound;
                return response;
            }

            IEnumerable<ShiftTemplateBreaks> shiftTemplateBreaks = await _shiftTemplateBreaksRepository.GetShiftTemplateBreaksByBreakId(shiftBreakTemplateId);
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if(shiftTemplateBreaks.Count() != 0)
                {
                    await _shiftTemplateBreaksRepository.DeleteRange(shiftTemplateBreaks);
                }

                await _shiftBreakTemplateRepository.Delete(shiftBreakTemplateId);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateDeletedSuccessfuly;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Delete Shift Template

        public async Task<BaseResponse<bool>> DeleteShiftTemplate(int shiftTemplateId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(shiftTemplateId == 0)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftTemplateIdentifierIsZero;
                return response;
            }

            ShiftTemplate shiftTemplate = await _shiftTemplateRepository.GetById(shiftTemplateId);
            if(shiftTemplate == null)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftTemplateNotFound;
                return response;
            }

            IEnumerable<ShiftTemplateBreaks> shiftTemplateBreaks = await _shiftTemplateBreaksRepository.GetShiftTemplateBreaksByShiftId(shiftTemplateId);
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                if(shiftTemplateBreaks.Count() != 0)
                {
                    await _shiftTemplateBreaksRepository.DeleteRange(shiftTemplateBreaks);
                }

                await _shiftTemplateRepository.Delete(shiftTemplateId);
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Message = ShiftTemplateRelatedMessages.ShiftTemplateDeletedSuccessfuly;
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
            }
            finally 
            { 
                _unitOfWork.Dispose(); 
            }

            return response;
            
        }

        #endregion

        #region Get Shift Break Templates

        public async Task<List<ShiftBreakTemplateDTO>> GetShiftBreakTemplates(string lcode)
        {
            List<ShiftBreakTemplateDTO> shiftBreakTemplateDTOs = new List<ShiftBreakTemplateDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _shiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(lcode);
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // Get all shift break templates
            IEnumerable<ShiftBreakTemplate> shiftBreakTemplates = await _shiftBreakTemplateRepository.GetAll();

            // Get average clicks
            double averageClicks = 0;
            if(shiftBreakTemplates.Count() != 0)
                averageClicks = shiftBreakTemplates.Sum(i => i.TemplateClicks) / shiftBreakTemplates.Count();

            // For each record, map it, set break type display value and determine if its popular
            foreach(ShiftBreakTemplate shiftBreakTemplate in shiftBreakTemplates)
            {
                ShiftBreakTemplateDTO shiftBreakTemplateDTO = _mapper.Map<ShiftBreakTemplateDTO>(shiftBreakTemplate);
                shiftBreakTemplateDTO.ShiftBreakTypeDisplayValue = shiftBreakTypeLocalizations.Where(i => i.ShiftBreakTypeId.Equals(shiftBreakTemplateDTO.ShiftBreakTypeId)).FirstOrDefault().ShiftBreakTypeDisplayValue;
                shiftBreakTemplateDTO.IsPopular = shiftBreakTemplate.TemplateClicks > averageClicks ? true : false;

                shiftBreakTemplateDTOs.Add(shiftBreakTemplateDTO);
            }

            return shiftBreakTemplateDTOs;
        }

        #endregion

        #region Get Shift Template By Id

        public async Task<ShiftTemplate> GetShiftTemplateById(int id)
        {
            ShiftTemplate shiftTemplate = new ShiftTemplate();

            if(id != 0)
            {
                shiftTemplate = await _shiftTemplateRepository.GetById(id);
                IEnumerable<ShiftTemplateBreaks> shiftTemplateBreaks = await _shiftTemplateBreaksRepository.GetShiftTemplateBreaksByShiftId(id);
                
            }

            return shiftTemplate;
        }

        #endregion

        #region Get Shift Templates

        public async Task<List<ShiftTemplateDTO>> GetShiftTemplates(string lcode)
        {
            List<ShiftTemplateDTO> shiftTemplatesDTOs = new List<ShiftTemplateDTO>();

            if (!string.IsNullOrEmpty(lcode))
            {
                if (lcode.Contains("-"))
                    lcode = lcode.Split('-')[0];

                
                // Get all Shift Templates and Shift Break Templates
                IEnumerable<ShiftTemplate> shiftTemplates = await _shiftTemplateRepository.GetAll();

                double average = shiftTemplates.Sum(i=>i.TemplateClicks) / shiftTemplates.Count();

                List<ShiftBreakTemplateDTO> shiftBreakTemplateDTOs = await this.GetShiftBreakTemplates(lcode);

                // For each shift template
                foreach (ShiftTemplate shiftTemplate in shiftTemplates)
                {
                    ShiftTemplateDTO shiftTemplateDTO = _mapper.Map<ShiftTemplateDTO>(shiftTemplate);

                    // Get associated Shift Template Breaks
                    IEnumerable<ShiftTemplateBreaks> shiftTemplateBreaks = await _shiftTemplateBreaksRepository.GetShiftTemplateBreaksByShiftId(shiftTemplate.ShiftTemplateId);

                    // For each one found
                    foreach(ShiftTemplateBreaks templateBreaks in shiftTemplateBreaks)
                    {
                        // Get the matching break template and add it to the ShiftTemplateDTO list
                        ShiftBreakTemplateDTO shiftBreakTemplateDTO = shiftBreakTemplateDTOs.Where(i => i.ShiftBreakTemplateId.Equals(templateBreaks.ShiftBreakTemplateId)).FirstOrDefault();
                        if(shiftBreakTemplateDTO != null)
                            shiftTemplateDTO.ShiftBreakTemplates.Add(shiftBreakTemplateDTO);
                    }

                    shiftTemplateDTO.IsPopular = shiftTemplate.TemplateClicks > average ? true : false;

                    // Add Shift Template DTO to the returning list
                    shiftTemplatesDTOs.Add(shiftTemplateDTO);
                }
                
            }

            return shiftTemplatesDTOs;
        }

        #endregion

        #region Update Shift Break Template

        public async Task<BaseResponse<bool>> UpdateShiftBreakTemplate(ShiftBreakTemplateDTO shiftBreakTemplateDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(shiftBreakTemplateDTO != null)
            {
                if (string.IsNullOrEmpty(shiftBreakTemplateDTO.ShiftBreakTemplateName))
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateEmptyName;
                    return response;
                }

                else if (shiftBreakTemplateDTO.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTypeIdentifierIsZero;
                    return response;
                }

                else if (shiftBreakTemplateDTO.ShiftBreakDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftDurationIsNotZero;
                    return response;
                }

                else if(shiftBreakTemplateDTO.ShiftBreakTemplateId == 0)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateIdentifierIsZero;
                    return response;
                }

                ShiftBreakTemplate shiftBreakTemplate = await _shiftBreakTemplateRepository.GetById(shiftBreakTemplateDTO.ShiftBreakTemplateId);
                if(shiftBreakTemplate == null)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateNotFound;
                    return response;
                }

                shiftBreakTemplate = _mapper.Map<ShiftBreakTemplate>(shiftBreakTemplateDTO);
                await _shiftBreakTemplateRepository.Update(shiftBreakTemplate);

                response.Success = true;
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateUpdatedSuccessfuly;
            }


            return response;
        }

        #endregion

        #region Update Shift Break Template Pop Count

        public async Task<BaseResponse<bool>> UpdateShiftBreakTemplatePopCount(int shiftBreakTemplateId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(shiftBreakTemplateId == 0)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateIdentifierIsZero;
                return response;
            }

            ShiftBreakTemplate shiftBreakTemplate = await _shiftBreakTemplateRepository.GetById(shiftBreakTemplateId);
            if(shiftBreakTemplate == null)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplateNotFound;
                return response;
            }

            shiftBreakTemplate.TemplateClicks++;

            await _shiftBreakTemplateRepository.Update(shiftBreakTemplate);
            response.Success = true;
            response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplatePopUpdateSuccessfuly;

            return response;
        }

        #endregion

        #region Update Shift Template

        public async Task<BaseResponse<bool>> UpdateShiftTemplate(ShiftTemplateUpdateDTO shiftTemplateUpdateDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if(shiftTemplateUpdateDTO != null)
            {
                if(shiftTemplateUpdateDTO.ShiftTemplateId == 0)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftTemplateIdentifierIsZero;
                    return response;
                }

                else if (string.IsNullOrEmpty(shiftTemplateUpdateDTO.ShiftTemplateName))
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftTemplateEmptyName;
                    return response;
                }

                else if (shiftTemplateUpdateDTO.ShiftDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftDurationIsNotZero;
                    return response;
                }

                ShiftTemplate shiftTemplate = await _shiftTemplateRepository.GetById(shiftTemplateUpdateDTO.ShiftTemplateId);
                if(shiftTemplate == null)
                {
                    response.Message = ShiftTemplateRelatedMessages.ShiftTemplateNotFound;
                    return response;
                }

                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    if (shiftTemplateUpdateDTO.ShiftBreakTemplatesIdsIn.Count != 0 || shiftTemplateUpdateDTO.ShiftBreakTemplatesIdsOut.Count != 0)
                    {
                        // Get Current Shift Template Breaks
                        IEnumerable<ShiftTemplateBreaks> currentShiftTemplateBreaks = await _shiftTemplateBreaksRepository.GetShiftTemplateBreaksByShiftId(shiftTemplateUpdateDTO.ShiftTemplateId);
                        IEnumerable<ShiftBreakTemplate> shiftBreakTemplates = await _shiftBreakTemplateRepository.GetAll();

                        // For each break going in
                        foreach (int shiftTemplateBreakId in shiftTemplateUpdateDTO.ShiftBreakTemplatesIdsIn)
                        {
                            // Check if it exists in the current breaks
                            bool isInCurrent = currentShiftTemplateBreaks.Any(i => i.ShiftBreakTemplateId == shiftTemplateBreakId);

                            // If not, add it
                            if (!isInCurrent)
                            {
                                ShiftTemplateBreaks shiftTemplateBreak = new ShiftTemplateBreaks
                                {
                                    ShiftBreakTemplateId = shiftTemplateBreakId,
                                    ShiftTemplateId = shiftTemplateUpdateDTO.ShiftTemplateId
                                };

                                await _shiftTemplateBreaksRepository.Add(shiftTemplateBreak);
                            }
                        }

                        // For each record going out
                        foreach (int shiftTemplateBreakId in shiftTemplateUpdateDTO.ShiftBreakTemplatesIdsOut)
                        {
                            ShiftTemplateBreaks shiftTemplateBreaks = currentShiftTemplateBreaks.Where(i => i.ShiftBreakTemplateId.Equals(shiftTemplateBreakId)).FirstOrDefault();
                            if (shiftTemplateBreaks != null)
                            {
                                await _shiftTemplateBreaksRepository.DeleteShiftTemplateBreak(shiftTemplateBreaks);
                            }
                        }
                    }

                    shiftTemplate = _mapper.Map<ShiftTemplate>(shiftTemplateUpdateDTO);
                    await _shiftTemplateRepository.Update(shiftTemplate);

                    await _unitOfWork.CommitAsync();
                    response.Success = true;
                    response.Message = ShiftTemplateRelatedMessages.ShiftTemplateUpdatedSuccessfuly;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
                finally
                {
                    _unitOfWork.Dispose();
                }
            }

            return response;
        }

        #endregion

        #region Update Shift Template Pop Count

        public async Task<BaseResponse<bool>> UpdateShiftTemplatePopCount(int shiftTemplateId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftTemplateRelatedMessages.UnexpectedError;

            if (shiftTemplateId == 0)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftTemplateIdentifierIsZero;
                return response;
            }

            ShiftTemplate shiftTemplate = await _shiftTemplateRepository.GetById(shiftTemplateId);
            if (shiftTemplate == null)
            {
                response.Message = ShiftTemplateRelatedMessages.ShiftTemplateNotFound;
                return response;
            }

            shiftTemplate.TemplateClicks++;

            await _shiftTemplateRepository.Update(shiftTemplate);
            response.Success = true;
            response.Message = ShiftTemplateRelatedMessages.ShiftBreakTemplatePopUpdateSuccessfuly;

            return response;
        }

        #endregion
    }
}
