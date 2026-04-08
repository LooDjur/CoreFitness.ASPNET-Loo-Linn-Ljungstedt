document.addEventListener('click', function(e) {
    const trigger = e.target.closest('.accordion-trigger');
    
    if (!trigger) return;

    const accordionContainer = trigger.closest('[data-accordion]');
    const parentItem = trigger.closest('.accordion-item');
    const panel = parentItem.querySelector('.accordion-panel');
    
    if (!panel) return;

    const isExpanded = trigger.getAttribute('aria-expanded') === 'true';

    if (accordionContainer) {
        accordionContainer.querySelectorAll('.accordion-trigger').forEach(btn => {
            btn.setAttribute('aria-expanded', 'false');
            const otherPanel = btn.closest('.accordion-item').querySelector('.accordion-panel');
            if (otherPanel) otherPanel.style.display = 'none';
        });
    }

    if (!isExpanded) {
        trigger.setAttribute('aria-expanded', 'true');
        panel.style.display = 'block';
    } else {
        trigger.setAttribute('aria-expanded', 'false');
        panel.style.display = 'none';
    }
});