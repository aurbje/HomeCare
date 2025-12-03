/**
 * DashboardPage.jsx - User/Client Dashboard
 *
 * Auth: Uses context/AuthContext.jsx (group's pattern)
 * Backend endpoint: GET /api/user/dashboard (UserController.GetDashboard)
 */

import { useEffect, useState, useMemo } from 'react'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'

/**
 * DashboardPage Component (User/Client Dashboard)
 * Compatible with group's UserController API
 * Displays client dashboard with reminders, bookings, and calendar
 * Features:
 * - Welcome message with user name
 * - List of reminders with time and message
 * - List of upcoming bookings
 * - Interactive calendar showing booked dates
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

  // Calendar state
  const [calendarYear, setCalendarYear] = useState(new Date().getFullYear())
  const [calendarMonth, setCalendarMonth] = useState(new Date().getMonth()) // 0-indexed

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

  /**
   * Convert date to YYYY-MM-DD string format (local timezone)
   */
  const toDateString = (date) => {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  /**
   * Set of booked date strings for calendar highlighting
   */
  const bookedDateSet = useMemo(() => {
    const dates = data?.bookings ?? []
    return new Set(dates.map(b => {
      const d = new Date(b.dateTime)
      return toDateString(d)
    }))
  }, [data?.bookings])

  /**
   * Map of bookings by date for calendar display
   */
  const bookingsByDate = useMemo(() => {
    const bookings = data?.bookings ?? []
    const map = new Map()
    bookings.forEach(booking => {
      const d = new Date(booking.dateTime)
      const dateStr = toDateString(d)
      if (!map.has(dateStr)) {
        map.set(dateStr, [])
      }
      map.get(dateStr).push(booking)
    })
    return map
  }, [data?.bookings])

  /**
   * Get today's bookings for "Dine timer" section
   */
  const todayBookings = useMemo(() => {
    const todayStr = toDateString(new Date())
    return bookingsByDate.get(todayStr) || []
  }, [bookingsByDate])

  /**
   * Navigate to previous month
   */
  const goToPrevMonth = () => {
    if (calendarMonth === 0) {
      setCalendarMonth(11)
      setCalendarYear(y => y - 1)
    } else {
      setCalendarMonth(m => m - 1)
    }
  }

  /**
   * Navigate to next month
   */
  const goToNextMonth = () => {
    if (calendarMonth === 11) {
      setCalendarMonth(0)
      setCalendarYear(y => y + 1)
    } else {
      setCalendarMonth(m => m + 1)
    }
  }

  /**
   * Generate calendar days for current month
   */
  const generateCalendarDays = () => {
    const firstDay = new Date(calendarYear, calendarMonth, 1)
    const lastDay = new Date(calendarYear, calendarMonth + 1, 0)
    const startPadding = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1 // Monday start
    const days = []

    // Add padding for days before month starts
    for (let i = 0; i < startPadding; i++) {
      days.push(null)
    }

    // Add days of the month
    for (let day = 1; day <= lastDay.getDate(); day++) {
      days.push(new Date(calendarYear, calendarMonth, day))
    }

    return days
  }

  // Show loading message while data is being fetched
  if (!data) return <div className="container mt-5">Laster...</div>

  const calendarDays = generateCalendarDays()
  const today = new Date()
  today.setHours(0, 0, 0, 0)

  const monthNames = [
    'Januar', 'Februar', 'Mars', 'April', 'Mai', 'Juni',
    'Juli', 'August', 'September', 'Oktober', 'November', 'Desember'
  ]

  const dayNames = ['Man', 'Tir', 'Ons', 'Tor', 'Fre', 'Lør', 'Søn']

  return (
    <div className="font-resizable-area user-dashboard">
      <div className="container py-3">
        {/* Header */}
        <div className="mb-3">
          <h1 className="display-6 fw-bold text-success mb-1">Din side</h1>
          <p className="lead text-muted mb-0">Du er logget inn som {data.userName || user?.fullName || 'Bruker'}</p>
        </div>

        <div className="row g-4">
          {/* Left column - Info cards */}
          <div className="col-12 col-lg-4">
            <div className="d-flex flex-column w-100" style={{ gap: '0.5rem' }}>

              {/* Today's Bookings Section (Dine timer i dag) */}
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 className="card-title fs-4 mb-3">
                    <i className="bi bi-calendar-day me-2"></i>Dine timer i dag
                  </h2>
                  {todayBookings.length > 0 ? (
                    <ul className="list-group" style={{ maxHeight: '280px', overflowY: 'auto' }}>
                      {todayBookings.map(booking => (
                        <li key={booking.id} className="list-group-item">
                          <div className="fw-bold">
                            <i className="bi bi-clock me-1"></i>
                            {new Date(booking.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                          </div>
                          <div className="small">
                            <i className="bi bi-person me-1"></i>
                            {booking.caregiver?.fullName || 'Ikke tildelt'} ({booking.category?.name || 'N/A'})
                          </div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted mb-0">
                      <i className="bi bi-check-circle me-2"></i>Ingen timer i dag.
                    </p>
                  )}
                  <div className="mt-3">
                    <a
                      href="/booking"
                      className="btn btn-primary w-100"
                      aria-label="Bestill ny time"
                    >
                      <i className="bi bi-calendar-plus me-2"></i>Book ny time
                    </a>
                  </div>
                </div>
              </div>

              {/* Reminders Section (Påminnelser) */}
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 className="card-title fs-4 mb-3">
                    <i className="bi bi-bell me-2"></i>Påminnelser
                  </h2>
                  {data.reminders && data.reminders.length > 0 ? (
                    <ul className="list-group" style={{ maxHeight: '280px', overflowY: 'auto' }}>
                      {data.reminders.map((reminder, idx) => (
                        <li key={idx} className="list-group-item">
                          <div className="fw-bold">{reminder.time}</div>
                          <div className="small text-muted">{reminder.message}</div>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted mb-0">
                      <i className="bi bi-check-circle me-2"></i>Ingen påminnelser.
                    </p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Right column - Calendar */}
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm">
              <div className="card-body">
                <h2 className="card-title fs-4 mb-3">
                  <i className="bi bi-calendar3 me-2"></i>Kalender
                </h2>
                <p className="text-muted small mb-3">
                  Dager med bestillinger er markert med grønn farge.
                </p>

                {/* Calendar navigation */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                  <button className="btn btn-outline-secondary" onClick={goToPrevMonth}>
                    <i className="bi bi-chevron-left"></i> Forrige
                  </button>
                  <h3 className="mb-0">{monthNames[calendarMonth]} {calendarYear}</h3>
                  <button className="btn btn-outline-secondary" onClick={goToNextMonth}>
                    Neste <i className="bi bi-chevron-right"></i>
                  </button>
                </div>

                {/* Calendar grid */}
                <div className="table-responsive">
                  <table className="table table-bordered text-center">
                    <thead>
                      <tr>
                        {dayNames.map(day => (
                          <th key={day} className="bg-light">{day}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody>
                      {Array.from({ length: Math.ceil(calendarDays.length / 7) }, (_, weekIdx) => (
                        <tr key={weekIdx}>
                          {calendarDays.slice(weekIdx * 7, (weekIdx + 1) * 7).map((date, dayIdx) => {
                            if (!date) {
                              return <td key={dayIdx} className="bg-light" style={{ minHeight: '60px' }}></td>
                            }

                            const dateStr = toDateString(date)
                            const isPast = date < today
                            const hasBooking = bookedDateSet.has(dateStr)
                            const isToday = date.toDateString() === today.toDateString()
                            const dayBookings = bookingsByDate.get(dateStr) || []

                            return (
                              <td
                                key={dayIdx}
                                className={`
                                  ${isPast ? 'bg-light text-muted' : ''}
                                  ${hasBooking ? 'bg-success-subtle' : ''}
                                  ${isToday ? 'border-primary border-2' : ''}
                                `}
                                style={{ minWidth: '80px', minHeight: '60px', verticalAlign: 'top', padding: '8px' }}
                              >
                                <div className="fw-bold mb-1">{date.getDate()}</div>
                                {dayBookings.length > 0 && (
                                  <div>
                                    {dayBookings.map((booking, idx) => (
                                      <div key={idx} className="small text-success" style={{ fontSize: '0.75rem' }}>
                                        {new Date(booking.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                                      </div>
                                    ))}
                                  </div>
                                )}
                              </td>
                            )
                          })}
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
