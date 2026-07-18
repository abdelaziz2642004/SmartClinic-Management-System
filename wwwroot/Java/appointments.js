Auth.requireLogin();

async function loadAppointments() {
  document.getElementById("loading").classList.remove("d-none");
  try {
    const data = await Api.get("/appointments?mine=true");
    renderTable(data);
  } catch (e) {
    document.getElementById("appointmentsBody").innerHTML =
      `<tr><td colspan="8" class="text-danger text-center">Could not load appointments: ${e.message}</td></tr>`;
  }
  document.getElementById("loading").classList.add("d-none");
}

function renderTable(data) {
  const tbody = document.getElementById("appointmentsBody");
  if (!data.length) {
    tbody.innerHTML =
      '<tr><td colspan="8" class="text-center text-muted">No appointments yet</td></tr>';
    return;
  }
  // Most recent first
  data.sort(
    (a, b) => new Date(b.appointmentDate) - new Date(a.appointmentDate),
  );

  tbody.innerHTML = data
    .map(
      (a) => `
        <tr>
            <td>${a.appointmentId}</td>
            <td>${a.doctor?.name ?? "#" + a.doctorId}</td>
            <td>${a.doctor?.specialtyName ?? "—"}</td>
            <td>${a.appointmentDate?.split("T")[0]}</td>
            <td>${(a.appointmentTime || "").slice(0, 5)}</td>
            <td><span class="badge badge-${a.status}">${a.status}</span></td>
            <td>${a.message ? a.message : "—"}</td>
            <td>
                ${
                  a.status === "Pending" || a.status === "Confirmed"
                    ? `<button class="btn btn-sm btn-outline-danger" onclick="cancelAppt(${a.appointmentId})">❌ Cancel</button>`
                    : '<span class="text-muted">—</span>'
                }
            </td>
        </tr>
    `,
    )
    .join("");
}

async function cancelAppt(id) {
  if (!confirm("Are you sure you want to cancel this appointment?")) return;
  try {
    await Api.put(`/appointments/${id}/cancel`);
    loadAppointments();
  } catch (e) {
    alert("Could not cancel the appointment: " + e.message);
  }
}

async function undoLastCancel() {
  const msg = document.getElementById("undoMsg");
  try {
    const result = await Api.post("/appointments/undo-last-cancel");
    msg.innerHTML = `<div class="alert alert-success py-2">${result.message}</div>`;
    loadAppointments();
  } catch (e) {
    msg.innerHTML = `<div class="alert alert-warning py-2">${e.message}</div>`;
  }
}

loadAppointments();
