mergeInto(LibraryManager.library, {
  downloadFile: function (fileNamePtr, fileNameLength, contentPtr, contentLength) {
    var fileName = UTF8ToString(fileNamePtr, fileNameLength);
    var content = new Uint8Array(Module.HEAPU8.buffer, contentPtr, contentLength);
    var blob = new Blob([content], { type: "application/octet-stream" });
    var link = document.createElement("a");
    link.href = window.URL.createObjectURL(blob);
    link.download = fileName;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  },
});