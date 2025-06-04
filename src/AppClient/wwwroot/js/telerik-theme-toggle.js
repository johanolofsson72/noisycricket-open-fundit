// Enhanced Telerik Theme Toggle for Dark Mode Support
// This script extends the existing toggleDarkMode functionality to better support Telerik components
window.telerikThemeEnhancer = (function() {
    
    function refreshTelerikComponents() {
        // Force refresh of Telerik components to pick up new dark mode styles
        // This helps ensure all Telerik controls properly reflect the new theme
        setTimeout(() => {
            // Find all Telerik grids and trigger a refresh
            document.querySelectorAll('.k-grid').forEach(grid => {
                // Check if this is a Blazor component with a refresh method
                if (grid._blazorComponent && grid._blazorComponent.refresh) {
                    try {
                        grid._blazorComponent.refresh();
                    } catch (e) {
                        console.log('Grid refresh not available:', e);
                    }
                }
            });

            // Force style recalculation for all Telerik widgets
            document.querySelectorAll('.k-widget').forEach(widget => {
                // Trigger a style recalculation
                widget.style.display = 'none';
                widget.offsetHeight; // Force reflow
                widget.style.display = '';
            });

            // Dispatch a custom event that Telerik components can listen to
            document.dispatchEvent(new CustomEvent('darkModeToggled', {
                detail: { 
                    isDark: document.documentElement.classList.contains('dark') 
                }
            }));
        }, 100);
    }

    // Listen for dark mode changes
    const observer = new MutationObserver((mutations) => {
        mutations.forEach((mutation) => {
            if (mutation.type === 'attributes' && 
                mutation.attributeName === 'class' && 
                mutation.target === document.documentElement) {
                refreshTelerikComponents();
            }
        });
    });

    // Start observing
    observer.observe(document.documentElement, {
        attributes: true,
        attributeFilter: ['class']
    });

    // Initial setup
    document.addEventListener('DOMContentLoaded', refreshTelerikComponents);
    
    // Also listen for Blazor enhanced navigation
    if (window.Blazor) {
        window.Blazor.addEventListener('enhancedload', refreshTelerikComponents);
    } else {
        document.addEventListener('DOMContentLoaded', () => {
            if (window.Blazor) {
                window.Blazor.addEventListener('enhancedload', refreshTelerikComponents);
            }
        });
    }

    return {
        refresh: refreshTelerikComponents
    };
})();

// Hook into the existing toggleDarkMode function if it exists
document.addEventListener('DOMContentLoaded', () => {
    const originalToggleDarkMode = window.toggleDarkMode;
    if (originalToggleDarkMode) {
        window.toggleDarkMode = function() {
            const result = originalToggleDarkMode.apply(this, arguments);
            // Give the original function time to complete, then refresh Telerik components
            setTimeout(() => {
                if (window.telerikThemeEnhancer) {
                    window.telerikThemeEnhancer.refresh();
                }
            }, 50);
            return result;
        };
    }
});
