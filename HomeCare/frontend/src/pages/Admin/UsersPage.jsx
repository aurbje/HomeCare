import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { getUsers, deleteUser } from "../../api/adminApi";

// Admin page for listing, searching, editing, and deleting users
function UsersPage() {
  const [users, setUsers] = useState([]);
  const [searchTerm, setSearchTerm] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(null);

  const navigate = useNavigate();

  // Fetches users from API
  const fetchUsers = async (search = "") => {
    try {
      setLoading(true);
      const data = await getUsers(search);
      setUsers(data);
      setError(null);
    } catch (err) {
      setError("Kunne ikke laste brukere");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  // Loads users whenever searchTerm changes
  useEffect(() => {
    let ignore = false;

    async function load() {
      if (!ignore) await fetchUsers(searchTerm);
    }

    load();
    return () => { ignore = true; };
  }, [searchTerm]);

  // Triggers search manually
  const handleSearch = (e) => {
    e.preventDefault();
    fetchUsers(searchTerm);
  };

  // Resets search and reloads all users
  const handleReset = () => {
    setSearchTerm("");
    fetchUsers("");
  };

  // Deletes a user after confirmation
  const handleDelete = async (id, fullName) => {
    if (!window.confirm(`Er du sikker på at du vil slette ${fullName}?`)) return;

    try {
      await deleteUser(id);
      setSuccess("Bruker slettet");
      await fetchUsers(searchTerm);
      setTimeout(() => setSuccess(null), 3000);
    } catch (err) {
      setError("Kunne ikke slette bruker");
      console.error(err);
      setTimeout(() => setError(null), 3000);
    }
  };

  // Redirects to edit page
  const handleEdit = (id) => {
    navigate(`/admin/users/edit/${id}`);
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Brukere ({users.length})</h1>

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
            placeholder="Søk navn / e-post"
          />
        </div>

        <div className="col-auto">
          <button type="submit" className="btn btn-sm btn-primary">Søk</button>
        </div>

        <div className="col-auto">
          <button
            type="button"
            onClick={handleReset}
            className="btn btn-primary btn-sm"
          >
            Nullstill
          </button>
        </div>
      </form>

      {/* Users table */}
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

                {/* Role badge */}
                <td>
                  <span className={`badge bg-${user.role === "admin" ? "danger" : "secondary"}`}>
                    {user.role}
                  </span>
                </td>

                {/* Edit button */}
                <td>
                  <button
                    onClick={() => handleEdit(user.id)}
                    className="btn btn-sm btn-primary"
                  >
                    Endre
                  </button>
                </td>

                {/* Delete button */}
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

      {/* Back navigation */}
      <button
        onClick={() => navigate("/admindashboard")}
        className="btn btn-primary btn-sm"
      >
        Tilbake
      </button>
    </div>
  );
}

export default UsersPage;
