import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../context/AuthContext'
import api from '../../api/api'

export default function BookingPage() {
  const navigate = useNavigate()
  const { user, isAuthenticated } = useAuth()

  // Core data states loaded from the API
  const [availableDates, setAvailableDates] = useState([])
  const [categories, setCategories] = useState([])
  const [bookings, setBookings] = useState([])
  const [clientName, setClientName] = useState('')

  // Form selection states
  const [selectedDateId, setSelectedDateId] = useState(null)
  const [selectedDate, setSelectedDate] = useState('')
  const [selectedTimeSlotId, setSelectedTimeSlotId] = useState(null)
  const [availableTimeSlots, setAvailableTimeSlots] = useState([])
  const [caregivers, setCaregivers] = useState([])
  const [selectedCaregiverId, setSelectedCaregiverId] = useState(null)
  const [categoryId, setCategoryId] = useState(0)
  const [notes, setNotes] = useState('')

  // ID used when editing an existing booking
  const [bookingId, setBookingId] = useState(0)
  const [editingBookingId, setEditingBookingId] = useState(null)

  // UI state
  const [success, setSuccess] = useState('')
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(true)

  // Fetch initial booking data (dates, categories, existing bookings)
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

  // Redirect users without correct role and load data after authentication
  useEffect(() => {
    const role = user?.role?.toLowerCase()
    const isClient = role === 'user' || role === 'client'

    // Force login if user is not a client
    if (!isAuthenticated || !isClient) {
      navigate('/login')
      return
    }

    loadBookingData()
  }, [isAuthenticated, user?.role, navigate])

  // Load available timeslots for the chosen date and filter out those with no available caregivers
  useEffect(() => {
    if (!selectedDateId) return

    const availDate = availableDates.find(d => d.id === selectedDateId)
    if (!availDate) return

    const filterTimeSlots = async () => {
      const filtered = []

      // Each timeslot is checked against API to verify caregiver availability
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

          // Only include timeslots where at least one caregiver is free
          if (Array.isArray(availableCaregivers) && availableCaregivers.length > 0) {
            filtered.push(slot)
          }
        } catch {
          // Ignore API errors for individual slots
        }
      }

      setAvailableTimeSlots(filtered)
      setSelectedTimeSlotId(null)
      setCaregivers([])
      setSelectedCaregiverId(null)
    }

    filterTimeSlots()
  }, [selectedDateId, availableDates, bookingId])

  // Fetch caregivers once both date and timeslot have been selected
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

  // Handle date selection
  const handleDateChange = (dateId, dateStr) => {
    setSelectedDateId(dateId)
    setSelectedDate(dateStr)
  }

  // Handle timeslot selection
  const handleTimeSlotChange = (slotId) => {
    setSelectedTimeSlotId(slotId)
  }

  // Handle booking creation or update
  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    setSuccess('')

    // Basic validation
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

      // Reset form and reload data after success
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

  // Populate form fields with existing booking for editing
  const handleEdit = async (booking) => {
    setEditingBookingId(booking.id)
    setBookingId(booking.id)
    setNotes(booking.notes || '')
    setCategoryId(booking.category?.id || 0)

    // Convert datetime to date-only format
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

    // Scroll to top for visibility
    window.scrollTo({ top: 0, behavior: 'smooth' })
  }

  // Cancel a booking entirely
  const handleCancel = async (id) => {
    if (!confirm('Er du sikker på at du vil avbestille denne timen?')) return

    try {
      await api.delete(`/booking/${id}`)
      setSuccess('Booking avbestilt.')
      setEditingBookingId(null)

      setTimeout(() => {
        loadBookingData()
        setSuccess('')
      }, 1000)
    } catch {
      setError('Kunne ikke avbestille.')
    }
  }

  // Exit edit mode and reset the form
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
    return <div className="container mt-5">Laster...</div>
  }

  // Determine if notes are required for the selected category
  const selectedCategory = categories.find(c => c.id === categoryId)
  const requireNotes = selectedCategory?.name?.toUpperCase() === 'ANNET'

  return (
    <div className="font-resizable-area user-dashboard">
      <div className="container py-4">

        {/* Page title */}
        {clientName && (
          <div className="text-center mb-4">
            <h1>Bestill time</h1>
          </div>
        )}

        {/* Feedback messages */}
        {success && <div className="alert alert-success">{success}</div>}
        {error && <div className="alert alert-danger">{error}</div>}

        <div className="row g-4">

          {/* Left column: Booking form */}
          <div className="col-12 col-lg-6">
            <section aria-labelledby="booking-heading">
              <div className="card shadow-sm">
                <div className="card-body">

                  <h2 id="booking-heading" className="fs-4 mb-3">Bestill her</h2>

                  {/* Edit mode indicator */}
                  {editingBookingId && (
                    <div className="alert alert-info d-flex justify-content-between align-items-center">
                      <span><i className="bi bi-pencil me-2"></i> Redigerer booking</span>
                      <button type="button" className="btn btn-sm btn-outline-secondary" onClick={handleCancelEdit}>
                        Avbryt redigering
                      </button>
                    </div>
                  )}

                  {/* Booking form */}
                  <form onSubmit={handleSubmit}>

                    {/* Date selection */}
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

                    {/* Timeslot selection */}
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

                    {/* Caregiver selection */}
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

                    {/* Category selection */}
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

                    {/* Notes field */}
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

                    {/* Submit button */}
                    <button type="submit" className="btn btn-success w-100 btn-lg">
                      {editingBookingId ? 'Oppdater booking' : 'Bestill time'}
                    </button>

                  </form>
                </div>
              </div>
            </section>
          </div>

          {/* Right column: User's bookings list */}
          <div className="col-12 col-lg-6">
            <section aria-labelledby="bookings-heading">
              <div className="card shadow-sm">
                <div className="card-body">

                  <h2 id="bookings-heading" className="fs-4 mb-3">Dine timer</h2>

                  {bookings.length > 0 ? (
                    <div className="list-group">

                      {/* Booking items */}
                      {bookings.map(b => (
                        <div
                          key={b.id}
                          className={`list-group-item ${editingBookingId === b.id ? 'list-group-item-warning border-warning border-2' : ''}`}
                        >

                          {/* Edit badge indicator */}
                          {editingBookingId === b.id && (
                            <span className="badge bg-warning text-dark mb-2">Redigerer</span>
                          )}

                          <div className="d-flex justify-content-between align-items-start">
                            <div>
                              {/* Date display */}
                              <h6 className="mb-1">
                                {new Date(b.dateTime).toLocaleDateString('nb-NO', {
                                  weekday: 'long',
                                  day: 'numeric',
                                  month: 'long',
                                  year: 'numeric'
                                })}
                              </h6>

                              {/* Time */}
                              <p className="mb-1">
                                <strong>Tid:</strong> {new Date(b.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                              </p>

                              {/* Category */}
                              <p className="mb-1">
                                <strong>Kategori:</strong> {b.category?.name || 'N/A'}
                              </p>

                              {/* Caregiver */}
                              <p className="mb-1">
                                <strong>Ansatt:</strong> {b.caregiver?.fullName || 'N/A'}
                              </p>

                              {/* Notes */}
                              {b.notes && (
                                <p className="mb-0 text-muted">
                                  <small>Notater: {b.notes}</small>
                                </p>
                              )}
                            </div>
                          </div>

                          {/* Action buttons */}
                          <div className="mt-2 d-flex gap-2">
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
