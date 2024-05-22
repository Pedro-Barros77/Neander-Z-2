mergeInto(LibraryManager.library, {
    SyncFileSystem: function() {
        FS.syncfs(false, function (err) {
            if (err) {
                console.error("Error syncing filesystem:", err);
            } else {
                console.log("Filesystem synced successfully.");
            }
        });
    }
});
