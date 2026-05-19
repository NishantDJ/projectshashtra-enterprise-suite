import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import axiosInstance from "../api/axiosInstance";
import { useNavigate } from "react-router-dom";

export default function Dashboard() {
  const { user, logout } = useAuth();
  const navigate         = useNavigate();
  const [products, setProducts] = useState([]);
  const [error, setError]       = useState("");

  useEffect(() => {
    // Fetch protected data on mount
    const fetchProducts = async () => {
      try {
        const res = await axiosInstance.get("/Product");
        setProducts(res.data);
      } catch {
        setError("Failed to load products.");
      }
    };
    fetchProducts();
  }, []);

  const handleLogout = async () => {
    await logout();
    navigate("/login");
  };

  return (
    <div style={styles.container}>
      <div style={styles.header}>
        <div>
          <h2>Welcome, {user?.fullName} 👋</h2>
          <p style={styles.role}>Role: <strong>{user?.role}</strong></p>
        </div>
        <button style={styles.logoutBtn} onClick={handleLogout}>Logout</button>
      </div>

      <h3>Products</h3>
      {error && <p style={styles.error}>{error}</p>}

      <ul style={styles.list}>
        {products.map((p, i) => (
          <li key={i} style={styles.item}>{p.name}</li>
        ))}
      </ul>

      {/* Admin-only UI */}
      {user?.role === "Admin" && (
        <button style={styles.adminBtn}>+ Add Product (Admin only)</button>
      )}
    </div>
  );
}

const styles = {
  container: { maxWidth:720, margin:"40px auto", padding:24 },
  header:    { display:"flex", justifyContent:"space-between", alignItems:"center", marginBottom:24 },
  role:      { color:"#666", fontSize:13 },
  logoutBtn: { padding:"8px 16px", background:"#ef4444", color:"#fff", border:"none", borderRadius:6, cursor:"pointer" },
  adminBtn:  { marginTop:16, padding:"10px 20px", background:"#4f46e5", color:"#fff", border:"none", borderRadius:6, cursor:"pointer" },
  list:      { listStyle:"none", padding:0 },
  item:      { padding:"10px 14px", marginBottom:8, background:"#f3f4f6", borderRadius:6 },
  error:     { color:"red" }
};