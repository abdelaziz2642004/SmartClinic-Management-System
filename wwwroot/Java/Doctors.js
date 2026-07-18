// Maps a specialty name coming from the API to the filter bucket + detail page
// used by this template (the template ships with exactly 3 specialties).
const SPECIALTY_MAP = {
  cardiology: {
    filter: "cardio",
    page: "CardiologyDetails.html",
    image: "images/cardiology Doc.jpg",
  },
  orthopedic: {
    filter: "ortho",
    page: "OrthopedicsDetails.html",
    image: "images/Orthopedics Doc.jpg",
  },
  dentist: {
    filter: "dent",
    page: "DentistDetail.html",
    image: "images/Dentist Doc.jpg",
  },
};

function specialtyInfo(name) {
  const key = (name || "").toLowerCase();
  return (
    SPECIALTY_MAP[key] || {
      filter: "all",
      page: "Doctors.html",
      image: "images/Doctors Photo serv1.avif",
    }
  );
}

function renderDoctorCards(doctors) {
  const cardsContainer = document.querySelector(".cards");
  if (!doctors.length) {
    cardsContainer.innerHTML = `<p style="text-align:center;width:100%;">No doctors available right now.</p>`;
    return;
  }

  cardsContainer.innerHTML = doctors
    .map((doc) => {
      const info = specialtyInfo(doc.specialtyName);
      return `
        <div class="card" data-category="${info.filter}">
          <div class="image">
            <a href="${info.page}?id=${doc.id}">
              <img src="${doc.imagePath ? doc.imagePath : info.image}" alt="${doc.name}">
            </a>
          </div>
          <div class="info">
            <h3>${doc.name}</h3>
            <p>${doc.specialtyName || ""}</p>
            <div class="social">
              <i class="fab fa-twitter"></i>
              <i class="fab fa-facebook-f"></i>
              <i class="fab fa-instagram"></i>
              <i class="fab fa-linkedin-in"></i>
            </div>
          </div>
        </div>`;
    })
    .join("");

  attachFilterHandlers();
}

function attachFilterHandlers() {
  const filterButtons = document.querySelectorAll(".filter-buttons button");
  const cards = document.querySelectorAll(".card");

  filterButtons.forEach((button) => {
    button.addEventListener("click", () => {
      filterButtons.forEach((btn) => btn.classList.remove("active"));
      button.classList.add("active");
      const filterValue = button.getAttribute("data-filter");
      cards.forEach((card) => {
        if (
          filterValue === "all" ||
          card.getAttribute("data-category") === filterValue
        ) {
          card.style.display = "block";
          setTimeout(() => {
            card.style.opacity = "1";
            card.style.transform = "scale(1)";
          }, 100);
        } else {
          card.style.opacity = "0";
          card.style.transform = "scale(0.9)";
          setTimeout(() => {
            card.style.display = "none";
          }, 300);
        }
      });
    });
  });
}

async function loadDoctors() {
  try {
    const doctors = await Api.get("/doctors");
    renderDoctorCards(doctors);
  } catch (err) {
    document.querySelector(".cards").innerHTML =
      `<p style="text-align:center;width:100%;color:#c0392b;">Could not load doctors: ${err.message}</p>`;
  }
}

loadDoctors();
