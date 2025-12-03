import axios from 'axios';

const api = axios.create({
  baseURL: 'https://localhost:7263/api', // Changed from http://localhost:5148
  withCredentials: true,
  headers: {
    'Content-Type': 'application/json',
  },
});

export default api;
