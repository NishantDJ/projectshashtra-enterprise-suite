import { useState } from "react";
import { useAuth } from "../context/AuthContext";
import { useNavigate, Link } from "react-router-dom";

export default function Register() {
  const { register } = useAuth();
  const navigate     = useNavigate();

  const [form, setForm]       = useState({ fullName: "", email: "", password: "" });
  const [error, setError]     = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e) =>
    setForm({ ...form, [e.target.name]: e.target.value });

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError("");
    setLoading(true);

    try {
      await register(form);
      navigate("/login"); // redirect to login after success
    } catch (err) {
      setError(err.response?.data?.message || "Registration failed.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2>Create Account</h2>

        {error && <p style={styles.error}>{error}</p>}

        <input
          style={styles.input}
          name="fullName"
          placeholder="Full Name"
          value={form.fullName}
          onChange={handleChange}
        />
        <input
          style={styles.input}
          name="email"
          type="email"
          placeholder="Email"
          value={form.email}
          onChange={handleChange}
        />
        <input
          style={styles.input}
          name="password"
          type="password"
          placeholder="Password"
          value={form.password}
          onChange={handleChange}
        />

        <button style={styles.button} onClick={handleSubmit} disabled={loading}>
          {loading ? "Registering..." : "Register"}
        </button>

        <p>Already have an account? <Link to="/login">Login</Link></p>
      </div>
    </div>
  );
}

const styles = {
  container: { display:"flex", justifyContent:"center", alignItems:"center", height:"100vh" },
  card:      { display:"flex", flexDirection:"column", gap:12, width:320, padding:32, boxShadow:"0 2px 12px rgba(0,0,0,0.15)", borderRadius:8 },
  input:     { padding:"10px 12px", fontSize:14, borderRadius:6, border:"1px solid #ccc" },
  button:    { padding:"10px 12px", background:"#4f46e5", color:"#fff", border:"none", borderRadius:6, cursor:"pointer", fontSize:15 },
  error:     { color:"red", fontSize:13 }
};