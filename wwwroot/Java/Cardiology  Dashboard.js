Auth.requireLogin();

// Resolves to the logged-in user's own Doctor profile via /api/doctors/me.
// A ?doctorId=X in the URL still works as an override (useful for the seeded demo doctors,
// which aren't linked to any login yet).
const urlParams = new URLSearchParams(window.location.search);
let doctorId = Number(urlParams.get("doctorId")) || null;

let doctor = { name: "", spec: "", email: "", phone: "" };
let appointments = []; // raw AppointmentDetailDto[] from the API
let currentStatusFilter = "all";
let pendingModalAction = null;

function navigate(pageName, clickedNavLink) {
  document.querySelectorAll(".page-section").forEach((p) => p.classList.remove("active"));
  document.querySelectorAll(".left-nav-link").forEach((l) => l.classList.remove("active"));

  document.getElementById("page-" + pageName).classList.add("active");
  if (clickedNavLink) clickedNavLink.classList.add("active");

  const pageRenderers = {
    appts: renderAppointmentsTable,
    patients: renderPatientsList,
    reports: renderReportsPage,
    profile: renderDoctorProfile,
  };
  if (pageRenderers[pageName]) pageRenderers[pageName]();
}

function filterAppts(clickedButton) {
  document.querySelectorAll(".appointments-filter-button").forEach((b) => b.classList.remove("active"));
  clickedButton.classList.add("active");
  currentStatusFilter = clickedButton.dataset.f;
  renderAppointmentsTable();
}

function statusKey(status) {
  return (status || "").toLowerCase();
}

function renderAppointmentsTable() {
  const filteredList =
    currentStatusFilter === "all"
      ? appointments
      : appointments.filter((a) => statusKey(a.status) === currentStatusFilter);

  const tableBody = document.getElementById("apptTbody");

  if (!filteredList.length) {
    tableBody.innerHTML = `
      <tr>
        <td colspan="6">
          <div class="empty-state-container">
            <div class="empty-state-icon">📭</div>
            No appointments found.
          </div>
        </td>
      </tr>`;
    return;
  }

  tableBody.innerHTML = filteredList
    .map((appointment) => {
      const status = statusKey(appointment.status);
      const statusBadgeHTML =
        {
          pending: `<span class="appointment-status-badge appointment-status-badge--pending"><span class="appointment-status-dot"></span>Pending</span>`,
          confirmed: `<span class="appointment-status-badge appointment-status-badge--confirmed"><span class="appointment-status-dot"></span>Confirmed</span>`,
          completed: `<span class="appointment-status-badge appointment-status-badge--completed"><span class="appointment-status-dot"></span>Completed</span>`,
          cancelled: `<span class="appointment-status-badge appointment-status-badge--cancelled"><span class="appointment-status-dot"></span>Cancelled</span>`,
        }[status] || "";

      let actionButtonsHTML = "";
      if (status === "pending") {
        actionButtonsHTML = `
        <button class="row-action-button row-action-button--confirm"  onclick="openConfirmationModal(${appointment.appointmentId}, 'confirm')">Confirm</button>
        <button class="row-action-button row-action-button--complete" onclick="openConfirmationModal(${appointment.appointmentId}, 'complete')">Complete</button>
        <button class="row-action-button row-action-button--cancel"   onclick="openConfirmationModal(${appointment.appointmentId}, 'cancel')">Cancel</button>`;
      } else if (status === "confirmed") {
        actionButtonsHTML = `
        <button class="row-action-button row-action-button--complete" onclick="openConfirmationModal(${appointment.appointmentId}, 'complete')">Complete</button>
        <button class="row-action-button row-action-button--cancel"   onclick="openConfirmationModal(${appointment.appointmentId}, 'cancel')">Cancel</button>`;
      } else {
        actionButtonsHTML = `<span style="color:#94a3b8;font-size:13px;">&mdash;</span>`;
      }

      return `
      <tr>
        <td class="patient-name">${appointment.patient?.fullName || "—"}</td>
        <td>${appointment.appointmentDate?.split("T")[0]}</td>
        <td>${(appointment.appointmentTime || "").slice(0, 5)}</td>
        <td>${appointment.message || "—"}</td>
        <td>${statusBadgeHTML}</td>
        <td><div class="appointment-row-actions">${actionButtonsHTML}</div></td>
      </tr>`;
    })
    .join("");
}

