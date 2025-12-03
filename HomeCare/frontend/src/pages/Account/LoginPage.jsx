/**
 * LoginPage.jsx - User Login Page
 *
 * Auth: Uses context/AuthContext.jsx (group's pattern)
 * Backend endpoint: POST /api/account/signin (AccountController.SignIn)
 *
 * After successful login:
 * 1. Calls authApi.loginUser() to authenticate
 * 2. Calls AuthContext.loginUser() to update global state
 * 3. Navigates to dashboard
 */

import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { login } from "../../api/authApi";
import { useAuth } from "../../context/AuthContext";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();
  const { loginUser } = useAuth(); // ⬅️ legger bruker i global state

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const result = await login({ email, password });

      // save user in React context
      loginUser(result.user);

      const role = result.user.role;

      // redirect based on role
      if (role === "Admin") navigate("/admindashboard");
      else if (role === "Caregiver") navigate("/caregiver");
      else navigate("/dashboard");
      
    } catch (err) {
      setError(err.message || "Feil ved innlogging");
    }
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Logg inn</h2>

        {error && <div className="alert alert-danger">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="mb-3">
            <label>E-post</label>
            <input
              type="email"
              className="form-control"
              placeholder="anna@epost.no"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div className="mb-4">
            <label>Passord</label>
            <input
              type="password"
              className="form-control"
              placeholder="Skriv inn passord"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button className="btn btn-success w-100">Logg inn</button>

          <p className="text-center mt-3">
            Har du ikke konto?{" "}
            <Link to="/register" className="fw-bold">Registrer deg her</Link>
          </p>
        </form>
      </div>
    </div>
  );
}
