/**
 * App.jsx - Main Application Routes
 *
 * Updated to match group's Final_Alexander branch:
 * - Router (BrowserRouter) moved to index.js
 * - Added AdminDashboardPage route from group's code
 *
 * Note: Uses <Routes> directly since BrowserRouter is in index.js
 */

import React from 'react';
import { Routes, Route } from 'react-router-dom'; // Router is now in index.js
import Layout from './components/Layout';

import LoginPage from "./pages/Account/LoginPage";
import RegisterPage from "./pages/Account/RegisterPage";
import DashboardPage from "./pages/User/DashboardPage";
import HomePage from "./pages/Home/HomePage";
import AboutPage from "./pages/Home/AboutPage";
import ContactPage from "./pages/Home/ContactPage";
import BookingPage from "./pages/User/BookingPage";
// Added from group's Final_Alexander branch
import AdminDashboardPage from "./pages/Admin/AdminDashboardPage";
// Caregiver dashboard
import CaregiverDashboardPage from "./pages/Caregiver/DashboardPage";

export default function App() {
  return (
    // Note: BrowserRouter wrapper removed - now in index.js with AuthProvider
    <Routes>
      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="about" element={<AboutPage />} />
        <Route path="contact" element={<ContactPage />} />
        <Route path="booking" element={<BookingPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="dashboard" element={<DashboardPage />} />
        {/* Caregiver dashboard */}
        <Route path="caregiver/dashboard" element={<CaregiverDashboardPage />} />
        {/* Added from group's Final_Alexander branch */}
        <Route path="admin" element={<AdminDashboardPage />} />
      </Route>
    </Routes>
  );
}


