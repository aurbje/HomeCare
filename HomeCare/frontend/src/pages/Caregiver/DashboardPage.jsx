/**
 * DashboardPage.jsx - Caregiver Dashboard
 *
 * Auth: Uses context/AuthContext.jsx (group's pattern)
 * Backend endpoint: GET /api/caregiver/dashboard (CaregiverController.GetDashboard)
 */

import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'

/**
 * CaregiverDashboardPage Component
 * Dashboard for caregivers to manage availability and view bookings
 * Compatible with group's cookie-based authentication
 * Features:
 * - View today's visits with client details
 * - View all registered available days
 * - Interactive calendar to select/register available days
 * - Batch registration of multiple available days
 * - Delete individual available days
 * - View upcoming bookings
 * - Font resizable area for accessibility
 */
export default function CaregiverDashboardPage() {
  const navigate = useNavigate()
  const { isAuthenticated, user } = useAuth()

  // Dashboard data state
  const [data, setData] = useState(null)
  const [error, setError] = useState(null)
  const [success, setSuccess] = useState(null)
  const [loading, setLoading] = useState(true)
  const [submitting, setSubmitting] = useState(false)

  // No data state
  // if (!data) return <div className="container mt-5 alert alert-danger">{error || 'Ingen data tilgjengelig.'}</div>

  // Calendar state
  const [calendarYear, setCalendarYear] = useState(new Date().getFullYear())
  const [calendarMonth, setCalendarMonth] = useState(new Date().getMonth()) // 0-indexed
  const [selectedDates, setSelectedDates] = useState(new Set())

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
   * Fetch dashboard data from backend using cookie-based auth
   */
  const fetchDashboard = useCallback(async (year, month) => {
    try {
      setLoading(true)
      setError(null)
      setSuccess(null)
      const params = new URLSearchParams()
      if (year !== undefined) params.set('year', String(year))
      if (month !== undefined) params.set('month', String(month + 1)) // API expects 1-indexed
      const url = `/caregiver/dashboard${params.toString() ? `?${params}` : ''}`

      const response = await api.get(url)
      setData(response.data)
    } catch (e) {
      if (e.response?.status === 401) {
        navigate('/login')
        return
      }
      // Log error for debugging
      console.error('Dashboard fetch error:', e)
      // Only set error if it's a real server error
      if (e.response?.status >= 400) {
        const errorMsg = e.response?.data?.message || e.response?.data?.error || 'En feil oppstod ved lasting av data.'
        setError(errorMsg)
      }
    } finally {
      setLoading(false)
    }
  }, [navigate])

  /**
   * Check authentication and fetch data on mount
   */
  useEffect(() => {
    if (!isAuthenticated) {
      navigate('/login', { replace: true })
      return
    }

    const role = user?.role?.toLowerCase()

    // Wait for user data to load if role is not yet available
    if (!role) {
      return
    }

    if (role !== 'caregiver' && role !== 'admin') {
      // Wrong role - redirect to appropriate dashboard
      navigate('/dashboard', { replace: true })
      return
    }

    fetchDashboard(calendarYear, calendarMonth)
  }, [calendarYear, calendarMonth, fetchDashboard, isAuthenticated, user?.role, navigate])

  /**
   * Set of available date strings for quick lookup
   */
  const availableDateSet = useMemo(() => {
    const dates = data?.availableDates ?? []
    return new Set(dates.map(d => toDateString(new Date(d))))
  }, [data?.availableDates])

  /**
   * Sorted list of available dates for display
   */
  const availableDates = useMemo(() => {
    return (data?.availableDates ?? [])
      .map(d => new Date(d))
      .sort((a, b) => a.getTime() - b.getTime())
  }, [data?.availableDates])

  /**
   * Map of calendar events by date for quick lookup
   */
  const eventsByDate = useMemo(() => {
    const events = data?.model?.calendarEvents ?? []
    const map = new Map()
    events.forEach(event => {
      const dateStr = toDateString(new Date(event.startTime))
      if (!map.has(dateStr)) {
        map.set(dateStr, [])
      }
      map.get(dateStr).push(event)
    })
    return map
  }, [data?.model?.calendarEvents])

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
   * Handle checkbox change for selecting dates
   */
  const handleCheckboxChange = (dateStr, checked) => {
    setSelectedDates(prev => {
      const next = new Set(prev)
      if (checked) {
        next.add(dateStr)
      } else {
        next.delete(dateStr)
      }
      return next
    })
  }

  /**
   * Register multiple selected dates as available
   */
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

      // Refresh data without clearing success message yet
      const params = new URLSearchParams()
      params.set('year', String(calendarYear))
      params.set('month', String(calendarMonth + 1))
      const response = await api.get(`/caregiver/dashboard?${params}`)
      setData(response.data)

      // Auto-clear success message after 3 seconds
      setTimeout(() => setSuccess(null), 3000)
    } catch (e) {
      setError(e.message || 'En feil oppstod.')
    } finally {
      setSubmitting(false)
    }
  }

  /**
   * Delete a single available day
   */
  const handleDeleteAvailability = async (date) => {
    if (!confirm('Er du sikker på at du vil slette denne dagen?')) return

    setError(null)
    setSuccess(null)

    const dateStr = toDateString(date)

    try {
      const response = await fetch(
        `/api/caregiver/availability/request-deletion?caregiverId=${data?.caregiverId}&date=${dateStr}`,
        {
          method: 'POST',
          credentials: 'include'
        }
      )

      if (response.status === 409) {
        const errorData = await response.json().catch(() => ({}))
        setError(errorData.message || 'Kan ikke slette dag med bookinger.')
        window.scrollTo({ top: 0, behavior: 'smooth' })
      } else if (!response.ok) {
        throw new Error('Kunne ikke slette dagen.')
      } else {
        setSuccess('Dag slettet.')
        await fetchDashboard(calendarYear, calendarMonth)
      }
    } catch (e) {
      setError(e.message || 'En feil oppstod.')
      window.scrollTo({ top: 0, behavior: 'smooth' })
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

  // Loading state
  if (loading) return <div className="container mt-5">Laster...</div>

  const model = data?.model
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
          <h1 className="display-6 fw-bold text-success mb-1">Min arbeidsplan</h1>
          <p className="lead text-muted mb-0">Du er logget inn som {model?.caregiverName || ''}.</p>
        </div>

        {/* Alert messages */}
        {error && <div className="alert alert-danger">{error}</div>}
        {success && <div className="alert alert-success">{success}</div>}

        <div className="row g-4">
          {/* Left column - Info cards */}
          <div className="col-12 col-lg-4">
            <div className="d-flex flex-column w-100" style={{ gap: '0.5rem' }}>

              {/* Today's visits */}
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 className="card-title fs-4 mb-3">
                    <i className="bi bi-calendar-day me-2"></i>Dagens besøk
                  </h2>
                  {model?.todayVisits && model.todayVisits.length > 0 ? (
                    <ul className="list-group">
                      {model.todayVisits.map((visit, idx) => (
                        <li key={idx} className="list-group-item">
                          <div className="fw-bold">{visit.clientName}</div>
                          <div className="small text-muted">
                            <i className="bi bi-clock me-1"></i>
                            {new Date(visit.time).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                          </div>
                          <div className="small">
                            <i className="bi bi-geo-alt me-1"></i>{visit.address}
                          </div>
                          <div className="small">
                            <i className="bi bi-telephone me-1"></i>{visit.phone}
                          </div>
                          {visit.tasks && visit.tasks.length > 0 && (
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

              {/* Available dates section */}
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
                              weekday: 'long',
                              day: '2-digit',
                              month: 'long'
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

          {/* Right column - Calendar */}
          <div className="col-12 col-lg-8">
            <div className="card shadow-sm">
              <div className="card-body">
                <h2 className="card-title fs-4 mb-3">
                  <i className="bi bi-calendar3 me-2"></i>Kalender
                </h2>
                <p className="text-muted small mb-3">
                  Kryss av dager du er tilgjengelig og trykk på knappen for å registrere.
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
                              return <td key={dayIdx} className="bg-light"></td>
                            }

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

                                {/* Display bookings for this day */}
                                {dayEvents.length > 0 && (
                                  <div className="mb-2">
                                    {dayEvents.map((event, eventIdx) => (
                                      <div
                                        key={eventIdx}
                                        className="small text-dark"
                                        style={{ fontSize: '0.75rem' }}
                                      >
                                        <span className="fw-bold">
                                          {new Date(event.startTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                                        </span>
                                        <span className="ms-1">{event.clientName}</span>
                                      </div>
                                    ))}
                                  </div>
                                )}

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

                {/* Submit selected dates */}
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
