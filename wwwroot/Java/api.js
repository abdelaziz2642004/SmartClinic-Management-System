const API_BASE = "/api";

const Auth = {
  TOKEN_KEY: "sc_token",

  getToken() {
    return localStorage.getItem(this.TOKEN_KEY);
  },

  setToken(token) {
    localStorage.setItem(this.TOKEN_KEY, token);
  },

  logout() {
    localStorage.removeItem(this.TOKEN_KEY);
    window.location.href = "index.html";
  },

  isLoggedIn() {
    return !!this.getToken();
  },

  getUser() {
    const token = this.getToken();
    if (!token) return null;
    try {
      const payload = token.split(".")[1];
      const json = decodeURIComponent(
        atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
          .split("")
          .map((c) => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
          .join(""),
      );
      const claims = JSON.parse(json);
      return {
        id: claims[
          "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
        ],
        email:
          claims[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"
          ],
        firstName:
          claims[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname"
          ],
        lastName:
          claims[
            "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname"
          ],
        role: claims[
          "http://schemas.microsoft.com/ws/2008/06/identity/claims/role"
        ],
      };
    } catch (e) {
      return null;
    }
  },

  requireLogin() {
    if (!this.isLoggedIn()) {
      window.location.href = "index.html";
      return false;
    }
    return true;
  },
};

const Api = {
  async request(path, options = {}) {
    const headers = Object.assign(
      { "Content-Type": "application/json" },
      options.headers || {},
    );

    const token = Auth.getToken();
    if (token) headers["Authorization"] = `Bearer ${token}`;

    const res = await fetch(`${API_BASE}${path}`, {
      ...options,
      headers,
    });

    let data = null;
    const text = await res.text();
    if (text) {
      try {
        data = JSON.parse(text);
      } catch (e) {
        data = text;
      }
    }

    if (!res.ok) {
      let message = `Request failed (${res.status})`;
      if (Array.isArray(data)) {
        message = data
          .map((e) => e.description || e.code || JSON.stringify(e))
          .join(" | ");
      } else if (data && typeof data === "object") {
        if (data.error || data.message || data.title) {
          message = data.error || data.message || data.title;
        } else if (data.errors && typeof data.errors === "object") {
          // ASP.NET ModelState validation errors: { field: ["msg1", "msg2"] }
          message = Object.values(data.errors).flat().join(" | ");
        }
      } else if (typeof data === "string" && data) {
        message = data;
      }
      throw new Error(message);
    }

    return data;
  },

  get(path) {
    return this.request(path, { method: "GET" });
  },
  post(path, body) {
    return this.request(path, { method: "POST", body: JSON.stringify(body) });
  },
  put(path, body) {
    return this.request(path, {
      method: "PUT",
      body: body ? JSON.stringify(body) : undefined,
    });
  },
  del(path) {
    return this.request(path, { method: "DELETE" });
  },

  async uploadFile(path, file, fieldName = "file") {
    const formData = new FormData();
    formData.append(fieldName, file);

    const token = Auth.getToken();
    const headers = {};
    if (token) headers["Authorization"] = `Bearer ${token}`;

    const res = await fetch(`${API_BASE}${path}`, {
      method: "POST",
      headers,
      body: formData,
    });

    let data = null;
    const text = await res.text();
    if (text) {
      try {
        data = JSON.parse(text);
      } catch (e) {
        data = text;
      }
    }

    if (!res.ok) {
      const message =
        (data && (data.error || data.message || data.title)) ||
        (typeof data === "string" ? data : null) ||
        `Upload failed (${res.status})`;
      throw new Error(message);
    }

    return data;
  },
};

function renderNavAuthState() {
  const profileIcon = document.getElementById("profileIcon");
  const loggedOutNav = document.getElementById("loggedOutNav");

  if (!profileIcon) return;

  if (!Auth.isLoggedIn()) {
    if (loggedOutNav) {
      loggedOutNav.style.display = "";
      profileIcon.style.display = "none";
    } else {
      profileIcon.style.display = "";
    }
    profileIcon.innerHTML = `
      <li class="nav-item">
        <a class="nav-link" href="index.html"><i class="fas fa-right-to-bracket me-1"></i> Login</a>
      </li>`;
    return;
  }

  if (loggedOutNav) loggedOutNav.style.display = "none";
  profileIcon.style.display = "";

  const user = Auth.getUser();
  const name = user && user.firstName ? user.firstName : "Account";

  const logoutLink = profileIcon.querySelector(".text-danger");
  if (logoutLink) {
    logoutLink.setAttribute("href", "#");
    logoutLink.addEventListener("click", (e) => {
      e.preventDefault();
      Auth.logout();
    });
  }

  const userMenu = document.getElementById("userMenu");
  if (userMenu) {
    userMenu.innerHTML = `<i class="fas fa-user-circle fa-lg me-1"></i> ${name}`;
  }

  const dashboardLink = document.getElementById("Dashboard");
  if (dashboardLink) {
    dashboardLink.style.display = user && user.role === "Doctor" ? "" : "none";
  }
  const adminLink = document.getElementById("AdminNav");
  if (adminLink) {
    adminLink.style.display = user && user.role === "Admin" ? "" : "none";
  }
}

document.addEventListener("DOMContentLoaded", renderNavAuthState);
