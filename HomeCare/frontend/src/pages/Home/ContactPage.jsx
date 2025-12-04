import React from "react";

export default function Contact() {
  return (
    <div className="container py-5">
      <div className="row align-items-center">

        {/* Left column – Contact information section */}
        <div className="col-lg-6 mb-4 mb-lg-0">
          <h1 className="fw-bold mb-4">Kontakt HomeCare</h1>

          {/* Intro text explaining purpose of the contact page */}
          <p className="lead">
            Vi er her for å hjelpe deg – enten du ønsker informasjon,
            trenger veiledning eller vil bestille hjelp i hjemmet.
          </p>

          {/* List of contact details */}
          <ul className="list-unstyled mt-4">
            <li className="mb-2">
              <i className="bi bi-telephone me-2"></i>
              <strong>Telefon:</strong>{" "}
              <a href="tel:22222222" className="text-dark">22 22 22 22</a>
            </li>

            <li className="mb-2">
              <i className="bi bi-envelope me-2"></i>
              <strong>E-post:</strong>{" "}
              <a href="mailto:kontakt@homecare.no" className="text-dark">kontakt@homecare.no</a>
            </li>

            <li className="mb-2">
              <i className="bi bi-geo-alt me-2"></i>
              <strong>Adresse:</strong>{" "}
              Storgata 5, 0155 Oslo
            </li>

            <li className="mb-2">
              <i className="bi bi-clock me-2"></i>
              <strong>Åpningstider:</strong>{" "}
              Man–fre 08:00–16:00
            </li>

            <li>
              <i className="bi bi-heart me-2"></i>
              <strong>Vakttelefon (akutt):</strong>{" "}
              22 22 22 99
            </li>
          </ul>

          {/* Contact form – Non-functional form for user messages */}
          <div className="mt-4">
            <h5 className="fw-semibold text-secondary">Send oss en melding</h5>

            <form>
              {/* Name input */}
              <div className="mb-3">
                <input type="text" className="form-control" placeholder="Navn" required />
              </div>

              {/* Email input */}
              <div className="mb-3">
                <input type="email" className="form-control" placeholder="E-post" required />
              </div>

              {/* Message input */}
              <div className="mb-3">
                <textarea className="form-control" rows="4" placeholder="Din melding..." required></textarea>
              </div>

              {/* Submit button */}
              <button type="submit" className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">
                Send oss melding
              </button>
            </form>
          </div>
        </div>

        {/* Right column – Contact page Image */}
        <div className="col-lg-6 text-center">
          <img
            src="/images/forside5.png"
            alt="HomeCare team"
            className="img-fluid rounded-4 shadow-sm"
            style={{ maxHeight: "420px", objectFit: "cover" }}
          />
        </div>

      </div>
    </div>
  );
}
