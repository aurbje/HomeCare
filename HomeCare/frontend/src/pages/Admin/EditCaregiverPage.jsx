import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getCaregiverById, updateCaregiver } from '../../api/adminApi';

function EditCaregiverPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [formData, setFormData] = useState({
    fullName: '',
    userName: '',
    email: '',
    tlfNumber: '',
    address: '',
    personnelId: ''
  });

  useEffect(() => {
    fetchCaregiver();
  }, [id]);

  const fetchCaregiver = async () => {
    try {
      setLoading(true);
      const data = await getCaregiverById(id);
      setFormData({
        fullName: data.fullName || '',
        userName: data.userName || '',
        email: data.email || '',
        tlfNumber: data.tlfNumber || '',
        address: data.address || '',
        personnelId: data.personnelId || ''
      });
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste ansatt');
      console.error(err);
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
      await updateCaregiver(id, formData);
      navigate('/admin/caregivers');
    } catch (err) {
      setError('Kunne ikke oppdatere ansatt');
      console.error(err);
    }
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Rediger ansatt: {formData.fullName}</h1>

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
          <label htmlFor="userName" className="form-label">Brukernavn</label>
          <input
            type="text"
            className="form-control"
            id="userName"
            name="userName"
            value={formData.userName}
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

        <div className="mb-3">
          <label htmlFor="caregiverId" className="form-label">Ansatt ID</label>
          <input
            type="number"
            className="form-control"
            id="caregiverId"
            name="caregiverId"
            value={formData.caregiverId}
            onChange={handleChange}
            readOnly
          />
          <small className="text-muted">Dette feltet kan ikke endres.</small>
        </div>

        <button type="submit" className="btn btn-primary">Lagre endringer</button>
        <button
          type="button"
          onClick={() => navigate('/admin/caregiver')}
          className="btn btn-outline-secondary ms-2"
        >
          Avbryt
        </button>
      </form>
    </div>
  );
}

export default EditCaregiverPage;
