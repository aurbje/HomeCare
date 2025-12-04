/**
 * App.jsx
 *
 * Root routing configuration for the HomeCare platform.
 * Uses React Router v6 with nested routing via <Layout />.
 *
 * Route groups:
 * - Public pages (Home, About, Contact, Login, Register)
 * - User pages (Dashboard, Booking)
 * - Admin pages (Dashboard, Users, Bookings, Caregivers)
 * - Caregiver pages (Dashboard)
 *
 * All pages inherit shared layout, header, and footer from Layout.jsx.
 */

import React from "react";
import { Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";

// Account / Auth pages
import LoginPage from "./pages/Account/LoginPage";
import RegisterPage from "./pages/Account/RegisterPage";

// Client/User pages
import DashboardPage from "./pages/User/DashboardPage";
import BookingPage from "./pages/User/BookingPage";

// Public pages
import HomePage from "./pages/Home/HomePage";
import AboutPage from "./pages/Home/AboutPage";
import ContactPage from "./pages/Home/ContactPage";

// Admin pages
import AdminDashboardPage from "./pages/Admin/AdminDashboardPage";
import UsersPage from "./pages/Admin/UsersPage";
import Bookings from "./pages/Admin/Bookings";
import CaregiverPage from "./pages/Admin/CaregiverPage";
import EditUserPage from "./pages/Admin/EditUserPage";
import EditBookingPage from "./pages/Admin/EditBookingPage";
import EditCaregiverPage from "./pages/Admin/EditCaregiverPage";

// Caregiver pages
import CaregiverDashboardPage from "./pages/Caregiver/DashboardPage";

export default function App() {
  return (
    <Routes>
      {/* Main layout wrapper (header, footer, etc.) */}
      <Route path="/" element={<Layout />}>

        {/* Public Routes */}
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="register" element={<RegisterPage />} />
        <Route path="about" element={<AboutPage />} />
        <Route path="contact" element={<ContactPage />} />

        {/* User / Client Routes */}
        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="booking" element={<BookingPage />} />

        {/* Admin Routes */}
        <Route path="adminDashboard" element={<AdminDashboardPage />} />
        <Route path="admin/users" element={<UsersPage />} />
        <Route path="admin/bookings" element={<Bookings />} />
        <Route path="admin/caregivers" element={<CaregiverPage />} />
        <Route path="admin/users/edit/:id" element={<EditUserPage />} />
        <Route path="admin/bookings/edit/:id" element={<EditBookingPage />} />
        <Route path="admin/caregivers/edit/:id" element={<EditCaregiverPage />} />

        {/* Caregiver Routes */}
        <Route path="caregiver/dashboard" element={<CaregiverDashboardPage />} />
      </Route>
    </Routes>
  );
}
