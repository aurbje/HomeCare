import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getBookings, deleteBooking } from '../../api/adminApi';

// Admin page for viewing, searching, editing, and deleting bookings
function BookingsPage() {
  const [bookings, setBookings] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const navigate = useNavigate();

  // Load bookings on initial render
  useEffect(() => {
    fetchBookings();
  }, []);

  // Fetch bookings from API with optional search filter
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

  // Triggers search filter
  const handleSearch = (e) => {
    e.preventDefault();
    fetchBookings(searchTerm);
  };

  // Clears search and reloads all bookings
  const handleReset = () => {
    setSearchTerm('');
    fetchBookings('');
  };

  // Deletes a booking after user confirmation
  const handleDelete = async (id) => {
    if (!window.confirm(`Slette booking ${id}?`)) return;

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

  // Redirects to booking edit page
  const handleEdit = (id) => {
    navigate(`/admin/bookings/edit/${id}`);
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Bookinger ({bookings.length})</h1>

      {/* Success feedback */}
      {success && (
        <div className="alert alert-success alert-dismissible fade show" role="alert">
          {success}
          <button type="button" className="btn-close" onClick={() => setSuccess(null)}></button>
        </div>
      )}

      {/* Error feedback */}
      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError(null)}></button>
        </div>
      )}

      {/* Search bar */}
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
          <button type="submit" className="btn btn-sm btn-primary">Søk</button>
        </div>
        <div className="col-auto">
          <button type="button" onClick={handleReset} className="btn btn-primary btn-sm">
            Nullstill
          </button>
        </div>
      </form>

      {/* Bookings table */}
      <table className="table table-sm table-striped align-middle">
        <thead>
          <tr>
            <th>BrukerID</th>
            <th>Navn</th>
            <th>Dato</th>
            <th>Tidspunkt</th>
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
                <td>{booking.id}</td>
                <td>{booking.user?.fullName || `Bruker ${booking.userId}`}</td>
                <td>{new Date(booking.dateTime).toLocaleDateString('nb-NO')}</td>
                <td>{booking.timeSlot?.slot || 'N/A'}</td>
                <td>{booking.category?.name || 'N/A'}</td>
                <td>{booking.caregiverId ? `${booking.caregiverId}` : 'Ikke tildelt'}</td>

                {/* Edit action */}
                <td>
                  <button
                    onClick={() => handleEdit(booking.id)}
                    className="btn btn-sm btn-primary"
                  >
                    Endre
                  </button>
                </td>

                {/* Delete action */}
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

      {/* Back navigation */}
      <button onClick={() => navigate('/admindashboard')} className="btn btn-primary btn-sm">
        Tilbake
      </button>
    </div>
  );
}

export default BookingsPage;
