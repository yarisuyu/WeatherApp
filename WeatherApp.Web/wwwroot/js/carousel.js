const prefersReducedMotion = () =>
    window.matchMedia('(prefers-reduced-motion: reduce)').matches;

export function scrollToHour(id) {
    const el = document.getElementById(id);
    if (!el) {
        console.warn('[carousel] Element not found:', id);
        return false;
    }

    const container = el.closest('[data-carousel="hourly"]');
    if (!container) {
        console.warn('[carousel] Container not found for', id);
        return false;
    }

    el.scrollIntoView({
        behavior: prefersReducedMotion() ? 'auto' : 'smooth',
        block: 'nearest',
        inline: 'start'
    });
    return true;
}
