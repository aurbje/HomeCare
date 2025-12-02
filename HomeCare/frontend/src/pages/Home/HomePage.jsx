import React from "react";

export default function HomePage() {
  return (
    <>
      {/* HERO SECTION */}
      <section className="hero-homecare d-flex align-items-center justify-content-center text-center position-relative">
        <div className="hero-overlay"></div>

        <div className="hc-hero-panel container position-relative">
          <h1 className="hc-hero-title animate-fade">Omsorg der du er</h1>

          <p className="hc-hero-subtitle animate-fade-delay">
            Trygghet, nærhet og støtte – akkurat når du trenger det.
          </p>

          <a href="/about" className="btn btn-main hc-hero-cta shadow-lg animate-fade">
            Les mer om oss
          </a>
        </div>
      </section>

      {/* CONTACT CARDS */}
      <section className="contact-cards-section py-5">
        <div className="container">
          <h2 className="text-center fw-bold mb-4">Finn ditt lokale HomeCare-team</h2>
          <p className="text-center text-muted mb-5">
            Finn avdelingen som dekker ditt område. Vi er et dedikerte team, med lokal kjennskap i alle Oslos bydeler.
          </p>

          <div className="row g-4">

            {/* --- Repeat block 1 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Hjemmesykepleie Vest</h5>
                <p className="mb-3 text-muted">Ullern, Vestre Aker og Nordre Aker</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:vest@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Vakttlf. alle dager 07.00–22.30</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4747918109" className="hc-contact-phone">+47 47 91 81 09</a>
                </div>
              </div>
            </div>

            {/* --- Block 2 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Hjemmesykepleie Sentrum</h5>
                <p className="mb-3 text-muted">Gamle Oslo, Grünerløkka og Sagene</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:sentrum@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Vakttlf. alle dager 07.00–22.30</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4748880402" className="hc-contact-phone">+47 48 88 04 02</a>
                </div>
              </div>
            </div>

            {/* --- Block 3 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Hjemmesykepleie Nord</h5>
                <p className="mb-3 text-muted">Alna, Grorud og Stovner</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:nord@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Vakttlf. alle dager 07.00–22.30</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4795932179" className="hc-contact-phone">+47 95 93 21 79</a>
                </div>
              </div>
            </div>

            {/* --- Block 4 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Hjemmesykepleie Syd</h5>
                <p className="mb-3 text-muted">Nordstrand, Østensjø, Søndre Nordstrand</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:syd@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Vakttlf. alle dager 07.00–22.30</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4790419123" className="hc-contact-phone">+47 90 41 91 23</a>
                </div>
              </div>
            </div>

            {/* --- Block 5 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Hjemmesykepleie Natt</h5>
                <p className="mb-3 text-muted">Nattjeneste i hele byen</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:natt@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Vakttlf. natt 22.00–07.00</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4745839066" className="hc-contact-phone">+47 45 83 90 66</a>
                </div>
              </div>
            </div>

            {/* --- Block 6 --- */}
            <div className="col-md-6 col-lg-4">
              <div className="hc-contact-card">
                <h5 className="fw-bold mb-1">Praktisk bistand</h5>
                <p className="mb-3 text-muted">Hverdagsstøtte og praktiske tjenester</p>

                <div className="d-flex align-items-center mb-2">
                  <i className="bi bi-envelope-open me-2"></i>
                  <a href="mailto:praktisk@homecare.no" className="hc-contact-link">Send e-post</a>
                </div>

                <p className="mb-1 text-muted">Hverdager 08.30–15.30</p>
                <div className="d-flex align-items-center">
                  <i className="bi bi-telephone-outbound me-2"></i>
                  <a href="tel:+4722373300" className="hc-contact-phone">+47 22 37 33 00</a>
                </div>
              </div>
            </div>

          </div>
        </div>
      </section>

      {/* NEWS SECTION */}
      <section className="news-section py-5">
        <div className="container">
          <h2 className="text-center fw-bold mb-5">Siste nytt</h2>

          <div className="row g-4">

            <div className="col-md-4">
              <div className="news-card shadow-sm rounded-4 overflow-hidden h-100">
                <img src="/images/forside4.png" className="w-100" alt="Nyhet 1" />
                <div className="p-4">
                  <h4 className="fw-semibold mb-2">
                    Høstkampanje på private tjenester!
                  </h4>
                  <p className="text-muted">
                    Private tjenester fra HomeCare bestilles direkte av deg. Dette gjør det enklere å få den hjelpen du trenger.
                  </p>
                  <a href="#" className="text-green fw-semibold">Les mer her</a>
                </div>
              </div>
            </div>

            <div className="col-md-4">
              <div className="news-card shadow-sm rounded-4 overflow-hidden h-100">
                <img src="/images/forside3.png" className="w-100" alt="Nyhet 2" />
                <div className="p-4">
                  <h4 className="fw-semibold mb-2">
                    HomeCare er nå Miljøfyrtårn-sertifisert!
                  </h4>
                  <p className="text-muted">
                    Vi er stolte av å være godkjent som Miljøfyrtårn.
                  </p>
                  <a href="#" className="text-green fw-semibold">Les mer her</a>
                </div>
              </div>
            </div>

            <div className="col-md-4">
              <div className="news-card shadow-sm rounded-4 overflow-hidden h-100">
                <img src="/images/forside2.png" className="w-100" alt="Nyhet 3" />
                <div className="p-4">
                  <h4 className="fw-semibold mb-2">
                    Bedre måltider – samarbeid med GodMatLyst
                  </h4>
                  <p className="text-muted">
                    Vi lanserer matkonsepter som sikrer gode måltider.
                  </p>
                  <a href="#" className="text-green fw-semibold">Les mer her</a>
                </div>
              </div>
            </div>

          </div>

          <div className="text-center mt-4">
            <a href="#" className="btn btn-main px-4 py-2">Se flere nyhetssaker</a>
          </div>
        </div>
      </section>

      {/* HELP FORM SECTION */}
      <section className="help-form-section py-5 bg-light">
        <div className="container">
          <div className="row align-items-center g-5">

            <div className="col-md-6">
              <img
                src="/images/fysio3.png"
                className="img-fluid rounded-4 shadow"
                alt="Få hjelp i hjemmet"
              />
            </div>

            <div className="col-md-6">
              <h2 className="fw-bold mb-3">Hvilke muligheter finnes for deg?</h2>
              <p className="lead text-muted mb-4">
                Legg igjen navn og telefonnummer, så tar vi kontakt.
              </p>

              <form className="p-4 bg-white rounded-4 shadow-sm">
                <div className="mb-3">
                  <label className="form-label">Navn</label>
                  <input type="text" className="form-control form-control-lg" required />
                </div>

                <div className="mb-3">
                  <label className="form-label">Telefon</label>
                  <input type="text" className="form-control form-control-lg" required />
                </div>

                <div className="mb-3">
                  <label className="form-label">E-post</label>
                  <input type="email" className="form-control form-control-lg" required />
                </div>

                <div className="mb-3">
                  <label className="form-label">Beskjed</label>
                  <textarea className="form-control form-control-lg" rows="3"></textarea>
                </div>

                <button className="btn btn-main btn-lg w-100 py-3">
                  Send forespørsel
                </button>
              </form>
            </div>

          </div>
        </div>
      </section>

      {/* SURVEY SECTION */}
      <section className="survey-section py-5 bg-light">
        <div className="container">
          <h2 className="text-center fw-bold mb-3">Brukerundersøkelse</h2>
          <p className="text-center text-muted mb-5" style={{ maxWidth: "720px", margin: "0 auto" }}>
            Resultatene viser at brukere og pårørende opplever HomeCare som trygg og tilgjengelig.
          </p>

          <div className="row g-4 justify-content-center">
            <div className="col-md-6">
              <div className="hc-survey-card">
                <div className="hc-survey-gauge">
                  <div className="hc-survey-needle"></div>
                  <div className="hc-survey-inner">
                    <span className="hc-survey-value">93%</span>
                  </div>
                </div>
                <p className="mt-3 text-center text-muted">
                  opplever at de blir møtt med respekt og trygghet
                </p>
              </div>
            </div>

            <div className="col-md-6">
              <div className="hc-survey-card">
                <div className="hc-survey-gauge">
                  <div className="hc-survey-needle needle-2"></div>
                  <div className="hc-survey-inner">
                    <span className="hc-survey-value">91%</span>
                  </div>
                </div>
                <p className="mt-3 text-center text-muted">
                  ville anbefalt HomeCare til andre
                </p>
              </div>
            </div>
          </div>

          <p className="text-center small text-muted mt-4">
            Hentet fra HomeCare sin brukerundersøkelse i 2025.
          </p>

          <div className="text-center mt-3">
            <a href="#" className="btn btn-main px-4 py-2">Les mer om undersøkelsen</a>
          </div>
        </div>
      </section>

      {/* STEPS SECTION */}
      <section className="steps-section py-5">
        <div className="container">
          <h2 className="text-center fw-bold mb-4">Slik fungerer HomeCare</h2>
          <p className="text-center text-muted mb-5" style={{ maxWidth: "720px", margin: "0 auto" }}>
            Vi har gjort det enkelt å få hjelp.
          </p>

          <div className="row g-4">
            <div className="col-md-4">
              <div className="hc-step-card text-center h-100">
                <div className="hc-step-number">1</div>
                <h4 className="fw-semibold mb-2">Ta kontakt</h4>
                <p className="text-muted">
                  Ring oss, send e-post eller bruk kontaktskjemaet.
                </p>
              </div>
            </div>

            <div className="col-md-4">
              <div className="hc-step-card text-center h-100">
                <div className="hc-step-number">2</div>
                <h4 className="fw-semibold mb-2">Planlegg sammen</h4>
                <p className="text-muted">
                  Vi lager en plan som passer deg.
                </p>
              </div>
            </div>

            <div className="col-md-4">
              <div className="hc-step-card text-center h-100">
                <div className="hc-step-number">3</div>
                <h4 className="fw-semibold mb-2">Trygg oppstart</h4>
                <p className="text-muted">
                  Du får faste kontaktpersoner.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* STATS SECTION */}
      <section className="stats-section py-5 bg-white">
        <div className="container">
          <div className="row g-4 justify-content-center text-center">
            <div className="col-6 col-md-3">
              <div className="hc-stat-pill">
                <span className="hc-stat-number">15+</span>
                <span className="hc-stat-label">års erfaring</span>
              </div>
            </div>

            <div className="col-6 col-md-3">
              <div className="hc-stat-pill">
                <span className="hc-stat-number">12</span>
                <span className="hc-stat-label">lokale team</span>
              </div>
            </div>

            <div className="col-6 col-md-3">
              <div className="hc-stat-pill">
                <span className="hc-stat-number">24/7</span>
                <span className="hc-stat-label">beredskap</span>
              </div>
            </div>

            <div className="col-6 col-md-3">
              <div className="hc-stat-pill">
                <span className="hc-stat-number">9,2</span>
                <span className="hc-stat-label">kundetilfredshet</span>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* ABOUT SECTION */}
      <section className="about-section py-5 bg-white rounded-5">
        <div className="container d-flex flex-column flex-md-row align-items-center gap-5">
          <div className="about-text flex-fill">
            <h2 className="fw-bold mb-3">Et varmt og pålitelig team</h2>
            <p className="lead text-muted mb-4">
              Våre ansatte er dedikerte fagfolk med hjerte for omsorg.
            </p>
            <a href="/about" className="btn btn-main px-4 py-2">Les mer om oss</a>
          </div>

          <div className="about-img flex-fill text-center">
            <img
              src="/images/forside4.png"
              alt="Omsorg"
              className="img-fluid rounded-4 shadow-lg"
              style={{ maxWidth: "80%" }}
            />
          </div>
        </div>
      </section>

      {/* TESTIMONIALS */}
      <section className="testimonials-section py-5 bg-light">
        <div className="container">
          <h2 className="text-center fw-bold mb-4">Hva sier brukerne våre?</h2>
          <p className="text-center text-muted mb-5" style={{ maxWidth: "720px", margin: "0 auto" }}>
            Pårørende og brukere setter mest pris på trygghet og respekt.
          </p>

          <div className="row g-4">
            <div className="col-md-4">
              <div className="hc-testimonial-card h-100">
                <p>«Vi opplever alltid at det er tid til en prat.»</p>
                <p className="fw-semibold mb-0">Datter til Arne, 78 år</p>
              </div>
            </div>

            <div className="col-md-4">
              <div className="hc-testimonial-card h-100">
                <p>«Det gir trygghet å vite at det er de samme personene som kommer.»</p>
                <p className="fw-semibold mb-0">Roar – pårørende</p>
              </div>
            </div>

            <div className="col-md-4">
              <div className="hc-testimonial-card h-100">
                <p>«Jeg føler meg virkelig ivaretatt og tatt på alvor.»</p>
                <p className="fw-semibold mb-0">Else, 82 år</p>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="cta-section text-center text-white py-5">
        <div className="container">
          <h2 className="fw-bold mb-3">Vil du vite mer?</h2>
          <p className="lead mb-4">Les om våre verdier eller ta kontakt – vi er her for deg.</p>

          <div className="d-flex justify-content-center gap-3 flex-wrap">
            <a href="/about" className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">Om oss</a>
            <a href="/contact" className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">Kontakt oss</a>
          </div>
        </div>
      </section>
    </>
  );
}
