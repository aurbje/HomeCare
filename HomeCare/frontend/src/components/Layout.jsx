import React, { useState } from "react";
import { Link, Outlet, useNavigate } from "react-router-dom";
import "../styles/global.css";
import { useAuth } from "../context/AuthContext";
import { logout as apiLogout } from "../api/authApi";

export default function Layout() {
  const [menuOpen, setMenuOpen] = useState(false);
  const [fontSize, setFontSize] = useState(16);
  const { user, logoutUser } = useAuth();
  const navigate = useNavigate();

  const openMenu = () => setMenuOpen(true);
  const closeMenu = () => setMenuOpen(false);

  const getDashboardPath = (u) => {
    const role = u?.role?.toLowerCase();
    if (role === "admin") return "/admindashboard";
    if (role === "caregiver") return "/caregiver";
    return "/dashboard";
  };

  const handleLogout = async () => {
    try {
      await apiLogout();
    } finally {
      logoutUser();
      navigate("/");
    }
  };

  // new: font size
  const changeFontSize = (delta) => {
    const newSize = Math.max(12, Math.min(24, fontSize + delta * 2));
    setFontSize(newSize);
  };

  const handleMenuKeyDown = (e) => {
    if (e.key === 'Escape') {
      closeMenu();
    }
  };

  return (
    <>
      <a href="#main-content" className="skip-link visually-hidden-focusable">
        Hopp til hovedinnhold
      </a>

      <header>
        <nav
          className="navbar navbar-expand-lg bg-white shadow-sm py-3 fixed-top"
          role="navigation"
          aria-label="Hovednavigasjon"
          style={{ fontSize: '1rem' }}
        >
          <div className="container">
            <Link
              className="navbar-brand d-flex align-items-center"
              to="/"
              aria-label="HomeCare - Gå til forsiden"
            >
              <img
                src="/images/logoen.jpg"
                alt=""
                aria-hidden="true"
                className="homecare-logo me-2"
              />
              <span className="fw-semibold text-success fs-4">HomeCare</span>
            </Link>

            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#navbarNav"
              aria-controls="navbarNav"
              aria-expanded="false"
              aria-label="Åpne navigasjonsmeny"
            >
              <span className="navbar-toggler-icon" aria-hidden="true"></span>
            </button>

            <div className="collapse navbar-collapse justify-content-between w-100" id="navbarNav">
              <ul className="navbar-nav align-items-center gap-2" role="menubar">
                <li className="nav-item" role="none">
                  <Link className="nav-link" to="/" role="menuitem">
                    <i className="bi bi-house-door me-1" aria-hidden="true"></i>
                    <span className="d-none d-lg-inline">Hjem</span>
                  </Link>
                </li>

                <li className="nav-item" role="none">
                  <Link className="nav-link" to="/about" role="menuitem">
                    <i className="bi bi-info-circle me-1" aria-hidden="true"></i>
                    <span className="d-none d-lg-inline">Om oss</span>
                  </Link>
                </li>

                <li className="nav-item" role="none">
                  <Link className="nav-link" to="/contact" role="menuitem">
                    <i className="bi bi-envelope me-1" aria-hidden="true"></i>
                    <span className="d-none d-lg-inline">Kontakt</span>
                  </Link>
                </li>

                {user?.role === "User" && (
                  <li className="nav-item" role="none">
                    <Link className="nav-link" to="/booking" role="menuitem">
                      <i className="bi bi-calendar-check me-1" aria-hidden="true"></i>
                      <span className="d-none d-lg-inline">Booking</span>
                    </Link>
                  </li>
                )}

                {user ? (
                  <>
                    <li className="nav-item" role="none">
                      <Link
                        className="btn btn-outline-primary btn-sm px-3"
                        to={getDashboardPath(user)}
                        role="menuitem"
                      >
                        <i className="bi bi-person-circle me-1" aria-hidden="true"></i>
                        <span className="d-none d-md-inline">Min side</span>
                      </Link>
                    </li>
                    <li className="nav-item" role="none">
                      <button
                        className="btn btn-outline-danger btn-sm px-3"
                        onClick={handleLogout}
                        role="menuitem"
                      >
                        <i className="bi bi-box-arrow-right me-1" aria-hidden="true"></i>
                        <span className="d-none d-md-inline">Logg ut</span>
                      </button>
                    </li>
                  </>
                ) : (
                  <>
                    <li className="nav-item" role="none">
                      <Link className="btn btn-success btn-sm px-3" to="/login" role="menuitem">
                        <i className="bi bi-box-arrow-in-right me-1" aria-hidden="true"></i>
                        <span className="d-none d-md-inline">Logg inn</span>
                      </Link>
                    </li>
                    <li className="nav-item" role="none">
                      <Link className="btn btn-outline-success btn-sm px-3" to="/register" role="menuitem">
                        <i className="bi bi-person-plus me-1" aria-hidden="true"></i>
                        <span className="d-none d-md-inline">Registrer</span>
                      </Link>
                    </li>
                  </>
                )}

                {/* Font size controls */}
                <li className="nav-item d-flex align-items-center" role="none">
                  <div className="btn-group" role="group" aria-label="Tekststørrelse">
                    <button
                      className="btn btn-outline-secondary btn-sm"
                      onClick={() => changeFontSize(1)}
                      aria-label="Øk skriftstørrelse"
                      title="Øk tekststørrelse"
                    >
                      A+
                    </button>
                    <button
                      className="btn btn-outline-secondary btn-sm"
                      onClick={() => changeFontSize(-1)}
                      aria-label="Reduser skriftstørrelse"
                      title="Reduser tekststørrelse"
                    >
                      A−
                    </button>
                  </div>
                </li>

                {/* Side menu toggle */}
                <li className="nav-item ms-2" role="none">
                  <button
                    className="btn hc-more-toggle"
                    onClick={openMenu}
                    aria-expanded={menuOpen}
                    aria-controls="hcSideMenu"
                    aria-label="Åpne sidemeny med flere alternativer"
                  >
                    <i className="bi bi-list" aria-hidden="true"></i>
                  </button>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        <div
          id="hcSideMenu"
          className={`hc-side-menu ${menuOpen ? "open" : ""}`}
          aria-hidden={!menuOpen}
          aria-label="Sidemeny"
          role="dialog"
          aria-modal="true"
          onKeyDown={handleMenuKeyDown}
          style={{ fontSize: '1rem' }}
        >
          <div className="hc-side-menu-inner">
            <div className="hc-side-menu-header">
              <div className="d-flex align-items-center gap-2">
                <img
                  src="/images/logoen.jpg"
                  alt=""
                  aria-hidden="true"
                  className="homecare-logo me-1"
                />
              </div>
              <button
                type="button"
                className="hc-side-menu-close"
                onClick={closeMenu}
                aria-label="Lukk sidemeny"
              >
                <i className="bi bi-x-lg" aria-hidden="true"></i>
              </button>
            </div>

            <nav className="hc-side-menu-links" aria-label="Sidemeny navigasjon">
              <div className="hc-side-menu-section-title" id="section-homecare">HomeCare</div>

              <Link to="#" onClick={closeMenu}>Nyheter</Link>
              <Link to="/about" onClick={closeMenu}>Om Oss</Link>


              <hr className="hc-side-menu-divider" aria-hidden="true" />

              <div className="hc-side-menu-section-title" id="section-services">Våre tjenester</div>
              <Link to="#" onClick={closeMenu}>Hjemmetjenester</Link>
              <Link to="#" onClick={closeMenu}>BPA</Link>
              <Link to="#" onClick={closeMenu}>Stasjonær hjemmesykepleie</Link>
              <Link to="#" onClick={closeMenu}>Fysioterapi</Link>
              <Link to="#" onClick={closeMenu}>Matbestilling</Link>


              <hr className="hc-side-menu-divider" aria-hidden="true" />

              <div className="hc-side-menu-section-title" id="section-info">Informasjon</div>

              <Link to="#" onClick={closeMenu}>Pårørende</Link>
              <Link to="#" onClick={closeMenu}>Lover og regler</Link>
              <Link to="/contact" onClick={closeMenu}>Kontakt oss</Link>


              <hr className="hc-side-menu-divider" aria-hidden="true" />

              <div className="hc-side-menu-section-title" id="section-ansatte">For ansatte</div>
              <Link to="#" onClick={closeMenu}>Jobb hos oss</Link>
              {!user && (
                <Link to="/login" className="hc-side-menu-employee" onClick={closeMenu}>
                  Logg inn
                </Link>
              )}


              <hr className="hc-side-menu-divider" aria-hidden="true" />

              <div className="hc-side-menu-footer mt-4 pb-4">
                <small className="text-white">Personvern • © 2025 HomeCare</small>
              </div>
            </nav>
          </div>
        </div>

        <div
          className={`hc-side-menu-backdrop ${menuOpen ? "visible" : ""}`}
          onClick={closeMenu}
          aria-hidden="true"
        ></div>
      </header>

      <main id="main-content" className="pb-5 pt-4" role="main" style={{ fontSize: `${fontSize}px` }}>
        <Outlet />
      </main>

      <footer className="border-top bg-light text-muted py-4 mt-5" role="contentinfo" style={{ fontSize: '1rem' }}>
        <div className="container text-center">
          <p className="mb-0">© {new Date().getFullYear()} - HomeCare | Personvern</p>
        </div>
      </footer>
    </>
  );
}