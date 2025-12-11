document.addEventListener("click", function (e) {
    const menu = document.getElementById("menu");
    const burger = document.querySelector(".burger-icon");
    if (!menu.contains(e.target) && !burger.contains(e.target)) {
        menu.classList.remove("show");
        burger.classList.remove("active");
    }
});

function toggleMenu() {
    const menu = document.getElementById("menu");
    const burger = document.querySelector(".burger-icon");
    menu.classList.toggle("show");
    burger.classList.toggle("active");
}

// Close menu when clicking on menu items
document.addEventListener("DOMContentLoaded", function () {
    const menuItems = document.querySelectorAll(".dropdown-menu li a");
    const menu = document.getElementById("menu");
    const burger = document.querySelector(".burger-icon");

    menuItems.forEach(item => {
        item.addEventListener("click", function () {
            menu.classList.remove("show");
            burger.classList.remove("active");
        });
    });
});

// Password Strength Meter
document.addEventListener("DOMContentLoaded", function () {
    const passwordInput = document.getElementById("password");
    const strengthMeter = document.getElementById("password-strength-meter");
    const strengthBar = document.getElementById("password-strength-bar");

    if (!passwordInput || !strengthMeter || !strengthBar) {
        return;
    }

    const requirements = [
        { id: "length", regex: /.{8,}/ },
        { id: "lowercase", regex: /[a-z]/ },
        { id: "uppercase", regex: /[A-Z]/ },
        { id: "number", regex: /[0-9]/ },
        { id: "special", regex: /[^A-Za-z0-9]/ }
    ];


    passwordInput.addEventListener("keyup", function () {
        const password = passwordInput.value;

        if (password.length > 0) {
            strengthMeter.style.display = "block";
            //strengthMeter.style.backgroundColor = "#eee";
            //strengthMeter.style.border = "1px solid #eee";
        }

        // Calculate strength
        let passed = 0;
        requirements.forEach(req => {
            if (req.regex.test(password)) {
                passed++;
            }
        });

        // Set progress bar width and color
        const percent = (passed / requirements.length) * 100;
        strengthBar.style.width = percent + "%";

        strengthBar.classList.remove("weak", "medium", "strong");
        if (passed <= 2) {
            strengthBar.classList.add("weak");
        } else if (passed === 3 || passed === 4) {
            strengthBar.classList.add("medium");
        } else if (passed === 5) {
            strengthBar.classList.add("strong");
        }
    });
});
