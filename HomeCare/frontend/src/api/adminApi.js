const API_URL = "https://localhost:7263/api";

// --- Helper for making authenticated fetch calls ---
const authenticatedFetch = async (url, options = {}) => {
  const defaultOptions = {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include', // This is the crucial part
  };

  const response = await fetch(url, { ...defaultOptions, ...options });

  if (!response.ok) {
    throw new Error(`API request failed: ${response.status}`);
  }
  return response.json();
};

// --- Users ---
export const getUsers = (search = "") => {
  const qs = search && search.trim() ? `?q=${encodeURIComponent(search.trim())}` : "";
  return authenticatedFetch(`${API_URL}/admin/users${qs}`);
};

export const deleteUser = (id) => {
  return authenticatedFetch(`${API_URL}/admin/users/${id}`, { method: 'DELETE' });
};

export const getUserById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/users/${id}`);
};

export const updateUser = (id, userData) => {
  const dto = {
    fullName: userData.fullName,
    email: userData.email,
    tlfNumber: userData.tlfNumber ?? null,
    address: userData.address ?? null,
  };
  return authenticatedFetch(`${API_URL}/admin/users/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  });
};

// --- Caregivers ---
export const getCaregivers = (searchTerm = "") => {
  const params = searchTerm ? `?q=${encodeURIComponent(searchTerm)}` : "";
  return authenticatedFetch(`${API_URL}/admin/caregivers${params}`);
};

export const getCaregiverById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/caregivers/${id}`);
};

export const updateCaregiver = (id, userData) => {
  // The backend DTO doesn't need the ID in the body, so we create a clean object.
  const dto = {
    fullName: userData.fullName,
    email: userData.email,
    tlfNumber: userData.tlfNumber ?? null,
    address: userData.address ?? null,
  };
  return authenticatedFetch(`${API_URL}/admin/caregivers/${id}`, {
    method: 'PUT',
    body: JSON.stringify(dto),
  });
};

export const deleteCaregiver = (id) => {
  return authenticatedFetch(`${API_URL}/admin/caregivers/${id}`, { method: 'DELETE' });
};

// --- Bookings ---
export const getBookings = (searchTerm = "") => {
  const params = searchTerm ? `?q=${encodeURIComponent(searchTerm)}` : "";
  return authenticatedFetch(`${API_URL}/admin/bookings${params}`);
};

export const getBookingById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`);
};

export const updateBooking = (id, bookingData) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`, {
    method: 'PUT',
    body: JSON.stringify(bookingData),
  });
};

export const deleteBooking = (id) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`, { method: 'DELETE' });
};

export const getBookingData = () => {
  return authenticatedFetch(`${API_URL}/admin/booking-data`);
};