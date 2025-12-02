import React, { useState } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { resetPassword } from '../../api/accountApi';

function ResetPasswordPage() {
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [formData, setFormData] = useState({
    password: '',
    confirmPassword: ''
  });

  // Get token and email from URL query parameters
  const token = searchParams.get('token') || '';
  const email = searchParams.get('email') || '';

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    if (formData.password !== formData.confirmPassword) {
      setError('Passordene matcher ikke');
      return;
    }

    if (formData.password.length < 6) {
      setError('Passordet må være minst 6 tegn');
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await resetPassword(email, token, formData.password, formData.confirmPassword);
      setSuccess(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Kunne ikke tilbakestille passord');
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  if (success) {
    return (
      <div className="signup-container container">
        <div className="signup-card">
          <div className="alert alert-success">
            <h4>Passord tilbakestilt!</h4>
            <p>Ditt passord har blitt oppdatert. Du kan nå logge inn med ditt nye passord.</p>
          </div>
          <button
            onClick={() => navigate('/account/login')}
            className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg"
          >
            Gå til innlogging
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <h2 className="text-center mb-4">Tilbakestill passord</h2>
        <p className="text-center text-muted mb-4">
          Skriv inn ditt nye passord.
        </p>

        {error && (
          <div className="alert alert-danger alert-dismissible fade show" role="alert">
            {error}
            <button type="button" className="btn-close" onClick={() => setError(null)}></button>
          </div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-3">
            <label htmlFor="password" className="form-label">Nytt passord</label>
            <input
              type="password"
              className="form-control"
              id="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              placeholder="Minimum 6 tegn"
              required
            />
          </div>

          <div className="mb-4">
            <label htmlFor="confirmPassword" className="form-label">Bekreft passord</label>
            <input
              type="password"
              className="form-control"
              id="confirmPassword"
              name="confirmPassword"
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="Skriv inn passordet på nytt"
              required
            />
          </div>

          <button
            type="submit"
            className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg"
            disabled={loading}
          >
            {loading ? 'Tilbakestiller...' : 'Tilbakestill passord'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default ResetPasswordPage;
