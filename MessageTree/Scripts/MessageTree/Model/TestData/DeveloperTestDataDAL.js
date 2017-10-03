t14Lab.MessageTree.DeveloperTestDataDAL = {
    filterUsers: function (idList) {
        var result = [];
        this.users.forEach(function (user) {
            idList.forEach(function (filter) {
                if (user.id == filter) {
                    result.push(user);
                }
            });
        });
        return result;
    },
    getStructures: function () {
        return this.structures;
    },
    getMessagesForStructure: function (structureId) {
        var result = [];
        // get first level messages (with .childs property)
        this.messages.forEach(function (obj) {
            if (obj['parentId'] == structureId) {
                result.push(obj);
            }
        });
        return result;
    },
    getOptionsForStructure: function (structureId) {
        var result = [];
        // get first level messages (with .childs property)
        this.options.forEach(function (obj) {
            if (obj['parentId'] == structureId) {
                result.push(obj);
            }
        });
        return result;
    },
    getMessageById: function (messageId, callback) {
        var result;
        this.recursive(this.messages, messageId, result, function (message) {
            callback(message);
        });
    },
    getStructureById: function (structureId, callback) {
        var result;
        this.recursive(this.structures, structureId, result, function (result) {
            callback(result);
        });
    },
    getOptionById: function (optionId, callback) {
        var result;
        this.recursive(this.options, optionId, result, function (message) {
            callback(message);
        });
    },
    getOptionsByParentId: function (parentId) {
        var result = [];
        recursiveFilter(options, parentId, result);
        return result;
    },
    getStructureNavigationPath: function (structure, pathArray) {
        if (!pathArray) {
            pathArray = [];
        }
        pathArray.push({
            id: structure.id,
            name: structure.messageText
        });
        if (structure.hasOwnProperty("parent") && structure.parent) {
            MT.DeveloperTestDataDAL.getStructureNavigationPath(structure.parent, pathArray);
        }
        return pathArray;
    },
    insertAfter: function (insertAfterId, itemType, asChild, successFunction) {
        switch (itemType) {
            case "message":
                this.getMessageById(insertAfterId, function (item) {
                    var newItem = {
                        id: t14Lab.generateUUID(),
                        itemType: "message",
                        indexAsString: "",
                        parentId: item.parentId,
                        messageText: "[Empty]"
                    }
                    if (item.hasOwnProperty("parent") && item.parent) {
                        var indexOfItem = item.parent.childs.indexOf(item);
                        item.parent.childs.splice(indexOfItem, 0, newItem);
                    } else {
                        if (item.hasOwnProperty("childs")) {
                            item.childs.splice(0, 0, newItem);
                        } else {
                            item.childs = [];
                            item.childs.push(newItem);
                        }
                    }
                    successFunction(newItem);
                });

                break;
            case "structure":
                break;
            case "option":
                break;
            default:
                break;
        }
    },
    deleteItem: function (items, itemId, success) {
        MT.DeveloperTestDataDAL.getMessageById(itemId, function (item) {
            if (item.parent) {
                var index = item.parent.childs.indexOf(item);
                item.parent.childs.splice(index, 1);
            } else {
                var index = items.indexOf(item);
                items.splice(index, 1);
            }
            success(item);
        });
    },
    editItem: function (itemType, itemId, newText, success) {
        switch (itemType) {
            case "structure":
                this.getStructureById(itemId, function (item) {
                    item.messageText = newText;
                    success(item);
                })
                break;
            case "message":
                this.getMessageById(itemId, function (item) {
                    item.messageText = newText;
                    success(item);
                })
                break;
            case "option":
                this.getOptionById(itemId, function (item) {
                    item.messageText = newText;
                    success(item);
                })
                break;
            default:
                break;
        }
    },
    recursive: function (objects, messageId, result, callback) {
        var BreakException = {};
        try {
            objects.forEach(function (message) {
            if (message.id == messageId) {
                result = message;
                callback(message);
                throw BreakException;
            }
            if (message.hasOwnProperty('childs')) {
                MT.DeveloperTestDataDAL.recursive(message.childs, messageId, result, callback);
            }
        });
        } catch (e) {
            if (e !== BreakException) throw e;
        }
    },
    recursiveFilter: function (objects, structureId, result) {
        objects.forEach(function (obj) {
            if (obj['parentId'] == structureId) {
                result.push(obj);
            }
            if (obj.hasOwnProperty('childs')) {
                recursiveFilter(obj.childs, structureId, result);
            }
        });
        return result;
    },
    setParents: function (objects, level, parent) {
        var dal = this;
        if (!level) level = 1;
        objects.forEach(function (item) {
            item.level = level;
            if (level > 1) {
                item.parent = parent;
            }
            if (item.hasOwnProperty("childs")) {
                level++;
                dal.setParents(item.childs, level, item);
            }
        });
    },

    // Data ----------------------------------------------------------------
    users: [
        { "id": "1", "avatar": "User1.jpg", "name": "User 1" },
        { "id": "2", "avatar": "User2.jpg", "name": "User 2" },
        { "id": "3", "avatar": "User3.jpg", "name": "User 3" },
        { "id": "4", "avatar": "User4.jpg", "name": "User 4" },
        { "id": "5", "avatar": "User5.jpg", "name": "User 5" },
        { "id": "6", "avatar": "User6.jpg", "name": "User 6" },
        { "id": "7", "avatar": "User7.jpg", "name": "User 7" },
        { "id": "8", "avatar": "User8.jpg", "name": "User 8" },
        { "id": "9", "avatar": "User9.jpg", "name": "User 9" },
        { "id": "10", "avatar": "User10.jpg", "name": "User 10" },
    ],

    structures: [
        {
            id: 100000,
            itemType: "structure",
            indexAsString: "100.0.0.0.0",
            parentId: 1,
            messageText: "1 All combinations",
            participants: [1, 2, 3],
            childs: [
                {
                    id: 110000,
                    itemType: "structure",
                    indexAsString: "100.100.0.0.0",
                    parentId: 1,
                    messageText: "Document 1.1",
                    participants: [1, 2, 3],
                },
                {
                    id: 120000,
                    itemType: "structure",
                    indexAsString: "100.200.0.0.0",
                    parentId: 1,
                    messageText: "Document 1.2",
                    participants: [1, 2],
                }
            ]
        },
        {
            id: 200000,
            itemType: "structure",
            indexAsString: "200.0.0.0.0",
            parentId: 1,
            messageText: "2 Empty document",
            participants: [1],
        },
        {
            id: 300000,
            itemType: "structure",
            indexAsString: "300.0.0.0.0",
            parentId: 1,
            messageText: "3 Level 1",
            participants: [1],
            childs: [
                {
                    id: 310000,
                    itemType: "structure",
                    indexAsString: "300.100.0.0.0",
                    parentId: 1,
                    messageText: "3.1 Level 2",
                    participants: [1, 2, 3],
                    childs: [
                        {
                            id: 311000,
                            itemType: "structure",
                            indexAsString: "300.100.100.0.0",
                            parentId: 1,
                            messageText: "3.1.1 Level 3",
                            participants: [1, 2, 3],
                            childs: [
                                {
                                    id: 311100,
                                    itemType: "structure",
                                    indexAsString: "300.100.100.100.0",
                                    parentId: 1,
                                    messageText: "3.1.1.1 Level 4",
                                    participants: [1, 2, 3],
                                    childs: [
                                        {
                                            id: 311110,
                                            itemType: "structure",
                                            indexAsString: "300.100.100.100.100",
                                            parentId: 1,
                                            messageText: "3.1.1.1.1 Level 5",
                                            participants: [1, 2, 3],
                                        }
                                    ]
                                },
                            ]
                        }
                    ]
                }
            ]
        },
        {
            id: 400000,
            itemType: "structure",
            indexAsString: "400.0.0.0.0",
            parentId: 1,
            messageText: "Test move",
            participants: [1]
        },
        {
            id: 500000,
            itemType: "structure",
            indexAsString: "500.0.0.0.0",
            parentId: 1,
            messageText: "Document 5",
            participants: [1]
        },
        {
            id: 600000,
            itemType: "structure",
            indexAsString: "600.0.0.0.0",
            parentId: 1,
            messageText: "Document 6",
            participants: [1]
        },
        {
            id: 700000,
            itemType: "structure",
            indexAsString: "700.0.0.0.0",
            parentId: 1,
            messageText: "Document 7",
            participants: [1]
        },
        {
            id: 800000,
            itemType: "structure",
            indexAsString: "800.0.0.0.0",
            parentId: 1,
            messageText: "Document 8",
            participants: [1]
        },
        {
            id: 900000,
            itemType: "structure",
            indexAsString: "900.0.0.0.0",
            parentId: 1,
            messageText: "Document 9",
            participants: [1]
        },
        {
            id: 1000000,
            itemType: "structure",
            indexAsString: "1000.0.0.0.0",
            parentId: 1,
            messageText: "Document 10",
            participants: [1]
        }
    ],

    messages: [
            {
                id: 101,
                itemType: "message",
                indexAsString: "100.0.0.0.0",
                parentId: 100000,
                messageText: "Message 1",
                childs: [
                    {
                        id: 102,
                        itemType: "message",
                        indexAsString: "100.100.0.0.0",
                        parentId: 100000,
                        messageText: "Message 1.1",
                    },
                    {
                        id: 103,
                        itemType: "message",
                        indexAsString: "100.200.0.0.0",
                        parentId: 100000,
                        messageText: "Message 1.2",
                    }
                ]
            },
            {
                id: 104,
                itemType: "message",
                indexAsString: "200.0.0.0.0",
                parentId: 100000,
                messageText: "Message 2"
            },
            {
                id: 105,
                itemType: "message",
                indexAsString: "300.0.0.0.0",
                parentId: 100000,
                messageText: "Message 3",
                childs: [
                    {
                        id: 106,
                        itemType: "message",
                        indexAsString: "300.100.0.0.0",
                        parentId: 100000,
                        messageText: "Message 3.1",
                        childs: [
                            {
                                id: 107,
                                itemType: "message",
                                indexAsString: "300.100.100.0.0",
                                parentId: 100000,
                                messageText: "Message 3.1.1",
                                childs: [
                                    {
                                        id: 108,
                                        itemType: "message",
                                        indexAsString: "300.100.100.100.0",
                                        parentId: 100000,
                                        messageText: "Message 3.1.1.1",
                                        childs: [
                                            {
                                                id: 109,
                                                itemType: "message",
                                                indexAsString: "300.100.100.100.100",
                                                parentId: 100000,
                                                messageText: "Message 3.1.1.1.1"
                                            }
                                        ]
                                    }
                                ]
                            }
                        ]
                    }
                ]
            },
            {
                id: 201,
                itemType: "message",
                indexAsString: "100.0.0.0.0",
                parentId: 110000,
                messageText: "Message 1.1"
            },
            {
                id: 301,
                itemType: "message",
                indexAsString: "100.0.0.0.0",
                parentId: 120000,
                messageText: "Message 1.2"
            },



            {
                id: 501,
                itemType: "message",
                indexAsString: "300.0.0.0.0",
                parentId: 300000,
                messageText: "Message 3"
            },
            {
                id: 502,
                itemType: "message",
                indexAsString: "300.100.0.0.0",
                parentId: 310000,
                messageText: "Message 3.1"
            },
            {
                id: 503,
                itemType: "message",
                indexAsString: "300.100.100.0.0",
                parentId: 311000,
                messageText: "Message 3.1.1"
            },
            {
                id: 504,
                itemType: "message",
                indexAsString: "300.100.100.100.0",
                parentId: 311100,
                messageText: "Message 3.1.1.1"
            },
            {
                id: 505,
                itemType: "message",
                indexAsString: "300.100.100.100.100",
                parentId: 311110,
                messageText: "Message 3.1.1.1.1"
            },

            {
                id: 604,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "First Element"
            }, {
                id: 605,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "Second Element"
            },

            {
                id: 606,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#Root First (Move this before 'First Element')"
            }, {
                id: 607,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#Root Middle (Move this between 'First Element' and 'Second Element')"
            }, {
                id: 608,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#Root Last (Move this after 'Last Element')"
            }, {
                id: 609,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#First Child (Move this before 'First Child')"
            }, {
                id: 610,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#Middle Child (Move this between 'First Child' and 'Last Child')"
            }, {
                id: 611,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#Last Child (Move this after 'Last Child')"
            }, {
                id: 612,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "#New Child (Move this under 'Second Element' as child)"
            }, {
                id: 650,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "Element with childs",
                childs: [
                    {
                        id: 651,
                        itemType: "message",
                        indexAsString: "400.100.100.100.100",
                        parentId: 400000,
                        messageText: "First Child"
                    }, {
                        id: 652,
                        itemType: "message",
                        indexAsString: "400.100.100.100.100",
                        parentId: 400000,
                        messageText: "Last Child"
                    }
                ]
            }, {
                id: 690,
                itemType: "message",
                indexAsString: "400.100.100.100.100",
                parentId: 400000,
                messageText: "Last Element"
            }


    ],

    options: [
    {
        id: 1,
        itemType: "option",
        indexAsString: "100.0.0.0.0",
        parentId: 100000,
        messageText: '{ name: "editor", mode: "plain" }'
    },
    {
        id: 2,
        itemType: "option",
        indexAsString: "200.0.0.0.0",
        parentId: 110000,
        messageText: '{ name: "editor", mode: "html" }'
    },
    {
        id: 3,
        itemType: "option",
        indexAsString: "300.0.0.0.0",
        parentId: 120000,
        messageText: '{ name: "editor", mode: "json" }'
    },
    {
        id: 4,
        itemType: "option",
        indexAsString: "400.0.0.0.0",
        parentId: 200000,
        messageText: '{ name: "editor", mode: "plain" }'
    },
    {
        id: 5,
        itemType: "option",
        indexAsString: "500.0.0.0.0",
        parentId: 300000,
        messageText: '{ name: "editor", mode: "plain" }'
    }
    ]
}