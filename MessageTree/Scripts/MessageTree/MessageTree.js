// namespace alias MT = t14Lab.MessageTree see on the end document
var MT;

t14Lab.MessageTree = {
    requestCommand: {
        loadMessages: "loadMessages",
        loadStructures: "loadStructures",
        loadOptions: "loadOptions",

        loadItem: "loadItem",
        addItem: "addItem",
        editItem: "editItem",
        deleteItem: "deleteItem",
        moveItem: "moveItem",
        duplicateItem: "duplicateItem",
        markItemAsRead: "markItemAsRead",

        usersUpdated: "usersUpdated",
        empty: "empty"
    },
    responseCommand: {
        messagesLoaded: "messagesLoaded",
        structuresLoaded: "structuresLoaded",
        optionsLoaded: "optionsLoaded",

        itemLoaded: "itemLoaded",
        itemAdded: "itemAdded",
        itemEdited: "itemEdited",
        itemDeleted: "itemDeleted",
        itemMoved: "itemMoved",
        itemDuplicated: "itemDuplicated",
        itemMarkedAsRead: "itemMarkedAsRead",

        usersUpdated: "usersUpdated",
        empty: "empty"
    },
    handleServerCommand: function (response, params) {
        var errorLog = [];
        if (!response.command || response.command == this.responseCommand.empty) {
            errorLog.push('response.command is empty');
        }
        if (response.code == MT.Connector.ResponseCode.OK) {
            t14Lab.MessageTree.mapCommandToMethod(response, params);
        } else {
            console.error("MessageTree.js handleServerCommand() : i do nothing now, just leave me alone");
            if (!response.code) {
                // Debug
                console.error("t14Lab.MessageTree.handleCommand(response, params) response.code is empty");
                errorLog.push('response.code is empty');
                // DebugEnd    
            } else {
                /*
                console.group("t14Lab.MessageTree.handleCommand(response, params) Server Code is not 'OK'");
                errorLog.push('response.code is "'+response.code+'" not OK');
                console.dir(response);
                console.dir(params);
                console.groupEnd();
                */
            }
        }
        if (errorLog.length > 0) {

            var blkstr = [];
            $.each(errorLog, function (idx2, val2) {
                var str = idx2 + ":" + val2;
                blkstr.push(str);
            });
            alert(blkstr.join(", "));
        }
    },
    mapCommandToMethod: function (response, params) {
        console.info(" MessageTree.mapCommandToMethod() --> Command: " + response.command);
        console.timeEnd("Between Send() and mapCommandToMethod()");
        

        switch (response.command) {
            case this.responseCommand.structuresLoaded:
                MT.ItemContainer.handleStructuresLoadedCommand(response,params.panelName);
                break;
            case this.responseCommand.messagesLoaded:
                MT.ItemContainer.handleMessagesLoadedCommand(response, params.panelName);
                break;
            case this.responseCommand.optionsLoaded:
                MT.ItemContainer.handleOptionsLoadedCommand(response,params.panelName);
                break;
            case this.responseCommand.itemLoaded:
                MT.Item.handleLoadedCommand(response.type,params.messageId,response.messageText);
                break;
            case this.responseCommand.itemAdded:
                MT.Item.handleAddedCommand(
                    response.panelId,
                    response.insertAfterId,
                    response.newMessage,
                    response.itemType,
                    response.asChild
                );
                break;
            case this.responseCommand.itemEdited:
                MT.Item.handleEditedCommand(response, params);
                break;
            case this.responseCommand.itemDeleted:
                MT.Item.handleDeletedCommand(response, params);
                break;
            case this.responseCommand.itemMoved:
                MT.Item.handleMovedCommand(response,params);
                break;
            case this.responseCommand.itemDuplicated:
                MT.Item.handleDuplicatedCommand(response,params);
                break;
            case this.responseCommand.itemMarkedAsRead:
                MT.Item.handleMarkedAsReadCommand(response.data.messageId,params.structureId);
                break;
            case this.responseCommand.usersUpdated:
                GUI.Panel.usersUpdated(response, params);
                break;

            default:
                console.error('MessageTree.mapCommandToMethod() -> Unknown command: ' + response.command);
                break;
        }

    },

    panelList: [],
    createPanel: function (context) {
        this.panelList.push(new MT.Panel(context));
    },
    getPanel: function (panelName) {
        var result;
        this.panelList.forEach(function (panel) {
            if (panel.panelName == panelName) {
                result = panel;
            }
        });
        return result;
    },

    itemContainerTemplate: null,
    itemTemplate: null,
    libraryHeaderTemplate: null,
    documentHeaderTemplate: null,
    optionsHeaderTemplate: null,
    itemSmallEditorTemplate: null,

    init: function (panelsSetup, successFunction) {
        var mt = this;
        MT.getTemplate('Scripts/MessageTree/View/ItemContainer.html', function (itemContainerTemplate) {
            mt.itemContainerTemplate = itemContainerTemplate;
            MT.getTemplate('Scripts/MessageTree/View/Item.html', function (itemTemplate) {
                mt.itemTemplate = itemTemplate;
                MT.getTemplate('Scripts/MessageTree/View/LibraryHeader.html', function (libraryHeader) {
                    mt.libraryHeaderTemplate = libraryHeader;
                    MT.getTemplate('Scripts/MessageTree/View/DocumentHeader.html', function (documentHeader) {
                        mt.documentHeaderTemplate = documentHeader;
                        MT.getTemplate('Scripts/MessageTree/View/OptionsHeader.html', function (optionsHeader) {
                            mt.optionsHeaderTemplate = optionsHeader;
                            MT.getTemplate('Scripts/MessageTree/View/itemSmallEditor.html', function (itemSmallEditor) {
                                mt.itemSmallEditorTemplate = itemSmallEditor;

                                panelsSetup.forEach(function (panel) {
                                    MT.createPanel(panel);
                                    $('#playArea').append('<div id="' + panel.panelName + '" data-type="' + panel.type + '" class="panel">');
                                });

                                panelsSetup.forEach(function (panel) {
                                    if (panel.childPanel) {
                                        var nextPanel = $("#" + panel.panelName).nextAll('.panel').first();
                                        nextPanel.addClass('manadgetPanel');
                                    }
                                });

                                var debugPanel = '<div class="debugPanel">';
                                debugPanel += '<div class="debugButton"><i class="fa fa-plus"></i><br />Add Panel</div>';
                                debugPanel += '<div class="debugButton showJson"><i class="fa fa-code"></i><br />Show JSON</div>';
                                debugPanel += '<div>';

                                $('#playArea').append(debugPanel).find('.showJson').click(function () {
                                    console.log(JSON.stringify(MT.DeveloperTestDataDAL.messages, ["id", "itemType", "indexAsString", "parentId", "messageText", "participants", "childs"], null, '\t'));
                                });
                                panelsSetup.forEach(function (panel) {
                                    if (panel.type == "structure") {
                                        MT.API.loadStructures(panel.panelName);
                                    } else {
                                        MT.API.loadMessages(panel.panelName, 400000);
                                    }
                                });
                                successFunction();
                            });

                            /**
                            collapseItem("sortable1", $('.sortable1 li'), true);
                            var cookie = $.cookie("expandedNodes");
                            var items = cookie ? cookie.split(/,/) : new Array();
                            $.each(items, function (key, value) {
                                expandItem('sortable1', $('#' + value));
                            });
                        
                            if (typeof get_structureId !== 'undefined') {
                                MT.API.loadMessages('sortable2', get_structureId);
                            } else {
                                var structureId = $.cookie('panelsortable2');
                                if (structureId < 1 || !structureId) {
                                    structureId = 1;
                                }
                                MT.API.loadMessages('sortable2', structureId);
                            }
                        
                        
                            if (typeof get_messageId !== 'undefined') {
                                $('html, body').animate({ scrollTop: ($(".messageItem" + get_messageId).offset().top - 109) }, 400);
                                $(".messageItem" + get_messageId).css('border-top', '3px solid black');
                            }
                        
                            t14Lab.MessageTree.checkNewMessages();
                                */

                        });
                    });
                });
            });
        });
    },

    checkNewMessages: function () {
        $('.dontReadedStucture').remove();
        $('.newMessage').removeClass('newMessage');
        $.each(dontReadedstructures, function (index, value) {
            $('#structureText_' + index).prepend('<div class="dontReadedStucture">' + value + '</div>');
        });
        $.each(dontReadedMessages, function (index, value) {
            var messageItem = $('.messageItem' + value).find('.message');
            //messageItem.css('border','1px solid black');
            messageItem.addClass('newMessage');
            messageItem.unbind('click');
            messageItem.click(function () {
                Server.Panel.Message.markAsReaded($(this));
            });
        });
    },
    blink: function (selector) {
        $(selector).animate({
            opacity: 0.25
        }, 500, function () {
            $(this).animate({
                opacity: 1
            }, 500, function () {
                GUI.blink(this);
            });
        });
    },

    getTemplate: function (path, callback) {
        var source;
        var template;

        $.ajax({
            url: path,
            success: function (data) {
                source = data;
                template = Handlebars.compile(source);
                if (callback) callback(template);
            }
        });
    }
}

MT = t14Lab.MessageTree;
