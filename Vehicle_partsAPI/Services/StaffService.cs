using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using vehicle_parts.Dto.Requests;
using vehicle_parts.Dto.Responses;
using vehicle_parts.Models;
using vehicle_parts.Repositories;

namespace vehicle_parts.Services
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _staffRepository;

        public StaffService(IStaffRepository staffRepository)
        {
            _staffRepository = staffRepository;
        }

        public async Task<ApiResponse<StaffResponse>> CreateStaffAsync(CreateStaffRequest request)
        {
            var existingUser = await _staffRepository.GetUserByEmailAsync(request.Email);
            if (existingUser != null)
            {
                return new ApiResponse<StaffResponse> { Success = false, Message = "Email already in use." };
            }

            var staffRole = await _staffRepository.GetRoleByNameAsync("STAFF");
            if (staffRole == null)
            {
                return new ApiResponse<StaffResponse> { Success = false, Message = "STAFF role not found in database." };
            }

            var newUser = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Address = request.Address,
                RoleID = staffRole.RoleID,
                CreatedDate = DateTime.UtcNow,
                Status = "Active"
            };

            await _staffRepository.CreateUserAsync(newUser);
            newUser.Role = staffRole;

            return new ApiResponse<StaffResponse>
            {
                Success = true,
                Message = "Staff created successfully",
                Data = MapToResponse(newUser)
            };
        }

        public async Task<ApiResponse<IEnumerable<StaffResponse>>> GetAllStaffAsync()
        {
            var staffRole = await _staffRepository.GetRoleByNameAsync("STAFF");
            if (staffRole == null)
            {
                return new ApiResponse<IEnumerable<StaffResponse>> { Success = false, Message = "STAFF role not found in database." };
            }

            var staff = await _staffRepository.GetAllStaffAsync(staffRole.RoleID);
            var response = staff.Select(MapToResponse);

            return new ApiResponse<IEnumerable<StaffResponse>>
            {
                Success = true,
                Message = "Staff retrieved successfully",
                Data = response
            };
        }

        public async Task<ApiResponse<StaffResponse>> GetStaffByIdAsync(int id)
        {
            var user = await _staffRepository.GetUserByIdAsync(id);
            if (user == null || user.Role?.RoleName != "STAFF")
            {
                return new ApiResponse<StaffResponse> { Success = false, Message = "Staff not found." };
            }

            return new ApiResponse<StaffResponse>
            {
                Success = true,
                Message = "Staff retrieved successfully",
                Data = MapToResponse(user)
            };
        }

        public async Task<ApiResponse<StaffResponse>> UpdateStaffAsync(int id, UpdateStaffRequest request)
        {
            var user = await _staffRepository.GetUserByIdAsync(id);
            if (user == null || user.Role?.RoleName != "STAFF")
            {
                return new ApiResponse<StaffResponse> { Success = false, Message = "Staff not found." };
            }

            if (!string.IsNullOrEmpty(request.FullName)) user.FullName = request.FullName;
            if (!string.IsNullOrEmpty(request.Phone)) user.Phone = request.Phone;
            if (!string.IsNullOrEmpty(request.Address)) user.Address = request.Address;

            await _staffRepository.UpdateUserAsync(user);

            return new ApiResponse<StaffResponse>
            {
                Success = true,
                Message = "Staff updated successfully",
                Data = MapToResponse(user)
            };
        }

        public async Task<ApiResponse> ChangeStaffStatusAsync(int id, string status)
        {
            var user = await _staffRepository.GetUserByIdAsync(id);
            if (user == null || user.Role?.RoleName != "STAFF")
            {
                return new ApiResponse { Success = false, Message = "Staff not found." };
            }

            user.Status = status;
            await _staffRepository.UpdateUserAsync(user);

            return new ApiResponse { Success = true, Message = "Staff status updated successfully" };
        }

        public async Task<ApiResponse> ChangeStaffRoleAsync(int id, string roleName)
        {
            var user = await _staffRepository.GetUserByIdAsync(id);
            if (user == null)
            {
                return new ApiResponse { Success = false, Message = "User not found." };
            }

            var newRole = await _staffRepository.GetRoleByNameAsync(roleName);
            if (newRole == null)
            {
                return new ApiResponse { Success = false, Message = "Role not found." };
            }

            user.RoleID = newRole.RoleID;
            await _staffRepository.UpdateUserAsync(user);

            return new ApiResponse { Success = true, Message = "User role updated successfully" };
        }

        public async Task<ApiResponse> DeleteStaffAsync(int id)
        {
            var user = await _staffRepository.GetUserByIdAsync(id);
            if (user == null || user.Role?.RoleName != "STAFF")
            {
                return new ApiResponse { Success = false, Message = "Staff not found." };
            }

            user.Status = "Deleted";
            await _staffRepository.UpdateUserAsync(user);

            return new ApiResponse { Success = true, Message = "Staff deleted (soft delete) successfully" };
        }

        private StaffResponse MapToResponse(User user)
        {
            return new StaffResponse
            {
                UserID = user.UserID,
                RoleID = user.RoleID,
                RoleName = user.Role?.RoleName,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Address = user.Address,
                CreatedDate = user.CreatedDate,
                Status = user.Status
            };
        }
    }
}
