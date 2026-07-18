Auth.requireLogin();

const appointment = JSON.parse(sessionStorage.getItem("last_appointment") || "null");
const payment = JSON.parse(sessionStorage.getItem("last_payment") || "null");

if (!appointment || !payment) {
  // Nobody just finished a checkout - nothing to show, send them to their appointments list.
  window.location.href = "appointments.html";
} else {
  document.getElementById("orderNumber").textContent = "#" + appointment.appointmentId;
  document.getElementById("orderDate").textContent = new Date(payment.paymentDate).toLocaleDateString();
  document.getElementById("orderEmail").textContent = appointment.patient?.email || "—";
  document.getElementById("orderTotalAmount").textContent = `${payment.amount} EGP`;
  document.getElementById("orderPaymentMethod").textContent = payment.paymentMethod;

  document.getElementById("cardHeaderTitle").textContent = `${appointment.doctor?.name || "Doctor"} Booking (×1)`;
  document.getElementById("rowFee").textContent = `${payment.amount} EGP`;
  document.getElementById("rowTotal").textContent = `${payment.amount} EGP`;
  document.getElementById("rowTime").textContent = appointment.appointmentTime?.slice(0, 5) || "—";
  document.getElementById("rowDate").textContent = appointment.appointmentDate?.split("T")[0] || "—";
  document.getElementById("rowStatus").textContent = appointment.status || "Pending";
  document.getElementById("rowDoctorName").textContent = appointment.doctor?.name || "—";
  document.getElementById("rowSpecialty").textContent = appointment.doctor?.specialtyName || "—";
  document.getElementById("rowPatientName").textContent = appointment.patient?.fullName || "—";
  document.getElementById("rowPatientEmail").textContent = appointment.patient?.email || "—";

  // These are one-time confirmation values - clear them so refreshing/revisiting doesn't reuse stale data.
  sessionStorage.removeItem("last_appointment");
  sessionStorage.removeItem("last_payment");
}
