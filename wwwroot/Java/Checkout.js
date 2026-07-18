Auth.requireLogin();

const doctorId = sessionStorage.getItem("booking_doctorId");
const bookingDate = sessionStorage.getItem("booking_date");
const bookingTime = sessionStorage.getItem("booking_time");
const bookingMessage = sessionStorage.getItem("booking_message") || "";

const checkoutMsg = document.getElementById("checkoutMsg");
const placeOrderBtn = document.getElementById("placeOrderBtn");

// Load the doctor's fee for the order summary
(async function loadOrderSummary() {
  if (!doctorId) return;
  try {
    const doc = await Api.get(`/doctors/${doctorId}`);
    document.getElementById("orderDoctorLabel").textContent = `${doc.name} Appointment × 1`;
    document.getElementById("orderSubtotal").textContent = `${doc.consultationFees} EGP`;
    document.getElementById("orderTotal").textContent = `${doc.consultationFees} EGP`;
  } catch (err) {
    console.error("Could not load doctor fee:", err.message);
  }
})();

// Prefill billing name/email from the logged-in user
(function prefill() {
  const user = Auth.getUser();
  if (user) {
    if (user.firstName) document.getElementById("billFirstName").value = user.firstName;
    if (user.lastName) document.getElementById("billLastName").value = user.lastName;
  }
})();

placeOrderBtn.addEventListener("click", async function () {
  checkoutMsg.textContent = "";

  if (!doctorId || !bookingDate || !bookingTime) {
    checkoutMsg.textContent = "Missing booking details - please start the booking again from the doctor's page.";
    return;
  }

  const firstName = document.getElementById("billFirstName").value.trim();
  const lastName = document.getElementById("billLastName").value.trim();
  const city = document.getElementById("billCity").value.trim();
  const street = document.getElementById("billStreet").value.trim();

  if (!firstName || !lastName || !city || !street) {
    checkoutMsg.textContent = "Please fill in all the required billing fields.";
    return;
  }

  const paymentMethod = document.querySelector('input[name="payment"]:checked')?.value || "Cash";

  placeOrderBtn.disabled = true;
  placeOrderBtn.textContent = "Placing order...";

  try {
    const appointment = await Api.post("/appointments", {
      doctorId: Number(doctorId),
      appointmentDate: bookingDate,
      appointmentTime: bookingTime,
      message: bookingMessage,
    });

    const payment = await Api.post("/payments", {
      appointmentId: appointment.appointmentId,
      paymentMethod,
    });

    // Hand off the confirmation details to report.html
    sessionStorage.setItem("last_appointment", JSON.stringify(appointment));
    sessionStorage.setItem("last_payment", JSON.stringify(payment));
    sessionStorage.setItem("last_billing_name", `${firstName} ${lastName}`);
    sessionStorage.setItem("last_billing_address", `${street}, ${city}`);

    // Clean up the booking-in-progress keys
    ["booking_doctorId", "booking_date", "booking_time", "booking_time_display", "booking_message",
     "booking_contact_name", "booking_contact_phone", "booking_contact_email"]
      .forEach((k) => sessionStorage.removeItem(k));

    window.location.href = "report.html";
  } catch (err) {
    checkoutMsg.textContent = err.message || "Could not complete the booking. Please try again.";
    placeOrderBtn.disabled = false;
    placeOrderBtn.textContent = "PLACE ORDER";
  }
});
