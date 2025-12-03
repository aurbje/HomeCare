import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { useAuth } from '../../hooks/useAuth'
import api from '../../api/api'

export default function BookingPage() {
  const navigate = useNavigate()
  const { user, isAuthenticated } = useAuth()

  const [data, setData] = useState(null)
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

  useEffect(() => {
    const role = user?.role?.toLowerCase()
    const isClient = role === 'user' || role === 'client'
    if (!isAuthenticated || !isClient) {
      navigate('/login')
      return
    }
    api.get('/booking/init')
      .then(res => {
        setData(res.data)
        setCategoryId(res.data.model?.categoryId || 0)
      })
      .catch(() => setError('Kunne ikke laste booking data.'))
      .finally(() => setLoading(false))
  }, [isAuthenticated, user?.role, navigate])

  useEffect(() => {
    if (!selectedDateId || !data) return
    const availDate = data.model?.availableDates?.find(d => d.id === selectedDateId)
    if (!availDate) return
    const filterTimeSlots = async () => {
      const filtered = []
      for (const slot of availDate.timeSlots) {
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
  }, [selectedDateId, data, bookingId])

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
        api.get('/booking/init')
          .then(res => setData(res.data))
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
    if (!data) return
    const availDate = data.model?.availableDates?.find(d => d.date === dateStr)
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
    if (!confirm('Er du sikker på at du vil avbestille denne timen?')) return
    try {
      await api.delete(`/booking/${id}`)
      setSuccess('Booking avbestilt.')
      setEditingBookingId(null)
      setTimeout(() => {
        api.get('/booking/init')
          .then(res => setData(res.data))
        setSuccess('')
      }, 1000)
    } catch {
      setError('Kunne ikke avbestille.')
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
    return <div className="container mt-5">Laster...</div>
  }

  if (!data) {
    return <div className="container mt-5">Ingen data tilgjengelig.</div>
  }

  const selectedCategory = data.model?.categories?.find(c => c.id === categoryId)
  const requireNotes = selectedCategory?.name?.toUpperCase() === 'ANNET'

  return (
    <div className="font-resizable-area user-dashboard">
      <div className="container py-4">
        {data.clientName && (
          <div className="text-center mb-4">
            <h1>Bestill time, {data.clientName}</h1>
          </div>
        )}
        {success && <div className="alert alert-success">{success}</div>}
        {error && <div className="alert alert-danger">{error}</div>}
        <div className="row g-4">
          <div className="col-12 col-lg-6">
            <section aria-labelledby="booking-heading">
              <div className="card border-start border-4 border-success">
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
                        {data.model?.availableDates?.length > 0 ? (
                          data.model.availableDates.map(d => (
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
                        {data.model?.categories?.map(c => (
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
              <div className="card border-start border-4 border-info">
                <div className="card-body">
                  <h2 id="bookings-heading" className="fs-4 mb-3">Dine timer</h2>
                  {data.bookings && data.bookings.length > 0 ? (
                    <div className="list-group">
                      {data.bookings.map(b => (
                        <div
                          key={b.id}
                          className={`list-group-item ${editingBookingId === b.id ? 'list-group-item-warning border-warning border-2' : ''}`}
                        >
                          {editingBookingId === b.id && (
                            <span className="badge bg-warning text-dark mb-2">Redigerer</span>
                          )}
                          <div className="d-flex justify-content-between align-items-start">
                            <div>
                              <h6 className="mb-1">
                                {new Date(b.dateTime).toLocaleDateString('nb-NO', {
                                  weekday: 'long',
                                  day: 'numeric',
                                  month: 'long',
                                  year: 'numeric'
                                })}
                              </h6>
                              <p className="mb-1">
                                <strong>Tid:</strong> {new Date(b.dateTime).toLocaleTimeString('nb-NO', { hour: '2-digit', minute: '2-digit' })}
                              </p>
                              <p className="mb-1">
                                <strong>Kategori:</strong> {b.category?.name || 'N/A'}
                              </p>
                              <p className="mb-1">
                                <strong>Ansatt:</strong> {b.caregiver?.fullName || 'N/A'}
                              </p>
                              {b.notes && (
                                <p className="mb-0 text-muted">
                                  <small>Notater: {b.notes}</small>
                                </p>
                              )}
                            </div>
                          </div>
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