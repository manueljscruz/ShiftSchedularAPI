using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.UnitOfWork;

namespace ShiftSchedularUnitTesting.AbsenceTypeController
{
    public class AbsenceTypeTestController
    {
        public IUnitOfWork _repository;
        public IMapper _mapper;
        public static DbContextOptions<DataContext> _dbContextOptions { get; }
        public static string connString = "Data Source=DESKTOP-T0HQHVC\\SQLEXPRESS;Database=ShiftSchedular; Integrated Security=True; Trusted_Connection=True; Trust Server Certificate=False; MultipleActiveResultSets=True; Encrypt=False";

        static AbsenceTypeTestController()
        {
            _dbContextOptions = new DbContextOptionsBuilder<DataContext>()
                .UseSqlServer(connString)
                .Options;
        }

        public AbsenceTypeTestController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                // cfg.AddProfile( using ShiftSchedularIL.Mappers);
            });

            _mapper = config.CreateMapper();
            var context = new DataContext(_dbContextOptions);
            _repository = new UnitOfWork(context);
        }
    }
}
