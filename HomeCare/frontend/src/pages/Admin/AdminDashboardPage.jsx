import React from 'react';
import { MdPeople, MdEvent, MdMedicalServices } from "react-icons/md";
import { useNavigate } from 'react-router-dom';

function AdminDashboardPage() {
  const navigate = useNavigate();

  return (
    <div className="container py-5">
      <h1 className="mb-4 text-center">Logget inn som: Administrator</h1>

      <div className="row g-4">
        {/* users */}
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/users')}
          >
            <div className="card-body text-center py-4 admin-card">
              <div className="admin-card-icon mb-3">
                <MdPeople aria-hidden="true" />
              </div>
              <h2 className="h5 mb-1">Brukere</h2>
              <p className="text-muted small mb-0">
                Administrer brukerkontoer og roller
              </p>
            </div>
          </div>
        </div>

        {/* bookings */}
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/bookings')}
          >
            <div className="card-body text-center py-4 admin-card">
              <div className="admin-card-icon mb-3">
                <MdEvent aria-hidden="true" />
              </div>
              <h2 className="h5 mb-1">Bookinger</h2>
              <p className="text-muted small mb-0">
                Administrer bookinger og tider
              </p>
            </div>
          </div>
        </div>

        {/* caregivers */}
        <div className="col-12 col-md-4">
          <div
            className="text-decoration-none card shadow-sm h-100 border-0 hover-shadow"
            style={{ cursor: 'pointer' }}
            onClick={() => navigate('/admin/caregivers')}
          >
            <div className="card-body text-center py-4 admin-card">
              <div className="admin-card-icon mb-3">
                <MdMedicalServices aria-hidden="true" />
              </div>
              <h2 className="h5 mb-1">Ansatte</h2>
              <p className="text-muted small mb-0">
                Administrer ansatte og deres informasjon
              </p>
            </div>
          </div>
        </div>
      </div>

      <style jsx>{`
        .hover-shadow:hover {
          box-shadow: 0 0.75rem 1.25rem rgba(0, 0, 0, 0.15);
          transition: box-shadow 0.2s;
        }

        .admin-card-icon svg {
          font-size: 2.8rem;
        }
      `}</style>
    </div>
  );
}

export default AdminDashboardPage;
