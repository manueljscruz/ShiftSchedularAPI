namespace ShiftSchedularEntity.Models.DataTransferObjects.Admin
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalEntities { get; set; }
        public int ActiveMembers { get; set; }
        public List<RecentEntityDTO> RecentEntities { get; set; } = new();
        public List<RecentMemberDTO> RecentMembers { get; set; } = new();
    }

    public class RecentEntityDTO
    {
        public string EntityName { get; set; }
        public string EntityTypeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RecentMemberDTO
    {
        public string UserDisplayName { get; set; }
        public string EntityName { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}
