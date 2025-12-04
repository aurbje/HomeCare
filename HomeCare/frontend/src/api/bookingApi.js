import api from './api';

// Retrieves complete data for the booking overview page
export async function getBookingPage() {
  const response = await api.get('/booking');
  return response.data;
}

// Loads initial data required to start creating a new booking
export async function getBookingInit() {
  const response = await api.get('/booking/init');
  return response.data;
}

// Fetches booking details needed for editing an existing booking
export async function getBookingForEdit(id) {
  const response = await api.get(`/booking/${id}`);
  return response.data;
}

// Creates a new booking or updates an existing one
export async function createOrUpdateBooking(model) {
  const response = await api.post('/booking', model);
  return response.data;
}

// Cancels or deletes a booking by ID
export async function cancelBooking(id) {
  const response = await api.delete(`/booking/${id}`);
  return response.data;
}

// Retrieves available caregivers based on date, time slot, and optional booking ID
export async function getAvailableCaregivers(selectedDate, timeSlotId, bookingId) {
  const params = new URLSearchParams({ selectedDate });
  if (timeSlotId) params.append('timeSlotId', timeSlotId);
  if (bookingId) params.append('bookingId', bookingId);

  const response = await api.get(`/booking/select-caregiver?${params}`);
  return response.data;
}
