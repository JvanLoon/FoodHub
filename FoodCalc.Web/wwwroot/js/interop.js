window.themeInterop = {
    // The theme key is written as a plain string (not JSON) so the inline
    // bootstrap script in App.razor <head> can read it without deserializing.
    get: () => document.documentElement.getAttribute('data-bs-theme') || 'light',
    set: (theme) => {
        document.documentElement.setAttribute('data-bs-theme', theme);
        localStorage.setItem('foodhub-theme', theme);
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
