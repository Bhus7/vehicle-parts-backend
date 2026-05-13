using System.Collections.Generic;
using System.Threading.Tasks;
using vehicle_parts.Dto.Requests;
using vehicle_parts.Dto.Responses;

namespace vehicle_parts.Services
{
    public interface IStaffService
    {
        Task<ApiResponse<StaffResponse>> CreateStaffAsync(CreateStaffRequest request);
        Task<ApiResponse<IEnumerable<StaffResponse>>> GetAllStaffAsync();
        Task<ApiResponse<StaffResponse>> GetStaffByIdAsync(int id);
        Task<ApiResponse<StaffResponse>> UpdateStaffAsync(int id, UpdateStaffRequest request);
        Task<ApiResponse> ChangeStaffStatusAsync(int id, string status);
        Task<ApiResponse> ChangeStaffRoleAsync(int id, string roleName);
        Task<ApiResponse> DeleteStaffAsync(int id);
    }
}
