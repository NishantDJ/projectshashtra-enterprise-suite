import axios from "axios";

// ✅ FIXED: removed extra space
const BASE_URL = "https://localhost:7200/api";

// Main axios instance
const axiosInstance = axios.create({
  baseURL: BASE_URL,
});

// ── Request Interceptor ──
// Attach access token to every request
axiosInstance.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");

    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }

    return config;
  },
  (error) => Promise.reject(error)
);

// ── Response Interceptor ──
// Handle token refresh on 401
axiosInstance.interceptors.response.use(
  (response) => response,

  async (error) => {
    const originalRequest = error.config;

    // prevent infinite loop
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;

      try {
        const refreshToken = localStorage.getItem("refreshToken");
        if (!refreshToken) throw new Error("No refresh token");

        // ✅ use SAME axios instance OR base axios carefully
        const res = await axios.post(`${BASE_URL}/Auth/refresh`, {
          refreshToken,
        });

        const { token, refreshToken: newRefreshToken } = res.data;

        // store new tokens
        localStorage.setItem("token", token);
        localStorage.setItem("refreshToken", newRefreshToken);

        // retry original request
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return axiosInstance(originalRequest);

      } catch (refreshError) {
        console.error("Refresh failed:", refreshError);

        // force logout
        localStorage.removeItem("token");
        localStorage.removeItem("refreshToken");
        localStorage.removeItem("user");

        window.location.href = "/login";
      }
    }

    return Promise.reject(error);
  }
);

export default axiosInstance;