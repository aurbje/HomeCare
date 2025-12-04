import React from "react";

// Renders a list of upcoming reminders or a fallback when none exist
const ReminderList = ({ reminders }) => {
  // Displays a simple message when no reminders are available
  if (!reminders || reminders.length === 0) {
    return <p className="text-muted">Ingen kommende avtaler.</p>;
  }

  // Displays each reminder with date, time, and service type
  return (
    <ul className="list-group">
      {reminders.map((r) => (
        <li key={r.id} className="list-group-item d-flex justify-content-between">
          <div>
            <strong>{r.serviceType}</strong>
            <div className="text-muted small">
              {new Date(r.date).toLocaleDateString("nb-NO")} – {r.time}
            </div>
          </div>

          {/* Status badge indicating the appointment is upcoming */}
          <span className="badge bg-success align-self-center">Kommer</span>
        </li>
      ))}
    </ul>
  );
};

export default ReminderList;
