// listen for page resize
function resizeListener(dotnethelper) {
    $(window).resize(() => {
        let browserHeight = $(window).innerHeight();
        let browserWidth = $(window).innerWidth();
        dotnethelper.invokeMethodAsync('SetBrowserDimensions', browserWidth, browserHeight).then(() => {
            // success, do nothing
        }).catch(error => {
            console.log("Error during browser resize: " + error);
        });
    });
}

function scrollToBottom(id) {
    setTimeout(function() {
        var element = document.getElementById(id);
        if (element) {
            element.scrollTop = element.scrollHeight + 1500;
        }
    }, 500);
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
    const arrayBuffer = await contentStreamReference.arrayBuffer();
    const blob = new Blob([arrayBuffer]);
    const url = URL.createObjectURL(blob);
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.click();
    anchorElement.remove();
    URL.revokeObjectURL(url);
}

window.saveFile = function (bytesBase64, mimeType, fileName) {
    var fileUrl = "data:" + mimeType + ";base64," + bytesBase64;
    fetch(fileUrl)
        .then(response => response.blob())
        .then(blob => {
            var link = window.document.createElement("a");
            link.href = window.URL.createObjectURL(blob, { type: mimeType });
            link.download = fileName;
            document.body.appendChild(link);
            link.click();
            document.body.removeChild(link);
        });
}

window.downloadFile = function (filename, base64) {
    const link = document.createElement("a");
    link.href = "data:text/csv;base64," + base64;
    link.download = filename;
    link.click();
};

window.BlazorDownloadFile = (fileName, contentType, data) => {
    const link = document.createElement('a');
    link.download = fileName;
    link.href = URL.createObjectURL(new Blob([data], { type: contentType }));
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};