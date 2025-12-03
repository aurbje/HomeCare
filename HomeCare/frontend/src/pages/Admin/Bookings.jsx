import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getBookings, deleteBooking } from '../../api/adminApi';

function BookingsPage() {
  const [bookings, setBookings] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    fetchBookings();
  }, []);

  const fetchBookings = async (search = '') => {
    try {
      setLoading(true);
      const data = await getBookings(search);
      setBookings(data);
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste bookinger');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchBookings(searchTerm);
  };

  const handleReset = () => {
    setSearchTerm('');
    fetchBookings('');
  };

  const handleDelete = async (id) => {
    if (!window.confirm(`Slette booking ${id}?`)) {
      return;
    }

    try {
      await deleteBooking(id);
      setSuccess('Booking slettet');
      fetchBookings(searchTerm);
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Kunne ikke slette booking');
      console.error(err);
      setTimeout(() => setError(null), 3000);
    }
  };

  const handleEdit = (id) => {
    navigate(`/admin/bookings/edit/${id}`);
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Bookinger ({bookings.length})</h1>

      {success && (
        <div className="alert alert-success alert-dismissible fade show" role="alert">
          {success}
          <button type="button" className="btn-close" onClick={() => setSuccess(null)}></button>
        </div>
      )}

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError(null)}></button>
        </div>
      )}

      <form onSubmit={handleSearch} className="row g-2 mb-3">
        <div className="col-auto">
          <input
            name="q"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="form-control form-control-sm"
            placeholder="Søk (kunde / ansatt / tjeneste)"
          />
        </div>
        <div className="col-auto">
          <button type="submit" className="btn btn-sm hc-btn-outline hc-btn-outline-green me-2">Søk</button>
        </div>
        <div className="col-auto">
          <button type="button" onClick={handleReset} className="btn btn-sm hc-btn-outline hc-btn-outline-grey">
            Nullstill
          </button>
        </div>
      </form>

      <table className="table table-sm table-striped align-middle">
        <thead>
          <tr>
            <th>BookingID</th>
            <th>BrukerID</th>
            <th>Dato</th>
            <th>Tid</th>
            <th>Tjeneste</th>
            <th>AnsattID</th>
            <th>Endre</th>
            <th>Slette</th>
          </tr>
        </thead>
        <tbody>
          {bookings.length === 0 ? (
            <tr>
              <td colSpan="8" className="text-muted">Ingen bookinger funnet.</td>
            </tr>
          ) : (
            bookings.map((booking) => (
              <tr key={booking.id}>
                <td className="text-truncate" style={{ maxWidth: '140px' }}>{booking.id}</td>
                <td>{booking.clientId}</td>
                <td>{new Date(booking.date).toLocaleDateString('nb-NO')}</td>
                <td>{booking.time}</td>
                <td>{booking.serviceType}</td>
                <td>{booking.personnelId}</td>
                <td>
                  <button
                    onClick={() => handleEdit(booking.id)}
                    className="btn btn-sm btn-warning"
                  >
                    Endre
                  </button>
                </td>
                <td>
                  <button
                    type="button"
                    onClick={() => handleDelete(booking.id)}
                    className="btn btn-sm btn-danger"
                  >
                    Slette
                  </button>
                </td>
              </tr>
            ))
          )}
        </tbody>
      </table>

      <button onClick={() => navigate('/admindashboard')} className="btn btn-main btn-sm">
        Tilbake
      </button>
    </div>
  );
}

export default BookingsPage;
