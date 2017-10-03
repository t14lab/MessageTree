t14Lab.MessageTree.API = {
    url: "/messageboard/",


    loadStructures: function (panelName) {
        MT.Connector.Send(
            this.url,
            'get',
            {
                command: MT.requestCommand.loadStructures,
                panelName: panelName
            },
            function (response) {
                MT.handleServerCommand(response, { panelName: panelName });
            }
        );
    },
    loadOptions: function (panelName, structureId) {
        MT.Connector.Send(
            this.url,
            'get',
            {
                command: MT.requestCommand.loadOptions,
                structureId: structureId,
                panelName: panelName
            },
            function (response) {
                MT.handleServerCommand(response, { panelName: panelName });
            }
        );
    },
    loadMessages: function (panelName, structureId) {
        MT.Connector.Send(
            this.url,
            'get',
            {
                command: MT.requestCommand.loadMessages,
                structureId: structureId,
                panelName: panelName
            }
            , function (response) {
                MT.handleServerCommand(response, { panelName: panelName });
            }
        );
    },
    loadItem: function (id, itemType, callback) {
        MT.Connector.Send(
            this.url,
            'get',
            {
                command: MT.requestCommand.loadItem,
                type: itemType,
                id: id
            },
            function (response) {
                if (typeof callback === "function") {
                    callback(response);
                } else {
                    MT.handleServerCommand(response,
                        { messageId: id, itemType: itemType }
                    );
                }
            }
        );
    },
    deleteItem: function (itemId, itemType) {
        MT.Connector.Send(
            this.url,
            'delete',
            {
                command: MT.requestCommand.deleteItem,
                itemId: itemId,
                itemType: itemType
            },
            function (response) {
                MT.handleServerCommand(
                    response, { messageId: itemId, itemType: itemType }
                );
            }
        );
    },

    addItem: function (panelId, id, type, asChild) {
        MT.Connector.Send(
            this.url,
            'post',
            {
                command: MT.requestCommand.addItem,
                panelId: panelId,
                insertAfterId: id,
                itemType: type,
                asChild: asChild
            },
            function (response) {
                MT.handleServerCommand(response, {
                    insertAfterId: id,
                    itemType: type
                });
            }
        );
    },
    moveItem: function (source) {
        var currentItem = source;

        var current = parseItem(currentItem);
        var targetPrev = parseItem(currentItem.prev());
        var targetNext = parseItem(currentItem.next());
        var targetParent = parseItem(currentItem.parent().parent());
        
        MT.Connector.Send(
            this.url,
            'put',
            {
                command: MT.requestCommand.moveItem,
                current: current,
                text: '',
                targetPrev: targetPrev,
                targetNext: targetNext,
                targetParent: targetParent
            },
            function (response) {
                MT.handleServerCommand(response, { movedItem: source });
            }
        );
    },
    editItem: function (itemType, itemId, newText) {
        MT.Connector.Send(
            this.url,
            'put',
            {
                command: MT.requestCommand.editItem,
                itemType: itemType,
                itemId: itemId,
                newText: newText
            },
            function (response) {
                MT.handleServerCommand(response, {
                    itemId: itemId,
                    itemType: itemType
                });
            }
        );
    },
    markItemAsRead: function (source) {
        var id = source.data('id');
        var structureId = source.data('parentid');
        MT.Connector.Send(
            this.url,
            'put',
            {
                command: MT.requestCommand.markItemAsRead,
                messageId: id
            },
            function (response) {
                MT.handleServerCommand(response, {
                    messageId: id,
                    structureId: structureId
                });
            }
        );
    },
    duplicateItem: function (copiedFrom, newItem) {
        var current = parseItem(newItem);
        var targetPrev = parseItem(newItem.prev());
        var targetNext = parseItem(newItem.next());
        var targetParent = parseItem(newItem.parent().parent());
        var copyFrom = parseItem(copiedFrom);

        MT.Connector.Send(
            this.url + 'messageslist/',
            'post',
            {
                command: MT.requestCommand.duplicateItem,
                current: current,
                text: '',
                targetPrev: targetPrev,
                targetNext: targetNext,
                targetParent: targetParent,
                itemType: $(newItem).attr('data-itemType'),
                copiedFrom: copyFrom.id
            },
            function (response) {
                MT.handleServerCommand(response, { duplicatedItem: newItem, copyFrom: copiedFrom, itemType: $(newItem).attr('data-itemType') });
            }
        );
    },

    changeBranchUsers: function (panelName, brachId, userIdList) {
        MT.Connector.Send(
            this.url,
            'put',
            {
                command: MT.requestCommand.changeBranchUsers,
                branchId: brachId,
                userIdList: userIdList,
                panelName: panelName
            },
            function (response) {
                t14Lab.MessageTree.handleServerCommand(response, {});
            }
        );
    }
}

function parseItem(item) {
    var result = {
        id: item.attr('data-id'),
        index: item.attr('data-index'),
        parentId: item.attr('data-parentId'),
        type: item.attr('data-itemType')
    };
    return result;
}
