import api from './api';

// Booking operations
export const getBookings = async (searchTerm = '') => {
  const response = await api.get('/api/admin/bookings', {
    params: { q: searchTerm }
  });
  return response.data;
};

export const deleteBooking = async (id) => {
  const response = await api.delete(`/api/admin/bookings/${id}`);
  return response.data;
};

export const getBookingById = async (id) => {
  const response = await api.get(`/api/admin/bookings/${id}`);
  return response.data;
};

export const updateBooking = async (id, bookingData) => {
  const response = await api.put(`/api/admin/bookings/${id}`, bookingData);
  return response.data;
};

// User operations
export const getUsers = async (searchTerm = '') => {
  const response = await api.get('/api/admin/users', {
    params: { q: searchTerm }
  });
  return response.data;
};

export const deleteUser = async (id) => {
  const response = await api.delete(`/api/admin/users/${id}`);
  return response.data;
};

export const getUserById = async (id) => {
  const response = await api.get(`/api/admin/users/${id}`);
  return response.data;
};

export const updateUser = async (id, userData) => {
  const response = await api.put(`/api/admin/users/${id}`, userData);
  return response.data;
};

// Caregiver operations
export const getCaregivers = async (searchTerm = '') => {
  const response = await api.get('/api/admin/caregivers', {
    params: { q: searchTerm }
  });
  return response.data;
};

export const deleteCaregiver = async (id) => {
  const response = await api.delete(`/api/admin/caregivers/${id}`);
  return response.data;
};

export const getCaregiverById = async (id) => {
  const response = await api.get(`/api/admin/caregivers/${id}`);
  return response.data;
};

export const updateCaregiver = async (id, caregiverData) => {
  const response = await api.put(`/api/admin/caregivers/${id}`, caregiverData);
  return response.data;
};

// Get clients and personnel for dropdowns
export const getClientsAndPersonnel = async () => {
  const response = await api.get('/api/admin/booking-data');
  return response.data;
};
