import { useEffect, useState, useMemo } from 'react'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'

export default function DashboardPage() {
  const { user } = useAuth()
  // State for dashboard data: userName, reminders, and bookings
  const [data, setData] = useState(null)

  // Calendar state
  const [calendarYear, setCalendarYear] = useState(new Date().getFullYear())
  const [calendarMonth, setCalendarMonth] = useState(new Date().getMonth()) // 0-indexed


  useEffect(() => {
    api.get('/user/dashboard')
      .then(res => setData(res.data))
      .catch(err => console.error('Failed to load dashboard:', err))
  }, [])

  const toDateString = (date) => {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

  const bookedDateSet = useMemo(() => {
    const bookings = data?.calendarBookings ?? []
    return new Set(bookings.map(b => {
      const d = new Date(b.dateTime)
      return toDateString(d)
    }))
  }, [data?.calendarBookings])

  // Map of bookings by date for calendar display

  const bookingsByDate = useMemo(() => {
    const bookings = data?.calendarBookings ?? []
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
  }, [data?.calendarBookings])

  // Navigate to previous month

  const goToPrevMonth = () => {
    if (calendarMonth === 0) {
      setCalendarMonth(11)
      setCalendarYear(y => y - 1)
    } else {
      setCalendarMonth(m => m - 1)
    }
  }

  // Navigate to next month

  const goToNextMonth = () => {
    if (calendarMonth === 11) {
      setCalendarMonth(0)
      setCalendarYear(y => y + 1)
    } else {
      setCalendarMonth(m => m + 1)
    }
  }

  // Generate calendar days for current month

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

  // Show loading spinner while data is being fetched
  if (!data) {
    return (
      <div className="container mt-5">
        <div className="loading-spinner-container">
          <div className="loading-spinner" role="status" aria-label="Laster inn data"></div>
          <p className="text-muted mt-3">Laster inn dine data...</p>
        </div>
      </div>
    )
  }

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
              <div className="card shadow-sm" role="region" aria-labelledby="today-bookings-heading">
                <div className="card-body">
                  <h2 id="today-bookings-heading" className="card-title fs-4 mb-3">
                    <i className="bi bi-calendar-day me-2" aria-hidden="true"></i>Dine timer i dag
                  </h2>
                  {(data?.todayBookings?.length > 0) ? (
                    <ul className="list-group" style={{ maxHeight: '280px', overflowY: 'auto' }}>
                      {data.todayBookings.map(booking => (
                        <li key={booking.id} className="list-group-item text-center py-3">
                          <div className="fw-bold fs-5 text-primary mb-1">
                            <i className="bi bi-clock me-2" aria-hidden="true"></i>
                            {new Date(booking.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                            {/* <div className="small text-secondary"> */}
                            &ensp;{booking.categoryName || 'N/A'}
                            {/* </div> */}
                          </div>

                          <div className="text-muted">
                            <i className="bi bi-person me-1" aria-hidden="true"></i>
                            {booking.caregiverName || 'Ikke tildelt'}
                          </div>

                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted mb-0 text-center">
                      <i className="bi bi-check-circle me-2" aria-hidden="true"></i>Ingen timer i dag.
                    </p>
                  )}
                  <div className="mt-3">
                    <a
                      href="/booking"
                      className="btn btn-primary w-100"
                      aria-label="Bestill ny time"
                    >
                      <i className="bi bi-calendar-plus me-2" aria-hidden="true"></i>Book ny time
                    </a>
                  </div>
                </div>
              </div>

              {/* Reminders Section (Påminnelser) */}
              <div className="card shadow-sm" role="region" aria-labelledby="reminders-heading">
                <div className="card-body">
                  <h2 id="reminders-heading" className="card-title fs-4 mb-3">
                    <i className="bi bi-bell me-2" aria-hidden="true"></i>Påminnelser
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
                      <i className="bi bi-check-circle me-2" aria-hidden="true"></i>Ingen påminnelser.
                    </p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* Right column - Calendar */}
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm" role="region" aria-labelledby="calendar-section-heading">
              <div className="card-body">
                <h2 id="calendar-section-heading" className="card-title fs-4 mb-3">
                  <i className="bi bi-calendar3 me-2" aria-hidden="true"></i>Kalender
                </h2>
                <p className="text-muted small mb-3">
                  Dager med bestillinger er markert med grønn farge.
                </p>

                {/* Calendar navigation */}
                <div className="d-flex justify-content-between align-items-center mb-3">
                  <button
                    className="btn btn-outline-secondary calendar-nav-btn"
                    onClick={goToPrevMonth}
                    aria-label={`Gå til forrige måned, ${monthNames[calendarMonth === 0 ? 11 : calendarMonth - 1]}`}
                  >
                    <i className="bi bi-chevron-left" aria-hidden="true"></i> <span className="d-none d-sm-inline">Forrige</span>
                  </button>
                  <h3 id="calendar-heading" className="mb-0 fs-5 fs-sm-4" aria-live="polite">{monthNames[calendarMonth]} {calendarYear}</h3>
                  <button
                    className="btn btn-outline-secondary calendar-nav-btn"
                    onClick={goToNextMonth}
                    aria-label={`Gå til neste måned, ${monthNames[calendarMonth === 11 ? 0 : calendarMonth + 1]}`}
                  >
                    <span className="d-none d-sm-inline">Neste</span> <i className="bi bi-chevron-right" aria-hidden="true"></i>
                  </button>
                </div>

                {/* Calendar grid */}
                <div className="table-responsive" style={{ overflowX: 'auto' }}>
                  <table className="table table-bordered text-center mb-0" style={{ tableLayout: 'fixed', width: '100%' }} aria-labelledby="calendar-heading">
                    <caption className="visually-hidden">
                      Kalender for {monthNames[calendarMonth]} {calendarYear}. Dager med bestillinger er markert.
                    </caption>
                    <thead>
                      <tr>
                        {dayNames.map(day => (
                          <th key={day} className="bg-light p-1 p-sm-2" scope="col" style={{ width: '14.28%' }}>{day}</th>
                        ))}
                      </tr>
                    </thead>
                    <tbody>
                      {Array.from({ length: Math.ceil(calendarDays.length / 7) }, (_, weekIdx) => (
                        <tr key={weekIdx}>
                          {calendarDays.slice(weekIdx * 7, (weekIdx + 1) * 7).map((date, dayIdx) => {
                            if (!date) {
                              return <td key={dayIdx} className="bg-light p-1 p-sm-2"></td>
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
                                  p-1 p-sm-2
                                  ${isPast ? 'bg-light text-muted' : ''}
                                  ${hasBooking ? 'bg-success-subtle' : ''}
                                  ${isToday ? 'border-primary border-2' : ''}
                                `}
                                style={{ verticalAlign: 'top' }}
                              >
                                <div className="fw-bold mb-1" style={{ fontSize: '0.9rem' }}>{date.getDate()}</div>
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
