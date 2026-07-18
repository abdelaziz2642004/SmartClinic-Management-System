// Simple script to toggle active state on service cards
const cards = document.querySelectorAll('.action-card');

cards.forEach(card => {
    card.addEventListener('mouseover', () => {
        // Remove active class from all
        cards.forEach(c => c.classList.remove('active'));
        // Add to hovered
        card.classList.add('active');
    });
});