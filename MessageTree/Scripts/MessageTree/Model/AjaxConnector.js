t14Lab.MessageTree.Connector = {
    ResponseCode: {
        init: 0,
        OK: 200,
        serverError: 500,
        clientError: 600,
        wrongJson: 1000
    },

    Send: function (url, type, params, successFunction) {
        var response = new t14Lab.MessageTree.Response('');
        $.ajax({
            url: url,
            cache: false,
            type: type,
            data: params,
            async: false,
            timeout: 30000,
            dataType: "html",
            password: "dinnerout",

            error: function (jqXHR, textStatus, errorThrown) {
                response.code = MT.Connector.ResponseCode.serverError;
                // Debug
                console.group("t14Lab.MessageTree.Connector.Send() $.ajax error:");
                console.log('Net Error: ' + response.code);
                console.dir(jqXHR);
                console.dir(textStatus);
                console.dir(errorThrown);
                console.groupEnd();
                console.timeEnd("t14Lab.MessageTree.Connector.Send(" + url + ", " + type + ", " + params + ")");
                throw data;
                // DebugEnd
            },

            success: function (serverData) {
                //response = serverData;
                response = new t14Lab.MessageTree.Response(serverData);

                if (response.code != MT.Connector.ResponseCode.OK) {
                    // Debug
                    console.group("connector error");
                    console.log(serverData);
                    if (response.code == MT.Connector.ResponseCode.wrongJson) {
                        console.log('> Wrong JSON.');
                    }
                    console.dir(response);
                    console.groupEnd();
                    // DebugEnd
                } else {
                    if (!response.code) {
                        throw 'Response code is empty';
                    }
                }
                if (!response.command) {
                }
                successFunction(response);
            }
        });
    }
}


t14Lab.MessageTree.Response = function (serverData) {
    if (serverData.length > 0) {
        try {
            result = JSON.parse(serverData);
            this.code = result.code;
            this.command = result.command;
            this.html = result.html;
            this.data = result;
        } catch (e) {
            this.code = MT.Connector.ResponseCode.wrongJson;
        } finally {
        }
    }
}




