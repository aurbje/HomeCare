import React from "react";
import { Routes, Route } from "react-router-dom";   // <-- viktig!
import Layout from "./components/Layout";

import LoginPage from "./pages/Account/LoginPage";
import RegisterPage from "./pages/Account/RegisterPage";
import DashboardPage from "./pages/User/DashboardPage";
import HomePage from "./pages/Home/HomePage";
import AboutPage from "./pages/Home/AboutPage";
import ContactPage from "./pages/Home/ContactPage";
import BookingPage from "./pages/User/BookingPage";
import AdminDashboardPage from "./pages/Admin/AdminDashboardPage";
import UsersPage from "./pages/Admin/UsersPage";
import Bookings from "./pages/Admin/Bookings";
import PersonnelPage from "./pages/Admin/PersonnelPage";
import EditUserPage from "./pages/Admin/EditUserPage";
import EditBookingPage from "./pages/Admin/EditBookingPage";
import EditPersonnelPage from "./pages/Admin/EditPersonnelPage";

export default function App() {
  return (
    <Routes>

      <Route path="/" element={<Layout />}>
        <Route index element={<HomePage />} />
        <Route path="login" element={<LoginPage />} />
        <Route path="about" element={<AboutPage />} />
        <Route path="contact" element={<ContactPage />} />
        <Route path="booking" element={<BookingPage />} />
        <Route path="register" element={<RegisterPage />} />

        <Route path="dashboard" element={<DashboardPage />} />
        <Route path="adminDashboard" element={<AdminDashboardPage />} />
        <Route path="admin/users" element={<UsersPage />} />
        <Route path="admin/bookings" element={<Bookings />} />
        <Route path="admin/personnel" element={<PersonnelPage />} />
        <Route path="admin/users/edit/" element={<EditUserPage />} />
        <Route path="admin/bookings/edit" element={<EditBookingPage />} />
        <Route path="admin/personnel/edit" element={<EditPersonnelPage />} />
      </Route>

    </Routes>
  );
}
