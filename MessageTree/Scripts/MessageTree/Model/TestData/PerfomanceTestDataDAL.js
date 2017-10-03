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
    getOptionsByParentId: function (parentId) {
        var result = [];
        recursiveFilter(options, parentId, result);
        return result;
    },

    recursive: function (objects, messageId, result, callback) {
        objects.forEach(function (message) {
            if (message.id == messageId) {
                result = message;
                callback(message);
            }
            if (message.hasOwnProperty('childs')) {
                MT.DeveloperTestDataDAL.recursive(message.childs, messageId, result, callback);
            }
        });
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
            messageText: "Structure 1",
            participants: [1, 2, 3],
            childs: [
                {
                    id: 110000,
                    itemType: "structure",
                    indexAsString: "100.100.0.0.0",
                    parentId: 1,
                    messageText: "Structure 1.1",
                    participants: [1, 2, 3],
                },
                {
                    id: 120000,
                    itemType: "structure",
                    indexAsString: "100.200.0.0.0",
                    parentId: 1,
                    messageText: "Structure 1.2",
                    participants: [1, 2],
                }
            ]
        },
        {
            id: 200000,
            itemType: "structure",
            indexAsString: "200.0.0.0.0",
            parentId: 1,
            messageText: "Structure 2 (Empty)",
            participants: [1],
        },
        {
            id: 300000,
            itemType: "structure",
            indexAsString: "300.0.0.0.0",
            parentId: 1,
            messageText: "Structure 3 Level 1",
            participants: [1],
            childs: [
                {
                    id: 310000,
                    itemType: "structure",
                    indexAsString: "300.100.0.0.0",
                    parentId: 1,
                    messageText: "Structure 3.1 Level 2",
                    participants: [1, 2, 3],
                    childs: [
                        {
                            id: 311000,
                            itemType: "structure",
                            indexAsString: "300.100.100.0.0",
                            parentId: 1,
                            messageText: "Structure 3.1.1 Level 3",
                            participants: [1, 2, 3],
                            childs: [
                                {
                                    id: 311100,
                                    itemType: "structure",
                                    indexAsString: "300.100.100.100.0",
                                    parentId: 1,
                                    messageText: "Structure 3.1.1.1 Level 4",
                                    participants: [1, 2, 3],
                                    childs: [
                                        {
                                            id: 311110,
                                            itemType: "structure",
                                            indexAsString: "300.100.100.100.100",
                                            parentId: 1,
                                            messageText: "Structure 3.1.1.1.1 Level 5",
                                            participants: [1, 2, 3],
                                        }
                                    ]
                                },
                            ]
                        }
                    ]
                }
            ]
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
            }

    ],

    options: [
    {
        id: 1,
        itemType: "option",
        indexAsString: "100.0.0.0.0",
        parentId: 100000,
        value: {
            name: "editor",
            mode: "plain"
        }
    },
    {
        id: 2,
        itemType: "option",
        indexAsString: "200.0.0.0.0",
        parentId: 110000,
        value: {
            name: "editor",
            mode: "html"
        }
    },
    {
        id: 3,
        itemType: "option",
        indexAsString: "300.0.0.0.0",
        parentId: 120000,
        value: {
            name: "editor",
            mode: "json"
        }
    },
    {
        id: 4,
        itemType: "option",
        indexAsString: "400.0.0.0.0",
        parentId: 200000,
        value: {
            name: "editor",
            mode: "plain"
        }
    },
    {
        id: 5,
        itemType: "option",
        indexAsString: "500.0.0.0.0",
        parentId: 300000,
        value: {
            name: "editor",
            mode: "plain"
        }
    }
    ]
}