import React from "react";
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Layout from "./components/Layout";

import LoginPage from "./pages/Account/LoginPage";
import DashboardPage from "./pages/User/DashboardPage";
import HomePage from "./pages/Home/HomePage";

export default function App() {
  return (
    <Router>
      <Routes>

        <Route path="/" element={<Layout />}>
          <Route index element={<HomePage />} />
          <Route path="login" element={<LoginPage />} />
          <Route path="about" element={<h1>About Page</h1>} />
          <Route path="contact" element={<h1>Contact Page</h1>} />
          <Route path="booking" element={<h1>Booking Page</h1>} />

          <Route path="login" element={<LoginPage />} />
          <Route path="register" element={<h1>Register Page</h1>} />

          <Route path="dashboard" element={<DashboardPage />} />
        </Route>

      </Routes>
    </Router>
  );
}
