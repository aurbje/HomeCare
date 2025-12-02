import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';

import LoginPage from "./pages/Account/LoginPage";
import RegisterPage from "./pages/Account/RegisterPage";
import DashboardPage from "./pages/User/DashboardPage";
import HomePage from "./pages/Home/HomePage";
import AboutPage from "./pages/Home/AboutPage";
import ContactPage from "./pages/Home/ContactPage";
import BookingPage from "./pages/User/BookingPage";

export default function App() {
  return (
    <Router>
      <Routes>

        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="about" element={<AboutPage />} />
          <Route path="contact" element={<ContactPage />} />
          <Route path="booking" element={<BookingPage />} />

          <Route path="register" element={<RegisterPage />} />

          <Route path="dashboard" element={<DashboardPage />} />
        </Route>

      </Routes>
    </Router>
  );
}


