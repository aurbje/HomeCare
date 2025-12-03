import React, { useState } from "react";

const Calendar = ({ appointments }) => {
  const today = new Date();
  const [currentMonth, setCurrentMonth] = useState(
    new Date(today.getFullYear(), today.getMonth(), 1)
  );

  const month = currentMonth.getMonth();
  const year = currentMonth.getFullYear();

  const daysInMonth = new Date(year, month + 1, 0).getDate();
  const firstDay = new Date(year, month, 1);
  const offset = (firstDay.getDay() + 6) % 7;

  const apptMap = {};
  appointments.forEach((appt) => {
    const dateStr = new Date(appt.date).toDateString();
    if (!apptMap[dateStr]) apptMap[dateStr] = [];
    apptMap[dateStr].push(appt);
  });

  const changeMonth = (direction) => {
    setCurrentMonth(new Date(year, month + direction, 1));
  };

  const culture = "nb-NO";

  // Build grid
  const cells = [];
  for (let i = 0; i < offset; i++) cells.push(null);
  for (let d = 1; d <= daysInMonth; d++) cells.push(new Date(year, month, d));

  return (
    <div>
      <div className="calendar-nav d-flex justify-content-between align-items-center mb-2">
        <button className="btn btn-outline-secondary" onClick={() => changeMonth(-1)}>
          ← {new Date(year, month - 1).toLocaleDateString(culture, { month: "long", year: "numeric" })}
        </button>

        <h4 className="mb-0">
          {currentMonth.toLocaleDateString(culture, { month: "long", year: "numeric" })}
        </h4>

        <button className="btn btn-outline-secondary" onClick={() => changeMonth(1)}>
          {new Date(year, month + 1).toLocaleDateString(culture, { month: "long", year: "numeric" })} →
        </button>
      </div>

      <table className="table table-calendar">
        <thead>
          <tr>
            <th>Man</th>
            <th>Tir</th>
            <th>Ons</th>
            <th>Tor</th>
            <th>Fre</th>
            <th>Lør</th>
            <th>Søn</th>
          </tr>
        </thead>
        <tbody>
          {Array.from({ length: Math.ceil(cells.length / 7) }).map((_, row) => (
            <tr key={row}>
              {cells.slice(row * 7, row * 7 + 7).map((date, idx) => {
                if (!date) return <td key={idx} className="empty"></td>;

                const dateStr = date.toDateString();
                const dayAppts = apptMap[dateStr] || [];
                const isSunday = date.getDay() === 0;

                return (
                  <td
                    key={idx}
                    className={`calendar-day ${dayAppts.length ? "has-appointment" : ""} ${
                      isSunday ? "sunday" : ""
                    }`}
                  >
                    <div className="date-label">{date.getDate()}</div>

                    {dayAppts.map((a) => (
                      <div key={a.id} className="appt-entry small">
                        {a.time} – {a.serviceType}
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
  );
};

export default Calendar;
