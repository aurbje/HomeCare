import React from "react";

export default function About() {
  return (
    <>
      {/* hero-section */}
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

      {/* what is homecare-section */}
      <section className="py-5 bg-light text-center">
        <div className="container">
          <h2 className="fw-bold mb-4 text-green">Hva er HomeCare?</h2>
          <p className="lead text-muted mx-auto" style={{ maxWidth: "800px" }}>
            HomeCare har levert pålitelige og omsorgsfulle hjemmetjenester siden 1999.
            Vi tilbyr tjenester både gjennom offentlige ordninger og for privatpersoner.
            Respekt, nærhet og personlig oppfølging ligger til grunn for alt vi gjør.
          </p>
        </div>
      </section>

      {/* vision and mision */}
      <section className="py-5 bg-white">
        <div className="container">
          <div className="row g-4">

            <div className="col-md-6">
              <div className="card h-100 border-0 shadow-lg rounded-4 hover-lift">
                <div className="card-body p-4">
                  <h3 className="fw-bold text-gradient mb-3">
                    <i className="bi bi-eye-fill me-2"></i>Vår visjon
                  </h3>
                  <p className="text-muted">
                    Et samfunn der alle eldre kan leve trygt og selvstendig hjemme,
                    støttet av moderne omsorgsteknologi.
                  </p>
                </div>
              </div>
            </div>

            <div className="col-md-6">
              <div className="card h-100 border-0 shadow-lg rounded-4 hover-lift">
                <div className="card-body p-4">
                  <h3 className="fw-bold text-gradient mb-3">
                    <i className="bi bi-heart-fill me-2"></i>Vår misjon
                  </h3>
                  <p className="text-muted">
                    Å tilby en brukervennlig og pålitelig plattform som forenkler
                    hjemmetjenester – med trygghet, kvalitet og omsorg i fokus.
                  </p>
                </div>
              </div>
            </div>

          </div>
        </div>
      </section>

      {/* homecares values */}
      <section className="py-5 bg-light text-center">
        <div className="container">
          <h2 className="fw-bold mb-5 text-green">Våre kjerneverdier</h2>

          <div className="row g-4">

            <div className="col-md-3 col-sm-6">
              <div className="card shadow-sm border-0 h-100 rounded-4 hover-lift">
                <div className="card-body">
                  <i className="bi bi-people-fill fs-1 mb-3"></i>
                  <h5 className="fw-bold text-gradient">Omsorg</h5>
                  <p className="text-muted small">
                    Vi setter brukernes behov først og sørger for personlig oppfølging.
                  </p>
                </div>
              </div>
            </div>

            <div className="col-md-3 col-sm-6">
              <div className="card shadow-sm border-0 h-100 rounded-4 hover-lift">
                <div className="card-body">
                  <i className="bi bi-shield-lock-fill fs-1 mb-3"></i>
                  <h5 className="fw-bold text-gradient">Trygghet</h5>
                  <p className="text-muted small">
                    Sikkerhet, personvern og pålitelighet er kjernen i tjenestene våre.
                  </p>
                </div>
              </div>
            </div>

            <div className="col-md-3 col-sm-6">
              <div className="card shadow-sm border-0 h-100 rounded-4 hover-lift">
                <div className="card-body">
                  <i className="bi bi-lightning-charge-fill fs-1 mb-3"></i>
                  <h5 className="fw-bold text-gradient">Enkelhet</h5>
                  <p className="text-muted small">
                    HomeCare skal være lett å bruke for alle, uansett erfaring.
                  </p>
                </div>
              </div>
            </div>

            <div className="col-md-3 col-sm-6">
              <div className="card shadow-sm border-0 h-100 rounded-4 hover-lift">
                <div className="card-body">
                  <i className="bi bi-cpu-fill fs-1 mb-3"></i>
                  <h5 className="fw-bold text-gradient">Innovasjon</h5>
                  <p className="text-muted small">
                    Vi forbedrer løsningen kontinuerlig for å møte fremtidens behov.
                  </p>
                </div>
              </div>
            </div>

          </div>

        </div>
      </section>

      {/* history */}
      <section className="py-5">
        <div className="container">
          <h2 className="fw-bold text-center mb-5 text-gradient">Vår historie</h2>

          <div className="timeline mx-auto" style={{ maxWidth: "700px" }}>
            <div className="timeline-item mb-4">
              <h6 className="fw-bold text-gradient">2024 – Idéen oppstår</h6>
              <p className="text-muted small">
                Behovet for trygg digital hjemmetjeneste kartlegges.
              </p>
            </div>

            <div className="timeline-item mb-4">
              <h6 className="fw-bold text-gradient">2025 – Utvikling starter</h6>
              <p className="text-muted small">
                Første prototype lanseres i samarbeid med hjemmetjenesten.
              </p>
            </div>

            <div className="timeline-item mb-4">
              <h6 className="fw-bold text-gradient">2025 – Pilot og tilbakemelding</h6>
              <p className="text-muted small">
                Pilotprosjekt gir verdifulle innspill til forbedringer.
              </p>
            </div>

            <div className="timeline-item">
              <h6 className="fw-bold text-gradient">2025 – Lansering</h6>
              <p className="text-muted small">
                HomeCare lanseres nasjonalt med fokus på kvalitet og trygghet.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* team section */}
      <section className="py-5 bg-light text-center">
        <div className="container">
          <h2 className="fw-bold mb-5 text-green">Teamet bak HomeCare</h2>

          <div className="row g-5 justify-content-center">

            {/* person 1 */}
            <div className="col-md-3 col-sm-6">
              <div className="card border-0 shadow-lg rounded-4 hover-lift h-100">
                <div className="card-body">
                  <img
                    src="/images/alexander.png"
                    alt="Alexander Jahr"
                    className="rounded-circle mb-3 shadow-sm"
                    width="100"
                    height="100"
                  />
                  <h5 className="fw-bold text-gradient">Alexander Jahr</h5>
                  <p className="text-muted small">Grunnlegger & Backend</p>
                </div>
              </div>
            </div>

            {/* person 2 */}
            <div className="col-md-3 col-sm-6">
              <div className="card border-0 shadow-lg rounded-4 hover-lift h-100">
                <div className="card-body">
                  <img
                    src="/images/yu.png"
                    alt="Yu Kaland"
                    className="rounded-circle mb-3 shadow-sm"
                    width="100"
                    height="100"
                  />
                  <h5 className="fw-bold text-gradient">Yu Terada Kaland</h5>
                  <p className="text-muted small">Grunnlegger & Backend</p>
                </div>
              </div>
            </div>

            {/* person 3 */}
            <div className="col-md-3 col-sm-6">
              <div className="card border-0 shadow-lg rounded-4 hover-lift h-100">
                <div className="card-body">
                  <img
                    src="/images/ulrik.png"
                    alt="Ulrik Rasmussen"
                    className="rounded-circle mb-3 shadow-sm"
                    width="100"
                    height="100"
                  />
                  <h5 className="fw-bold text-gradient">Ulrik Rasmussen</h5>
                  <p className="text-muted small">Grunnlegger & UX-design</p>
                </div>
              </div>
            </div>

            {/* person 4 */}
            <div className="col-md-3 col-sm-6">
              <div className="card border-0 shadow-lg rounded-4 hover-lift h-100">
                <div className="card-body">
                  <img
                    src="/images/aurora.png"
                    alt="Aurora Bjerke"
                    className="rounded-circle mb-3 shadow-sm"
                    width="100"
                    height="100"
                  />
                  <h5 className="fw-bold text-gradient">Aurora Bjerke</h5>
                  <p className="text-muted small">Grunnlegger & UX-design</p>
                </div>
              </div>
            </div>

          </div>
        </div>
      </section>

      {/* cta */}
      <section className="cta-section text-white text-center py-5">
        <div className="container">
          <h2 className="fw-bold mb-3 text-gradient">Bli en del av HomeCare</h2>
          <p className="lead mb-4">
            Kontakt oss for samarbeid, investering eller mer informasjon.
          </p>
          <a href="/contact" className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg">
            Kontakt oss
          </a>
        </div>
      </section>
    </>
  );
}
