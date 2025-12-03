/**
 * index.js - Application Entry Point
 *
 * Updated to match group's Final_Alexander branch structure:
 * - BrowserRouter is now here (was in App.jsx)
 * - AuthProvider wraps the app for global auth state
 *
 * Structure: BrowserRouter > AuthProvider > App
 * This allows AuthContext to use useNavigate() for redirects
 */

import React from 'react';
import ReactDOM from 'react-dom/client';
import 'bootstrap/dist/css/bootstrap.min.css';
import './styles/global.css';
import App from './App';

// AuthContext provides global authentication state
// Based on group's Final_Alexander branch
import { AuthProvider } from './context/AuthContext';
import { BrowserRouter } from 'react-router-dom';

const root = ReactDOM.createRoot(document.getElementById('root'));
root.render(
  <BrowserRouter>
    <AuthProvider>
      <App />
    </AuthProvider>
  </BrowserRouter>
);
