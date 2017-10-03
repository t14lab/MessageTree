t14Lab.MessageTree.Connector = {
    ResponseCode: {
        init: 0,
        OK: 200,
        serverError: 500,
        clientError: 600,
        wrongJson: 1000
    },
    Send: function (url, type, params, successFunction) {
        console.info("send() --> command: " + params.command);
        console.time("Between Send() and mapCommandToMethod()");
        if (!params.command) {
            throw "JsonConnector.Send() -> Empty Command.";
        }
        var method = "handle" + t14Lab.capitalizeFirstLetter(params.command) + "Command";
        t14Lab.MessageTree.Connector[method](url, type, params, successFunction);
    },
    handleLoadMessagesCommand: function (url, type, params, successFunction) {
        var testData = MT.DeveloperTestDataDAL.getMessagesForStructure(params.structureId);
        var structureOptions = MT.DeveloperTestDataDAL.getOptionsForStructure(params.structureId);
        MT.DeveloperTestDataDAL.getStructureById(params.structureId, function (structure) {
            var navigationPath = MT.DeveloperTestDataDAL.getStructureNavigationPath(structure);
            navigationPath.reverse();
            var response = {
                command: MT.responseCommand.messagesLoaded,
                code: MT.Connector.ResponseCode.OK,
                structureId: params.structureId,
                structureName: structure.messageText,
                navigationPath: navigationPath,
                data: testData,
                structureOptions: structureOptions,
                participants: MT.DeveloperTestDataDAL.filterUsers(structure.participants)

            };
            successFunction(response);
        });
    },
    handleLoadOptionsCommand: function (url, type, params, successFunction) {
        var testData = MT.DeveloperTestDataDAL.getOptionsForStructure(params.structureId);
        MT.DeveloperTestDataDAL.getStructureById(params.structureId, function (structure) {
            var response = {
                structureId: params.structureId,
                structureName: structure.messageText,
                code: MT.Connector.ResponseCode.OK,
                command: MT.responseCommand.optionsLoaded,
                data: testData,
                participants: MT.DeveloperTestDataDAL.filterUsers(structure.participants)
            };

            successFunction(response);
        });

    },
    handleLoadStructuresCommand: function (url, type, params, successFunction) {
        var response = {
            code: MT.Connector.ResponseCode.OK,
            command: MT.responseCommand.structuresLoaded,
            data: MT.DeveloperTestDataDAL.structures
        };
        successFunction(response);
    },
    handleLoadItemCommand: function (url, type, params, successFunction) {
        // TODO: do here something, use type
        MT.DeveloperTestDataDAL.getMessageById(params.id, function (testData) {
            testData.structureOption = MT.DeveloperTestDataDAL.getOptionsForStructure(params.structureId);
            var response = {
                code: MT.Connector.ResponseCode.OK,
                command: MT.responseCommand.itemLoaded,
                data: testData,
                messageText: testData.messageText,
                type: params.type
            };
            successFunction(response);
        });
    },
    handleAddItemCommand: function (url, type, params, successFunction) {
        MT.DeveloperTestDataDAL.insertAfter(params.insertAfterId, params.itemType, params.asChild, function (newItem) {
            var response = {
                panelId: params.panelId,
                code: MT.Connector.ResponseCode.OK,
                command: MT.responseCommand.itemAdded,
                insertAfterId: params.insertAfterId,
                newMessage: newItem,
                itemType: params.itemType,
                asChild: params.asChild
            };
            successFunction(response);
        });
    },
    handleEditItemCommand: function (url, type, params, successFunction) {
        MT.DeveloperTestDataDAL.editItem(params.itemType, params.itemId, params.newText, function (item) {
            var response = {
                code: MT.Connector.ResponseCode.OK,
                command: MT.responseCommand.itemEdited,
                item: item,
                itemType: params.type
            };
            successFunction(response);
        });

    },
    handleDeleteItemCommand: function (url, type, params, successFunction) {
        MT.DeveloperTestDataDAL.getMessageById(params.itemId, function (message) {
            var index = message.parent.childs.indexOf(message);
            message.parent.childs.splice(index);
            var response = {
                code: 200,
                command: MT.responseCommand.itemDeleted,
                id: params.itemId,
                type: params.itemType
            };
            successFunction(response);
        });
    },
    handleMoveItemCommand: function (url, type, params, successFunction) {
        var checksum = (params.targetParent.id ? "1" : "0") + (params.targetPrev.id ? "1" : "0") + (params.targetNext.id ? "1" : "0");

        
        // 001 1. only next - first ellement                                        $next / 2
        // 011 2. next & prev - first level, not first, not last                    $prev + (($next - $prev) / 2)
        // 010 3. only prev - last ellement	                                        $prev + 10
        // 101 4. next  & parent prev - child first                                 $parentPrevLevel + '.' + $next[$lastNext] / 2
        // 111 5. next & prev & parent prev  - child middle	                        $parentPrevLevel + '.' + ($next[$lastNext] - $prev[$lastPrev]) / 2
        // 110 6. prev  & parent prev - last child                                  $parentPrevLevel + '.' + $prev[$lastPrev] + 10
        // 100 7. parent prev 	- new sub ellement	      
        switch (checksum) {
            default: // 000
                break;
            case "101":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.getMessageById(params.targetParent.id, function (parent) {
                        parent.childs.unshift(item);
                        item.parent = parent;
                        item.parentId = parent.parentId;

                        successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });
                    })
                });
                break;
            case "001":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    item.parent = null;
                    MT.DeveloperTestDataDAL.messages.unshift(item);
                    successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });

                });
                break;
            case "111":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.getMessageById(params.targetParent.id, function (parent) {
                        MT.DeveloperTestDataDAL.getMessageById(params.targetPrev.id, function (prev) {
                            var newIndex = parent.childs.indexOf(prev) + 1;
                            parent.childs.splice(newIndex, 0, item);
                            item.parent = parent;
                            item.parentId = parent.parentId;

                            successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });

                        });
                    });
                });
                break;
            case "011":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.getMessageById(params.targetPrev.id, function (prev) {
                        var newIndex = MT.DeveloperTestDataDAL.messages.indexOf(prev) + 1;
                        MT.DeveloperTestDataDAL.messages.splice(newIndex, 0, item);
                        item.parent = null;

                        successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });
                    });
                });
                break;
            case "110":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.getMessageById(params.targetParent.id, function (parent) {
                        parent.childs.push(item);
                        item.parent = parent;
                        item.parentId = parent.parentId;

                        successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });

                    })
                });
                break;
            case "010":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.messages.push(item);
                    item.parent = null;

                    successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });

                });
                break;
            case "100":
                MT.DeveloperTestDataDAL.deleteItem(MT.DeveloperTestDataDAL.messages, params.current.id, function (item) {
                    MT.DeveloperTestDataDAL.getMessageById(params.targetParent.id, function (parent) {
                        parent.childs = [];
                        parent.childs.unshift(item);
                        item.parent = parent;
                        item.parentId = parent.parentId;

                        successFunction({ code: 200, command: MT.responseCommand.itemMoved, id: params.current.id });
                    })
                });
                break;
        }
    },
    handleDuplicateItemCommand: function (url, type, params, successFunction) {
    },
    handleMarkItemAsReadCommand: function (url, type, params, successFunction) {
    },
    handleUsersUpdatedCommand: function (url, type, params, successFunction) {
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




