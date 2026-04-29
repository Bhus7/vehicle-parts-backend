using vehicle_parts.Dto.Response;
using vehicle_parts.Repositories;

namespace vehicle_parts.Services
{
    public interface IStaffService
    {
        Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query);
        Task<IEnumerable<CustomerReportResponse>> GetRegularCustomersAsync();
        Task<IEnumerable<CustomerReportResponse>> GetHighSpendersAsync();
        Task<IEnumerable<CustomerReportResponse>> GetPendingCreditsAsync();
    }

    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;

        public StaffService(IStaffRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<CustomerSearchResponse>> SearchCustomersAsync(string query)
        {
            return await _repository.SearchCustomersAsync(query);
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetRegularCustomersAsync()
        {
            return await _repository.GetRegularCustomersAsync();
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetHighSpendersAsync()
        {
            return await _repository.GetHighSpendersAsync();
        }

        public async Task<IEnumerable<CustomerReportResponse>> GetPendingCreditsAsync()
        {
            return await _repository.GetPendingCreditsAsync();
        }
    }
}
