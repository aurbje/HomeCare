

import { useEffect, useState, useMemo } from 'react'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'


 // DashboardPage Component (User/Client Dashboard)
 // Provides an overview of appointments and reminders.
export default function DashboardPage() {
  const { user } = useAuth()

  // Dashboard data from backend (userName, reminders[], bookings[])
  const [data, setData] = useState(null)

  // Calendar date state
  const [calendarYear, setCalendarYear] = useState(new Date().getFullYear())
  const [calendarMonth, setCalendarMonth] = useState(new Date().getMonth()) 

  
  // Fetch client dashboard data on mount.
  // Uses cookie-based auth (handled automatically by Axios instance).
  useEffect(() => {
    api.get('/user/dashboard')
      .then(res => setData(res.data))
      .catch(err => console.error('Failed to load dashboard:', err))
  }, [])

  // Convert a JS Date object → YYYY-MM-DD string (local timezone)
  const toDateString = (date) => {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  // Set of booked dates for calendar highlighting
  const bookedDateSet = useMemo(() => {
    const dates = data?.bookings ?? []
    return new Set(
      dates.map(b => toDateString(new Date(b.dateTime)))
    )
  }, [data?.bookings])

  // Group bookings by date for easy lookup inside the calendar
  const bookingsByDate = useMemo(() => {
    const map = new Map()
    const bookings = data?.bookings ?? []

    bookings.forEach(booking => {
      const dateStr = toDateString(new Date(booking.dateTime))
      if (!map.has(dateStr)) map.set(dateStr, [])
      map.get(dateStr).push(booking)
    })

    return map
  }, [data?.bookings])

  // Extract today's bookings to show under "Dine timer i dag"
  const todayBookings = useMemo(() => {
    const todayStr = toDateString(new Date())
    return bookingsByDate.get(todayStr) || []
  }, [bookingsByDate])

  
  // Calendar navigation: Previous month
  const goToPrevMonth = () => {
    if (calendarMonth === 0) {
      setCalendarMonth(11)
      setCalendarYear(y => y - 1)
    } else {
      setCalendarMonth(m => m - 1)
    }
  }

  //Calendar navigation: Next month
  const goToNextMonth = () => {
    if (calendarMonth === 11) {
      setCalendarMonth(0)
      setCalendarYear(y => y + 1)
    } else {
      setCalendarMonth(m => m + 1)
    }
  }

  // Generate all calendar slots (null represents leading empty days)
  
  const generateCalendarDays = () => {
    const firstDay = new Date(calendarYear, calendarMonth, 1)
    const lastDay = new Date(calendarYear, calendarMonth + 1, 0)

    // Calculate padding to make Monday the first column
    const startPadding = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1

    const days = []

    // Fill padding days
    for (let i = 0; i < startPadding; i++) {
      days.push(null)
    }

    // Fill calendar days
    for (let day = 1; day <= lastDay.getDate(); day++) {
      days.push(new Date(calendarYear, calendarMonth, day))
    }

    return days
  }

  // Show loading state until dashboard data is fetched
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
          <p className="lead text-muted mb-0">
            Du er logget inn som {data.userName || user?.fullName || 'Bruker'}
          </p>
        </div>

        <div className="row g-4">

          {/* Left column: Today's appointments + reminders */}
          <div className="col-12 col-lg-4">
            <div className="d-flex flex-column w-100" style={{ gap: '0.5rem' }}>

              {/* Today's bookings */}
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
                            {new Date(booking.dateTime).toLocaleTimeString('nb-NO', {
                              hour: '2-digit',
                              minute: '2-digit'
                            })}
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
                    <a href="/booking" className="btn btn-primary w-100">
                      <i className="bi bi-calendar-plus me-2"></i>Book ny time
                    </a>
                  </div>
                </div>
              </div>

              {/* Reminders */}
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

          {/* Right column: Calendar */}
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm">
              <div className="card-body">
                <h2 className="card-title fs-4 mb-3">
                  <i className="bi bi-calendar3 me-2"></i>Kalender
                </h2>
                <p className="text-muted small mb-3">
                  Dager med bestillinger er markert med grønn farge.
                </p>

                {/* Calendar controls */}
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
                          {calendarDays
                            .slice(weekIdx * 7, (weekIdx + 1) * 7)
                            .map((date, dayIdx) => {

                              // Empty cell padding
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
                                  style={{
                                    minWidth: '80px',
                                    minHeight: '60px',
                                    verticalAlign: 'top',
                                    padding: '8px'
                                  }}
                                >
                                  <div className="fw-bold mb-1">{date.getDate()}</div>

                                  {/* Show booking times inside cell */}
                                  {dayBookings.length > 0 && (
                                    <div>
                                      {dayBookings.map((booking, idx) => (
                                        <div
                                          key={idx}
                                          className="small text-success"
                                          style={{ fontSize: '0.75rem' }}
                                        >
                                          {new Date(booking.dateTime).toLocaleTimeString('nb-NO', {
                                            hour: '2-digit',
                                            minute: '2-digit'
                                          })}
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
