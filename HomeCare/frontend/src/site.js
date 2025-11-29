// handle navbar scroll effect
window.addEventListener("scroll", () => {
    const navbar = document.querySelector(".navbar");
    if (!navbar) return;

    const scrollY = window.scrollY || document.documentElement.scrollTop;
    if (scrollY > 50) {
        navbar.classList.add("scrolled");
    } else {
        navbar.classList.remove("scrolled");
    }
});

// handle homecare side menu toggle
document.addEventListener("DOMContentLoaded", () => {
    const toggleButton = document.getElementById("hcMenuToggle");
    const sideMenu = document.getElementById("hcSideMenu");
    const backdrop = document.querySelector(".hc-side-menu-backdrop");
    const closeButton = document.querySelector(".hc-side-menu-close");

    // stop if elements are missing
    if (!toggleButton || !sideMenu || !backdrop || !closeButton) return;

    // open menu
    function openMenu() {
        sideMenu.classList.add("open");
        backdrop.classList.add("visible");

        toggleButton.setAttribute("aria-expanded", "true");
        sideMenu.setAttribute("aria-hidden", "false");
    }

    // close menu
    function closeMenu() {
        sideMenu.classList.remove("open");
        backdrop.classList.remove("visible");

        toggleButton.setAttribute("aria-expanded", "false");
        sideMenu.setAttribute("aria-hidden", "true");
    }

    // toggle on button click
    toggleButton.addEventListener("click", () => {
        if (sideMenu.classList.contains("open")) {
            closeMenu();
        } else {
            openMenu();
        }
    });

    // close on backdrop click
    backdrop.addEventListener("click", closeMenu);

    // close on x-button click
    closeButton.addEventListener("click", closeMenu);

    // close on escape key
    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") {
            closeMenu();
        }
    });
});
