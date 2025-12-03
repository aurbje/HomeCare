/* ============================================================
 * TEMPORARY IMPLEMENTATION - To be replaced by the responsible team member
 * This is a minimal implementation to allow testing of other components.
 * The final implementation should include proper state management,
 * error handling, and integration with the team's auth strategy.
 * ============================================================ */

import { useState, useEffect, useCallback } from 'react';
import api from '../api/api';

/**
 * TEMPORARY: Custom hook for authentication state management
 * Uses localStorage to persist user data and cookie-based auth for API calls
 * 
 * TODO (for final implementation):
 * - Consider using Context API for global state
 * - Add proper error handling and loading states
 * - Implement token refresh if needed
 * - Add session timeout handling
 */
export function useAuth() {
  const [user, setUser] = useState(() => {
    // TEMPORARY: Initialize from localStorage
    const stored = localStorage.getItem('user');
    return stored ? JSON.parse(stored) : null;
  });

  const [isAuthenticated, setIsAuthenticated] = useState(() => {
    return !!localStorage.getItem('user');
  });

  const [loading, setLoading] = useState(false);

  /**
   * TEMPORARY: Login function
   * Calls the backend API and stores user data in localStorage
   */
  const login = useCallback(async (email, password) => {
    setLoading(true);
    try {
      const response = await api.post('/account/signin', { email, password });
      const userData = response.data.user || response.data;

      // TEMPORARY: Store in localStorage for persistence
      localStorage.setItem('user', JSON.stringify(userData));
      setUser(userData);
      setIsAuthenticated(true);

      return userData;
    } finally {
      setLoading(false);
    }
  }, []);

  /**
   * TEMPORARY: Logout function
   * Clears local storage and calls backend logout
   */
  const logout = useCallback(async () => {
    try {
      await api.post('/account/logout');
    } catch (e) {
      console.error('Logout API call failed:', e);
    } finally {
      // TEMPORARY: Clear localStorage regardless of API result
      localStorage.removeItem('user');
      setUser(null);
      setIsAuthenticated(false);
    }
  }, []);

  /**
   * TEMPORARY: Check if user session is still valid
   * Called on mount to verify authentication status
   */
  const checkAuth = useCallback(async () => {
    try {
      const response = await api.get('/account/me');
      const userData = response.data;
      localStorage.setItem('user', JSON.stringify(userData));
      setUser(userData);
      setIsAuthenticated(true);
    } catch (e) {
      // Session expired or invalid
      localStorage.removeItem('user');
      setUser(null);
      setIsAuthenticated(false);
    }
  }, []);

  // TEMPORARY: Verify session on mount (optional, can be removed if not needed)
  useEffect(() => {
    if (isAuthenticated) {
      // Optionally verify session is still valid
      // checkAuth();
    }
  }, []);

  return {
    user,
    isAuthenticated,
    loading,
    login,
    logout,
    checkAuth,
  };
}

export default useAuth;
