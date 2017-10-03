t14Lab.MessageTree.Item = {
    type: {
        message: "message",
        structure: "structure",
        option: "option",
        todo: "todo"
    },

    events: function (messageItems, selectorAddNewButton, enable) {
        t14Lab.MessageTree.Item.operationsEvents(messageItems, enable);

        if (enable) {

            messageItems.click(function () {
                if (!$(this).hasClass('selected')) {
                    $('.dd3-content').removeClass('selected');
                    $(this).addClass('selected');
                } else {
                    $(this).removeClass('selected');
                }
            });

            messageItems.on("taphold", function (e) {
                var messageItemOuter = $(this).parent();
                var messageId = messageItemOuter.attr('data-id');
                var panelName = $(this).parents('.panel').attr('id');
                t14Lab.MessageTree.Item.editMode(panelName, messageId, true, messageItemOuter.attr('data-itemType'));
            });

        } else {
            // todo find panel and disable events of readmore links from current panel	
            $('.readmore').unbind('click');
            messageItems.unbind('click');
        }

        t14Lab.MessageTree.Item.addNewEvents(selectorAddNewButton, enable);
    },
    operationsEvents: function (selector, enable) {
        if (logEvents) {
            console.log('event: t14Lab.MessageTree.Paragraph.Operations - ' + selector + ' - ' + enable);
        }
        if (!enable) {
            selector.unbind('hover');
            selector.find(".buttonEdit").remove();
            selector.find(".buttonDelete").remove();
        } else {
            selector.hover(
                function () {
                    if (!$(this).hasClass('editMode') && !$(this).hasClass('movableItem')) {
                        var messageItemOuter = $(this).parent();
                        var messageId = messageItemOuter.attr('data-id');
                        $(this).append('<div class="messageItemButton buttonDelete"><i class="fa fa-trash-o"></i>').find('.buttonDelete')
                        .click(function (e) {
                            e.stopPropagation();
                            if (Defaults.GUI.Panel.showDeleteConfirmation) {
                                $('#myModal').modal({
                                    keyboard: true
                                }).find('.btn-danger').click(function () {
                                    MT.API.deleteItem(messageId, messageItemOuter.attr('data-itemType'));
                                });
                                $('.modal-body p').html($('#messageText_' + messageId).html());
                            } else {
                                MT.API.deleteItem(messageId, messageItemOuter.attr('data-itemType'));
                            }
                            $('#myModal .btn-success').click(function () {
                                $('#myModal').modal('hide');
                            });
                        });

                        $(this).append('<div class="messageItemButton buttonEdit"><i class="fa fa-pencil"></i></div>');
                        $(this).find(".buttonEdit").bind('click', function (e) {
                            e.stopPropagation();
                            
                            var panelId = $(this).parents('.panel').attr("id");
                            MT.Item.editMode(panelId, messageId, true, messageItemOuter.attr('data-itemType'));
                        });
                    }
                },
                function () {
                    $(this).find(".messageItemButton").unbind('click').remove();
                }
            );
        }
    },
    addNewEvents: function (selector, enable) {
        if (logEvents) {
            console.log('event: t14Lab.MessageTree.Paragraph.AddNewButton - ' + selector + ' - ' + enable);
        }
        if (!enable) {
            selector.unbind('hover click');
        } else {
            selector.hover(
                function () {
                    if (Defaults.GUI.Panel.insertMessageMode == 3) {
                        $('#mbItem' + $(this).attr('data-id')).append('<li id="newMessagePlatzhalter" class="neww"><div class="movableItem message"><p>New message</p></div><div class="addmessageItem"></div></li>');
                        MT.blink('#newMessagePlatzhalter');
                    }
                },
                function () {
                    if (Defaults.GUI.Panel.insertMessageMode == 3) {
                        $('#newMessagePlatzhalter').remove();
                    }
                }
            );
            selector.on("taphold", function (e) {
                var id = $(this).attr('data-id');
                var type = $(this).attr('data-itemType');
                var panelId = $(this).parents('.panel').attr("id");
                if ($(e.target).hasClass('addmessageItemRight')) {
                    MT.API.addItem(panelId, id, type, true);
                } else {
                    MT.API.addItem(panelId, id, type, false);
                }
            });
        }
    },
    structureEvents: function (panelName, targetPanelName, enable) {
        if (!enable) {
            t14Lab.unbindEvent($('.' + panelName + ' .structure'), "click");
        } else {
            t14Lab.bindEvent($('.' + panelName + ' .structure'), "click", function () {
                MT.API.loadMessages(targetPanelName, $(this).attr('data-id'));

                try {
                    $.cookie("panel" + targetPanelName, $(this).attr('data-id'));
                } catch (e) { };

            });
            $('.showAllAvatars').hover(function () {
                $(this).parent().find('.hiddenAvatar').show();
            }, function () {
                $(this).parent().find('.hiddenAvatar').hide();
            });
        }
    },

    editMode: function (panelName, messageId, enable, itemType) {
        var wyEditor = true;

        var textShortcut;
        //var messageItemOuter = $('#mbItem'+messageId);
        //var messageItemInner = messageItemOuter.find('#mbHandle'+messageId);  
        var panel = MT.getPanel(panelName);
        var itemLi = $("#" + panelName + ' .' + itemType + 'Item' + messageId);
        var itemDiv = itemLi.find('.' + itemType + 'handle' + messageId).first();
        var itemId = itemLi.attr('data-id');
        var itemParentId = itemLi.attr('data-parentid');
        itemDiv.off("taphold");
        var isStructure = false;
        switch (itemType) {
            case t14Lab.MessageTree.Item.type.structure:
                textShortcut = '#' + itemType + "Text_";
                isStructure = true;
                break;
            case t14Lab.MessageTree.Item.type.message:
                textShortcut = '#' + itemType + "Text_";
                break;
            case t14Lab.MessageTree.Item.type.option:
                wyEditor = false;
                textShortcut = '#' + itemType + "Text_";
                break;
            default:
                break;
        }
        if (!enable) {
            try {
                itemDiv.resizable('destroy');
            } catch (ex) { };

            itemDiv.css('height', "auto");
            itemDiv.css('width', "auto");

            //$(".nightMode").fadeOut('fast');

            //itemDiv.css('margin-top', '0px');
            //itemDiv.css('margin-left', '0px');
        } else {
            MT.API.loadItem(itemId, itemType, function (response) {
                console.log("111");
                MT.ItemEditor.init(itemId, itemType, itemLi, itemDiv, textShortcut, panel, response, wyEditor, isStructure);
            });
        }
    },

    handleLoadedCommand: function (type, itemId, content) {
        switch (type) {
            case "structure":
                this.renderStructureItem(itemId, content);
                break;
            case "message":
                this.renderMessageItem(itemId, content);
                break;
            case "option":
                this.renderOptionItem(itemId, content);
                break;
            default:
                throw "Item.handleLoadedCommand() -> Unknow Type";
        }
    },
    handleDeletedCommand: function (response, params) {
        if (Defaults.GUI.Panel.showDeleteConfirmation) {
            $('#myModal').modal('hide');
        }
        var itemLi = $("." + response.type + "Item" + response.id);
        t14Lab.MessageTree.Item.events($('.messagehandle' + response.id),
            itemLi.find('.addmessageItem'), false);
        itemLi.remove();
    },
    handleEditedCommand: function (response, params) {
        var messageId = response.item.id;
        var messageOuter = $('#mbItem' + messageId);
        $('#editTextArea' + messageId).text(response.item.messageText);
    },
    handleMovedCommand: function (response, params) {
        /*
        params.movedItem.attr("data-index", response.data.newIndex);
        params.movedItem.attr("data-parentId", response.data.newParentId);
        $("#label_" + response.data.currentItemId).text(response.data.newIndex);
        jQuery.each(response.data.childs, function (i, val) {
            $("#label_" + val.id).text(val.index);
        });
        */
    },
    handleDuplicatedCommand: function (response, params) {
        params.duplicatedItem.attr("data-index", response.data.newIndex);
        params.duplicatedItem.attr("data-parentId", response.data.newParentId);

        params.duplicatedItem.find('.messageText').html('<div class="duplicatedParentPreview">' + params.copyFrom.find('.messageText').html() + '</div>');
        $("#label_" + response.data.currentItemId).text(response.data.newIndex);
        jQuery.each(response.data.childs, function (i, val) {
            $("#label_" + val.id).text(val.index);
        });
    },
    handleAddedCommand: function (panelId, insertAfterId, newMessage, itemType, childAdded) {
        var newItem = MT.itemTemplate(newMessage);
        if (childAdded) {
            var elementChilds = $('#mbItem' + insertAfterId).find('ol:first');

            if (elementChilds.length > 0) {
                elementChilds.prepend(newItem);
            } else {
                $('#mbItem' + insertAfterId).append('<ol class="dd-list">' + newItem + '</ol>');
            }
        } else {
            $(newItem).insertAfter('#mbItem' + insertAfterId);
        }

        t14Lab.MessageTree.Item.events(
            $('#mbHandle' + newMessage.id)
            , $('#mbItem' + newMessage.id).find('.addmessageItem')
            , true
        );
        t14Lab.MessageTree.Item.editMode(panelId, newMessage.id, true, itemType);
        try {
        } catch (e) {
            $('#mbItem' + newMessage.id).html('<div style="padding: 5px;background-color: darkred;color: white;margin-bottom: 9px;">Error, please do reload</div>');
        }

    },
    handleMarkedAsReadCommand: function (messageId, structureId) {
        $.each(dontReadedMessages, function (i) {
            if (dontReadedMessages[i] == messageId) {
                dontReadedMessages.splice(i, 1);
                return false;
            }
        });
        $.each(dontReadedstructures, function (i, value) {
            if (i == structureId) {
                dontReadedstructures[i] = dontReadedstructures[i] - 1;
                return false;
            }
        });

        var messageItem = $('.messageItem' + messageId).find('.message');
        messageItem.removeClass('newMessage');

        var counterLabel = $('#structureText_' + structureId).find('.dontReadedStucture');
        var count = parseInt(counterLabel.html());
        count--;
        var newCount = count;
        if (newCount > 0) {
            counterLabel.text(newCount);
        } else {
            counterLabel.remove();
        }
    },

    renderMessageItem: function (messageId, content) {
        $('#messageText_' + messageId).html(content);
        t14Lab.MessageTree.Item.editMode(messageId, false, t14Lab.MessageTree.Item.type.message);
        t14Lab.MessageTree.Item.operationsEvents(
            $('#mbHandle' + messageId),
            true
        );
        SyntaxHighlighter.highlight();
    },
    renderStructureItem: function (structureId, content) {
        $('#structureText_' + structureId).html(content);
        t14Lab.MessageTree.Item.editMode(structureId, false, t14Lab.MessageTree.Item.type.structure);
        t14Lab.MessageTree.Item.operationsEvents(
        $('#mbHandle' + structureId),
        true
        );
    },
    renderOptionItem: function (optionId, content) {
        $('#optionText_' + optionId).html(content);
        t14Lab.MessageTree.Item.editMode(optionId, false, t14Lab.MessageTree.Item.type.option);
        t14Lab.MessageTree.Item.operationsEvents(
            $('#mbHandle' + optionId),
            true
        );

        $('#mbItem' + optionId).parents('ol.sortable').attr('data-panelName');
        SyntaxHighlighter.highlight();
    },
};

