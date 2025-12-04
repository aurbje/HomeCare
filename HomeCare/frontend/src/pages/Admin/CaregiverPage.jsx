import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getCaregivers, deleteCaregiver } from '../../api/adminApi';

// Admin page for listing, searching, editing, and deleting caregivers
function CaregiverPage() {
  const [caregivers, setCaregivers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const navigate = useNavigate();

  // Loads caregiver list on initial render
  useEffect(() => {
    fetchCaregivers();
  }, []);

  // Fetches caregiver data 
  const fetchCaregivers = async (search = '') => {
    try {
      setLoading(true);
      const data = await getCaregivers(search);
      setCaregivers(data);
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste ansatte');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Submits search request
  const handleSearch = (e) => {
    e.preventDefault();
    fetchCaregivers(searchTerm);
  };

  // Resets search and reloads all caregivers
  const handleReset = () => {
    setSearchTerm('');
    fetchCaregivers('');
  };

  // Deletes a caregiver after confirmation
  const handleDelete = async (id, fullName) => {
    if (!window.confirm(`Er du sikker på at du vil slette ${fullName}?`)) return;

    try {
      await deleteCaregiver(id);
      setSuccess('Ansatt slettet');
      fetchCaregivers(searchTerm);
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Kunne ikke slette ansatt');
      console.error(err);
      setTimeout(() => setError(null), 3000);
    }
  };

  // Navigates to edit form
  const handleEdit = (id) => {
    navigate(`/admin/caregivers/edit/${id}`);
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Ansatte ({caregivers.length})</h1>

      {/* Success alert */}
      {success && (
        <div className="alert alert-success alert-dismissible fade show" role="alert">
          {success}
          <button type="button" className="btn-close" onClick={() => setSuccess(null)}></button>
        </div>
      )}

      {/* Error alert */}
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
            placeholder="Søk navn / e-post"
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

      {/* Caregiver table */}
      <table className="table table-sm table-striped align-middle">
        <thead>
          <tr>
            <th>BrukerID</th>
            <th>Navn</th>
            <th>E-post</th>
            <th>Telefon</th>
            <th>Adresse</th>
            <th>Rolle</th>
            <th>Endre</th>
            <th>Slette</th>
          </tr>
        </thead>
        <tbody>
          {caregivers.length === 0 ? (
            <tr>
              <td colSpan="9" className="text-muted">Ingen ansatte funnet.</td>
            </tr>
          ) : (
            caregivers.map((caregiver) => (
              <tr key={caregiver.id}>
                <td>{caregiver.id}</td>
                <td>{caregiver.fullName}</td>
                <td>{caregiver.email}</td>
                <td>{caregiver.tlfNumber}</td>
                <td>{caregiver.address}</td>
                <td>
                  <span className="badge bg-info">{caregiver.role}</span>
                </td>

                {/* Edit button */}
                <td>
                  <button
                    onClick={() => handleEdit(caregiver.id)}
                    className="btn btn-sm btn-primary"
                  >
                    Endre
                  </button>
                </td>

                {/* Delete button */}
                <td>
                  <button
                    type="button"
                    onClick={() => handleDelete(caregiver.id, caregiver.fullName)}
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

      {/* Back button */}
      <button onClick={() => navigate('/admindashboard')} className="btn btn-primary btn-sm">
        Tilbake
      </button>
    </div>
  );
}

export default CaregiverPage;
