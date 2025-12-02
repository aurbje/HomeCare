import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { forgotPassword } from '../../api/accountApi';

function ForgotPasswordPage() {
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);
    setError(null);

    try {
      await forgotPassword(email);
      setSuccess(true);
    } catch (err) {
      setError(err.response?.data?.message || 'Kunne ikke sende tilbakestillingslenke');
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
            <h4>E-post sendt!</h4>
            <p>Vi har sendt deg en lenke for å tilbakestille passordet til {email}.</p>
            <p>Sjekk innboksen din og følg instruksjonene.</p>
          </div>
          <button
            onClick={() => navigate('/account/login')}
            className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg"
          >
            Tilbake til innlogging
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="signup-container container">
      <div className="signup-card">
        <button
          onClick={() => navigate('/account/login')}
          className="back-btn"
          style={{ background: 'none', border: 'none', color: '#007bff', cursor: 'pointer' }}
        >
          ← Tilbake til innlogging
        </button>

        <h2 className="text-center mb-4">Glemt passord?</h2>
        <p className="text-center text-muted mb-4">
          Skriv inn din e-postadresse så sender vi deg en lenke for å tilbakestille passordet.
        </p>

        {error && (
          <div className="alert alert-danger alert-dismissible fade show" role="alert">
            {error}
            <button type="button" className="btn-close" onClick={() => setError(null)}></button>
          </div>
        )}

        <form onSubmit={handleSubmit} noValidate>
          <div className="mb-4">
            <label htmlFor="email" className="form-label">E-postadresse</label>
            <input
              type="email"
              className="form-control"
              id="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="F.eks. anna@epost.no"
              required
            />
          </div>

          <button
            type="submit"
            className="btn btn-main btn-lg px-4 py-2 bg-green shadow-lg"
            disabled={loading}
          >
            {loading ? 'Sender...' : 'Send tilbakestillingslenke'}
          </button>
        </form>
      </div>
    </div>
  );
}

export default ForgotPasswordPage;
