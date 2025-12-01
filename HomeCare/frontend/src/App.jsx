import React, { useEffect } from 'react';
import api from './api/api';

function App() {
  useEffect(() => {
    async function loadBookings() {
      try {
        const res = await api.get('/booking');
        console.log('Bookings:', res.data);
      } catch (err) {
        console.error('Error fetching bookings:', err);
      }
    }

    loadBookings();
  }, []);

  return (
    <div className="container mt-4">
      <h1>HomeCare Dashboard</h1>
      <p>Open the browser console to view booking data.</p>
    </div>
  );
}

export default App;
