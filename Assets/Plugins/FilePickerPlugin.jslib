mergeInto(LibraryManager.library, {
  openFilePicker: function () {
    var input = document.createElement("input");
    input.type = "file";
    input.accept = ".nzsave"; // Limit to .nzsave files
    input.onchange = function (event) {
      var file = event.target.files[0];
      if (file) {
        var reader = new FileReader();
        reader.onload = function (e) {
          var content = e.target.result;
          // Pass the content to Unity
          SendMessage(
            "FileDialogManager",
            "OnFileSelected",
            content
          );
        };
        reader.readAsText(file);
      }
    };
    input.click();
  },
});
