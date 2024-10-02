﻿document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector('.sidebar');

    sidebar.addEventListener('click', function (event) {
        if (event.target.closest('.activeable') && event.target.tagName === 'A') {
            const activeable = event.target.closest('.activeable');
            const links = sidebar.querySelectorAll('.activeable');

            links.forEach(link => {
                link.classList.remove('active');
            });

            activeable.classList.add('active');
            sessionStorage.setItem('sidebarState', sidebar.innerHTML);
        }

        if (event.target.closest('.btn-toggle')) {
            const btn = event.target.closest('.btn-toggle');
            btn.querySelector('.fa-chevron-down').classList.toggle('collapsed');
            sessionStorage.setItem('sidebarState', sidebar.innerHTML);
        }
    });

    const savedSidebarState = sessionStorage.getItem('sidebarState');
    if (savedSidebarState) {
        sidebar.innerHTML = savedSidebarState;
    }
});


function clearSessionStorage() {
    sessionStorage.clear();
}