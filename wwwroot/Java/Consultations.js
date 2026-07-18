document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("appointmentForm");
    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const location = form.querySelectorAll("select")[0].value;
        const gender = form.querySelectorAll("select")[1].value;
        const speciality = document.getElementById("speciality").value;
        if (location !== "" && gender !== "" && speciality !== "") {
            window.location.href = speciality;
        } else {
            alert("Please fill all fields");
        }
    });
});



