// src/api/authApi.js

const API_BASE = "https://localhost:7016/api/auth";

// Helper: same as bookingApi
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

    if (response.status === 204) return null;
    return response.json();
}

/* =============================================
   POST /api/auth/login
============================================= */
export async function loginUser(credentials) {
    const response = await fetch(`${API_BASE}/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",  // for cookies/session
        body: JSON.stringify(credentials),
    });

    return handleResponse(response);
}

/* =============================================
   POST /api/auth/register
============================================= */
export async function registerUser(model) {
    const response = await fetch(`${API_BASE}/register`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(model),
    });

    return handleResponse(response);
}

/* =============================================
   POST /api/auth/logout
============================================= */
export async function logoutUser() {
    const response = await fetch(`${API_BASE}/logout`, {
        method: "POST",
        credentials: "include",
    });

    return handleResponse(response);
}

/* =============================================
   GET /api/auth/me (optional)
============================================= */
export async function getCurrentUser() {
    const response = await fetch(`${API_BASE}/me`, {
        method: "GET",
        credentials: "include",
    });

    return handleResponse(response);
}
