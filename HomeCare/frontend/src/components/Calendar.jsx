import React, { useState } from "react";

// appointments = [
//   { id, dateTime, category: { name } }
// ]

export default function Calendar({ appointments = [] }) {
  const today = new Date();
  const [year, setYear] = useState(today.getFullYear());
  const [month, setMonth] = useState(today.getMonth()); // 0–11

  const culture = "nb-NO";

  const currentMonth = new Date(year, month, 1);
  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const firstDay = new Date(year, month, 1);

  // Start monday = 0
  const offset = (firstDay.getDay() + 6) % 7;
  const totalCells = offset + daysInMonth;
  const rows = Math.ceil(totalCells / 7);

  // Group appointments by date
  const apptByDate = {};
  appointments.forEach((a) => {
    const dateKey = new Date(a.dateTime).toDateString();
    apptByDate[dateKey] = apptByDate[dateKey] || [];
    apptByDate[dateKey].push(a);
  });

  function prevMonth() {
    setMonth((prev) => (prev === 0 ? 11 : prev - 1));
    if (month === 0) setYear((y) => y - 1);
  }

  function nextMonth() {
    setMonth((prev) => (prev === 11 ? 0 : prev + 1));
    if (month === 11) setYear((y) => y + 1);
  }

  return (
    <div>
      {/* NAVIGATION */}
      <div className="calendar-nav mb-2 d-flex justify-content-between align-items-center">
        <button className="btn btn-outline-secondary" onClick={prevMonth}>
          ← {new Date(year, month - 1, 1).toLocaleDateString(culture, { month: "long", year: "numeric" })}
        </button>

        <h4 className="mb-0">
          {currentMonth.toLocaleDateString(culture, { month: "long", year: "numeric" })}
        </h4>

        <button className="btn btn-outline-secondary" onClick={nextMonth}>
          {new Date(year, month + 1, 1).toLocaleDateString(culture, { month: "long", year: "numeric" })} →
        </button>
      </div>

      {/* CALENDAR TABLE */}
      <div className="calendar-container">
        <table className="table table-calendar">
          <thead>
            <tr>
              <th>Man</th><th>Tir</th><th>Ons</th><th>Tor</th><th>Fre</th><th>Lør</th><th>Søn</th>
            </tr>
          </thead>
          <tbody>
            {Array.from({ length: rows }).map((_, r) => (
              <tr key={r}>
                {Array.from({ length: 7 }).map((_, c) => {
                  const cellIndex = r * 7 + c;

                  if (cellIndex < offset || cellIndex >= offset + daysInMonth) {
                    return <td key={c} className="empty" />;
                  }

                  const day = cellIndex - offset + 1;
                  const dateObj = new Date(year, month, day);
                  const dateKey = dateObj.toDateString();
                  const isSunday = dateObj.getDay() === 0;
                  const hasAppointments = apptByDate[dateKey];

                  return (
                    <td
                      key={c}
                      className={`calendar-day ${isSunday ? "sunday" : ""} ${hasAppointments ? "has-appointment" : ""}`}
                    >
                      <div className="date-label">{day}</div>

                      {hasAppointments &&
                        hasAppointments.map((a) => (
                          <div key={a.id} className="appt-entry">
                            <small>
                              {new Date(a.dateTime).toLocaleTimeString(culture, { hour: "2-digit", minute: "2-digit" })}
                              {" - "}
                              {a.category?.name}
                            </small>
                          </div>
                        ))}
                    </td>
                  );
                })}
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}
