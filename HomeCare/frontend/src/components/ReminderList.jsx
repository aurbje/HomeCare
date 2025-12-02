import React from "react";

export default function ReminderList({ reminders = [] }) {
  if (!reminders.length)
    return <p className="text-muted">Du har ingen påminnelser.</p>;

  return (
    <ul className="list-group">
      {reminders.map((r) => (
        <li key={r.id} className="list-group-item d-flex justify-content-between">
          <span>
            {new Date(r.dateTime).toLocaleDateString("nb-NO")} –{" "}
            {new Date(r.dateTime).toLocaleTimeString("nb-NO", { hour: "2-digit", minute: "2-digit" })}
          </span>

          <span className="fw-semibold">{r.category?.name}</span>
        </li>
      ))}
    </ul>
  );
}
