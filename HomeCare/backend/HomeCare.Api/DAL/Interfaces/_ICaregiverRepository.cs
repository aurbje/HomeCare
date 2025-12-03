/* ============================================================
 * ORIGINAL GROUP CODE - Commented out
 * Replaced with merged version in ICaregiverRepository.cs
 * This file kept for reference purposes
 * 
 * TYPE CONFLICT NOTE:
 * This file used `string caregiverId` parameters, but the unified
 * codebase now uses `int CaregiverId` consistently.
 * 
 * Files that had string CaregiverId (now commented out):
 * - _ICaregiverRepository.cs (this file): string caregiverId parameters
 * - _CaregiverRepository.cs: string caregiverId parameters  
 * - _AvailableDate.cs: string? CaregiverId property
 * - _Booking.cs: string CaregiverId property
 * 
 * Files using int CaregiverId (active codebase):
 * - ICaregiverRepository.cs: int CaregiverId parameters
 * - CaregiverRepository.cs: int CaregiverId parameters
 * - CaregiverAvailability.cs: int CaregiverId property
 * - Booking.cs: int? CaregiverId property
 * 
 * Decision: Use int type for CaregiverId as it maps directly to User.Id
 * which is the primary key of the User entity (int type).
 * ============================================================

using HomeCare.Api.Models;

namespace HomeCare.Api.DAL.Interfaces
{
    // Handles all data access for caregiver-related functionality
    public interface ICaregiverRepository
    {
        // Gets all clients assigned to a specific caregiver
        Task<IEnumerable<User>> GetClientsForCaregiverAsync(string caregiverId);

        // Gets all bookings scheduled for a specific caregiver
        Task<IEnumerable<Booking>> GetBookingsForCaregiverAsync(string caregiverId);
    }
}

*/