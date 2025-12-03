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
import { loginUser as apiLogin } from "../../api/authApi";
// Use AuthContext to update global auth state after login
import { useAuth } from "../../context/AuthContext";

export default function LoginPage() {
  const navigate = useNavigate();
  // Get loginUser from AuthContext to update global state
  const { loginUser } = useAuth();

  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      // Call API to authenticate
      const result = await apiLogin({ email, password });
      console.log("Login success:", result);

      // Update AuthContext with user data
      // The response contains { message, user: { userId, fullName, email, role } }
      loginUser(result.user);

      // Navigate to appropriate dashboard based on role
      const role = result.user?.role?.toLowerCase();
      if (role === 'admin') {
        navigate("/admin");
      } else if (role === 'caregiver') {
        navigate("/caregiver/dashboard");
      } else {
        navigate("/dashboard"); // default user dashboard
      }
    } catch (err) {
      setError(err.message || "Innlogging feilet.");
    }
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Logg inn på HomeCare</h2>
        <p className="text-center text-muted mb-4">
          Velkommen tilbake! Logg inn for å få tilgang til dine tjenester og varsler.
        </p>

        {error && <div className="alert alert-danger">{error}</div>}

        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-3">
            <label className="form-label">E-postadresse</label>
            <input
              type="email"
              className="form-control"
              placeholder="F.eks. anna@epost.no"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
            />
          </div>

          <div className="mb-4">
            <label className="form-label">Passord</label>
            <input
              type="password"
              className="form-control"
              placeholder="Skriv inn passord"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">
            Logg inn
          </button>

          <div className="text-center mt-3">
            <p className="text-muted">
              Har du ikke konto?{" "}
              <Link to="/register" className="fw-bold text-decoration-none">
                Registrer deg her
              </Link>
            </p>
          </div>
        </form>
      </div>
    </div>
  );
}
