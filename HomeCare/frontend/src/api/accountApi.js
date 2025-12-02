import api from './api';

export const forgotPassword = async (email) => {
  const response = await api.post('/api/account/forgot-password', { email });
  return response.data;
};

export const resetPassword = async (email, token, password, confirmPassword) => {
  const response = await api.post('/api/account/reset-password', {
    email,
    token,
    password,
    confirmPassword
  });
  return response.data;
};

export const login = async (email, password) => {
  const response = await api.post('/api/account/login', { email, password });
  return response.data;
};

export const register = async (userData) => {
  const response = await api.post('/api/account/register', userData);
  return response.data;
};

export const logout = async () => {
  const response = await api.post('/api/account/logout');
  return response.data;
};
