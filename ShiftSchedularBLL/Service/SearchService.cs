using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularRL.Resources.Search;
using ShiftSchedularRL.Resources.Shared;

namespace ShiftSchedularBLL.Service
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        // Constants for result types - makes code more maintainable
        private const string ResultTypeWorker = "Worker";
        private const string ResultTypeEntity = "Entity";

        public SearchService(IUnitOfWork unitOfWork, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        #region Search

        public async Task<BaseResponse<PagedList<SearchResultDTO>>> Search(SearchRequestDTO searchRequest)
        {
            BaseResponse<PagedList<SearchResultDTO>> response = new BaseResponse<PagedList<SearchResultDTO>>();
            response.Message = SharedMessages.UnexpectedError;

            // Step 1: Validate the request
            if (string.IsNullOrWhiteSpace(searchRequest.Query))
            {
                response.Message = SearchRelatedMessages.SearchQueryRequired;
                return response;
            }

            // Normalize the query (trim whitespace, lowercase for case-insensitive search)
            string normalizedQuery = searchRequest.Query.Trim().ToLower();

            // Step 2: Determine which types to search
            // If ResultType is null or empty, search both. Otherwise, search only the specified type.
            bool searchWorkers = string.IsNullOrEmpty(searchRequest.ResultType) ||
                                 searchRequest.ResultType.Equals(ResultTypeWorker, StringComparison.OrdinalIgnoreCase);
            bool searchEntities = string.IsNullOrEmpty(searchRequest.ResultType) ||
                                  searchRequest.ResultType.Equals(ResultTypeEntity, StringComparison.OrdinalIgnoreCase);

            // Step 3: Collect results from both sources
            List<SearchResultDTO> allResults = new List<SearchResultDTO>();

            // Search Workers (ApplicationUsers)
            if (searchWorkers)
            {
                var workerResults = await SearchWorkers(normalizedQuery);
                allResults.AddRange(workerResults);
            }

            // Search Entities
            if (searchEntities)
            {
                var entityResults = await SearchEntities(normalizedQuery, searchRequest.LanguageCode);
                allResults.AddRange(entityResults);
            }

            // Step 4: Sort the combined results (by Title alphabetically)
            allResults = allResults.OrderBy(r => r.Title).ToList();

            // Step 5: Calculate pagination
            int totalCount = allResults.Count;
            int pageSize = searchRequest.ItemsPerPage > 0 ? searchRequest.ItemsPerPage : 10;
            int currentPage = searchRequest.NextPage > 0 ? searchRequest.NextPage : 1;

            // Step 6: Apply Skip/Take for the current page
            var pagedResults = allResults
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Step 7: Create the PagedList and return
            response.Result = PagedList<SearchResultDTO>.Create(
                pagedResults.AsQueryable(),
                totalCount,
                currentPage,
                pageSize
            );
            response.Message = string.Empty;
            response.Success = true;

            return response;
        }

        #endregion

        #region Search Workers

        /// <summary>
        /// Searches ApplicationUsers by their DisplayName
        /// </summary>
        private async Task<List<SearchResultDTO>> SearchWorkers(string normalizedQuery)
        {
            // Query users whose DisplayName contains the search query
            // Using UserManager.Users gives us an IQueryable we can filter
            var matchingWorkers = _userManager.Users
                .Where(u => u.DisplayName.ToLower().Contains(normalizedQuery))
                .Select(u => new SearchResultDTO
                {
                    Identifier = u.Id,
                    ResultType = ResultTypeWorker,
                    Title = u.DisplayName,
                    TypeDisplay = ResultTypeWorker // You could localize this later
                })
                .ToList();

            return await Task.FromResult(matchingWorkers);
        }

        #endregion

        #region Search Entities

        /// <summary>
        /// Searches Entities by their EntityName, includes localized type display
        /// </summary>
        private async Task<List<SearchResultDTO>> SearchEntities(string normalizedQuery, string languageCode)
        {
            // Get localization for the TypeDisplay
            Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(languageCode);

            // Query entities whose EntityName contains the search query
            var matchingEntities = await _unitOfWork.EntityRepository.SearchEntitiesByName(normalizedQuery, localization?.LocalizationId ?? 1);

            // Map to SearchResultDTO
            var results = matchingEntities.Select(e => new SearchResultDTO
            {
                Identifier = e.EntityId.ToString(),
                ResultType = ResultTypeEntity,
                Title = e.EntityName,
                // Get the localized entity type name, fallback to EntityType value if no localization
                TypeDisplay = e.EntityType?.EntityTypeLocalizations?.FirstOrDefault()?.EntityTypeDisplayValue
                              ?? e.EntityType?.EntityTypeValue
                              ?? ResultTypeEntity
            }).ToList();

            return results;
        }

        #endregion

        #region Get Public Profile Worker

        public async Task<BaseResponse<WorkerPublicProfileDTO>> GetPublicProfileWorker(string workerId, string languageCode)
        {
            BaseResponse<WorkerPublicProfileDTO> response = new BaseResponse<WorkerPublicProfileDTO>();
            response.Message = SharedMessages.UnexpectedError;

            ApplicationUser user = await _userManager.FindByIdAsync(workerId);

            if(user == null)
            {
                response.Message = SearchRelatedMessages.UserProfileNotFound;
                return response;
            }

            GenderLocalization genderLocalized = await _unitOfWork.GenderLocalizationRepository.GetById(user.GenderId);

            response.Result = new WorkerPublicProfileDTO
            {
                WorkerId = user.Id,
                DisplayName = user.DisplayName,
                GenderLocalized = genderLocalized.GenderDisplayValue
            };
            response.Message = string.Empty;
            response.Success = true;

            return response;
        }

        #endregion

        #region Get Public Entity Profile

        public async Task<BaseResponse<EntityPublicProfileDTO>> GetPublicEntityProfile(BaseViewModelRequest request)
        {
            BaseResponse<EntityPublicProfileDTO> response = new BaseResponse<EntityPublicProfileDTO>();
            response.Message = SharedMessages.UnexpectedError;

            // Get Entity
            Entity entity = await _unitOfWork.EntityRepository.GetEntityById(request.EntityId, request.LanguageCode);

            // Not found
            if(entity == null)
            {
                response.Message = SearchRelatedMessages.EntityProfileNotFound;
                return response;
            }

            response.Result = _mapper.Map<Entity, EntityPublicProfileDTO>(entity);
            response.Message = string.Empty;
            response.Success = true;

            return response;
        }

        #endregion
    }
}
