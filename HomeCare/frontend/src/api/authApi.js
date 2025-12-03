// src/api/authApi.js
// Uses axios api instance with baseURL from .env (REACT_APP_API_URL)

import api from './api';

/* =============================================
   POST /account/signin
============================================= */
export async function loginUser(credentials) {
    const response = await api.post('/account/signin', credentials);
    return response.data;
}

/* =============================================
   POST /account/signup
============================================= */
export async function registerUser(model) {
    const response = await api.post('/account/signup', model);
    return response.data;
}

/* =============================================
   POST /account/logout
============================================= */
export async function logoutUser() {
    const response = await api.post('/account/logout');
    return response.data;
}

/* =============================================
   GET /account/me
============================================= */
export async function getCurrentUser() {
    const response = await api.get('/account/me');
    return response.data;
}
