using System.Collections.Generic;
using System.Threading.Tasks;
using HomeCare.Models;

namespace HomeCare.Repositories.Interfaces
{
    public interface ICaregiverRepository
    {
        Task<Caregiver?> GetCaregiverByIdAsync(string id);

        // visits / appointment for the caregivers
        Task<IEnumerable<Visit>> GetTodayVisitsAsync(string caregiverId);
        Task<Visit?> GetVisitByIdAsync(int id);
        Task UpdateVisitStatusAsync(int visitId, bool isCompleted);

        // care Tasks
        Task<IEnumerable<CareTask>> GetCareTasksAsync(string caregiverId);
        Task<CareTask?> GetCareTaskByIdAsync(int id);
        Task UpdateCareTaskAsync(CareTask task);

        Task SaveChangesAsync();
    }
}
