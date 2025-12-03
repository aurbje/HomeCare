/**
 * BookingPage.jsx - Advanced Booking Page with Caregiver Selection
 *
 * This is YOUR advanced implementation (kept instead of group's simpler version)
 * Features that group's version doesn't have:
 * - Caregiver selection (select specific caregiver for booking)
 * - Time slot filtering based on caregiver availability
 * - Edit existing bookings
 *
 * Backend endpoints used:
 * - GET /api/booking/init (BookingController.GetBookingPage)
 * - GET /api/booking/select-caregiver (BookingController.GetAvailableCaregivers)
 * - POST /api/booking (BookingController.CreateOrUpdateBooking)
 * - DELETE /api/booking/{id} (BookingController.CancelBooking)
 *
 * Auth: Uses context/AuthContext.jsx (group's pattern)
 */

import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'

export default function BookingPage() {
  const navigate = useNavigate()
  const { user, isAuthenticated } = useAuth()

  // Main data states - initialized as empty for null safety
  const [availableDates, setAvailableDates] = useState([])
  const [categories, setCategories] = useState([])
  const [bookings, setBookings] = useState([])
  const [clientName, setClientName] = useState('')

  // Selection states
  const [selectedDateId, setSelectedDateId] = useState(null)
  const [selectedDate, setSelectedDate] = useState('')
  const [selectedTimeSlotId, setSelectedTimeSlotId] = useState(null)
  const [availableTimeSlots, setAvailableTimeSlots] = useState([])
  const [caregivers, setCaregivers] = useState([])
  const [selectedCaregiverId, setSelectedCaregiverId] = useState(null)
  const [categoryId, setCategoryId] = useState(0)
  const [notes, setNotes] = useState('')
  const [bookingId, setBookingId] = useState(0)
  const [editingBookingId, setEditingBookingId] = useState(null)
  const [success, setSuccess] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  // Load booking data from API
  const loadBookingData = async () => {
    try {
      const res = await api.get('/booking/init')
      const data = res.data
      setAvailableDates(data?.model?.availableDates ?? [])
      setCategories(data?.model?.categories ?? [])
      setBookings(data?.bookings ?? [])
      setClientName(data?.clientName ?? '')
      setCategoryId(data?.model?.categoryId || 0)
    } catch {
      setError('Kunne ikke laste booking data.')
    } finally {
      setLoading(false)
    }
  }

  useEffect(() => {
    const role = user?.role?.toLowerCase()
    const isClient = role === 'user' || role === 'client'
    if (!isAuthenticated || !isClient) {
      navigate('/login')
      return
    }
    loadBookingData()
  }, [isAuthenticated, user?.role, navigate])

  useEffect(() => {
    if (!selectedDateId) return
    const availDate = availableDates.find(d => d.id === selectedDateId)
    if (!availDate) return
    const filterTimeSlots = async () => {
      const filtered = []
      for (const slot of (availDate.timeSlots ?? [])) {
        if (slot.isBooked) continue
        const params = new URLSearchParams({
          selectedDate: availDate.date,
          timeSlotId: slot.id.toString(),
          ...(bookingId > 0 && { bookingId: bookingId.toString() })
        })
        try {
          const res = await api.get(`/booking/select-caregiver?${params}`)
          const availableCaregivers = res.data
          if (Array.isArray(availableCaregivers) && availableCaregivers.length > 0) {
            filtered.push(slot)
          }
        } catch { /* skip */ }
      }
      setAvailableTimeSlots(filtered)
      setSelectedTimeSlotId(null)
      setCaregivers([])
      setSelectedCaregiverId(null)
    }
    filterTimeSlots()
  }, [selectedDateId, availableDates, bookingId])

  useEffect(() => {
    if (!selectedDate || !selectedTimeSlotId) {
      setCaregivers([])
      setSelectedCaregiverId(null)
      return
    }
    const params = new URLSearchParams({
      selectedDate,
      timeSlotId: selectedTimeSlotId.toString(),
      ...(bookingId > 0 && { bookingId: bookingId.toString() })
    })
    api.get(`/booking/select-caregiver?${params}`)
      .then(res => setCaregivers(res.data))
      .catch(() => setCaregivers([]))
  }, [selectedDate, selectedTimeSlotId, bookingId])

  const handleDateChange = (dateId, dateStr) => {
    setSelectedDateId(dateId)
    setSelectedDate(dateStr)
  }

  const handleTimeSlotChange = (slotId) => {
    setSelectedTimeSlotId(slotId)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess('')
    if (!selectedDate || !selectedTimeSlotId || !selectedCaregiverId || !categoryId) {
      setError('Vennligst fyll ut alle påkrevde felt.')
      return
    }
    const payload = {
      selectedDate,
      timeSlotId: selectedTimeSlotId,
      categoryId,
      notes,
      selectedCaregiverId,
      bookingId
    }
    try {
      await api.post('/booking', payload)
      setSuccess('Booking vellykket!')
      setTimeout(() => {
        setSelectedDateId(null)
        setSelectedDate('')
        setAvailableTimeSlots([])
        setSelectedTimeSlotId(null)
        setCaregivers([])
        setSelectedCaregiverId(null)
        setNotes('')
        setBookingId(0)
        setEditingBookingId(null)
        setSuccess('')
        loadBookingData()
      }, 1500)
    } catch (err) {
      const messages = Object.values(err.response?.data?.errors || {}).flat().join(' ')
      setError(messages || 'Booking feilet.')
    }
  }

  const handleEdit = async (booking) => {
    setEditingBookingId(booking.id)
    setBookingId(booking.id)
    setNotes(booking.notes || '')
    setCategoryId(booking.category?.id || 0)
    const dateStr = new Date(booking.dateTime).toISOString().split('T')[0]
    const availDate = availableDates.find(d => d.date === dateStr)
    if (availDate) {
      setSelectedDateId(availDate.id)
      setSelectedDate(dateStr)
      setSelectedTimeSlotId(booking.timeSlotId)
      const params = new URLSearchParams({
        selectedDate: dateStr,
        timeSlotId: booking.timeSlotId.toString(),
        bookingId: booking.id.toString()
      })
      api.get(`/booking/select-caregiver?${params}`)
        .then(res => {
          setCaregivers(res.data)
          setSelectedCaregiverId(booking.caregiver?.id || null)
        })
        .catch(() => setCaregivers([]))
    }
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  const handleCancel = async (id) => {
    // Find booking details for confirmation message
    const booking = bookings.find(b => b.id === id)
    const bookingInfo = booking
      ? `${booking.category?.name || 'Tjeneste'} den ${new Date(booking.dateTime).toLocaleDateString('nb-NO', { weekday: 'long', day: 'numeric', month: 'long' })}`
      : 'denne timen'

    // Enhanced confirmation dialog
    if (!confirm(`Er du sikker på at du vil avbestille ${bookingInfo}?\n\nDenne handlingen kan ikke angres.`)) {
      return
    }

    try {
      await api.delete(`/booking/${id}`)
      setSuccess('Bestillingen er avbestilt.')
      setEditingBookingId(null)
      setTimeout(() => {
        loadBookingData()
        setSuccess('')
      }, 1500)
    } catch (err) {
      // User-friendly error messages
      if (err.response?.status === 404) {
        setError('Bestillingen finnes ikke lenger. Siden vil oppdateres.')
        loadBookingData()
      } else if (err.response?.status === 401) {
        setError('Du er ikke logget inn. Vennligst logg inn på nytt.')
      } else if (err.response?.status === 400) {
        setError('Kan ikke avbestille denne timen. Kontakt oss for hjelp.')
      } else {
        setError('Noe gikk galt. Prøv igjen senere eller kontakt support.')
      }
    }
  }

  const handleCancelEdit = () => {
    setEditingBookingId(null)
    setBookingId(0)
    setSelectedDateId(null)
    setSelectedDate('')
    setAvailableTimeSlots([])
    setSelectedTimeSlotId(null)
    setCaregivers([])
    setSelectedCaregiverId(null)
    setNotes('')
    setCategoryId(0)
  }

  if (loading) {
    return (
      <div className="container mt-5">
        <div className="loading-spinner-container">
          <div className="loading-spinner" role="status" aria-label="Laster inn data"></div>
          <p className="text-muted mt-3">Laster inn booking data...</p>
        </div>
      </div>
    )
  }

  const selectedCategory = categories.find(c => c.id === categoryId)
  const requireNotes = selectedCategory?.name?.toUpperCase() === 'ANNET'

  return (
    <div className="font-resizable-area user-dashboard booking-page">
      <div className="container py-4">
        {/* Back to Dashboard button */}
        <div className="mb-3">
          <button
            type="button"
            className="btn btn-outline-secondary"
            onClick={() => navigate('/dashboard')}
            aria-label="Gå tilbake til oversikt"
          >
            <i className="bi bi-arrow-left me-2" aria-hidden="true"></i>Tilbake til oversikt
          </button>
        </div>

        {clientName && (
          <div className="text-center mb-4">
            <h1>Bestill time</h1>
          </div>
        )}
        {success && (
          <div className="alert alert-success" role="status" aria-live="polite">
            <i className="bi bi-check-circle-fill me-2 alert-icon" aria-hidden="true"></i>
            {success}
          </div>
        )}
        {error && (
          <div className="alert alert-danger" role="alert" aria-live="assertive">
            <i className="bi bi-exclamation-triangle-fill me-2 alert-icon" aria-hidden="true"></i>
            {error}
          </div>
        )}
        <div className="row g-4">
          <div className="col-12 col-lg-6">
            <section aria-labelledby="booking-heading">
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 id="booking-heading" className="fs-4 mb-3">Bestill her</h2>
                  {editingBookingId && (
                    <div className="alert alert-info d-flex justify-content-between align-items-center">
                      <span><i className="bi bi-pencil me-2"></i> Redigerer booking</span>
                      <button type="button" className="btn btn-sm btn-outline-secondary" onClick={handleCancelEdit}>
                        Avbryt redigering
                      </button>
                    </div>
                  )}
                  <form onSubmit={handleSubmit}>
                    <div className="form-group mb-3">
                      <label className="form-label fw-bold">Dato</label>
                      <div className="date-options">
                        {availableDates.length > 0 ? (
                          availableDates.map(d => (
                            <label
                              key={d.id}
                              className={`date-option btn btn-outline-secondary m-1 ${selectedDateId === d.id ? 'active btn-success text-white' : ''}`}
                            >
                              <input
                                type="radio"
                                name="selectedDate"
                                value={d.date}
                                checked={selectedDateId === d.id}
                                onChange={() => handleDateChange(d.id, d.date)}
                                style={{ display: 'none' }}
                              />
                              {new Date(d.date).toLocaleDateString('nb-NO', { month: 'short', day: 'numeric' })}
                            </label>
                          ))
                        ) : (
                          <p className="text-danger">Ingen tilgjengelige datoer.</p>
                        )}
                      </div>
                    </div>
                    <div className="form-group mb-3">
                      <label className="form-label fw-bold">Tidspunkt</label>
                      <div id="time-slot-wrapper" className="mt-2">
                        {availableTimeSlots.length > 0 ? (
                          <div className="btn-group-vertical w-100" role="group">
                            {availableTimeSlots.map(ts => (
                              <button
                                key={ts.id}
                                type="button"
                                className={`btn btn-outline-primary text-start ${selectedTimeSlotId === ts.id ? 'active' : ''}`}
                                onClick={() => handleTimeSlotChange(ts.id)}
                              >
                                {ts.slot}
                              </button>
                            ))}
                          </div>
                        ) : selectedDateId ? (
                          <p className="text-danger">Ingen tilgjengelige tidspunkter.</p>
                        ) : (
                          <p className="text-muted">Velg en dato først.</p>
                        )}
                      </div>
                    </div>
                    <div className="form-group mb-3">
                      <label className="form-label fw-bold">Velg ansatt</label>
                      <div id="caregiverContainer">
                        {caregivers.length > 0 ? (
                          <select
                            className="form-select"
                            value={selectedCaregiverId || ''}
                            onChange={e => setSelectedCaregiverId(Number(e.target.value))}
                            required
                          >
                            <option value="">-- Velg ansatt --</option>
                            {caregivers.map(p => (
                              <option key={p.id} value={p.id}>{p.fullName}</option>
                            ))}
                          </select>
                        ) : selectedTimeSlotId ? (
                          <p className="text-danger">Ingen tilgjengelige ansatte.</p>
                        ) : (
                          <p className="text-muted">Velg et tidspunkt først.</p>
                        )}
                      </div>
                    </div>
                    <div className="form-group mb-3">
                      <label htmlFor="categoryId" className="form-label fw-bold">Kategori</label>
                      <select
                        id="categoryId"
                        className="form-select"
                        value={categoryId}
                        onChange={e => setCategoryId(Number(e.target.value))}
                        required
                      >
                        <option value="">-- Velg kategori --</option>
                        {categories.map(c => (
                          <option key={c.id} value={c.id}>{c.name}</option>
                        ))}
                      </select>
                    </div>
                    <div className="form-group mb-3">
                      <label htmlFor="notes" className="form-label fw-bold">
                        Notater {requireNotes ? '(påkrevd)' : '(valgfritt)'}
                      </label>
                      <textarea
                        id="notes"
                        className="form-control"
                        rows={3}
                        value={notes}
                        onChange={e => setNotes(e.target.value)}
                        required={requireNotes}
                      />
                    </div>
                    <button type="submit" className="btn btn-success w-100 btn-lg">
                      {editingBookingId ? 'Oppdater booking' : 'Bestill time'}
                    </button>
                  </form>
                </div>
              </div>
            </section>
          </div>
          <div className="col-12 col-lg-6">
            <section aria-labelledby="bookings-heading">
              <div className="card shadow-sm">
                <div className="card-body">
                  <h2 id="bookings-heading" className="fs-4 mb-3">Dine timer</h2>
                  {bookings.length > 0 ? (
                    <div className="list-group">
                      {bookings.map(b => (
                        <div
                          key={b.id}
                          className={`list-group-item text-center py-3 ${editingBookingId === b.id ? 'list-group-item-warning border-warning border-2' : ''}`}
                        >
                          {editingBookingId === b.id && (
                            <span className="badge bg-warning text-dark mb-2">Redigerer</span>
                          )}
                          <div>
                            <h6 className="mb-2">
                              {new Date(b.dateTime).toLocaleDateString('nb-NO', {
                                weekday: 'long',
                                day: 'numeric',
                                month: 'long',
                                year: 'numeric'
                              })}
                            </h6>
                            <div className="fw-bold fs-5 text-primary mb-1">
                              <i className="bi bi-clock me-2"></i>
                              {new Date(b.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                            </div>
                            <div className="text-muted mb-1">
                              <i className="bi bi-person me-1"></i>
                              {b.caregiver?.fullName || 'Ikke tildelt'}
                            </div>
                            <div className="small text-secondary mb-1">
                              ({b.category?.name || 'N/A'})
                            </div>
                            {b.notes && (
                              <p className="mb-0 text-muted small">
                                <i className="bi bi-chat-left-text me-1"></i>{b.notes}
                              </p>
                            )}
                          </div>
                          <div className="mt-3 d-flex justify-content-center gap-2">
                            <button
                              className="btn btn-sm btn-outline-warning"
                              onClick={() => handleEdit(b)}
                              disabled={editingBookingId !== null && editingBookingId !== b.id}
                            >
                              <i className="bi bi-pencil me-1"></i> Rediger
                            </button>
                            <button
                              className="btn btn-sm btn-outline-danger"
                              onClick={() => handleCancel(b.id)}
                              disabled={editingBookingId !== null && editingBookingId !== b.id}
                            >
                              <i className="bi bi-trash me-1"></i> Avbestill
                            </button>
                          </div>
                        </div>
                      ))}
                    </div>
                  ) : (
                    <p className="text-muted">Ingen bookinger ennå.</p>
                  )}
                </div>
              </div>
            </section>
          </div>
        </div>
      </div>
    </div>
  )
}