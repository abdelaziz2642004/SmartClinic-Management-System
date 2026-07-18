Auth.requireLogin();

const selectPaymentBtn = document.getElementById("selectPaymentBtn");
const back = document.getElementById("btn-back");
const bookingMsg = document.getElementById("bookingMsg");

const doctorId = sessionStorage.getItem("booking_doctorId");

// Prefill with the logged-in user's info + load the selected doctor's card details
(async function init() {
  const user = Auth.getUser();
  if (user) {
    if (user.firstName)
      document.getElementById("firstName").value =
        `${user.firstName} ${user.lastName || ""}`.trim();
    if (user.email) document.getElementById("emailAddress").value = user.email;
  }

  if (!doctorId) return;

  try {
    const doc = await Api.get(`/doctors/${doctorId}`);
    const nameEl = document.getElementById("doctorNameDisplay");
    const specEl = document.getElementById("doctorSpecialtyDisplay");
    const imgEl = document.getElementById("doctorImg");
    if (nameEl) nameEl.textContent = doc.name;
    if (specEl) specEl.textContent = doc.specialtyName || "";
    if (imgEl && doc.imagePath) imgEl.setAttribute("src", doc.imagePath);
  } catch (err) {
    console.error("Could not load doctor card:", err.message);
  }
})();

selectPaymentBtn.addEventListener("click", function () {
  const date = sessionStorage.getItem("booking_date");
  const time = sessionStorage.getItem("booking_time");

  if (!doctorId || !date || !time) {
    bookingMsg.textContent = "Please choose a date and time slot first.";
    return;
  }

  sessionStorage.setItem(
    "booking_message",
    document.getElementById("visitReason").value.trim(),
  );
  sessionStorage.setItem(
    "booking_contact_name",
    document.getElementById("firstName").value.trim(),
  );
  sessionStorage.setItem(
    "booking_contact_phone",
    document.getElementById("phoneNumber").value.trim(),
  );
  sessionStorage.setItem(
    "booking_contact_email",
    document.getElementById("emailAddress").value.trim(),
  );

  window.location.href = "Checkout.html";
});

back.addEventListener("click", function () {
  window.history.back();
});
