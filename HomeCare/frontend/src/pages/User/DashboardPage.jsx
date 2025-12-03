import { useEffect, useState } from 'react'
import { useAuth } from '../../hooks/useAuth'
import api from '../../api/api'

/**
 * DashboardPage Component (User/Client Dashboard)
 * Compatible with group's UserController API
 * Displays client dashboard with reminders, bookings, and calendar placeholder
 * Features:
 * - Welcome message with user name
 * - List of reminders with time and message
 * - List of upcoming bookings
 * - Quick link to book new bookings
 * - Font resizable area for accessibility
 *
 * Group's API endpoint:
 * - GET /api/user/dashboard - Get user dashboard data
 */
export default function DashboardPage() {
  const { user } = useAuth()
  // State for dashboard data: userName, reminders, and bookings
  const [data, setData] = useState(null)

  /**
   * Fetch dashboard data from backend on component mount
   * Uses cookie-based authentication
   * Data includes: userName, reminders[], bookings[]
   */
  useEffect(() => {
    api.get('/user/dashboard')
      .then(res => setData(res.data))
      .catch(err => console.error('Failed to load dashboard:', err))
  }, [])

  // Show loading message while data is being fetched
  if (!data) return <div className="container mt-5">Laster...</div>

  return (
    <div className="font-resizable-area user-dashboard">
      {/* Dashboard header with welcome message */}
      <div className="dashboard-header text-center py-4 mt-5">
        <h1 className="fs-2">Velkommen til din side!</h1>
        <h2 className="fs-3">Velkommen, {data.userName || user?.fullName || 'Bruker'}!</h2>
      </div>

      <div className="container-fluid py-2">
        <div className="row g-3">
          {/* Left column: Reminders and Bookings */}
          <div className="col-12 col-lg-6 d-flex flex-column gap-2">

            {/* Reminders Section */}
            <section role="region" aria-labelledby="reminder-heading">
              <div className="card border-start col-12 border-4 border-success">
                <div className="card-body">
                  <h2 id="reminder-heading" className="card-title fs-4 mb-3">Påminnelser</h2>
                  <ul className="list-group">
                    {data.reminders && data.reminders.length > 0 ? (
                      data.reminders.map((reminder, idx) => (
                        <li key={idx} className="list-group-item">
                          <strong>{reminder.time}</strong> - {reminder.message}
                        </li>
                      ))
                    ) : (
                      <li className="list-group-item text-muted">Ingen påminnelser.</li>
                    )}
                  </ul>
                </div>
              </div>
            </section>

            {/* Bookings Section */}
            <section role="region" aria-labelledby="booking-heading">
              <div className="card border-start border-4 border-success">
                <div className="card-body">
                  <h2 id="booking-heading" className="card-title fs-4 mb-3">Dine timer</h2>
                  <ul className="list-group mb-3">
                    {data.bookings && data.bookings.length > 0 ? (
                      data.bookings.map(booking => (
                        <li key={booking.id} className="list-group-item">
                          {/* Display booking date/time and service category */}
                          {new Date(booking.dateTime).toLocaleString('nb-NO')} - {booking.category?.name || 'N/A'}
                          {/* Display assigned caregiver name */}
                          {booking.caregiver && (
                            <div className="text-muted small">
                              Ansatt: {booking.caregiver.fullName}
                            </div>
                          )}
                        </li>
                      ))
                    ) : (
                      <li className="list-group-item text-muted">Ingen kommende timer.</li>
                    )}
                  </ul>
                  <div className="mt-4">
                    {/* Button to navigate to booking page */}
                    <a
                      href="/booking"
                      className="btn btn-primary btn-lg px-4 py-2"
                      aria-label="Bestill ny time"
                    >
                      <i className="bi bi-calendar-plus me-2"></i> Book time her
                    </a>
                  </div>
                </div>
              </div>
            </section>
          </div>

          {/* Right column: Calendar Placeholder */}
          <div className="col-12 col-lg-6">
            <section role="region" aria-labelledby="calendar-heading">
              <div className="card border-start border-4 border-success">
                <div className="card-body">
                  <h2 id="calendar-heading" className="card-title fs-4 mb-3">Kalender</h2>
                  {/* TODO: Implement calendar component for visual booking scheduling */}
                  <p className="text-muted">Kalender kommer snart...</p>
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>
    </div>
  )
}
