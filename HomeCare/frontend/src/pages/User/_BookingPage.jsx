import React, { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import {
  getBookingPage,
  createOrUpdateBooking,
  cancelBooking,
  getBookingForEdit,
} from "../../api/bookingApi";

export default function BookingPage() {
  const [loading, setLoading] = useState(true);
  const [availableDates, setAvailableDates] = useState([]);
  const [categories, setCategories] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);

  const [editingId, setEditingId] = useState(null);
  const [message, setMessage] = useState(null);

  const [form, setForm] = useState({
    appointmentId: null,
    selectedDate: "",
    timeSlotId: "",
    categoryId: "",
    notes: "",
  });

  // ----------------------------------------------------
  // LOAD BOOKING DATA FROM API
  // ----------------------------------------------------
  async function loadBookingPage() {
    try {
      setLoading(true);
      const data = await getBookingPage();

      setAvailableDates(data.availableDates);
      setCategories(data.categories);
      setAppointments(data.bookings);

      setLoading(false);
    } catch (err) {
      console.error("Feil ved henting av bookingdata:", err);
      setMessage("Kunne ikke laste bookingdata.");
      setLoading(false);
    }
  }

  useEffect(() => {
    loadBookingPage();
  }, []);

  // ----------------------------------------------------
  // HANDLE DATE SELECTION
  // ----------------------------------------------------
  const handleDateChange = (dateId, dateValue) => {
    setForm({
      ...form,
      selectedDate: dateValue,
      timeSlotId: "",
    });

    const date = availableDates.find((d) => d.id === dateId);
    setAvailableSlots(
      date?.timeSlots.filter((s) => s.isBooked === false) || []
    );
  };

  // ----------------------------------------------------
  // SUBMIT BOOKING (CREATE OR UPDATE)
  // ----------------------------------------------------
  const handleSubmit = async (e) => {
    e.preventDefault();

    try {
      const payload = {
        bookingId: form.appointmentId || 0,
        selectedDate: form.selectedDate,
        timeSlotId: Number(form.timeSlotId),
        categoryId: Number(form.categoryId),
        notes: form.notes,
      };

      const response = await createOrUpdateBooking(payload);

      setMessage(response.message || "Bestilling lagret!");

      // Reset form
      setForm({
        appointmentId: null,
        selectedDate: "",
        timeSlotId: "",
        categoryId: "",
        notes: "",
      });
      setAvailableSlots([]);
      setEditingId(null);

      // Refresh UI
      await loadBookingPage();
    } catch (err) {
      console.error("Booking-feil:", err);
      setMessage(err.message || "En feil oppstod under lagring.");
    }
  };

  // ----------------------------------------------------
  // EDIT BOOKING
  // ----------------------------------------------------
  const handleEdit = async (id) => {
    try {
      setEditingId(id);

      const data = await getBookingForEdit(id);

      setForm({
        appointmentId: data.bookingId,
        selectedDate: data.selectedDate,
        timeSlotId: data.timeSlotId,
        categoryId: data.categoryId,
        notes: data.notes || "",
      });

      setAvailableSlots(
        data.availableDates
          .find((d) => d.id === data.availableDates.find((x) => x.date === data.selectedDate)?.id)
          ?.timeSlots.filter((s) => s.isBooked === false) || []
      );
    } catch (err) {
      console.error("Feil ved lasting av booking:", err);
      setMessage("Kunne ikke laste booking for redigering.");
    }
  };

  // ----------------------------------------------------
  // CANCEL BOOKING
  // ----------------------------------------------------
  const handleCancel = async (id) => {
    if (!window.confirm("Vil du avlyse denne timen?")) return;

    try {
      await cancelBooking(id);
      setMessage("Timen ble kansellert.");
      await loadBookingPage();
    } catch (err) {
      console.error("Avbestilling feilet:", err);
      setMessage("Kunne ikke kansellere timen.");
    }
  };

  // ----------------------------------------------------
  // UI RENDER
  // ----------------------------------------------------
  if (loading) return <p>Laster booking...</p>;

  return (
    <div className="booking-container">
      <div className="booking-form">
        <h2>Book time her</h2>

        {message && <div className="alert alert-info">{message}</div>}

        <form onSubmit={handleSubmit}>
          {/* DATE PICKER */}
          <div className="form-group">
            <label>Dato</label>
            <div className="date-options">
              {availableDates.map((date) => (
                <label key={date.id} className="date-option btn btn-outline-secondary m-1">
                  <input
                    type="radio"
                    name="selectedDate"
                    checked={form.selectedDate === date.date}
                    onChange={() => handleDateChange(date.id, date.date)}
                  />
                  {new Date(date.date).toLocaleDateString("no-NO", {
                    day: "2-digit",
                    month: "short",
                  })}
                </label>
              ))}
            </div>
          </div>

          {/* TIME SLOTS */}
          <div className="form-group">
            <label>Tidspunkt</label>
            <div className="mt-2">
              {availableSlots.length > 0 ? (
                availableSlots.map((slot) => (
                  <button
                    key={slot.id}
                    type="button"
                    className={`btn btn-outline-primary mb-1 ${
                      form.timeSlotId == slot.id ? "active" : ""
                    }`}
                    onClick={() => setForm({ ...form, timeSlotId: slot.id })}
                  >
                    {slot.slot}
                  </button>
                ))
              ) : (
                <p className="text-muted">Velg en dato først.</p>
              )}
            </div>
          </div>

          {/* CATEGORY */}
          <div className="form-group">
            <label>Kategori</label>
            <select
              className="form-control"
              value={form.categoryId}
              onChange={(e) => setForm({ ...form, categoryId: e.target.value })}
            >
              <option value="">Velg kategori</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </select>
          </div>

          {/* NOTES */}
          {categories.find((c) => c.id == form.categoryId)?.name === "OTHER" && (
            <div className="form-group">
              <label>Notater</label>
              <textarea
                className="form-control"
                value={form.notes}
                onChange={(e) => setForm({ ...form, notes: e.target.value })}
              />
            </div>
          )}

          <button type="submit" className="btn btn-primary mt-3">
            {form.appointmentId ? "Oppdater timen" : "Book Time"}
          </button>
        </form>
      </div>

      {/* FUTURE APPOINTMENTS */}
      <div className="booking-list">
        <h2>Fremtidige Timer</h2>

        {appointments.length === 0 ? (
          <p>Ingen avtaler funnet.</p>
        ) : (
          <table className="table table-custom">
            <thead>
              <tr>
                <th>Dato & Tidspunkt</th>
                <th>Kategori</th>
                <th>Notater</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {appointments.map((a) => (
                <tr key={a.id} className={editingId === a.id ? "editing-row" : ""}>
                  <td>
                    {new Date(a.date).toLocaleDateString("no-NO")}{" "}
                    {a.time}
                    {editingId === a.id && (
                      <span className="badge bg-warning text-dark ms-2">
                        Endre timen
                      </span>
                    )}
                  </td>
                  <td>{a.serviceType}</td>
                  <td>{a.notes}</td>
                  <td className="text-end">
                    <button
                      className="btn btn-sm btn-outline-primary me-1"
                      onClick={() => handleEdit(a.id)}
                      disabled={editingId && editingId !== a.id}
                    >
                      Endre timen
                    </button>

                    <button
                      className="btn btn-sm btn-outline-danger"
                      onClick={() => handleCancel(a.id)}
                      disabled={editingId && editingId !== a.id}
                    >
                      Kanseller timen
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>

      <div className="mt-3 text-center">
        <Link to="/dashboard" className="btn btn-secondary">
          Tilbake til Min Side
        </Link>
      </div>
    </div>
  );
}
