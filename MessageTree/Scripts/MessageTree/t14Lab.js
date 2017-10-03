t14Lab = {
    version: function () {
        return "0.0.7";
    },
    about: function () {
        return "";
    },
    website: function () {
        return "www.tornado14lab.de";
    },

    capitalizeFirstLetter: function (string) {
        return string.charAt(0).toUpperCase() + string.slice(1);
    },

    eventLog: [],
    bindEvent: function (selectorOrObjects, eventName, eventFunction) {
        var source;
        if (typeof selectorOrObjects == "string") {
            source = selectorOrObjects;
        } else {
            source = selectorOrObjects.selector;
        }
        //console.log("Bind Event: " + source);
        var index = this.eventLog.indexOf(source + '_' + eventName);
        if (index > -1) {
            console.error("(E) Double Event " + source + '_' + eventName);
        }
        this.eventLog.push(source + '_' + eventName);
        $(source).on(eventName, eventFunction);
    },
    unbindEvent: function (selectorOrObjects, eventName) {
        var source;
        if (typeof selectorOrObjects == "string") {
            source = selectorOrObjects;
        } else {
            source = selectorOrObjects.selector;
        }

        //console.log("Unbind Event: " + source);
        var index = this.eventLog.indexOf(source + '_' + eventName);
        if (index > -1) {
            this.eventLog.splice(index, 1);
        } else {
            //console.log("No Events for " + source + '_' + eventName);
        }
        $(source).off(eventName);
    },
    generateUUID: function () {
        var d = new Date().getTime();
        var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = (d + Math.random() * 16) % 16 | 0;
            d = Math.floor(d / 16);
            return (c == 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });
        return uuid;
    }
};

