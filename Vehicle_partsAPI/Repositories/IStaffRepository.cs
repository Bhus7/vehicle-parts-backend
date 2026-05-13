using System.Collections.Generic;
using System.Threading.Tasks;
using vehicle_parts.Models;

namespace vehicle_parts.Repositories
{
    public interface IStaffRepository
    {
        Task<User> CreateUserAsync(User user);
        Task<IEnumerable<User>> GetAllStaffAsync(int staffRoleId);
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByEmailAsync(string email);
        Task UpdateUserAsync(User user);
        Task<Role> GetRoleByNameAsync(string roleName);
    }
}
