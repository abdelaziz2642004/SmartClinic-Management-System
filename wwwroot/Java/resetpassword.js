let Password = document.getElementById("password");
let passwordConfirmation = document.getElementById("passwordConfirmation");
let Label = document.getElementById("passlabel");
let passConfirmAlert = document.getElementById("passConfirmAlert");
function show() {
  if (Password.type == "password") {
    Password.type = "text";
    document.getElementById("eyeslash-icon").style = "visibility: hidden;";
    document.getElementById("eye-icon").style =
      "visibility: visible; color: black;";
    Label.style = "top: -1px;;";
  } else {
    Password.type = "password";
    document.getElementById("eye-icon").style = "visibility: hidden;";
    document.getElementById("eyeslash-icon").style = "visibility: visible;";
    Label.style = "top: -1px;;";
  }
}

passwordConfirmation.addEventListener(
  "input",
  function () {
    if (Password.value != passwordConfirmation.value) {
      passConfirmAlert.style = "display: block;";
    } else {
      passConfirmAlert.innerHTML = `<span><i class="fa-solid fa-check"></i></span><span>Passwords Are Matches</span>`;
      passConfirmAlert.style = "display: block; color:green;";
    }
  }
);
passwordConfirmation.addEventListener("blur", function () {
  passConfirmAlert.style = "display: none;";
});