import React, { useState } from "react";
import { Link } from "react-router-dom";

export default function BookingPage() {
  const [form, setForm] = useState({
    appointmentId: null,
    selectedDate: "",
    timeSlotId: "",
    categoryId: "",
    notes: "",
  });

  const [editingId, setEditingId] = useState(null);

  const availableDates = mockData.availableDates;
  const categories = mockData.categories;
  const appointments = mockData.appointments;

  const handleDateChange = (dateId, dateValue) => {
    setForm({ ...form, selectedDate: dateValue, timeSlotId: "" });
    setAvailableSlots(
      availableDates.find((d) => d.id === dateId)?.timeSlots || []
    );
  };

  const [availableSlots, setAvailableSlots] = useState([]);

  const handleSubmit = (e) => {
    e.preventDefault();
    console.log("Booking:", form);
    // TODO: Send form til backend
  };

  const handleEdit = (appointmentId) => {
    setEditingId(appointmentId);
    // TODO: Last inn booking og sett verdier
  };

  const handleCancel = (id) => {
    if (window.confirm("Vil du avlyse denne timen?")) {
      // TODO: kall backend for sletting
      console.log("Avlyser ID:", id);
    }
  };

  return (
    <div className="booking-container">
      <div className="booking-form">
        <h2>Book time her</h2>

        <form onSubmit={handleSubmit}>
          {/* Date picker */}
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
                      value={date.date}
                      onChange={() => handleDateChange(date.id, date.date)}
                    />
                    {new Date(date.date).toLocaleDateString("no-NO", {
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
                setForm({ ...form, categoryId: e.target.value })
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

          {/* Notes hvis "OTHER" */}
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

      {/* Fremtidige avtaler */}
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
                <tr key={a.id} className={editingId === a.id ? "editing-row" : ""}>
                  <td>
                    {new Date(a.dateTime).toLocaleString("no-NO", {
                      dateStyle: "short",
                      timeStyle: "short",
                    })}
                    {editingId === a.id && (
                      <span className="badge bg-warning text-dark ms-2">
                        Endre timen
                      </span>
                    )}
                  </td>
                  <td>{a.category.name}</td>
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
