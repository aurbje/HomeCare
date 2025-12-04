// Login page component for authenticating users
// Handles login via backend API and updates global auth state.
 
import React, { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { login } from "../../api/authApi";
import { useAuth } from "../../context/AuthContext";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();
  const { loginUser } = useAuth(); // Stores authenticated user globally

  // Handles login submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");

    try {
      const result = await login({ email, password });

      // Stores authenticated user in context
      loginUser(result.user);

      // Redirects based on user role
      const role = result.user.role;
      if (role === "Admin") navigate("/admindashboard");
      else if (role === "Caregiver") navigate("/caregiver/dashboard");
      else navigate("/dashboard");

    } catch (err) {
      // Shows fallback error when authentication fails
      setError(err.message || "Feil ved innlogging");
    }
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">
        {/* Back navigation link */}
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Logg inn</h2>

        {/* Error alert */}
        {error && <div className="alert alert-danger">{error}</div>}

        {/* Login form */}
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

          {/* Submit button */}
          <button className="btn btn-success w-100">Logg inn</button>

          {/* Registration link */}
          <p className="text-center mt-3">
            Har du ikke konto?{" "}
            <Link to="/register" className="fw-bold">Registrer deg her</Link>
          </p>
        </form>
      </div>
    </div>
  );
}
