import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { getUsers, deleteUser } from '../../api/adminApi';

function UsersPage() {
  const [users, setUsers] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    fetchUsers();
  }, []);

  const fetchUsers = async (search = '') => {
    try {
      setLoading(true);
      const data = await getUsers(search);
      setUsers(data);
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste brukere');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = (e) => {
    e.preventDefault();
    fetchUsers(searchTerm);
  };

  const handleReset = () => {
    setSearchTerm('');
    fetchUsers('');
  };

  const handleDelete = async (id, fullName) => {
    if (!window.confirm(`Er du sikker på at du vil slette ${fullName}?`)) {
      return;
    }

    try {
      await deleteUser(id);
      setSuccess('Bruker slettet');
      fetchUsers(searchTerm);
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError('Kunne ikke slette bruker');
      console.error(err);
      setTimeout(() => setError(null), 3000);
    }
  };

  const handleEdit = (id) => {
    navigate(`/admin/users/edit/${id}`);
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Brukere ({users.length})</h1>

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
            placeholder="Søk navn / e-post"
          />
        </div>
        <div className="col-auto">
          <button type="submit" className="btn btn-sm btn-primary">Søk</button>
        </div>
        <div className="col-auto">
          <button type="button" onClick={handleReset} className="btn btn-sm btn-outline-secondary">
            Nullstill
          </button>
        </div>
      </form>

      <table className="table table-sm table-striped align-middle">
        <thead>
          <tr>
            <th>ID</th>
            <th>Navn</th>
            <th>E-post</th>
            <th>Adresse</th>
            <th>Telefon</th>
            <th>Rolle</th>
            <th>Endre</th>
            <th>Slette</th>
          </tr>
        </thead>
        <tbody>
          {users.length === 0 ? (
            <tr>
              <td colSpan="8" className="text-muted">Ingen brukere funnet.</td>
            </tr>
          ) : (
            users.map((user) => (
              <tr key={user.id}>
                <td>{user.id}</td>
                <td>{user.fullName}</td>
                <td>{user.email}</td>
                <td>{user.address}</td>
                <td>{user.tlfNumber}</td>
                <td>
                  <span className={`badge bg-${user.role === 'admin' ? 'danger' : 'secondary'}`}>
                    {user.role}
                  </span>
                </td>
                <td>
                  <button
                    onClick={() => handleEdit(user.id)}
                    className="btn btn-sm btn-warning"
                  >
                    Endre
                  </button>
                </td>
                <td>
                  <button
                    type="button"
                    onClick={() => handleDelete(user.id, user.fullName)}
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

      <button onClick={() => navigate('/admin/dashboard')} className="btn btn-outline-secondary btn-sm">
        Tilbake
      </button>
    </div>
  );
}

export default UsersPage;
