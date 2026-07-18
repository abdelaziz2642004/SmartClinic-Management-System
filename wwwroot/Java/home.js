// ===================== Highlighted Doctors + live doctor count =====================
const SPECIALTY_BOOKING = {
  cardiology: {
    page: "BookingCardiology.html",
    image: "images/cardiology Doc.jpg",
  },
  orthopedic: {
    page: "BookingOrthopedics.html",
    image: "images/Orthopedics Doc.jpg",
  },
  dentist: { page: "BookingDen.html", image: "images/Dentist Doc.jpg" },
};

function bookingInfoFor(specialtyName) {
  const key = (specialtyName || "").toLowerCase();
  return (
    SPECIALTY_BOOKING[key] || {
      page: "Doctors.html",
      image: "images/Doctors Photo serv1.avif",
    }
  );
}

function renderHighlightedDoctors(doctors) {
  const container = document.getElementById("highlightedDoctors");
  if (!container) return;

  if (!doctors.length) {
    container.innerHTML = `<p style="text-align:center;width:100%;">No doctors available right now.</p>`;
    return;
  }

  container.innerHTML = doctors
    .slice(0, 3)
    .map((doc) => {
      const info = bookingInfoFor(doc.specialtyName);
      return `
        <div class="doc-card">
          <div class="media">
            <div class="rating"><span class="star">★</span> 4.8</div>
          </div>
          <img src="${doc.imagePath ? doc.imagePath : info.image}" alt="${doc.name}" />
          <p class="available">Available</p>
          <h3>${doc.name}</h3>
          <p class="specify">${doc.specialtyName || ""}</p>
          <div class="consult">
            <div class="price">
              <p>Consultations Fees</p>
              <span>$${doc.consultationFees}</span>
            </div>
            <div class="book">
              <a href="${info.page}?doctorId=${doc.id}"><button class="btn">Book Now</button></a>
            </div>
          </div>
        </div>`;
    })
    .join("");
}

async function loadHighlightedDoctorsAndCount() {
  try {
    const doctors = await Api.get("/doctors");
    renderHighlightedDoctors(doctors);

    const doctorCountEl = document.querySelector(".client-Feautres .num.one");
    if (doctorCountEl) doctorCountEl.dataset.goal = doctors.length || 0;
  } catch (err) {
    const container = document.getElementById("highlightedDoctors");
    if (container) {
      container.innerHTML = `<p style="text-align:center;width:100%;color:#c0392b;">Could not load doctors: ${err.message}</p>`;
    }
  }
}

loadHighlightedDoctorsAndCount();

let nums = document.querySelectorAll(".client-Feautres .num");
let section = document.querySelector(".client");
let started = false;

window.onscroll = function () {
  if (window.scrollY + window.innerHeight >= section.offsetTop) {
    if (!started) {
      nums.forEach((num) => startCount(num));
    }
    started = true;
  }
};
function startCount(el) {
  let goal = el.dataset.goal;
  let count = setInterval(() => {
    el.textContent++;
    if (el.textContent == goal) {
      clearInterval(count);
    }
  }, 1000 / goal);
}
