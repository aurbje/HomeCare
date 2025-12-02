// src/api/bookingApi.js

const API_BASE = "https://localhost:7016/api/booking"; 
// Endre til riktig port hvis din backend kjører på en annen adresse

// Helper: converts Fetch errors into readable exceptions
async function handleResponse(response) {
  if (!response.ok) {
    let error = "Ukjent feil";

    try {
      const data = await response.json();
      error = data.message || JSON.stringify(data);
    } catch {
      error = response.statusText;
    }

    throw new Error(error);
  }

  // Hvis det ikke finnes body
  if (response.status === 204) return null;

  return response.json();
}

/* =============================================
   GET: Hent komplett bookingside-data
   GET /api/booking
============================================= */
export async function getBookingPage() {
  const response = await fetch(API_BASE, {
    method: "GET",
    credentials: "include", // hvis cookies skal brukes
  });
  return handleResponse(response);
}

/* =============================================
   GET: Hent booking for redigering
   GET /api/booking/{id}
============================================= */
export async function getBookingForEdit(id) {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "GET",
    credentials: "include",
  });

  return handleResponse(response);
}

/* =============================================
   POST: Opprett eller oppdater booking
   POST /api/booking
============================================= */
export async function createOrUpdateBooking(model) {
  const response = await fetch(API_BASE, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    credentials: "include",
    body: JSON.stringify(model),
  });

  return handleResponse(response);
}

/* =============================================
   DELETE: Avbryt booking
   DELETE /api/booking/{id}
============================================= */
export async function cancelBooking(id) {
  const response = await fetch(`${API_BASE}/${id}`, {
    method: "DELETE",
    credentials: "include",
  });

  return handleResponse(response);
}
