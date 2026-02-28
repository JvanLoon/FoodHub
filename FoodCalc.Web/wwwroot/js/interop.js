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

window.getFileBytes = async (inputId) => {
    const input = document.getElementById(inputId);
    if (!input || !input.files || input.files.length === 0) return null;
    const buffer = await input.files[0].arrayBuffer();
    return new Uint8Array(buffer);
};
