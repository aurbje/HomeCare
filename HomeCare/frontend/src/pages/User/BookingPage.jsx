import React, { useEffect, useState } from "react";
import {
  getBookingPage,
  createOrUpdateBooking,
  cancelBooking,
  getBookingForEdit
} from "../../api/bookingApi";
import { Link, useNavigate } from "react-router-dom";

export default function BookingPage() {
  const navigate = useNavigate();

  const [form, setForm] = useState({
    bookingId: null,
    selectedDate: "",
    timeSlotId: "",
    categoryId: "",
    notes: "",
  });

  const [availableDates, setAvailableDates] = useState([]);
  const [categories, setCategories] = useState([]);
  const [appointments, setAppointments] = useState([]);
  const [availableSlots, setAvailableSlots] = useState([]);

  const [editingId, setEditingId] = useState(null);

  // -------------------------------
  // 1️⃣ Load full booking page data
  // -------------------------------
  useEffect(() => {
    loadBookingData();
  }, []);

  const loadBookingData = async () => {
    try {
      const data = await getBookingPage();
      setAvailableDates(data.availableDates);
      setCategories(data.categories);
      setAppointments(data.bookings);
    } catch (err) {
      console.error("Feil ved henting av bookingside-data:", err);
    }
  };

  // ------------------------------------
  // 2️⃣ Change date → update available slots
  // ------------------------------------
  const handleDateChange = (dateId, dateValue) => {
    setForm({ ...form, selectedDate: dateValue, timeSlotId: "" });

    const dateObj = availableDates.find((d) => d.id === dateId);
    setAvailableSlots(dateObj?.timeSlots.filter((ts) => !ts.isBooked) || []);
  };

  // ------------------------------------
  // 3️⃣ Handle booking submit (create/update)
  // ------------------------------------
  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      bookingId: form.bookingId || 0,
      selectedDate: form.selectedDate,
      timeSlotId: form.timeSlotId,
      categoryId: form.categoryId,
      notes: form.notes,
    };

    try {
      await createOrUpdateBooking(payload);
      await loadBookingData();

      alert(
        form.bookingId
          ? "Timen ble oppdatert!"
          : "Timen ble booket!"
      );

      resetForm();
    } catch (err) {
      console.error("Feil ved lagring:", err);
      alert("Kunne ikke lagre booking.");
    }
  };

  const resetForm = () => {
    setForm({
      bookingId: null,
      selectedDate: "",
      timeSlotId: "",
      categoryId: "",
      notes: "",
    });
    setAvailableSlots([]);
    setEditingId(null);
  };

  // ------------------------------------
  // 4️⃣ Edit booking → load booking into form
  // ------------------------------------
  const handleEdit = async (id) => {
    setEditingId(id);

    try {
      const data = await getBookingForEdit(id);

      setForm({
        bookingId: data.bookingId,
        selectedDate: data.selectedDate,
        timeSlotId: data.timeSlotId,
        categoryId: data.categoryId,
        notes: data.notes || "",
      });

      const dateObj = data.availableDates.find(
        (d) => d.date === data.selectedDate
      );

      setAvailableSlots(
        dateObj?.timeSlots.filter((ts) => !ts.isBooked || ts.id === data.timeSlotId) || []
      );

      setAvailableDates(data.availableDates);
      setCategories(data.categories);
      setAppointments(data.bookings);
    } catch (err) {
      console.error("Feil ved lasting av booking:", err);
    }
  };

  // ------------------------------------
  // 5️⃣ Cancel booking
  // ------------------------------------
  const handleCancel = async (id) => {
    if (!window.confirm("Vil du avlyse denne timen?")) return;

    try {
      await cancelBooking(id);
      await loadBookingData();
    } catch (err) {
      console.error("Feil ved avbestilling:", err);
    }
  };

  return (
    <div className="booking-container">
      {/* FORM SIDE */}
      <div className="booking-form">
        <h2>Book time her</h2>

        <form onSubmit={handleSubmit}>
          {/* Dato */}
          <div className="form-group">
            <label>Dato</label>
            <div className="date-options">
              {availableDates.length > 0 ? (
                availableDates.map((date) => (
                  <label
                    key={date.id}
                    className="date-option btn btn-outline-secondary m-1"
                  >
                    <input
                      type="radio"
                      name="selectedDate"
                      checked={form.selectedDate === date.date}
                      onChange={() => handleDateChange(date.id, date.date)}
                    />
                    {new Date(date.date).toLocaleDateString("nb-NO", {
                      day: "2-digit",
                      month: "short",
                    })}
                  </label>
                ))
              ) : (
                <p className="text-danger">Ingen ledige datoer funnet.</p>
              )}
            </div>
          </div>

          {/* Time slots */}
          <div className="form-group">
            <label>Tidspunkt</label>
            <div className="mt-2">
              {availableSlots.length > 0 ? (
                availableSlots.map((slot) => (
                  <button
                    key={slot.id}
                    type="button"
                    className={`btn btn-outline-primary mb-1 ${
                      form.timeSlotId === slot.id ? "active" : ""
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

          {/* Kategori */}
          <div className="form-group">
            <label>Kategori</label>
            <select
              className="form-control"
              value={form.categoryId}
              onChange={(e) =>
                setForm({ ...form, categoryId: Number(e.target.value) })
              }
            >
              <option value="">Velg kategori</option>
              {categories.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.name}
                </option>
              ))}
            </select>
          </div>

          {/* Notes hvis OTHER */}
          {categories.find((c) => c.id == form.categoryId)?.name === "OTHER" && (
            <div className="form-group">
              <label>Notater</label>
              <textarea
                className="form-control"
                value={form.notes}
                onChange={(e) =>
                  setForm({ ...form, notes: e.target.value })
                }
              />
            </div>
          )}

          <button type="submit" className="btn btn-primary mt-3">
            {form.bookingId ? "Oppdater timen" : "Book Time"}
          </button>
        </form>
      </div>

      {/* LISTE OVER FREMTIDIGE TIMER */}
      <div className="booking-list">
        <h2>Fremtidige Timer</h2>

        {appointments.length > 0 ? (
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
                <tr
                  key={a.id}
                  className={editingId === a.id ? "editing-row" : ""}
                >
                  <td>
                    {new Date(a.date).toLocaleDateString("nb-NO")} {a.time}
                    {editingId === a.id && (
                      <span className="badge bg-warning text-dark ms-2">
                        Endrer denne timen
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
        ) : (
          <p>Ingen avtaler funnet.</p>
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
