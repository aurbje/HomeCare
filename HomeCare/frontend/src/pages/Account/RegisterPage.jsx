import React, { useState } from "react";
import { Link } from "react-router-dom";

export default function RegisterPage() {
  const [form, setForm] = useState({
    fullName: "",
    email: "",
    address: "",
    tlfNumber: "",
    password: "",
    confirmPassword: "",
  });

  const handleChange = (e) => {
    setForm({ ...form, [e.target.name]: e.target.value });
  };

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Registrering:", form);

    // TODO: koble mot backend:
    // await accountApi.register(form)
  };

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <Link to="/" className="back-btn">← Tilbake til forsiden</Link>

        <h2 className="text-center mb-4">Opprett HomeCare-konto</h2>
        <p className="text-center text-muted mb-4">
          Fyll inn informasjonen under for å komme i gang.
          <br />Vi hjelper deg med å holde hjemmet trygt og komfortabelt.
        </p>

        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-3">
            <label className="form-label">Fullt navn</label>
            <input
              name="fullName"
              className="form-control"
              placeholder="Skriv inn ditt fulle navn"
              value={form.fullName}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label className="form-label">E-postadresse</label>
            <input
              name="email"
              type="email"
              className="form-control"
              placeholder="F.eks. anna@epost.no"
              value={form.email}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label className="form-label">Adresse</label>
            <input
              name="address"
              className="form-control"
              placeholder="F.eks. Solsiden 12, Oslo"
              value={form.address}
              onChange={handleChange}
            />
          </div>

          <div className="mb-4">
            <label className="form-label">Telefonnummer</label>
            <input
              name="tlfNumber"
              className="form-control"
              placeholder="F.eks. 91234567"
              value={form.tlfNumber}
              onChange={handleChange}
            />
          </div>

          <div className="mb-3">
            <label className="form-label">Passord</label>
            <input
              name="password"
              type="password"
              className="form-control"
              placeholder="Velg et passord"
              value={form.password}
              onChange={handleChange}
            />
          </div>

          <div className="mb-4">
            <label className="form-label">Bekreft passord</label>
            <input
              name="confirmPassword"
              type="password"
              className="form-control"
              placeholder="Gjenta passordet"
              value={form.confirmPassword}
              onChange={handleChange}
            />
          </div>

          <button className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">
            Opprett konto
          </button>
        </form>
      </div>
    </div>
  );
}
