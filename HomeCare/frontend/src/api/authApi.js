// frontend/src/api/authApi.js

const API_URL = "https://localhost:7263/api";

export async function login(credentials) {
  const res = await fetch(`${API_URL}/account/signin`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(credentials),
  });

  if (!res.ok) throw new Error("Login failed");
  return res.json();
}

export async function registerUser(data) {
  const res = await fetch(`${API_URL}/account/signup`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    credentials: "include",
    body: JSON.stringify(data),
  });

  if (!res.ok) throw new Error("Registration failed");
  return res.json();
}

export async function logout() {
  await fetch(`${API_URL}/account/logout`, {
    method: "POST",
    credentials: "include",
  });
}

export async function getCurrentUser() {
    try {
        const response = await api.get('/account/me');
        return response.data;
    } catch (error) {
        // 401 Unauthorized means user is not logged in - this is expected
        if (error.response?.status === 401) {
            return null;
        }
        throw error;
    }
}
