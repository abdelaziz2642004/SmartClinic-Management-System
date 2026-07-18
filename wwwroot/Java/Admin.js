Auth.requireLogin();

const user = Auth.getUser();
if (!user || user.role !== "Admin") {
  document.querySelector(".main-content-area").innerHTML = `
    <div class="empty-state-container" style="padding:60px 20px;">
      <div class="empty-state-icon">🚫</div>
      <p>You don't have access to the admin panel.</p>
    </div>`;
  throw new Error("Not an admin");
}

let specialties = [];
let users = [];

function showToast(type, message) {
  const iconMap = { success: "✅", error: "❌", info: "ℹ️" };
  const container = document.getElementById("toasts");
  const el = document.createElement("div");
  el.className = `toast-notification toast-notification--${type}`;
  el.innerHTML = `<span>${iconMap[type] || ""}</span>${message}`;
  container.appendChild(el);
  setTimeout(() => {
    el.classList.add("removing");
    setTimeout(() => el.remove(), 300);
  }, 3000);
}

function roleBadges(roles) {
  if (!roles.length)
    return `<span class="role-badge role-badge--patient">Patient</span>`;
  const classMap = { Admin: "admin", Doctor: "doctor", Patient: "patient" };
  return roles
    .map(
      (r) =>
        `<span class="role-badge role-badge--${classMap[r] || "patient"}">${r}</span>`,
    )
    .join("");
}

function renderUsers() {
  const tbody = document.getElementById("usersTbody");

  if (!users.length) {
    tbody.innerHTML = `<tr><td colspan="5">No users found.</td></tr>`;
    return;
  }

  tbody.innerHTML = users
    .map((u, index) => {
      const isDoctor = u.roles.includes("Doctor");
      const isAdmin = u.roles.includes("Admin");

      let actions = "";
      if (!isAdmin) {
        actions += `<button class="admin-action-btn admin-action-btn--admin" onclick="changeRole('${u.id}','Admin')">Make Admin</button>`;
      }
      if (!isDoctor && !isAdmin) {
        actions += `<button class="admin-action-btn admin-action-btn--promote" onclick="togglePromoteForm(${index})">Promote to Doctor</button>`;
      }
      if (isAdmin) {
        actions += `<button class="admin-action-btn admin-action-btn--patient" onclick="changeRole('${u.id}','Patient')">Make Patient</button>`;
      }
      if (isDoctor) {
        actions += `
          <span style="color:#94a3b8;font-size:12px;display:block;margin-bottom:4px;">Doctor profile #${u.doctorId}</span>
          <img src="${u.doctorImagePath ? u.doctorImagePath : "images/doctor-placeholder.png"}"
               style="width:36px;height:36px;border-radius:50%;object-fit:cover;vertical-align:middle;margin-right:6px;"
               onerror="this.style.display='none'" />
          <label class="admin-action-btn admin-action-btn--promote" style="cursor:pointer;display:inline-block;">
            📷 Upload Photo
            <input type="file" accept=".jpg,.jpeg,.png,.webp" style="display:none;" onchange="uploadDoctorPhoto(${u.doctorId}, this)" />
          </label>`;
      }

      const specialtyOptions = specialties
        .map((s) => `<option value="${s.specialtyID}">${s.name}</option>`)
        .join("");

      return `
      <tr>
        <td>${u.fullName || "—"}</td>
        <td>${u.email}</td>
        <td>${u.phone || "—"}</td>
        <td>${roleBadges(u.roles)}</td>
        <td>
          ${actions}
          <div class="promote-form" id="promote-form-${index}">
            <select id="promote-specialty-${index}">${specialtyOptions}</select>
            <input type="number" id="promote-fee-${index}" placeholder="Consultation fee" style="width:130px;" />
            <button class="admin-action-btn admin-action-btn--promote" onclick="confirmPromote('${u.id}', ${index})">Confirm</button>
          </div>
        </td>
      </tr>`;
    })
    .join("");
}

function togglePromoteForm(index) {
  document.getElementById(`promote-form-${index}`).classList.toggle("open");
}

async function changeRole(userId, role) {
  try {
    await Api.put(`/admin/users/${userId}/role`, { role });
    showToast("success", `Role updated to ${role}`);
    await loadUsers();
  } catch (err) {
    showToast("error", err.message);
  }
}

async function confirmPromote(userId, index) {
  const specialtyId = Number(
    document.getElementById(`promote-specialty-${index}`).value,
  );
  const fee = Number(document.getElementById(`promote-fee-${index}`).value);

  if (!specialtyId || !fee) {
    showToast(
      "error",
      "Please choose a specialty and enter a consultation fee.",
    );
    return;
  }

  try {
    await Api.post(`/admin/users/${userId}/promote-doctor`, {
      specialtyID: specialtyId,
      consultationFees: fee,
      description: "",
    });
    showToast("success", "User promoted to Doctor");
    await loadUsers();
  } catch (err) {
    showToast("error", err.message);
  }
}
async function uploadDoctorPhoto(doctorId, inputEl) {
  const file = inputEl.files[0];
  if (!file) return;

  try {
    await Api.uploadFile(`/doctors/${doctorId}/photo`, file);
    showToast("success", "Photo uploaded");
    await loadUsers();
  } catch (err) {
    showToast("error", err.message);
  }
}
async function loadUsers() {
  try {
    [specialties, users] = await Promise.all([
      specialties.length
        ? Promise.resolve(specialties)
        : Api.get("/specialties"),
      Api.get("/admin/users"),
    ]);
    renderUsers();
  } catch (err) {
    document.getElementById("usersTbody").innerHTML =
      `<tr><td colspan="5" style="color:#c0392b;">Could not load users: ${err.message}</td></tr>`;
  }
}

loadUsers();
