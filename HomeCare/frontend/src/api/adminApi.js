import api from './api';

// Users
export const getUsers = async (searchTerm = '') => {
  const params = searchTerm ? { q: searchTerm } : {};
  const response = await api.get('/admin/users', { params });
  return response.data;
};

export const getUserById = async (id) => {
  const response = await api.get(`/admin/users/${id}`);
  return response.data;
};

export const updateUser = async (id, userData) => {
  const response = await api.put(`/admin/users/${id}`, userData);
  return response.data;
};

export const deleteUser = async (id) => {
  const response = await api.delete(`/admin/users/${id}`);
  return response.data;
};

// Caregivers
export const getCaregivers = async (searchTerm = '') => {
  const params = searchTerm ? { q: searchTerm } : {};
  const response = await api.get('/admin/caregivers', { params });
  return response.data;
};

export const getCaregiverById = async (id) => {
  const response = await api.get(`/admin/caregivers/${id}`);
  return response.data;
};

export const updateCaregiver = async (id, userData) => {
  const response = await api.put(`/admin/caregivers/${id}`, userData);
  return response.data;
};

export const deleteCaregiver = async (id) => {
  const response = await api.delete(`/admin/caregivers/${id}`);
  return response.data;
};

// Bookings
export const getBookings = async (searchTerm = '') => {
  const params = searchTerm ? { q: searchTerm } : {};
  const response = await api.get('/admin/bookings', { params });
  return response.data;
};

export const getBookingById = async (id) => {
  const response = await api.get(`/admin/bookings/${id}`);
  return response.data;
};

export const updateBooking = async (id, bookingData) => {
  const response = await api.put(`/admin/bookings/${id}`, bookingData);
  return response.data;
};

export const deleteBooking = async (id) => {
  const response = await api.delete(`/admin/bookings/${id}`);
  return response.data;
};

export const getBookingData = async () => {
  const response = await api.get('/admin/booking-data');
  return response.data;
};
