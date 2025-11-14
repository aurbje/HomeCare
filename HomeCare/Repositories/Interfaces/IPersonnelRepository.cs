using HomeCare.ViewModels.Personnel;

public interface IPersonnelRepository
{
    Task<PersonnelDashboardViewModel> GetDashboardViewModelAsync(int personnelId);
    Task AddAvailabilityAsync(int personnelId, DateTime date);
    Task DeleteAvailabilityAsync(int personnelId, DateTime date);


}
