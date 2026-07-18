const container = document.querySelector(".parent");
const registerBtn = document.querySelector(".btn-register");
const loginBtn = document.querySelector(".btn-login");
let password = document.getElementsByClassName("pass")[0];
let passwordConfirmation = document.getElementById("passwordConfirmation");
let passAlert = document.getElementById("passConfirmAlert");

registerBtn.addEventListener("click", () => {
  container.classList.add("active");
});

loginBtn.addEventListener("click", () => {
  container.classList.remove("active");
});

function show(type) {
  let password = document.getElementById(`${type}-password`);
  let eyeSlash = document.getElementById(`${type}-eye-slash`);
  let eye = document.getElementById(`${type}-eye`);

  if (password.type === "password") {
    password.type = "text";
    eyeSlash.style.visibility = "hidden";
    eye.style.visibility = "visible";
    eye.style.color = "black";
  } else {
    password.type = "password";
    eyeSlash.style.visibility = "visible";
    eye.style.visibility = "hidden";
  }
}

passwordConfirmation.addEventListener("keyup", function () {
  if (password.value != passwordConfirmation.value) {
    passAlert.style = "display:block";
  } else {
    passAlert.innerHTML = `<span><i class="fa-solid fa-check"></i></span><span>Password Are Matches</span>`;
    passAlert.style = "display:block;color:green;";
  }
});
passwordConfirmation.addEventListener("mouseover", function () {
  if (password.value != passwordConfirmation.value) {
    passAlert.style = "display:block";
  } else {
    passAlert.style = "display:none;";
  }
});

// ===================== Real API wiring =====================

function showFormMessage(id, text, isError = true) {
  const el = document.getElementById(id);
  el.style.display = "block";
  el.style.color = isError ? "#c0392b" : "green";
  el.textContent = text;
}

// If already logged in, skip straight to home
if (Auth.isLoggedIn()) {
  window.location.href = "home.html";
}

document.getElementById("loginForm").addEventListener("submit", async function (e) {
  e.preventDefault();
  const email = document.getElementById("loginEmail").value.trim();
  const pwd = document.getElementById("password").value;

  const submitBtn = e.target.querySelector("button[type=submit]");
  submitBtn.disabled = true;

  try {
    const result = await Api.post("/auth/login", { email, password: pwd });
    Auth.setToken(result.token);
    window.location.href = "home.html";
  } catch (err) {
    showFormMessage("loginMsg", err.message || "Invalid email or password");
  } finally {
    submitBtn.disabled = false;
  }
});

document.getElementById("registerForm").addEventListener("submit", async function (e) {
  e.preventDefault();

  const firstName = document.getElementById("firstName").value.trim();
  const lastName = document.getElementById("lastName").value.trim();
  const email = document.getElementById("registerEmail").value.trim();
  const phone = document.getElementById("phone").value.trim();
  const gender = document.getElementById("gender").value;
  const address = document.getElementById("address").value.trim();
  const pwd = document.getElementById("register-password").value;
  const confirmPwd = passwordConfirmation.value;
  const birthdate = document.getElementById("birthdate").value;
  const roleInput = document.querySelector('input[name="user_type"]');

  if (pwd !== confirmPwd) {
    showFormMessage("registerMsg", "Passwords do not match.");
    return;
  }
  if (!roleInput) {
    showFormMessage("registerMsg", "Please choose Doctor or Patient.");
    return;
  }

  const dto = {
    firstName,
    lastName,
    fullName: `${firstName} ${lastName}`.trim(),
    phone,
    gender,
    address,
    birthday: birthdate ? new Date(birthdate).toISOString() : null,
    email,
    password: pwd,
    role: roleInput.value,
  };

  const submitBtn = e.target.querySelector("button[type=submit]");
  submitBtn.disabled = true;

  try {
    await Api.post("/auth/register", dto);
    // Auto-login right after successful registration
    const result = await Api.post("/auth/login", { email, password: pwd });
    Auth.setToken(result.token);
    window.location.href = "home.html";
  } catch (err) {
    showFormMessage("registerMsg", err.message || "Registration failed. Please check your details.");
  } finally {
    submitBtn.disabled = false;
  }
});
