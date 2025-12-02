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

// Personnel operations
export const getPersonnel = async (searchTerm = '') => {
  const response = await api.get('/api/admin/personnel', {
    params: { q: searchTerm }
  });
  return response.data;
};

export const deletePersonnel = async (id) => {
  const response = await api.delete(`/api/admin/personnel/${id}`);
  return response.data;
};

export const getPersonnelById = async (id) => {
  const response = await api.get(`/api/admin/personnel/${id}`);
  return response.data;
};

export const updatePersonnel = async (id, personnelData) => {
  const response = await api.put(`/api/admin/personnel/${id}`, personnelData);
  return response.data;
};

// Get clients and personnel for dropdowns
export const getClientsAndPersonnel = async () => {
  const response = await api.get('/api/admin/booking-data');
  return response.data;
};
