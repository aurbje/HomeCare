import React from "react";
import { Link } from "react-router-dom";

export default function AppointmentList({ appointments = [] }) {
  return (
    <table className="table table-booking-custom">
      <thead>
        <tr>
          <th>Dato & Tidspunkt</th>
          <th>Kategori</th>
          <th>Notater</th>
          <th>Handlinger</th>
        </tr>
      </thead>

      <tbody>
        {appointments.length > 0 ? (
          appointments.map((a) => (
            <tr key={a.id}>
              <td>
                {new Date(a.dateTime).toLocaleString("nb-NO", {
                  year: "numeric",
                  month: "2-digit",
                  day: "2-digit",
                  hour: "2-digit",
                  minute: "2-digit"
                })}
              </td>
              <td>{a.category?.name}</td>
              <td>{a.notes}</td>
              <td>
                <Link className="btn btn-primary btn-sm me-1" to={`/appointments/edit/${a.id}`}>
                  Endre timen
                </Link>

                <Link className="btn btn-danger btn-sm" to={`/appointments/delete/${a.id}`}>
                  Kanseller timen
                </Link>
              </td>
            </tr>
          ))
        ) : (
          <tr>
            <td colSpan="4" className="text-center">
              Ingen avtaler foreløpig.
            </td>
          </tr>
        )}
      </tbody>
    </table>
  );
}
