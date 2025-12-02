import React, { useState } from "react";
import { Link } from "react-router-dom";

export default function LoginPage() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Login:", { email, password });

    // TODO: koble til backend:
    // await accountApi.login({ email, password });
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Logg inn på HomeCare</h2>
        <p className="text-center text-muted mb-4">
          Velkommen tilbake! Logg inn for å få tilgang til dine tjenester og varsler.
        </p>

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
