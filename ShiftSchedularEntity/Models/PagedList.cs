using Microsoft.EntityFrameworkCore;

namespace ShiftSchedularEntity.Models
{
    public class PagedList<T>
    {
        #region Properties

        public readonly IReadOnlyList<int> AvailablePageSizes = [10, 25, 50, 100];
        public List<T> Data { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
        public bool IsPreviousPageExists => CurrentPage > 1;
        public bool IsNextPageExists => CurrentPage < TotalPages;

        #endregion

        #region Constructor

        private PagedList(IEnumerable<T> currentPage, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            Data = new List<T>();
            Data.AddRange(currentPage);
        }

        #endregion

        #region Methods

        public static PagedList<T> CreateEmpty()
        {
            return new PagedList<T>(Enumerable.Empty<T>(), 0, 0, 0);
        }

        public static PagedList<T> Create(IQueryable<T> source, int count, int pageNumber, int pageSize)
        {
            var items = source.ToList();

            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        #endregion
    }
}
