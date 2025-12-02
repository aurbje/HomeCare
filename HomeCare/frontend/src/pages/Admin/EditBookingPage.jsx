import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getBookingById, updateBooking, getClientsAndPersonnel } from '../../api/adminApi';

function EditBookingPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [clients, setClients] = useState([]);
  const [personnel, setPersonnel] = useState([]);
  const [formData, setFormData] = useState({
    clientId: '',
    personnelId: '',
    date: '',
    time: '',
    serviceType: '',
    notes: ''
  });

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [bookingData, dropdownData] = await Promise.all([
        getBookingById(id),
        getClientsAndPersonnel()
      ]);

      setFormData({
        clientId: bookingData.clientId || '',
        personnelId: bookingData.personnelId || '',
        date: bookingData.date ? bookingData.date.split('T')[0] : '',
        time: bookingData.time || '',
        serviceType: bookingData.serviceType || '',
        notes: bookingData.notes || ''
      });

      setClients(dropdownData.clients || []);
      setPersonnel(dropdownData.personnel || []);
      setError(null);
    } catch (err) {
      setError('Kunne ikke laste booking');
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
      await updateBooking(id, formData);
      navigate('/admin/bookings');
    } catch (err) {
      setError('Kunne ikke oppdatere booking');
      console.error(err);
    }
  };

  if (loading) {
    return <div className="container py-4">Laster...</div>;
  }

  return (
    <div className="container py-4">
      <h1 className="h4 mb-3">Rediger booking: #{id}</h1>

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError(null)}></button>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="clientId" className="form-label">Kunde</label>
          <select
            className="form-select"
            id="clientId"
            name="clientId"
            value={formData.clientId}
            onChange={handleChange}
            required
          >
            <option value="">-- Velg kunde --</option>
            {clients.map((client) => (
              <option key={client.id} value={client.id}>
                {client.fullName} ({client.email})
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="personnelId" className="form-label">Ansatt</label>
          <select
            className="form-select"
            id="personnelId"
            name="personnelId"
            value={formData.personnelId}
            onChange={handleChange}
          >
            <option value="">-- Velg ansatt --</option>
            {personnel.map((person) => (
              <option key={person.id} value={person.id}>
                {person.fullName} ({person.email})
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="date" className="form-label">Dato</label>
          <input
            type="date"
            className="form-control"
            id="date"
            name="date"
            value={formData.date}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="time" className="form-label">Tid</label>
          <input
            type="text"
            className="form-control"
            id="time"
            name="time"
            value={formData.time}
            onChange={handleChange}
            placeholder="09:00–10:00"
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="serviceType" className="form-label">Tjenestetype</label>
          <select
            className="form-select"
            id="serviceType"
            name="serviceType"
            value={formData.serviceType}
            onChange={handleChange}
            required
          >
            <option value="Cleaning">Cleaning</option>
            <option value="Nursing">Nursing</option>
            <option value="Cooking">Cooking</option>
            <option value="Other">Other</option>
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="notes" className="form-label">Notater</label>
          <textarea
            className="form-control"
            id="notes"
            name="notes"
            rows="3"
            value={formData.notes}
            onChange={handleChange}
          />
        </div>

        <button type="submit" className="btn btn-primary">Lagre endringer</button>
        <button
          type="button"
          onClick={() => navigate('/admin/bookings')}
          className="btn btn-outline-secondary ms-2"
        >
          Avbryt
        </button>
      </form>
    </div>
  );
}

export default EditBookingPage;
