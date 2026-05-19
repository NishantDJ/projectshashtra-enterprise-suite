import { Navigate } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

// Protect by login
const PrivateRoute = ({ children }) => {
  const { user } = useAuth();
  return user ? children : <Navigate to="/login" replace />;
};

// Protect by role
export const RoleRoute = ({ children, role }) => {
  const { user } = useAuth();
  if (!user)              return <Navigate to="/login"     replace />;
  if (user.role !== role) return <Navigate to="/dashboard" replace />;
  return children;
};

export default PrivateRoute;