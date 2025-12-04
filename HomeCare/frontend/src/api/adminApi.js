const API_URL = "https://localhost:7263/api";

// Wrapper for authenticated API requests using cookies
const authenticatedFetch = async (url, options = {}) => {
  const defaultOptions = {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
    credentials: 'include', // Ensures cookies are sent with requests
  };

  const response = await fetch(url, { ...defaultOptions, ...options });

  // Handles all non-OK responses uniformly
  if (!response.ok) {
    const errorText = await response.text();
    console.error(`API Error: ${response.status} - ${errorText}`);
    throw new Error(`API request failed: ${response.status}`);
  }

  return response.json(); // Returns parsed JSON response
};

// Users 

// Fetches all users with optional search filtering
export const getUsers = (search = "") => {
  const qs = search && search.trim() ? `?q=${encodeURIComponent(search.trim())}` : "";
  return authenticatedFetch(`${API_URL}/admin/users${qs}`);
};

// Retrieves a single user by ID
export const getUserById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/users/${id}`);
};

// Updates user information
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

// Removes a user from the system
export const deleteUser = (id) => {
  return authenticatedFetch(`${API_URL}/admin/users/${id}`, { method: 'DELETE' });
};

// Caregivers 

// Fetches caregiver list with optional filtering
export const getCaregivers = (searchTerm = "") => {
  const params = searchTerm ? `?q=${encodeURIComponent(searchTerm)}` : "";
  return authenticatedFetch(`${API_URL}/admin/caregivers${params}`);
};

// Retrieves detailed caregiver data
export const getCaregiverById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/caregivers/${id}`);
};

// Updates caregiver profile information
export const updateCaregiver = (id, userData) => {
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

// Deletes a caregiver by ID
export const deleteCaregiver = (id) => {
  return authenticatedFetch(`${API_URL}/admin/caregivers/${id}`, { method: 'DELETE' });
};

// Bookings 

// Retrieves all bookings with optional search filters
export const getBookings = (searchTerm = "") => {
  const params = searchTerm ? `?q=${encodeURIComponent(searchTerm)}` : "";
  return authenticatedFetch(`${API_URL}/admin/bookings${params}`);
};

// Fetches a specific booking by ID
export const getBookingById = (id) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`);
};

// Updates booking details
export const updateBooking = (id, bookingData) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`, {
    method: 'PUT',
    body: JSON.stringify(bookingData),
  });
};

// Deletes a booking entry
export const deleteBooking = (id) => {
  return authenticatedFetch(`${API_URL}/admin/bookings/${id}`, { method: 'DELETE' });
};

// Loads booking-related auxiliary data
export const getBookingData = () => {
  return authenticatedFetch(`${API_URL}/admin/booking-data`);
};
