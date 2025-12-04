import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { registerUser } from "../../api/authApi";

// Registration page for creating a new user account
export default function RegisterPage() {
  // Form fields for user input
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    address: "",
    tlfNumber: "",
    password: "",
    confirmPassword: "",
  });

  // Feedback messages
  const [error, setError] = useState("");
  const [success, setSuccess] = useState("");

  const navigate = useNavigate();

  // Updates form state on user input
  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  // Handles registration request
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setSuccess("");

    // Simple password confirmation check
    if (form.password !== form.confirmPassword) {
      setError("Passordene matcher ikke");
      return;
    }

    try {
      await registerUser(form);

      // Display success message before redirect
      setSuccess("Konto opprettet! Du videresendes til innlogging...");

      // Redirects after a short delay
      setTimeout(() => {
        navigate("/login");
      }, 1500);

    } catch (err) {
      // Shows backend or fallback error
      setError(err.message || "Registrering feilet");
    }
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">

        {/* Back navigation */}
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Opprett konto</h2>

        {/* Error message */}
        {error && <div className="alert alert-danger">{error}</div>}

        {/* Success message */}
        {success && <div className="alert alert-success">{success}</div>}

        {/* Registration form */}
        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label>Fullt navn</label>
            <input
              name="fullName"
              className="form-control"
              value={form.fullName}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label>E-post</label>
            <input
              name="email"
              type="email"
              className="form-control"
              value={form.email}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label>Adresse</label>
            <input
              name="address"
              className="form-control"
              value={form.address}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label>Telefonnummer</label>
            <input
              name="tlfNumber"
              className="form-control"
              value={form.tlfNumber}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label>Passord</label>
            <input
              name="password"
              type="password"
              className="form-control"
              value={form.password}
              onChange={handleChange}
            />
          </div>

          <div className="mb-4">
            <label>Bekreft passord</label>
            <input
              name="confirmPassword"
              type="password"
              className="form-control"
              value={form.confirmPassword}
              onChange={handleChange}
            />
          </div>

          {/* Submit button */}
          <button className="btn btn-success w-100">Opprett konto</button>
        </form>
      </div>
    </div>
  );
}
