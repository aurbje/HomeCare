// src/components/Layout.jsx
import React, { useState } from "react";
import { Link, Outlet, useNavigate } from "react-router-dom";
import "../styles/global.css";
import { useAuth } from "../context/AuthContext";
import { logout as apiLogout } from "../api/authApi";

export default function Layout() {
  const [menuOpen, setMenuOpen] = useState(false);
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
      await apiLogout();   // backend: fjern auth-cookie
    } finally {
      logoutUser();        // frontend: nullstill bruker
      navigate("/");       // tilbake til hjem
    }
  };

  return (
    <>
      {/* NAVBAR */}
      <header>
        <nav className="navbar navbar-expand-lg bg-white shadow-sm py-3 fixed-top">
          <div className="container">
            <Link className="navbar-brand d-flex align-items-center" to="/">
              <img
                src="/images/logoen.jpg"
                alt="HomeCare logo"
                className="homecare-logo me-2"
              />
              <span className="fw-semibold text-success fs-4">HomeCare</span>
            </Link>

            <button
              className="navbar-toggler"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#navbarNav"
            >
              <span className="navbar-toggler-icon"></span>
            </button>

            <div
              className="collapse navbar-collapse justify-content-end"
              id="navbarNav"
            >
              <ul className="navbar-nav align-items-center">
                <li className="nav-item mx-2">
                  <Link className="nav-link" to="/">
                    <i className="bi bi-house-door me-1"></i> Hjem
                  </Link>
                </li>

                <li className="nav-item mx-2">
                  <Link className="nav-link" to="/about">
                    <i className="bi bi-info-circle me-1"></i> Om oss
                  </Link>
                </li>

                <li className="nav-item mx-2">
                  <Link className="nav-link" to="/contact">
                    <i className="bi bi-envelope me-1"></i> Kontakt
                  </Link>
                </li>

                {user?.role === "User" && (
                <li className="nav-item mx-2">
                  <Link className="nav-link" to="/booking">
                    <i className="bi bi-calendar-check me-1"></i> Booking
                  </Link>
                </li>
                )}
                {/* left: depends on if your logged in */}
                {user ? (
                  <>
                    <li className="nav-item ms-3">
                      <Link
                        className="btn btn-outline-success px-3"
                        to={getDashboardPath(user)}
                      >
                        Min side
                      </Link>
                    </li>

                    <li className="nav-item ms-3">
                      <button
                        className="btn btn-outline-danger px-3"
                        onClick={handleLogout}
                      >
                        Logg ut
                      </button>
                    </li>
                  </>
                ) : (
                  <>
                    <li className="nav-item ms-3">
                      <Link className="btn btn-success px-3" to="/login">
                        <i className="bi bi-box-arrow-in-right me-1"></i> Logg inn
                      </Link>
                    </li>

                    <li className="nav-item ms-3">
                      <Link
                        className="btn btn-outline-success px-3 me-2"
                        to="/register"
                      >
                        <i className="bi bi-person-plus me-1"></i> Registrer deg
                      </Link>
                    </li>
                  </>
                )}

                {/* Side menu toggle */}
                <li className="nav-item ms-2">
                  <button
                    className="btn hc-more-toggle"
                    onClick={openMenu}
                    aria-expanded={menuOpen}
                  >
                    <i className="bi bi-list"></i>
                  </button>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        {/* SIDE MENU (her kan du evt. også bruke user/role senere) */}
        <div
          id="hcSideMenu"
          className={`hc-side-menu ${menuOpen ? "open" : ""}`}
          aria-hidden={!menuOpen}
        >
          <div className="hc-side-menu-inner">
            <div className="hc-side-menu-header">
              <div className="d-flex align-items-center gap-2">
                <img
                  src="/images/logoen.jpg"
                  alt="HomeCare logo"
                  className="homecare-logo me-1"
                />
              </div>

              <button
                type="button"
                className="hc-side-menu-close"
                onClick={closeMenu}
              >
                <i className="bi bi-x-lg"></i>
              </button>
            </div>

            <nav className="hc-side-menu-links">
              <div className="hc-side-menu-section-title">HomeCare</div>

              <Link to="#" onClick={closeMenu}>
                Nyheter
              </Link>
              <Link to="/about" onClick={closeMenu}>
                Om Oss
              </Link>

              <hr className="hc-side-menu-divider" />

              <div className="hc-side-menu-section-title">Våre tjenester</div>
              <Link to="#" onClick={closeMenu}>
                Hjemmetjenester
              </Link>
              <Link to="#" onClick={closeMenu}>
                BPA
              </Link>
              <Link to="#" onClick={closeMenu}>
                Stasjonær hjemmesykepleie
              </Link>
              <Link to="#" onClick={closeMenu}>
                Fysioterapi
              </Link>
              <Link to="#" onClick={closeMenu}>
                Matbestilling
              </Link>

              <hr className="hc-side-menu-divider" />

              <div className="hc-side-menu-section-title">Informasjon</div>
              <Link to="#" onClick={closeMenu}>
                Pårørende
              </Link>
              <Link to="#" onClick={closeMenu}>
                Lover og regler
              </Link>
              <Link to="/contact" onClick={closeMenu}>
                Kontakt oss
              </Link>

              <hr className="hc-side-menu-divider" />

              <div className="hc-side-menu-section-title">For ansatte</div>
              <Link to="#" onClick={closeMenu}>
                Jobb hos oss
              </Link>
              
              {!user && (
              <Link
                to="/login"
                className="hc-side-menu-employee"
                onClick={closeMenu}
              >
                Logg inn
              </Link>
              )}
              <hr className="hc-side-menu-divider" />

              <div className="hc-side-menu-footer mt-4 pb-4">
                <small className="text-white">
                  Personvern • © 2025 HomeCare
                </small>
              </div>
            </nav>
          </div>
        </div>

        {/* BACKDROP */}
        <div
          className={`hc-side-menu-backdrop ${menuOpen ? "visible" : ""}`}
          onClick={closeMenu}
        ></div>
      </header>

      {/* PAGE CONTENT */}
      <main className="pb-5 pt-4">
        <Outlet />
      </main>

      {/* FOOTER */}
      <footer className="border-top bg-light text-muted py-4 mt-5">
        <div className="container text-center">
          © {new Date().getFullYear()} - HomeCare | Personvern
        </div>
      </footer>
    </>
  );
}
