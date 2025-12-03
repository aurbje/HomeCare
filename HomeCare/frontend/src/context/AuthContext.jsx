/**
 * AuthContext.jsx - Global Authentication State Management
 *
 * Provides authentication state throughout the React application.
 * Checks current session on load via GET /api/account/me.
 */

import { createContext, useContext, useState, useEffect } from "react";
import { getCurrentUser, logout as apiLogout, logout } from "../api/authApi";
import { useNavigate } from "react-router-dom";

// Create the authentication context
const AuthContext = createContext();

/**
 * AuthProvider component - wraps the app to provide auth state
 */
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Load user from cookie when app starts
  useEffect(() => {
    async function loadUser() {
      try {
        const existing = await getCurrentUser();
        if (existing) setUser(existing);
      } catch (error) {
        // No active session is fine
        console.log("No active session found");
      } finally {
        setLoading(false);
      }
    }
    loadUser();
  }, []);

  // Set user data after successful login
  const loginUser = (userData) => {
    setUser(userData);
  };

  // Log out the current user
  const logoutUser = async () => {
    try {
      await apiLogout();
    } catch (error) {
      console.error("Logout API call failed:", error);
    }
    setUser(null);
    navigate("/"); // Redirect to homepage
  };

  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, loginUser, logout }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

/**
 * Custom hook to access authentication context
 */
export function useAuth() {
  return useContext(AuthContext);
}