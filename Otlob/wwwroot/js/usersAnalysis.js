document.addEventListener("DOMContentLoaded", function () {
    const counters = document.querySelectorAll('.count-up');

    counters.forEach(counter => {
        const updateCount = () => {
            const target = +counter.getAttribute('data-target');
            const current = +counter.innerText;
            const increment = Math.ceil(target / 100);

            if (current < target) {
                counter.innerText = Math.min(current + increment, target);
                setTimeout(updateCount, 15);
            } else {
                counter.innerText = target.toLocaleString();
            }
        };

        updateCount();
    });
});