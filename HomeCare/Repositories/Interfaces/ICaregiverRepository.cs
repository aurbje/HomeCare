using System.Collections.Generic;
using System.Threading.Tasks;
using HomeCare.Models;

namespace HomeCare.Repositories.Interfaces
{
    public interface ICaregiverRepository
    {
        Task<Caregiver?> GetCaregiverByIdAsync(string id);

        // visits / appointment for the caregivers
        Task<IEnumerable<Visit>> GetTodayVisitsAsync(string caregiverId); // get todays visits
        Task<Visit?> GetVisitByIdAsync(int id);
        Task UpdateVisitStatusAsync(int visitId, bool isCompleted);

        // care Tasks
        Task<IEnumerable<CareTask>> GetCareTasksAsync(string caregiverId); // get care tasks for the caregiver
        Task<CareTask?> GetCareTaskByIdAsync(int id);
        Task UpdateCareTaskAsync(CareTask task);

        Task SaveChangesAsync(); // save to db
    }
}
