// src/api/bookingApi.js
// Uses axios api instance with baseURL from .env (REACT_APP_API_URL)

import api from './api';

/* =============================================
   GET: Hent komplett bookingside-data
   GET /booking
============================================= */
export async function getBookingPage() {
  const response = await api.get('/booking');
  return response.data;
}

/* =============================================
   GET: Hent booking init data
   GET /booking/init
============================================= */
export async function getBookingInit() {
  const response = await api.get('/booking/init');
  return response.data;
}

/* =============================================
   GET: Hent booking for redigering
   GET /booking/{id}
============================================= */
export async function getBookingForEdit(id) {
  const response = await api.get(`/booking/${id}`);
  return response.data;
}

/* =============================================
   POST: Opprett eller oppdater booking
   POST /booking
============================================= */
export async function createOrUpdateBooking(model) {
  const response = await api.post('/booking', model);
  return response.data;
}

/* =============================================
   DELETE: Avbryt booking
   DELETE /booking/{id}
============================================= */
export async function cancelBooking(id) {
  const response = await api.delete(`/booking/${id}`);
  return response.data;
}

/* =============================================
   GET: Hent tilgjengelige caregivers for slot
   GET /booking/select-caregiver
============================================= */
export async function getAvailableCaregivers(selectedDate, timeSlotId, bookingId) {
  const params = new URLSearchParams({ selectedDate });
  if (timeSlotId) params.append('timeSlotId', timeSlotId);
  if (bookingId) params.append('bookingId', bookingId);
  
  const response = await api.get(`/booking/select-caregiver?${params}`);
  return response.data;
}
