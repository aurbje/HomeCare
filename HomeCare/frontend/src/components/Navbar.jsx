/**
 * Navbar.jsx - Main Navigation Bar
 *
 * Auth: Uses context/AuthContext.jsx (group's pattern)
 * Shows different nav items based on user role and login state
 */

import { useState, useRef, useEffect } from 'react'
import { useAuth } from '../context/AuthContext'

/**
 * Navbar Component
 * Main navigation bar with responsive design
 * Features:
 * - Logo and brand name
 * - Navigation links based on user role
 * - Font size controls for accessibility
 * - Mobile hamburger menu
 * - Login/Logout buttons
 */
export default function Navbar() {
  // AuthContext provides logoutUser (not logout) - aliased for compatibility
  const { user, logoutUser } = useAuth()
  const [isOpen, setIsOpen] = useState(false)
  const [fontSize, setFontSize] = useState(16)

  /**
   * Handle user logout
   * AuthContext's logoutUser already handles navigation to home
   */
  const handleLogout = () => {
    logoutUser()
  }

  /**
   * Change font size for accessibility
   */
  const changeFontSize = (delta) => {
    const newSize = Math.max(12, Math.min(24, fontSize + delta * 2))
    setFontSize(newSize)
    const elements = document.querySelectorAll('.font-resizable-area')
    elements.forEach(el => {
      el.style.fontSize = `${newSize}px`
    })
  }

  return (
    <header>
      <nav className="navbar navbar-expand-lg bg-white shadow-sm py-2 fixed-top">
        <div className="container-fluid px-3">
          {/* Logo */}
          <a href="/" className="navbar-brand d-flex align-items-center">
            <img src="/images/logoen.jpg" alt="HomeCare Logo" className="homecare-logo me-2" style={{ height: '45px' }} />
            <span className="fw-semibold text-success fs-3">HomeCare</span>
          </a>

          {/* Utility controls */}
          <div className="d-flex align-items-center gap-2 order-lg-last">
            {/* Font size controls */}
            <div className="text-size-controls d-flex">
              <button
                className="btn btn-light btn-sm shadow-sm border me-1"
                onClick={() => changeFontSize(1)}
                aria-label="Øk skriftstørrelse"
                style={{ padding: '4px 8px', lineHeight: 1 }}
              >
                A+
              </button>
              <button
                className="btn btn-light btn-sm shadow-sm border"
                onClick={() => changeFontSize(-1)}
                aria-label="Reduser skriftstørrelse"
                style={{ padding: '4px 8px', lineHeight: 1 }}
              >
                A−
              </button>
            </div>

            {/* Mobile toggle button */}
            <button
              className="navbar-toggler ms-2"
              type="button"
              data-bs-toggle="collapse"
              data-bs-target="#navbarNav"
              aria-controls="navbarNav"
              aria-expanded={isOpen}
              aria-label="Toggle navigation"
              onClick={() => setIsOpen(!isOpen)}
            >
              <span className="navbar-toggler-icon"></span>
            </button>
          </div>

          {/* Collapsible nav items */}
          <div className={`collapse navbar-collapse ${isOpen ? 'show' : ''}`} id="navbarNav">
            <ul className="navbar-nav align-items-lg-center flex-wrap">
              {/* Home link */}
              <li className="nav-item">
                <a href="/" className="nav-link px-3 py-2 fs-5">
                  <i className="bi bi-house-door me-1"></i> Hjem
                </a>
              </li>

              {/* About link */}
              <li className="nav-item">
                <a href="/about" className="nav-link px-3 py-2 fs-5">
                  <i className="bi bi-info-circle me-1"></i> Om oss
                </a>
              </li>

              {/* Caregiver schedule - only for caregivers */}
              {user && user.role === 'Caregiver' && (
                <li className="nav-item">
                  <a href="/caregiver/dashboard" className="nav-link px-3 py-2 fs-5">
                    <i className="bi bi-clipboard-check me-1"></i> Min arbeidsplan
                  </a>
                </li>
              )}

              {/* Admin panel - only for admins */}
              {user && user.role === 'Admin' && (
                <li className="nav-item">
                  <a href="/admin/dashboard" className="nav-link px-3 py-2 fs-5">
                    <i className="bi bi-gear me-1"></i> Admin
                  </a>
                </li>
              )}

              {/* My page - only for clients */}
              {user && user.role === 'Client' && (
                <li className="nav-item">
                  <a href="/dashboard" className="nav-link px-3 py-2 fs-5">
                    <i className="bi bi-person-circle me-1"></i> Min side
                  </a>
                </li>
              )}

              {/* Booking - only for clients */}
              {user && user.role === 'Client' && (
                <li className="nav-item">
                  <a href="/booking" className="nav-link px-3 py-2 fs-5">
                    <i className="bi bi-calendar-check me-1"></i> Bestill time
                  </a>
                </li>
              )}

              {/* Contact link */}
              <li className="nav-item">
                <a href="/contact" className="nav-link px-3 py-2 fs-5">
                  <i className="bi bi-envelope me-1"></i> Kontakt
                </a>
              </li>

              {/* Auth buttons */}
              {!user ? (
                <>
                  <li className="nav-item ms-lg-2 mt-2 mt-lg-0">
                    <a href="/login" className="btn btn-success px-3 py-2 fs-6">
                      <i className="bi bi-person-plus me-1"></i> Logg inn
                    </a>
                  </li>
                  <li className="nav-item ms-lg-2 mt-2 mt-lg-0">
                    <a href="/register" className="btn btn-outline-success px-3 py-2 fs-6">
                      <i className="bi bi-box-arrow-in-right me-1"></i> Registrer deg
                    </a>
                  </li>
                </>
              ) : (
                <>
                  <li className="nav-item ms-lg-2">
                    <span className="nav-link py-2 fs-5">
                      {user.fullName}
                    </span>
                  </li>
                  <li className="nav-item ms-lg-2 mt-2 mt-lg-0">
                    <button
                      onClick={handleLogout}
                      className="btn btn-outline-danger px-3 py-2 fs-6"
                    >
                      <i className="bi bi-box-arrow-right me-1"></i> Logg ut
                    </button>
                  </li>
                </>
              )}
            </ul>
          </div>
        </div>
      </nav>
    </header>
  )
}
