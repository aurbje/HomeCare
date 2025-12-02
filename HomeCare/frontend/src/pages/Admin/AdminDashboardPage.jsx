import React from 'react';
import { useNavigate } from 'react-router-dom';

function AdminDashboardPage() {
  const navigate = useNavigate();

  return (
    <div className="container py-5">
      <h1 className="mb-4 text-center">Adminpanel</h1>

      <div className="row g-4">
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/users')}
          >
            <div className="card-body text-center py-4">
              <div className="display-6 mb-3">👥</div>
              <h2 className="h5">Brukere</h2>
              <p className="text-muted small mb-0">Administrer brukerkontoer og roller</p>
            </div>
          </div>
        </div>
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/bookings')}
          >
            <div className="card-body text-center py-4">
              <div className="display-6 mb-3">📅</div>
              <h2 className="h5">Bookinger</h2>
              <p className="text-muted small mb-0">Se og juster bookingdata</p>
            </div>
          </div>
        </div>
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/personnel')}
          >
            <div className="card-body text-center py-4">
              <div className="display-6 mb-3">🧑‍⚕️</div>
              <h2 className="h5">Ansatte</h2>
              <p className="text-muted small mb-0">Håndter tilgjengelighet og tilganger</p>
            </div>
          </div>
        </div>
      </div>

      <style jsx>{`
        .hover-shadow:hover {
          box-shadow: 0 0.75rem 1.25rem rgba(0, 0, 0, 0.15);
          transition: box-shadow 0.2s;
        }
      `}</style>
    </div>
  );
}

export default AdminDashboardPage;
