import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getBookingById, updateBooking, getUsers, getCaregivers } from '../../api/adminApi';

function EditBookingPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [clients, setClients] = useState([]);
  const [caregivers, setCaregivers] = useState([]);
  const [formData, setFormData] = useState({
    userId: '',
    caregiverId: '',
    dateTime: '',
    timeSlotId: '',
    categoryId: '',
    notes: ''
  });

  useEffect(() => {
    fetchData();
  }, [id]);

  const fetchData = async () => {
    try {
      setLoading(true);
      const [bookingData, usersData, caregiversData] = await Promise.all([
        getBookingById(id),
        getUsers(''),
        getCaregivers('')
      ]);

      console.log("Booking data:", bookingData); // Debug log

      // Extract date and time from the DateTime property
      const dateTimeValue = bookingData.dateTime ? new Date(bookingData.dateTime).toISOString().slice(0, 16) : '';

      setFormData({
        userId: bookingData.userId || '',
        caregiverId: bookingData.caregiverId || '',
        dateTime: dateTimeValue,
        timeSlotId: bookingData.timeSlotId || '',
        categoryId: bookingData.categoryId || '',
        notes: bookingData.notes || ''
      });

      setClients(usersData || []);
      setCaregivers(caregiversData || []);
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
      
      const updateData = {
        userId: parseInt(formData.userId),
        caregiverId: formData.caregiverId ? parseInt(formData.caregiverId) : null,
        dateTime: formData.dateTime,
        timeSlotId: parseInt(formData.timeSlotId),
        categoryId: parseInt(formData.categoryId),
        notes: formData.notes || null
      };
      
      await updateBooking(id, updateData);
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
      <h1 className="h4 mb-3">Rediger booking: {id}</h1>

      {error && (
        <div className="alert alert-danger alert-dismissible fade show" role="alert">
          {error}
          <button type="button" className="btn-close" onClick={() => setError(null)}></button>
        </div>
      )}

      <form onSubmit={handleSubmit}>
        <div className="mb-3">
          <label htmlFor="userId" className="form-label">Bruker</label>
          <select
            className="form-select"
            id="userId"
            name="userId"
            value={formData.userId}
            onChange={handleChange}
            required
          >
            <option value=""> Velg bruker </option>
            {clients.map((client) => (
              <option key={client.id} value={client.id}>
                {client.fullName} ({client.email})
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="caregiverId" className="form-label">Ansatt</label>
          <select
            className="form-select"
            id="caregiverId"
            name="caregiverId"
            value={formData.caregiverId}
            onChange={handleChange}
          >
            <option value=""> Velg ansatt</option>
            {caregivers.map((caregiver) => (
              <option key={caregiver.id} value={caregiver.id}>
                {caregiver.fullName} ({caregiver.email})
              </option>
            ))}
          </select>
        </div>

        <div className="mb-3">
          <label htmlFor="dateTime" className="form-label">Dato</label>
          <input
            type="date"
            className="form-control"
            id="dateTime"
            name="dateTime"
            value={formData.date}
            onChange={handleChange}
            required
          />
        </div>

        <div className="mb-3">
          <label htmlFor="timeSlotId" className="form-label">Tid</label>
          <input
            type="number"
            className="form-control"
            id="timeSlotId"
            name="timeSlotId"
            value={formData.timeSlotId}
            onChange={handleChange}
            required
          />
          <small className="text-muted">Må matche ledig Tidspunkt i databasen</small>
        </div>

        <div className="mb-3">
          <label htmlFor="categoryId" className="form-label">Kategori</label>
          <input
            type="number"
            className="form-control"
            id="categoryId"
            name="categoryId"
            value={formData.categoryId}
            onChange={handleChange}
            required
          />
          
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