import api from "./api";

const API_URL = "https://localhost:7263/api";

// Users (non-admin, non-caregiver) — backend returns a plain array
export async function getUsers(search = "") {
  const qs = search && search.trim() ? `?q=${encodeURIComponent(search.trim())}` : "";
  const res = await fetch(`${API_URL}/admin/users${qs}`, {
    method: "GET",
    credentials: "include",
  });
  if (!res.ok) throw new Error(`Failed to fetch users: ${res.status}`);
  return res.json();
}

export async function deleteUser(id) {
  // --- FIX: Corrected a typo from API__URL to API_URL ---
  const res = await fetch(`${API_URL}/admin/users/${id}`, {
    method: "DELETE",
    credentials: "include",
  });
  if (!res.ok) throw new Error(`Failed to delete user ${id}: ${res.status}`);
  return res.json();
}

export const getUserById = async (id) => {
  const response = await api.get(`/admin/users/${id}`);
  return response.data;
};

export const updateUser = async (id, userData) => {
  const dto = {
    fullName: userData.fullName,
    email: userData.email,
    tlfNumber: userData.tlfNumber ?? null,
    address: userData.address ?? null,
  };
  const response = await api.put(`/admin/users/${id}`, dto);
  return response.data;
};

// Caregivers
export const getCaregivers = async (searchTerm = "") => {
  const params = searchTerm ? { q: searchTerm } : {};
  const response = await api.get("/admin/caregivers", { params });
  return response.data;
};

export const getCaregiverById = async (id) => {
  const response = await api.get(`/admin/caregivers/${id}`);
  return response.data;
};

export const updateCaregiver = async (id, userData) => {
  const dataWithId = { ...userData, id: parseInt(id) };
  const response = await api.put(`/admin/caregivers/${id}`, dataWithId);
  return response.data;
};

export const deleteCaregiver = async (id) => {
  const response = await api.delete(`/admin/caregivers/${id}`);
  return response.data;
};

// Bookings
export const getBookings = async (searchTerm = "") => {
  const params = searchTerm ? { q: searchTerm } : {};
  const response = await api.get("/admin/bookings", { params });
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
  const response = await api.get("/admin/booking-data");
  return response.data;
}