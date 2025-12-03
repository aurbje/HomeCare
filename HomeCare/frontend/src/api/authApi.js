/**
 * authApi.js - Authentication API Functions
 * 
 * This file provides functions to call authentication endpoints.
 * Uses axios API instance with baseURL from .env (REACT_APP_API_URL)
 * 
 * Backend controller: backend/HomeCare.Api/Controllers/AccountController.cs
 * 
 * Used by:
 * - frontend/src/context/AuthContext.jsx (getCurrentUser, logoutUser)
 * - frontend/src/pages/Account/LoginPage.jsx (loginUser)
 * - frontend/src/pages/Account/RegisterPage.jsx (registerUser)
 */

import api from './api';

/**
 * Login user with email and password
 * Endpoint: POST /api/account/signin
 * Controller: AccountController.SignIn()
 * 
 * @param {Object} credentials - { email: string, password: string }
 * @returns {Promise<Object>} Response with user data
 */
export async function loginUser(credentials) {
    const response = await api.post('/account/signin', credentials);
    return response.data;
}

/**
 * Register a new user account
 * Endpoint: POST /api/account/signup
 * Controller: AccountController.SignUp()
 * 
 * @param {Object} model - { fullName, email, password, tlfNumber, address }
 * @returns {Promise<Object>} Response with created user data
 */
export async function registerUser(model) {
    const response = await api.post('/account/signup', model);
    return response.data;
}

/**
 * Logout current user
 * Endpoint: POST /api/account/logout
 * Controller: AccountController.Logout()
 * 
 * @returns {Promise<Object>} Response with logout confirmation
 */
export async function logoutUser() {
    const response = await api.post('/account/logout');
    return response.data;
}

/**
 * Get current authenticated user from session cookie
 * Endpoint: GET /api/account/me
 * Controller: AccountController.GetCurrentUser()
 * 
 * Used by AuthContext.jsx to check if user is logged in on app load
 * Returns null if not authenticated (401 response)
 * 
 * @returns {Promise<Object|null>} User data or null if not logged in
 */
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
