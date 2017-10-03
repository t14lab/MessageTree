var T14Servert14Lab.MessageTree.Connector = (function (window, undefined) {

    function send() {
        $.ajax({
            cache: false,
            type: "GET",
            timeout: 5000,
            url: "",
            success: function (response) {
            	return response;
            },
            error: function (error) {
            	return error;
            }
        });
    }

    return {
        Send: send
    };

})(window);

