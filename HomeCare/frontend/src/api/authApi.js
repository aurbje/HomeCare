// Base URL for all authentication-related API requests
const API_URL = "https://localhost:7263/api";

// Sends login credentials and initializes an authenticated session
export async function login(credentials) {
  const res = await fetch(`${API_URL}/account/signin`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include", // Ensures session cookie is stored
    body: JSON.stringify(credentials),
  });

  if (!res.ok) throw new Error("Login failed");
  return res.json(); // Returns user data on success
}

// Registers a new user account
export async function registerUser(data) {
  const res = await fetch(`${API_URL}/account/signup`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include", // Allows backend to set cookies if needed
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error("Registration failed");
  return res.json();
}

// Logs the user out by clearing the authentication cookie
export async function logout() {
  await fetch(`${API_URL}/account/logout`, {
    method: "POST",
    credentials: "include",
  });
}

// Retrieves information about the currently authenticated user
export async function getCurrentUser() {
  const res = await fetch(`${API_URL}/account/me`, {
    method: "GET",
    credentials: "include", // Sends cookie to check login status
  });

  if (res.status === 401) return null; // User not authenticated
  if (!res.ok) throw new Error("Failed to fetch current user");

  return res.json(); // Returns user profile data
}
