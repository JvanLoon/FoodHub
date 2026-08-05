window.themeInterop = {
    // The theme key is written as a plain string (not JSON) so the inline
    // bootstrap script in App.razor <head> can read it without deserializing.
    get: () => document.documentElement.getAttribute('data-bs-theme') || 'light',
    set: (theme) => {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('foodhub-theme', theme);
    },
    // Called straight from the ThemeToggle button's onclick, so the switch works on the
    // statically rendered login page too, where there is no circuit to handle an @onclick.
    // Which icon is showing follows from the attribute set here, in CSS.
    toggle: () => themeInterop.set(themeInterop.get() === 'dark' ? 'light' : 'dark')
};

window.fieldInterop = {
    // TextField's password reveal. Same reason as themeInterop.toggle: it has to work without
    // a circuit. The button carries its own labels, since these are Dutch strings that live in
    // UiText on the server.
    toggleReveal: (button) => {
        const input = document.getElementById(button.dataset.revealFor);
        if (!input) return;

        const revealed = input.type === 'text';
        input.type = revealed ? 'password' : 'text';

        const label = revealed ? button.dataset.labelShow : button.dataset.labelHide;
        button.title = label;
        button.setAttribute('aria-label', label);
        button.setAttribute('aria-pressed', revealed ? 'false' : 'true');

        const icon = button.querySelector('i');
        if (icon) icon.className = revealed ? 'bi bi-eye' : 'bi bi-eye-slash';
    }
};

window.blazorDownloadFile = (fileName, contentType, base64Data) => {
    try {
        const link = document.createElement('a');
        link.download = fileName;
        link.href = `data:${contentType};base64,${base64Data}`;
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    } catch (e) {
        alert("Download failed: " + e.message);
    }
};

window.getFileName = (inputId) => {
    const input = document.getElementById(inputId);
    if (!input || !input.files || input.files.length === 0) return null;
    return input.files[0].name;
};

window.getFileBase64 = (inputId) => {
    return new Promise((resolve, reject) => {
        const input = document.getElementById(inputId);
        if (!input || !input.files || input.files.length === 0) { resolve(null); return; }
        const reader = new FileReader();
        reader.onload = () => resolve(reader.result.split(',')[1]);
        reader.onerror = () => reject(reader.error);
        reader.readAsDataURL(input.files[0]);
    });
};
