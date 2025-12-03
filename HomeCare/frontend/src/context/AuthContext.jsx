// frontend/src/context/AuthContext.jsx
import { createContext, useContext, useState, useEffect } from "react";
import { getCurrentUser, logout } from "../api/authApi";
import { useNavigate } from "react-router-dom";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  // Load user from cookie when app starts
  useEffect(() => {
    async function loadUser() {
      const existing = await getCurrentUser();
      if (existing) setUser(existing);
      setLoading(false);
    }
    loadUser();
  }, []);

  const loginUser = (userData) => {
    setUser(userData);
  };

  const logoutUser = async () => {
    await logout();
    setUser(null);
    navigate("/");         // Redirect to homepage
  };

  return (
    <AuthContext.Provider value={{ user, loginUser, logoutUser }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  return useContext(AuthContext);
}
