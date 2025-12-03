import React from "react";

const ReminderList = ({ reminders }) => {
  if (!reminders || reminders.length === 0) {
    return <p className="text-muted">Ingen kommende avtaler.</p>;
  }

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
          <span className="badge bg-success align-self-center">Kommer</span>
        </li>
      ))}
    </ul>
  );
};

export default ReminderList;
