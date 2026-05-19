import { createContext, useContext, useState } from "react";
import axiosInstance from "../api/axiosInstance";

const AuthContext = createContext(null);

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  // Register
  const register = async (formData) => {
    const res = await axiosInstance.post("/Auth/register", formData);
    return res.data;
  };

  // Login — store tokens and user info
  const login = async (formData) => {
    const payload = {
      username: formData.username,
      password: formData.password
    };
  
    const res = await axiosInstance.post("/Auth/login", payload);
  
    const { token, refreshToken, fullName, username, role } = res.data;
  
    localStorage.setItem("token", token);
    localStorage.setItem("refreshToken", refreshToken);
  
    const userData = { fullName, username, role };
    localStorage.setItem("user", JSON.stringify(userData));
    setUser(userData);
  
    return res.data;
  };

  // Logout — revoke refresh token + clear storage
  const logout = async () => {
    try {
      const refreshToken = localStorage.getItem("refreshToken");
      await axiosInstance.post("/auth/logout", { refreshToken });
    } catch {
      // proceed with logout even if API call fails
    } finally {
      localStorage.removeItem("token");
      localStorage.removeItem("refreshToken");
      localStorage.removeItem("user");
      setUser(null);
    }
  };

  return (
    <AuthContext.Provider value={{ user, login, logout, register }}>
      {children}
    </AuthContext.Provider>
  );
};

// Custom hook
export const useAuth = () => useContext(AuthContext);