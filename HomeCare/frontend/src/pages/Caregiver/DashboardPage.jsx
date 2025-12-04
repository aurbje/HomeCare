import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'


export default function CaregiverDashboardPage() {
  const navigate = useNavigate()
  const { isAuthenticated, user } = useAuth()

  const [data, setData] = useState(null)
  const [error, setError] = useState(null)
  const [success, setSuccess] = useState(null)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)

  // Calendar navigation and selections
  const [calendarYear, setCalendarYear] = useState(new Date().getFullYear())
  const [calendarMonth, setCalendarMonth] = useState(new Date().getMonth()) // 0-indexed month
  const [selectedDates, setSelectedDates] = useState(new Set())

  // Formats a Date into "YYYY-MM-DD" (backend-friendly)
   
  const toDateString = (date) => {
    const year = date.getFullYear()
    const month = String(date.getMonth() + 1).padStart(2, '0')
    const day = String(date.getDate()).padStart(2, '0')
    return `${year}-${month}-${day}`
  }

   // Loads entire caregiver dashboard state for a given month.
   // Includes: available dates, bookings, today's visits, calendar events.
  const fetchDashboard = useCallback(async (year, month) => {
    try {
      setLoading(true)
      setError(null)
      setSuccess(null)

      const params = new URLSearchParams()
      if (year !== undefined) params.set('year', String(year))
      if (month !== undefined) params.set('month', String(month + 1))

      const response = await api.get(`/caregiver/dashboard?${params}`)
      setData(response.data)
    } catch (e) {
      // Redirect unauthorized users
      if (e.response?.status === 401) {
        navigate('/login')
        return
      }

      console.error('Dashboard fetch error:', e)

      // Show only meaningful backend error messages
      if (e.response?.status >= 400) {
        setError(
          e.response?.data?.message ||
          e.response?.data?.error ||
          'En feil oppstod ved lasting av data.'
        )
      }
    } finally {
      setLoading(false)
    }
  }, [navigate])

   // Auth validation + initial dashboard fetch.
   // Redirects if user role is not caregiver or admin.
  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true })
      return
    }

    const role = user?.role?.toLowerCase()
    if (!role) return // wait for user state

    if (role !== 'caregiver' && role !== 'admin') {
      navigate('/dashboard', { replace: true })
      return
    }

    fetchDashboard(calendarYear, calendarMonth)
  }, [calendarYear, calendarMonth, fetchDashboard, isAuthenticated, user?.role, navigate])

  // Creates a Set of available date strings to speed up calendar lookup.
  const availableDateSet = useMemo(() => {
    const dates = data?.availableDates ?? []
    return new Set(dates.map(d => toDateString(new Date(d))))
  }, [data?.availableDates])

  // Sorted available dates for display in the sidebar.
  const availableDates = useMemo(() => {
    return (data?.availableDates ?? [])
      .map(d => new Date(d))
      .sort((a, b) => a.getTime() - b.getTime())
  }, [data?.availableDates])

  // Builds a map of date → calendar events (bookings) for fast rendering.
  const eventsByDate = useMemo(() => {
    const events = data?.model?.calendarEvents ?? []
    const map = new Map()

    events.forEach(event => {
      const dateStr = toDateString(new Date(event.startTime))
      if (!map.has(dateStr)) map.set(dateStr, [])
      map.get(dateStr).push(event)
    })

    return map
  }, [data?.model?.calendarEvents])

  //Calendar navigation handlers.
  const goToPrevMonth = () => {
    if (calendarMonth === 0) {
      setCalendarMonth(11)
      setCalendarYear(y => y - 1)
    } else {
      setCalendarMonth(m => m - 1)
    }
  }

  const goToNextMonth = () => {
    if (calendarMonth === 11) {
      setCalendarMonth(0)
      setCalendarYear(y => y + 1)
    } else {
      setCalendarMonth(m => m + 1)
    }
  }

  //Track which dates the user selects for batch registration.
  const handleCheckboxChange = (dateStr, checked) => {
    setSelectedDates(prev => {
      const next = new Set(prev)
      checked ? next.add(dateStr) : next.delete(dateStr)
      return next
    })
  }

  // Registers all selected dates as available in one batch request.

  const handleRegisterMultiple = async () => {
    if (selectedDates.size === 0) return

    setSubmitting(true)
    setError(null)
    setSuccess(null)

    try {
      const dates = Array.from(selectedDates)
      await api.post('/caregiver/availability/batch', dates)

      setSuccess(`${dates.length} dag(er) registrert som tilgjengelig.`)
      setSelectedDates(new Set())

      // Refresh dashboard after submitting
      const params = new URLSearchParams()
      params.set('year', String(calendarYear))
      params.set('month', String(calendarMonth + 1))

      const response = await api.get(`/caregiver/dashboard?${params}`)
      setData(response.data)

      setTimeout(() => setSuccess(null), 3000)
    } catch (e) {
      setError(e.message || 'En feil oppstod.')
    } finally {
      setSubmitting(false)
    }
  }

  // Deletes a single available date unless it contains bookings.
  // Backend returns 409 if deletion is blocked.
  const handleDeleteAvailability = async (date) => {
    if (!confirm('Er du sikker på at du vil slette denne dagen?')) return

    setError(null)
    setSuccess(null)

    const dateStr = toDateString(date)

    try {
      const response = await fetch(
        `/api/caregiver/availability/request-deletion?caregiverId=${data?.caregiverId}&date=${dateStr}`,
        { method: 'POST', credentials: 'include' }
      )

      // Backend prevents deletion when bookings exist
      if (response.status === 409) {
        const err = await response.json().catch(() => ({}))
        setError(err.message || 'Kan ikke slette dag med bookinger.')
        window.scrollTo({ top: 0, behavior: 'smooth' })
        return
      }

      if (!response.ok) throw new Error('Kunne ikke slette dagen.')

      setSuccess('Dag slettet.')
      await fetchDashboard(calendarYear, calendarMonth)
    } catch (e) {
      setError(e.message || 'En feil oppstod.')
      window.scrollTo({ top: 0, behavior: 'smooth' })
    }
  }

  // Builds the calendar structure, including placeholder cells at the start of the month.
  const generateCalendarDays = () => {
    const firstDay = new Date(calendarYear, calendarMonth, 1)
    const lastDay = new Date(calendarYear, calendarMonth + 1, 0)
    const startPadding = firstDay.getDay() === 0 ? 6 : firstDay.getDay() - 1

    const days = []

    for (let i = 0; i < startPadding; i++) days.push(null)
    for (let d = 1; d <= lastDay.getDate(); d++) {
      days.push(new Date(calendarYear, calendarMonth, d))
    }

    return days
  }

  // Loading UI
  if (loading) return <div className="container mt-5">Laster...</div>

  const model = data?.model
  const calendarDays = generateCalendarDays()

  const today = new Date()
  today.setHours(0, 0, 0, 0)

  // Localization lists
  const monthNames = ['Januar','Februar','Mars','April','Mai','Juni','Juli','August','September','Oktober','November','Desember']
  const dayNames = ['Man', 'Tir', 'Ons', 'Tor', 'Fre', 'Lør', 'Søn']

  return (
    <div className="font-resizable-area user-dashboard">
      <div className="container py-3">

        {/* Header section */}
        <div className="mb-3">
          <h1 className="display-6 fw-bold text-success mb-1">Min arbeidsplan</h1>
          <p className="lead text-muted mb-0">Du er logget inn som {model?.caregiverName || ''}.</p>
        </div>

        {/* Feedback messages */}
        {error && <div className="alert alert-danger">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <div className="row g-4">

          {/* LEFT COLUMN: Today’s visits + available days */}
          <div className="col-12 col-lg-4">
            <div className="d-flex flex-column w-100" style={{ gap: '0.5rem' }}>

              {/* Today's visits list */}
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 className="card-title fs-4 mb-3">
                    <i className="bi bi-calendar-day me-2"></i>Dagens besøk
                  </h2>

                  {model?.todayVisits?.length > 0 ? (
                    <ul className="list-group">
                      {model.todayVisits.map((visit, idx) => (
                        <li key={idx} className="list-group-item">
                          <div className="fw-bold">{visit.clientName}</div>

                          <div className="small text-muted">
                            <i className="bi bi-clock me-1"></i>
                            {new Date(visit.time).toLocaleTimeString('nb-NO', {
                              hour: '2-digit', minute: '2-digit'
                            })}
                          </div>

                          <div className="small">
                            <i className="bi bi-geo-alt me-1"></i>{visit.address}
                          </div>

                          <div className="small">
                            <i className="bi bi-telephone me-1"></i>{visit.phone}
                          </div>

                          {/* Optional task list */}
                          {visit.tasks?.length > 0 && (
                            <div className="small mt-1">
                              <strong>Oppgaver:</strong> {visit.tasks.join(', ')}
                            </div>
                          )}
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted mb-0">
                      <i className="bi bi-check-circle me-2"></i>Ingen besøk i dag.
                    </p>
                  )}
                </div>
              </div>

              {/* Available days sidebar */}
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 className="card-title fs-4 mb-3">
                    <i className="bi bi-calendar-check me-2"></i>Tilgjengelige dager
                  </h2>

                  {availableDates.length > 0 ? (
                    <ul className="list-group" style={{ maxHeight: '280px', overflowY: 'auto' }}>
                      {availableDates.map((date, idx) => (
                        <li key={idx} className="list-group-item d-flex justify-content-between align-items-center">
                          <span>
                            {date.toLocaleDateString('nb-NO', {
                              weekday: 'long', day: '2-digit', month: 'long'
                            })}
                          </span>

                          <button
                            type="button"
                            className="btn btn-sm btn-outline-danger"
                            onClick={() => handleDeleteAvailability(date)}
                          >
                            <i className="bi bi-trash me-1"></i> Slett
                          </button>
                        </li>
                      ))}
                    </ul>
                  ) : (
                    <p className="text-muted mb-0">Ingen tilgjengelige dager registrert.</p>
                  )}
                </div>
              </div>
            </div>
          </div>

          {/* RIGHT COLUMN: Calendar interface */}
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm">
              <div className="card-body">

                <h2 className="card-title fs-4 mb-3">
                  <i className="bi bi-calendar3 me-2"></i>Kalender
                </h2>
                <p className="text-muted small mb-3">
                  Kryss av dager du er tilgjengelig og trykk på knappen for å registrere.
                </p>

                {/* Calendar month navigation */}
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

                              if (!date) return <td key={dayIdx} className="bg-light"></td>

                              const dateStr = toDateString(date)
                              const isPast = date < today
                              const isAvailable = availableDateSet.has(dateStr)
                              const isSelected = selectedDates.has(dateStr)
                              const isToday = date.toDateString() === today.toDateString()
                              const dayEvents = eventsByDate.get(dateStr) || []

                              return (
                                <td
                                  key={dayIdx}
                                  className={`
                                    ${isPast ? 'bg-light text-muted' : ''}
                                    ${isAvailable ? 'bg-success-subtle' : ''}
                                    ${isToday ? 'border-primary border-2' : ''}
                                  `}
                                  style={{ minWidth: '100px', verticalAlign: 'top', padding: '8px' }}
                                >
                                  <div className="fw-bold mb-1">{date.getDate()}</div>

                                  {/* Render all bookings for this date */}
                                  {dayEvents.length > 0 && (
                                    <div className="mb-2">
                                      {dayEvents.map((event, idx) => (
                                        <div
                                          key={idx}
                                          className="small text-dark"
                                          style={{ fontSize: '0.75rem' }}
                                        >
                                          <span className="fw-bold">
                                            {new Date(event.startTime).toLocaleTimeString('nb-NO', {
                                              hour: '2-digit',
                                              minute: '2-digit'
                                            })}
                                          </span>
                                          <span className="ms-1">{event.clientName}</span>
                                        </div>
                                      ))}
                                    </div>
                                  )}

                                  {/* Availability controls for future dates */}
                                  {!isPast && (
                                    <div>
                                      {isAvailable ? (
                                        <button
                                          type="button"
                                          className="btn btn-sm btn-warning"
                                          title="Slett tilgjengelig dag"
                                          onClick={() => handleDeleteAvailability(date)}
                                        >
                                          Slett
                                        </button>
                                      ) : (
                                        <label title="Registrer som tilgjengelig">
                                          <input
                                            type="checkbox"
                                            checked={isSelected}
                                            onChange={e => handleCheckboxChange(dateStr, e.target.checked)}
                                            className="form-check-input"
                                            style={{ width: '18px', height: '18px' }}
                                          />
                                        </label>
                                      )}
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

                {/* Batch registration button */}
                <button
                  type="button"
                  className="btn btn-success btn-lg mt-3"
                  onClick={handleRegisterMultiple}
                  disabled={selectedDates.size === 0 || submitting}
                >
                  <i className="bi bi-check-circle me-2"></i>
                  {submitting
                    ? 'Registrerer...'
                    : `Registrer ${selectedDates.size} valgte dag(er)`}
                </button>

              </div>
            </div>
          </div>

        </div>
      </div>
    </div>
  )
}
