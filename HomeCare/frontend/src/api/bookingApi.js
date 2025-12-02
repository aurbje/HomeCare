const API_BASE = "https://localhost:7139/api/booking"; 
// → Bytt port hvis backend bruker en annen

// Helper for handling requests
async function request(url, options = {}) {
  const config = {
    headers: { "Content-Type": "application/json" },
    credentials: "include", // viktig for cookies/session
    ...options,
  };

  const res = await fetch(url, config);

  if (!res.ok) {
    const text = await res.text();
    throw new Error(`API Error ${res.status}: ${text}`);
  }

  // 204 No Content
  if (res.status === 204) return null;

  return res.json();
}

/* ---------------------------------------------------
   GET BOOKING PAGE (dates, categories, bookings)
------------------------------------------------------ */
export function getBookingPage() {
  return request(`${API_BASE}`); // GET /api/booking
}

/* ---------------------------------------------------
   GET BOOKING DETAILS FOR EDIT
------------------------------------------------------ */
export function getBookingForEdit(id) {
  return request(`${API_BASE}/${id}`); // GET /api/booking/{id}
}

/* ---------------------------------------------------
   CREATE or UPDATE BOOKING
   Backend determines action based on model.BookingId
------------------------------------------------------ */
export function createOrUpdateBooking(payload) {
  return request(`${API_BASE}`, {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

/* ---------------------------------------------------
   CANCEL BOOKING
------------------------------------------------------ */
export function cancelBooking(id) {
  return request(`${API_BASE}/${id}`, {
    method: "DELETE",
  });
}
