import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getUserById, updateUser } from '../../api/adminApi';

function EditUserPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    id: '',
    fullName: '',
    email: '',
    tlfNumber: '',
    address: '',
    role: 'Client'
  });

  useEffect(() => {
    fetchUser();
  }, [id]);

const fetchUser = async () => {
    try {
        setLoading(true);
        setError(null);
        const data = await getUserById(id);
        console.log('Fetched user data:', data); // Debug log
        setFormData({
            id: data.id,
            fullName: data.fullName || '',
            email: data.email || '',
            tlfNumber: data.tlfNumber || '',
            address: data.address || '',
            role: data.role || 'Client'
        });
    } catch (err) {
        console.error('Error fetching user:', err);
        setError('Kunne ikke laste bruker: ' + (err.response?.data?.message || err.message));
    } finally {
        setLoading(false);
    }
};

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
      e.preventDefault();
      try {
          console.log('Submitting data:', formData); // debug log
          await updateUser(id, formData); // send complete formData including role
          navigate('/admin/users');
      } catch (err) {
          console.error('Error updating user:', err);
          setError('Kunne ikke oppdatere bruker: ' + (err.response?.data?.message || err.message));
      }
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Rediger bruker: {formData.fullName}</h1>

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError(null)}></button>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="fullName" className="form-label">Fullt navn</label>
          <input
            type="text"
            className="form-control"
            id="fullName"
            name="fullName"
            value={formData.fullName}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="email" className="form-label">E-post</label>
          <input
            type="email"
            className="form-control"
            id="email"
            name="email"
            value={formData.email}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="tlfNumber" className="form-label">Telefon</label>
          <input
            type="text"
            className="form-control"
            id="tlfNumber"
            name="tlfNumber"
            value={formData.tlfNumber}
            onChange={handleChange}
          />
        </div>

        <div className="mb-3">
          <label htmlFor="address" className="form-label">Adresse</label>
          <input
            type="text"
            className="form-control"
            id="address"
            name="address"
            value={formData.address}
            onChange={handleChange}
          />
        </div>

        <button type="submit" className="btn btn-primary">Lagre endringer</button>
        
        <button
          type="button"
          onClick={() => navigate('/admin/users')}
          className="btn btn-outline-secondary ms-2"
        >
          Avbryt
        </button>
      </form>
    </div>
  );
}

export default EditUserPage;
