import React from "react";

const AppointmentList = ({ appointments }) => {
  if (!appointments || appointments.length === 0) {
    return (
      <table className="table table-booking-custom">
        <thead>
          <tr>
            <th>Dato & Tidspunkt</th>
            <th>Kategori</th>
            <th>Notater</th>
          </tr>
        </thead>
        <tbody>
          <tr>
            <td colSpan="3" className="text-center">
              Ingen avtaler foreløpig.
            </td>
          </tr>
        </tbody>
      </table>
    );
  }

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
        {appointments.map((a) => (
          <tr key={a.id}>
            <td>
              {new Date(a.date).toLocaleDateString("nb-NO")}{" "}
              {a.time}
            </td>
            <td>{a.serviceType}</td>
            <td>{a.notes}</td>
            <td>
              <a className="btn btn-outline-primary btn-sm" href={`/booking/edit/${a.id}`}>
                Endre
              </a>
              <a className="btn btn-outline-danger btn-sm ms-2" href={`/booking/cancel/${a.id}`}>
                Kanseller
              </a>
            </td>
          </tr>
        ))}
      </tbody>
    </table>
  );
};

export default AppointmentList;