const MODAL_CONFIG = {
  confirm: {
    icon: "🔵", title: "Confirm Appointment", submitButtonText: "Confirm",
    submitButtonColor: "modal-submit-button--blue", showMessageField: false,
  },
  complete: {
    icon: "✅", title: "Mark as Completed", submitButtonText: "Mark Complete",
    submitButtonColor: "modal-submit-button--green", showMessageField: false,
  },
  cancel: {
    icon: "❌", title: "Cancel Appointment", submitButtonText: "Cancel Appointment",
    submitButtonColor: "modal-submit-button--red", showMessageField: false,
  },
};

function openConfirmationModal(appointmentId, actionType) {
  const appointment = appointments.find((a) => a.appointmentId === appointmentId);
  if (!appointment) return;

  pendingModalAction = { id: appointmentId, action: actionType };

  const config = MODAL_CONFIG[actionType];
  document.getElementById("mIcon").textContent = config.icon;
  document.getElementById("mTitle").textContent = config.title;
  document.getElementById("mDesc").textContent = "";
  document.getElementById("mInfo").innerHTML =
    `<strong>${appointment.patient?.fullName || "—"}</strong>
     ${appointment.appointmentDate?.split("T")[0]} &middot; ${(appointment.appointmentTime || "").slice(0, 5)} &middot; ${appointment.message || ""}`;

  document.getElementById("mReasonWrap").style.display = "none";

  const submitButton = document.getElementById("mActionBtn");
  submitButton.textContent = config.submitButtonText;
  submitButton.className = "modal-submit-button " + config.submitButtonColor;

  document.getElementById("modalBg").classList.add("open");
}

function closeModal() {
  document.getElementById("modalBg").classList.remove("open");
  pendingModalAction = null;
}

document.getElementById("modalBg").addEventListener("click", function (event) {
  if (event.target === this) closeModal();
});

async function doAction() {
  if (!pendingModalAction) return;
  const { id, action } = pendingModalAction;

  const endpointMap = { confirm: "confirm", complete: "complete", cancel: "cancel" };
  const toastTypeMap = { confirm: "info", complete: "success", cancel: "error" };
  const toastMessageMap = { confirm: "Appointment confirmed", complete: "Appointment marked as completed", cancel: "Appointment cancelled" };

  try {
    await Api.put(`/appointments/${id}/${endpointMap[action]}`);
    closeModal();
    await loadAppointments();
    showToastNotification(toastTypeMap[action], toastMessageMap[action]);
  } catch (err) {
    closeModal();
    showToastNotification("error", err.message || "Something went wrong.");
  }
}

function renderPatientsList() {
  const listContainer = document.getElementById("patientList");

  // Derive the unique set of patients from this doctor's appointments
  const patientMap = new Map();
  appointments.forEach((a) => {
    if (a.patient) patientMap.set(a.patient.id, a.patient);
  });
  const patients = Array.from(patientMap.values());

  if (!patients.length) {
    listContainer.innerHTML = `<div class="empty-state-container"><div class="empty-state-icon">📭</div>No patients yet.</div>`;
    return;
  }

  listContainer.innerHTML = patients
    .map((patient, index) => {
      const initials = (patient.fullName || "?").split(" ").map((p) => p[0]).slice(0, 2).join("").toUpperCase();
      return `
    <div>
      <div class="patient-list-row" onclick="togglePatientDetails(${index}, this)">
        <div class="patient-initials-avatar">${initials}</div>
        <div class="patient-basic-info">
          <div class="patient-full-name">${patient.fullName}</div>
        </div>
        <div class="patient-contact-details">
          <span>&#9993; ${patient.email || "—"}</span>
          <span>&#9742; ${patient.phone || "—"}</span>
        </div>
        <span class="patient-expand-arrow">&#8250;</span>
      </div>
      <div class="patient-expanded-details" id="patient-details-${index}">
        <div class="patient-details-grid">
          <div><span class="patient-detail-label">Email: </span>${patient.email || "—"}</div>
          <div><span class="patient-detail-label">Phone: </span>${patient.phone || "—"}</div>
        </div>
      </div>
    </div>`;
    })
    .join("");
}

function togglePatientDetails(index, clickedRow) {
  document.getElementById("patient-details-" + index).classList.toggle("open");
  clickedRow.classList.toggle("open");
}

