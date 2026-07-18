function handleBooking(serviceType) {
    alert(`Thank you for your interest in our ${serviceType} service. A member of our clinical team will contact you shortly.`);
}

// Simple hover effect for the action cards
const cards = document.querySelectorAll('.action-card');
cards.forEach(card => {
    card.addEventListener('mouseenter', () => {
        card.style.transform = 'translateY(-5px)';
    });
    card.addEventListener('mouseleave', () => {
        card.style.transform = 'translateY(0)';
    });
});