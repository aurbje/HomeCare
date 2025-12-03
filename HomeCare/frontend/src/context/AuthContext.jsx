/**
 * AuthContext.jsx - Global Authentication State Management
 * 
 * This context provides authentication state throughout the React application.
 * Based on the group's Final_Alexander branch implementation, adapted to use
 * axios API calls (from your implementation) instead of fetch.
 * 
 * How it works:
 * - On app load, checks if user is logged in via GET /api/account/me
 * - Provides user state, loginUser(), and logoutUser() to all components
 * - Wrapped around the app in index.js
 * 
 * Backend endpoints used:
 * - GET /api/account/me (AccountController.GetCurrentUser)
 * - POST /api/account/logout (AccountController.Logout)
 * 
 * Components that use this context:
 * - frontend/src/pages/User/BookingPage.jsx
 * - frontend/src/pages/User/DashboardPage.jsx
 * - frontend/src/pages/Account/LoginPage.jsx
 * - frontend/src/components/Navbar.jsx (for navbar auth state)
 */

import { createContext, useContext, useState, useEffect } from "react";
import { getCurrentUser, logoutUser as apiLogout } from "../api/authApi";
import { useNavigate } from "react-router-dom";

// Create the authentication context
const AuthContext = createContext();

/**
 * AuthProvider component - wraps the app to provide auth state
 * Place this in index.js around <App /> inside <BrowserRouter>
 * 
 * @param {Object} props - Component props
 * @param {React.ReactNode} props.children - Child components
 */
export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Load user from cookie when app starts
  // Calls GET /api/account/me to check if session cookie is valid
  useEffect(() => {
    async function loadUser() {
      try {
        const existing = await getCurrentUser();
        if (existing) setUser(existing);
      } catch (error) {
        // User is not logged in or session expired - this is normal
        console.log("No active session found");
      }
      setLoading(false);
    }
    loadUser();
  }, []);

  /**
   * Set user data after successful login
   * Called from LoginPage after POST /api/account/signin succeeds
   * 
   * @param {Object} userData - User data from login response
   */
  const loginUser = (userData) => {
    setUser(userData);
  };

  /**
   * Log out the current user
   * Calls POST /api/account/logout and clears local state
   * Redirects to homepage after logout
   */
  const logoutUser = async () => {
    try {
      await apiLogout();
    } catch (error) {
      console.error("Logout API call failed:", error);
    }
    setUser(null);
    navigate("/"); // Redirect to homepage
  };

  // Compute isAuthenticated from user state (convenience property)
  const isAuthenticated = !!user;

  return (
    <AuthContext.Provider value={{ user, isAuthenticated, loginUser, logoutUser }}>
      {/* Don't render children until initial auth check is complete */}
      {!loading && children}
    </AuthContext.Provider>
  );
}

/**
 * Custom hook to access authentication context
 * Use this in components to get user state and auth functions
 * 
 * Usage:
 *   const { user, isAuthenticated, loginUser, logoutUser } = useAuth();
 * 
 * @returns {Object} Auth context value
 */
export function useAuth() {
  return useContext(AuthContext);
}
