const monthTitle = document.getElementById("monthTitle");
const calendarDates = document.getElementById("calendarDates");
const prevBtn = document.getElementById("prevMonth");
const nextBtn = document.getElementById("nextMonth");
const slotsBox = document.getElementById("slotsBox");
const addINfo = document.getElementById("nextBtn");

Auth.requireLogin();

const urlParams = new URLSearchParams(window.location.search);
const doctorId = Number(urlParams.get("doctorId")) || 1;
sessionStorage.setItem("booking_doctorId", doctorId);

// Load the selected doctor's real name/specialty/image into the card
(async function loadDoctorCard() {
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

let currentDate = new Date();
let selectedDate = null;
let selectedSlot = null;

const months = [
  "January",
  "February",
  "March",
  "April",
  "May",
  "June",
  "July",
  "August",
  "September",
  "October",
  "November",
  "December",
];

const ALL_SLOTS = [
  "09:00",
  "09:35",
  "10:10",
  "10:45",
  "11:20",
  "11:55",
  "12:30",
  "13:05",
  "13:40",
  "14:15",
  "14:50",
  "15:25",
];

function to12Hour(time24) {
  const [h, m] = time24.split(":").map(Number);
  const period = h >= 12 ? "pm" : "am";
  const hour12 = h % 12 === 0 ? 12 : h % 12;
  return `${hour12}:${String(m).padStart(2, "0")} ${period}`;
}

// ================= Calendar =================
function renderCalendar() {
  calendarDates.innerHTML = "";

  const year = currentDate.getFullYear();
  const month = currentDate.getMonth();

  monthTitle.textContent = `${months[month]} ${year}`;

  const firstDay = new Date(year, month, 1).getDay();
  const lastDate = new Date(year, month + 1, 0).getDate();
  const today = new Date();

  for (let i = 0; i < firstDay; i++) {
    calendarDates.appendChild(document.createElement("div"));
  }

  for (let day = 1; day <= lastDate; day++) {
    const dateEl = document.createElement("div");
    dateEl.textContent = day;

    if (
      day === today.getDate() &&
      month === today.getMonth() &&
      year === today.getFullYear()
    ) {
      dateEl.classList.add("today");
    }

    if (
      selectedDate &&
      day === selectedDate.getDate() &&
      month === selectedDate.getMonth() &&
      year === selectedDate.getFullYear()
    ) {
      dateEl.classList.add("selected");
    }

    dateEl.addEventListener("click", async () => {
      selectedDate = new Date(year, month, day);
      renderCalendar();

      const slots = await fetchAvailableSlots(selectedDate);
      renderSlots(slots);
    });

    calendarDates.appendChild(dateEl);
  }
}

prevBtn.onclick = () => {
  currentDate.setMonth(currentDate.getMonth() - 1);
  renderCalendar();
};

nextBtn.onclick = () => {
  currentDate.setMonth(currentDate.getMonth() + 1);
  renderCalendar();
};

// ================= Slots =================
// Real API call: pull this doctor's existing appointments for the selected day
// and hide whichever of the fixed daily slots are already taken.
async function fetchAvailableSlots(date) {
  const dateStr = date.toISOString().split("T")[0];

  try {
    const appointments = await Api.get(`/appointments?doctorId=${doctorId}`);
    const bookedTimes = appointments
      .filter(
        (a) =>
          a.appointmentDate.split("T")[0] === dateStr &&
          a.status !== "Cancelled",
      )
      .map((a) => a.appointmentTime.split(":").slice(0, 2).join(":"));

    return ALL_SLOTS.filter((t) => !bookedTimes.includes(t)).map(to12Hour);
  } catch (err) {
    console.error("Could not load availability:", err.message);
    return ALL_SLOTS.map(to12Hour);
  }
}

function renderSlots(slots) {
  slotsBox.innerHTML = "";
  selectedSlot = null;

  if (!slots.length) {
    slotsBox.innerHTML = `<p>No available slots for this day.</p>`;
    return;
  }

  slots.forEach((time) => {
    const slotEl = document.createElement("div");
    slotEl.className = "time-slot";
    slotEl.innerHTML = `${time} <span>Slots:1</span>`;

    slotEl.addEventListener("click", () => {
      document
        .querySelectorAll(".time-slot")
        .forEach((s) => s.classList.remove("active"));

      slotEl.classList.add("active");
      selectedSlot = time;
    });

    slotsBox.appendChild(slotEl);
  });
}

renderCalendar();

addINfo.addEventListener("click", function () {
  if (selectedDate && selectedSlot) {
    // Convert "9:00 am" back to 24h "09:00:00" for the API
    const [time, period] = selectedSlot.split(" ");
    let [h, m] = time.split(":").map(Number);
    if (period === "pm" && h !== 12) h += 12;
    if (period === "am" && h === 12) h = 0;
    const time24 = `${String(h).padStart(2, "0")}:${String(m).padStart(2, "0")}:00`;

    sessionStorage.setItem(
      "booking_date",
      selectedDate.toISOString().split("T")[0],
    );
    sessionStorage.setItem("booking_time", time24);
    sessionStorage.setItem("booking_time_display", selectedSlot);

    window.location.href = "appoinmentCardiology.html";
  } else {
    alert("Please select a date and a time slot.");
  }
});
