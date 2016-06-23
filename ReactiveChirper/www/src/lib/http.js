function makeRequest(method, uri, body, cb){
    var xhr = new XMLHttpRequest();
    xhr.open(method,uri,true);
    xhr.onreadystatechange = function(){
        if(xhr.readyState !== 4) return
        if(xhr.status < 400) return cb(null, JSON.parse(xhr.responseText || '{}'));
        console.log("error", xhr.status, xhr.responseText);
    	cb(xhr.status);
    };
    xhr.setRequestHeader('Content-Type','application/json');
    xhr.setRequestHeader('Accept','application/json');
    xhr.send(body);
};

module.exports.get = function get(url, cb){
	makeRequest('GET', url, null, cb);
}

module.exports.post = function post(path, params, method) {
    method = method || "post"; // Set method to post by default if not specified.

    // The rest of this code assumes you are not using a library.
    // It can be made less wordy if you use one.
    var form = document.createElement("form");
    form.setAttribute("method", method);
    form.setAttribute("action", path);

    for (var key in params) {
        if (params.hasOwnProperty(key)) {
            var hiddenField = document.createElement("input");
            hiddenField.setAttribute("type", "hidden");
            hiddenField.setAttribute("name", key);
            hiddenField.setAttribute("value", params[key]);

            form.appendChild(hiddenField);
        }
    }

    document.body.appendChild(form);
    form.submit();
}

