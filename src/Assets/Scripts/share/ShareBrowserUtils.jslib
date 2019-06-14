mergeInto(LibraryManager.library, {

    replaceState: function (id, name) {
        id = Pointer_stringify(id);
        name = Pointer_stringify(name);
        if (history && history.replaceState){
            history.replaceState(null, "RRCS | "+name, window.runtimeConfig.STATE_REPLACE_BASE + id);
            
            if (name){
                document.title = "RRCS | " + name;
            } else{
                document.title = "RRCS"
            }
        } 
    },
    
    getShareUrlBase: function () {
        var str = window.runtimeConfig && window.runtimeConfig.SHARE_URL_BASE;
        
        var bufferSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(str, buffer, bufferSize);
        return buffer;
    },

    getBlobUrl: function () {
        var str = window.runtimeConfig && window.runtimeConfig.BLOB_URL;

        var bufferSize = lengthBytesUTF8(str) + 1;
        var buffer = _malloc(bufferSize);
        stringToUTF8(str, buffer, bufferSize);
        return buffer;
    }
});