function renderReportsPage() {
  const totalAppointments = appointments.length;
  const completedCount = appointments.filter((a) => statusKey(a.status) === "completed").length;
  const pendingCount = appointments.filter((a) => statusKey(a.status) === "pending").length;
  const confirmedCount = appointments.filter((a) => statusKey(a.status) === "confirmed").length;
  const completionPercentage = totalAppointments ? Math.round((completedCount / totalAppointments) * 100) : 0;
  const uniquePatients = new Set(appointments.filter((a) => a.patient).map((a) => a.patient.id)).size;

  document.getElementById("rTotal").textContent = totalAppointments;
  document.getElementById("rCompleted").textContent = completedCount;
  document.getElementById("rPending").textContent = pendingCount;
  document.getElementById("rPatients").textContent = uniquePatients;

  document.getElementById("progBar").style.width = completionPercentage + "%";
  document.getElementById("progPct").textContent = completionPercentage + "%";

  document.getElementById("legCompleted").textContent = "Completed: " + completedCount;
  document.getElementById("legPending").textContent = "Pending: " + pendingCount;
  document.getElementById("legConfirmed").textContent = "Confirmed: " + confirmedCount;
}

function renderDoctorProfile() {
  document.getElementById("profBannerName").textContent = doctor.name;
  document.getElementById("profBannerSpec").textContent = doctor.spec;
  document.getElementById("pfName").textContent = doctor.name;
  document.getElementById("pfSpec").textContent = doctor.spec;
  document.getElementById("pfEmail").textContent = doctor.email;
  document.getElementById("pfPhone").textContent = doctor.phone;

  const sidebarName = document.querySelector(".left-nav-app-name");
  const sidebarSpec = document.querySelector(".left-nav-app-subtitle");
  if (sidebarName) sidebarName.textContent = doctor.name;
  if (sidebarSpec) sidebarSpec.textContent = doctor.spec;
}

function toggleEditForm() {
  const editFormEl = document.getElementById("editForm");
  editFormEl.classList.toggle("open");

  if (editFormEl.classList.contains("open")) {
    document.getElementById("ef-name").value = doctor.name;
    document.getElementById("ef-spec").value = doctor.spec;
    document.getElementById("ef-email").value = doctor.email;
    document.getElementById("ef-phone").value = doctor.phone;
  }
}

async function saveProfile() {
  const newName = document.getElementById("ef-name").value.trim();
  const newPhone = document.getElementById("ef-phone").value.trim();
  // Email + Specialization aren't editable through this endpoint yet (see DoctorsController.Update).

  try {
    const updated = await Api.put(`/doctors/${doctorId}`, {
      name: newName,
      phone: newPhone,
      description: null,
      consultationFees: 0,
    });
    doctor.name = updated.name;
    doctor.phone = updated.phone;

    renderDoctorProfile();
    document.getElementById("editForm").classList.remove("open");
    showToastNotification("success", "Profile updated successfully");
  } catch (err) {
    showToastNotification("error", err.message || "Could not update profile");
  }
}

function showToastNotification(type, message) {
  const iconMap = { success: "✅", error: "❌", info: "ℹ️" };
  const container = document.getElementById("toasts");

  const toastEl = document.createElement("div");
  toastEl.className = `toast-notification toast-notification--${type}`;
  toastEl.innerHTML = `<span>${iconMap[type] || ""}</span>${message}`;
  container.appendChild(toastEl);

  setTimeout(() => {
    toastEl.classList.add("removing");
    setTimeout(() => toastEl.remove(), 300);
  }, 3000);
}

async function loadDoctorInfo() {
  try {
    if (doctorId) {
      const doc = await Api.get(`/doctors/${doctorId}`);
      doctor = { name: doc.name, spec: doc.specialtyName, email: doc.email, phone: doc.phone };
    } else {
      const doc = await Api.get(`/doctors/me`);
      doctorId = doc.id;
      doctor = { name: doc.name, spec: doc.specialtyName, email: doc.email, phone: doc.phone };
    }
  } catch (err) {
    document.querySelector(".main-content-area").innerHTML = `
      <div class="empty-state-container" style="padding:60px 20px;">
        <div class="empty-state-icon">🚫</div>
        <p>${err.message.includes("No doctor profile")
          ? "This account doesn't have a doctor profile yet. Ask an admin to grant you doctor access from the Admin Panel."
          : "Could not load your dashboard: " + err.message}</p>
      </div>`;
    throw err;
  }
}

async function loadAppointments() {
  try {
    appointments = await Api.get(`/appointments?doctorId=${doctorId}`);
  } catch (err) {
    console.error("Could not load appointments:", err.message);
    appointments = [];
  }
  renderAppointmentsTable();
  renderPatientsList();
  renderReportsPage();
}

(async function init() {
  try {
    await loadDoctorInfo();
  } catch (err) {
    return; // error message already shown by loadDoctorInfo
  }
  renderDoctorProfile();
  await loadAppointments();
})();
