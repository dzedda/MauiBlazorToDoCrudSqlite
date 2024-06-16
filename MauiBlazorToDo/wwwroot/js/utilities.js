window.jsFunctions = {
    captureImage: async function (imageId, imageStream) {
        const arrayBuffer = await imageStream.arrayBuffer();
        const blob = new Blob([arrayBuffer]);
        const url = URL.createObjectURL(blob);
        document.getElementById(imageId).src = url;
        document.getElementById(imageId).style.display = 'block';
        return true;
    },
}