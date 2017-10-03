t14Lab.MessageTree.ItemContainer = {
    clean: function (panelName) {
        var panel = MT.getPanel(panelName);
        panel = new MT.Panel();
        // clean panel
        t14Lab.MessageTree.ItemContainer.moveMode(panelName, false);
        t14Lab.MessageTree.ItemContainer.events(panelName, false);
        $("#" + panelName).html("");
    },
    events: function (panelName, enable, type) {
        var panel = MT.getPanel(panelName);
        var jqPanel = $("#" + panelName);
        if (enable) {
            jqPanel.resizable({
                minWidth: 600,
                grid: 50
            });
        } else {
            if (jqPanel.resizable("instance")) {
                jqPanel.resizable("destroy");
            }
        }
        this.collapsibleEvents(panelName, enable);
        this.toolbarEvents(panelName, enable);
        MT.Item.events(
            $('#' + panelName + ' .sortable .movableItem, ' +
                "#" + panelName + ' .sortable .standardItem, ' +
                '#' + panelName + ' .sortable .structure'
                ),
            $('#' + panelName + ' .sortable .addmessageItem'),
            enable
        );
        if (panel.type == "structure") {
            MT.Item.structureEvents(panelName, panelName, enable);
        }

        if (enable) {
            switch (type) {
                case "message":
                    t14Lab.bindEvent($('#' + panelName).find('.optionsButton'), "click", function () {
                        var structureId = $(this).attr('data-structureId');
                        var panelName = $(this).attr('data-panelName');
                        MT.API.loadOptions(panelName, structureId);
                    });
                    break;
                case "option":
                    t14Lab.bindEvent($('#' + panelName).find('.documentButton'), "click", function () {
                        var structureId = $(this).attr('data-structureId');
                        var panelName = $(this).attr('data-panelName');
                        MT.API.loadMessages(panelName, structureId);
                    });
                    break;
                default:
                    break;
            }
        } else {
            t14Lab.unbindEvent($('#' + panelName).find('.optionsButton'), "click");
            t14Lab.unbindEvent($('#' + panelName).find('.documentButton'), "click");
        }
    },
    collapsibleEvents: function (panelName, enable) {
        var levelMenu = $("#" + panelName + ' .levelMenu');
        if (!enable) {
            t14Lab.unbindEvent($('.' + panelName + ' li button'), "click");
            t14Lab.unbindEvent(levelMenu.find('.level1'), "click");
            t14Lab.unbindEvent(levelMenu.find('.level2'), "click");
            t14Lab.unbindEvent(levelMenu.find('.level3'), "click");
            t14Lab.unbindEvent(levelMenu.find('.level4'), "click");
            t14Lab.unbindEvent(levelMenu.find('.levelExpandAll'), "click");
            t14Lab.unbindEvent(levelMenu.find('.levelCollapseAll'), "click");
        } else {
            $.each($('#' + panelName + ' .sortable li'), function (k, el) {
                setParent($(el), panelName);
            });

            t14Lab.bindEvent($('.' + panelName + ' li button'), "click", function () {
                var action = $(this).data('action'),
                    item = $(this).parent('li'),
                    panel = $(this).data('panel');
                if (action === 'collapse') {
                    collapseItem(panel, item);
                }
                if (action === 'expand') {
                    expandItem(panel, item);
                }
            });
            t14Lab.bindEvent(levelMenu.find('.level1'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass("expanded")) {
                    expandItem(panelName, $('.' + panelName + ' > li'));
                    $(this).removeClass("expanded");
                    localStorage[panelName + '_Level1expanded'] = true;
                } else {
                    collapseItem(panelName, $('.' + panelName + ' > li'));
                    $(this).addClass("expanded");
                    localStorage[panelName + '_Level1expanded'] = false;
                }
            });
            t14Lab.bindEvent(levelMenu.find('.level2'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass("expanded")) {
                    expandItem(panelName, findByDepth('.' + panelName, 'li', 2));
                    localStorage[panelName + '_Level2expanded'] = true;
                    $(this).removeClass("expanded");
                } else {
                    collapseItem(panelName, findByDepth('.' + panelName, 'li', 2));
                    localStorage[panelName + '_Level2expanded'] = false;
                    $(this).addClass("expanded");
                }
            });
            t14Lab.bindEvent(levelMenu.find('.level3'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass("expanded")) {
                    expandItem(panelName, findByDepth('.' + panelName, 'li', 3));
                    localStorage[panelName + '_Level3expanded'] = true;
                    $(this).removeClass("expanded");
                } else {
                    collapseItem(panelName, findByDepth('.' + panelName, 'li', 3));
                    localStorage[panelName + '_Level3expanded'] = false;
                    $(this).addClass("expanded");
                }
            });
            t14Lab.bindEvent(levelMenu.find('.level4'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass("expanded")) {
                    expandItem(panelName, findByDepth('.' + panelName, 'li', 4));
                    localStorage[panelName + '_Level4expanded'] = true;
                    $(this).removeClass("expanded");
                } else {
                    collapseItem(panelName, findByDepth('.' + panelName, 'li', 4));
                    localStorage[panelName + '_Level4expanded'] = false;
                    $(this).addClass("expanded");
                }
            });
            t14Lab.bindEvent(levelMenu.find('.levelExpandAll'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                expandItem(panelName, $('.' + panelName + ' li'));
                localStorage[panelName + '_Level1expanded'] = true;
                localStorage[panelName + '_Level2expanded'] = true;
                localStorage[panelName + '_Level3expanded'] = true;
                localStorage[panelName + '_Level4expanded'] = true;
                $(this).parent().find('.level1').removeClass("expanded");
                $(this).parent().find('.level2').removeClass("expanded");
                $(this).parent().find('.level3').removeClass("expanded");
                $(this).parent().find('.level4').removeClass("expanded");
            });
            t14Lab.bindEvent(levelMenu.find('.levelCollapseAll'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                collapseItem(panelName, $('.' + panelName + ' li'));
                localStorage[panelName + '_Level1expanded'] = false;
                localStorage[panelName + '_Level2expanded'] = false;
                localStorage[panelName + '_Level3expanded'] = false;
                localStorage[panelName + '_Level4expanded'] = false;
                $(this).parent().find('.level1').addClass("expanded");
                $(this).parent().find('.level2').addClass("expanded");
                $(this).parent().find('.level3').addClass("expanded");
                $(this).parent().find('.level4').addClass("expanded");
            });
        }
    },
    toolbarEvents: function (panelName, enable) {
        var panel = MT.getPanel(panelName);
        var toolbar = $('#' + panelName + ' > .toolbar');
        if (!enable) {
            t14Lab.unbindEvent(toolbar.find('.loadLibrary'), "click");
            t14Lab.unbindEvent(toolbar.find('.enableInfo'), "click");
            t14Lab.unbindEvent(toolbar.find('.enableMove'), "click");
            t14Lab.unbindEvent(toolbar.find('.masterMode'), "click");
        } else {
            toolbar.find('.addUsersButton').click(function (event) {
                var elt = $('#' + panelName + 'AddUserInput');
                $('.' + panelName + 'avatarPreview').hide();
                $('#' + panelName + 'SaveButton').show();
                $('#' + panelName + 'AddUsersButton').hide();
                var branchId = $('#' + panelName).attr('data-branchid');

                $('#' + panelName + 'SaveButton').click(function () {
                    var newUserList = elt.val();
                    //console.log(elt.tagsinput('items'));
                    $('#' + panelName + 'AddUsersButton').show();
                    $('.' + panelName + 'avatarPreview').show();
                    $('#' + panelName + 'SaveButton').hide();
                    $('#' + panelName + 'AddUserInput').val('');
                    $('#' + panelName + 'AddUserInput').tagsinput('destroy');
                    $('#' + panelName + 'AddUserInput').hide();
                    $('.bootstrap-tagsinput').remove();

                    MT.API.changeBranchUsers(panelName, branchId, newUserList);

                });

                elt.tagsinput({
                    freeInput: false,
                    tagClass: function (item) {
                        switch (item.continent) {
                            case 'Europe': return 'label';
                            case 'America': return 'label';
                            case 'Australia': return 'label';
                            case 'Africa': return 'label';
                            case 'Asia': return 'label';
                        }
                    },
                    tagStyle: function (item) {
                        return "background-image: url(" + item.avatar + ")";
                    },
                    itemValue: 'value',
                    itemText: 'text',
                    typeahead: {
                        name: 'cities',
                        displayKey: 'text',
                        source: [{ "value": "1", "text": "IIIIIKIROY", "continent": "Europe", "avatar": "/img/avatar/kiroy.gif" },
                                  { "value": "12", "text": "dypsilon", "continent": "Europe", "avatar": "/img/avatar/dypsilon.gif" },
                                  { "value": "13", "text": "Sigmund", "continent": "Europe", "avatar": "/img/avatar/Sigmund.gif" },
                                  { "value": "14", "text": "MadDorris", "continent": "America", "avatar": "/img/avatar/MadDorris.gif" },
                                  { "value": "18", "text": "mono", "continent": "America", "avatar": "/img/avatar/mono.gif" },
                                  { "value": "20", "text": "Vadaboom", "continent": "America", "avatar": "/img/avatar/Vadaboom.jpg" },
                                  { "value": "19", "text": "guest", "continent": "America", "avatar": "/img/avatar/8.jpg" }
                        ]
                    }
                });

                $.each(eval(panel.participants), function (key, value) {
                    elt.tagsinput('add', { "value": value.id, "text": "Amsterdam", "continent": "Europe", "avatar": value.avatar });
                });

                elt.on('itemAdded', function (event) {
                    //console.log('itemAdded');
                });

                elt.on('itemRemoved', function (event) {
                    //console.log('itemRemoved');
                });

            });

            t14Lab.bindEvent(toolbar.find('.loadLibrary'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                MT.API.loadStructures(panelName);
            });
            t14Lab.bindEvent(toolbar.find('.enableInfo'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass('active')) {
                    $('.' + panelName + ' .messageIndexLabel, .' + panelName + ' .IdLabel').hide();
                    $(this).removeClass('active');
                } else {
                    $('.' + panelName + ' .messageIndexLabel, .' + panelName + ' .IdLabel').show();
                    $(this).addClass('active');
                }
            });
            t14Lab.bindEvent(toolbar.find('.enableMove'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                if ($(this).hasClass('active')) {
                    $('.' + panelName + ' .movableItem').addClass('standardItem').removeClass('movableItem');
                    $(this).removeClass('active');
                } else {
                    $(this).addClass('active');
                    t14Lab.MessageTree.ItemContainer.moveMode(panelName, true);
                    $('.' + panelName + ' .standardItem').addClass('movableItem').removeClass('standardItem');
                }
            });
            t14Lab.bindEvent(toolbar.find('.masterMode'), "click", function () {
                var panelName = $(this).attr('data-panelName');
                var currentPanel = $("#" + panelName);
                var nextPanel = currentPanel.nextAll('.panel').first();
                if ($(this).hasClass('active')) {
                    nextPanel.removeClass('manadgetPanel');
                    MT.Item.structureEvents(panelName, null, false);
                    MT.Item.structureEvents(panelName, panelName, true);
                    nextPanel.find('.loadLibrary').show();
                } else {
                    nextPanel.addClass('manadgetPanel');
                    nextPanel.find('.loadLibrary').hide();
                    MT.Item.structureEvents(panelName, null, false);
                    MT.Item.structureEvents(panelName, nextPanel.attr('id'), true);

                    $('#panel2 .overlayMessageBox').remove();
                    $('.dd3-handle').hide();
                }
            });

        }
    },
    moveMode: function (panelName, enable) {
        if (!enable) {
            try {
                $('#' + panelName + ' ol').nestedSortable('destroy');
            } catch (e) {

            }
        } else {
            $('#' + panelName + ' ol').nestedSortable({
                disableNesting: 'no-nest',
                forcePlaceholderSize: true,
                handle: 'div',
                helper: 'clone',
                items: 'li',
                maxLevels: 5,
                opacity: .6,
                placeholder: 'placeholder',
                revert: 250,
                tabSize: 25,
                tolerance: 'pointer',
                toleranceElement: '> div',
                connectWith: 'ol.panel1, ol.panel2, ol.panel3',

                start: function (event, ui) {
                    //console.log(event);
                    //console.log(ui);
                    $(ui.item).show();
                    clone = $(ui.item).clone();
                    before = $(ui.item).prev();
                    if (event.ctrlKey) {
                        $(ui.helper).find('.dd3-content').css('border', '2px solid #7B4F9D');
                    }
                },

                stop: function (event, ui) {
                    if (event.ctrlKey) {
                        before.after(clone);
                        MT.API.duplicateItem(clone, $(ui.item));
                    } else {
                        MT.API.moveItem($(ui.item));
                        clone = {};
                        before = {};
                    }
                }

            }).disableSelection();
        }
    },

    handleStructuresLoadedCommand: function (response, panelName) {
        this.clean(panelName);
        var panel = MT.getPanel(panelName);
        if (panelName != panel.panelName) { throw "Something goes wrong."; }
        panel.type = "structure";
        panel.gridType = "messageGrid";

        if ($('#' + panelName).length) {
            $('#' + panelName).replaceWith(MT.itemContainerTemplate(panel));
        } else {
            $('#playArea').append(MT.itemContainerTemplate(panel));
        }
        var itemsCount = MT.ItemContainer.renderItems(response.data, $("#" + panelName + " ol.sortable"), "structure", 1, MT.itemTemplate);

        var headerContext = {
            panelName: panelName,
            itemType: "structure"
        };
        $('.' + panelName + 'ToolBar').find('.headerPlaceholder').html(MT.libraryHeaderTemplate(headerContext));
        MT.ItemContainer.events(panelName, true);

        $('.' + panelName + ' li[data-count="' + itemsCount + '"]').addClass("lastLiInDocument");

        $('.' + panelName + 'ToolBar').find('.masterMode').show();
        $('.' + panelName + 'ToolBar').find('.loadLibrary').hide();
        $("#" + panelName + " .masterMode").click();

        //collapseItem($('.' + panelName + ' li'));
    },
    handleMessagesLoadedCommand: function (response, panelName) {
        this.renderDocument(response, panelName, "message");
    },
    handleOptionsLoadedCommand: function (response, panelName) {
        this.renderDocument(response, panelName, "option");
    },
    renderDocument: function (response, panelName, type) {
        this.clean(panelName);
        var panel = MT.getPanel(panelName);
        if (panelName != panel.panelName) { throw "Something goes wrong."; }
        panel.type = "message";
        panel.gridType = "messageGridFilled";

        panel.participants = response.participants;

        if ($('#' + panelName).length) {
            $('#' + panelName).replaceWith(MT.itemContainerTemplate(panel));
        } else {
            $('#playArea').append(MT.itemContainerTemplate(panel));
        }
        var itemsCount = MT.ItemContainer.renderItems(response.data, $("#" + panelName + " ol.sortable"), "message", 1, MT.itemTemplate);
        //$('.' + panelName + ' li[data-count="' + itemsCount + '"]').addClass("lastLiInDocument");
        //$('.' + panelName + ' li[data-count="' + itemsCount + '"] .addmessageItemLeft').html('<i class="fa fa-plus-circle"></i> Add New Level 1');
        //$('.' + panelName + ' li[data-count="' + itemsCount + '"] .addmessageItemRight').html('<i class="fa fa-plus-circle"></i> Reply above');

        $('.' + panelName + 'ToolBar').find('.loadLibrary').show();
        $('.' + panelName + 'ToolBar').find('.masterMode').hide();

        var headerTemplate;
        var headerContext;
        switch (type) {
            case "structure":
                headerTemplate = MT.libraryHeaderTemplate;
                headerContext =
                {
                    panelName: panelName,
                    itemType: "structure"
                };
                break;
            case "message":
                headerTemplate = MT.documentHeaderTemplate;
                headerContext =
                {
                    panelName: panelName,
                    lastPart: response.structureName,
                    navigationPath: response.navigationPath,
                    participants: response.participants,
                    structureId: response.structureId,
                    itemType: "message"
                };
                break;
            case "option":
                headerTemplate = MT.optionsHeaderTemplate;
                headerContext =
                {
                    panelName: panelName,
                    lastPart: response.structureName,
                    navigationPath: response.navigationPath,
                    participants: response.participants,
                    structureId: response.structureId,
                    itemType: "option"
                };
                break;
            default:
                break;
        }
        $('.' + panelName + 'ToolBar').find('.headerPlaceholder').html(headerTemplate(headerContext));
        panel.options = response.structureOptions;

        $('.navigationPath a').click(function () {
            var id = $(this).attr('data-structureId');
            MT.API.loadMessages(panelName, id);
        });

        $('.dd3-content').removeClass('highlighted');
        $('#mbHandle' + response.data.structureId).addClass('highlighted');


        SyntaxHighlighter.defaults['gutter'] = false;
        SyntaxHighlighter.highlight();
        t14Lab.MessageTree.ItemContainer.events(panelName, true, type);
        MT.checkNewMessages();
        if ($(".newMessage").length > 0) {
            $('html, body').animate({ scrollTop: ($(".newMessage").offset().top - 109) }, 400);
        }
    },
    renderItems: function (itemList, itemContainer, type, level, template, count) {
        if (!level) level = 1;
        if (!count) count = 0;
        itemList.forEach(function (item) {
            item.level = level;
            count++;
            item.count = count;
            var appendedObject = itemContainer.append(template(item));
            if (item.hasOwnProperty("childs")) {
                var newOL = appendedObject.find("ol#" + type + "Item" + item.id);
                level++;
                count = MT.ItemContainer.renderItems(item.childs, newOL, type, level, template, count);
            }
        });
        return count;
    },


    handleUsersUpdatedCommand: function (response, params) {
        $('.' + response.data.panelName + 'avatarPreview').html(response.data.newUserPreviewHtml);
    }

}


var setParent = function (li, panelName1) {
    if (li.children('ol').length) {
        li.prepend($('<button data-action="expand" data-panel="' + panelName1 + '" type="button" class="disclose">Expand</button>'));
        li.prepend($('<button data-action="collapse" data-panel="' + panelName1 + '" type="button" class="disclose">Collapse</button>'));
    } else {
        li.removeClass('dd-collapsed');
    }
    li.children('[data-action="expand"]').hide();
}
var expandItem = function (panelName, li) {
    li.removeClass('dd-collapsed');
    li.children('[data-action="expand"]').hide();
    li.children('[data-action="collapse"]').show();
    li.children('ol').show();

    try {
        var expandedNodes = $.cookie("expandedNodes");
        if (expandedNodes) {
            expandedNodes = expandedNodes.replace(",,", ',');
            expandedNodes = expandedNodes.replace(",undefined", '');
            expandedNodes = expandedNodes.replace("undefined", '');
            expandedNodes = expandedNodes.replace("," + li.attr('id'), '');
            expandedNodes = expandedNodes.replace(li.attr('id'), '');
        }
        $.cookie("expandedNodes", expandedNodes + "," + li.attr('id'));
    } catch (e) { };
}
var collapseItem = function (panelName, li, withoutCookie) {
    if (typeof (withoutCookie) === 'undefined') withoutCookie = false;
    var lists = li.children('ol');
    if (lists.length) {
        $.each(li, function (index, liEllement) {
            if ($(liEllement).children('ol').length > 0) {
                $(liEllement).addClass('dd-collapsed');
                $(liEllement).children('[data-action="collapse"]').hide();
                $(liEllement).children('[data-action="expand"]').show();
                $(liEllement).children('ol').hide();
            }
        });
        if (!withoutCookie) {
            try {
                var expandedNodes = $.cookie("expandedNodes");
                if (expandedNodes) {
                    expandedNodes = expandedNodes.replace(",undefined", '');
                    expandedNodes = expandedNodes.replace("undefined", '');
                    expandedNodes = expandedNodes.replace("," + li.attr('id'), '');
                    expandedNodes = expandedNodes.replace(li.attr('id'), '');
                }
                $.cookie("expandedNodes", expandedNodes);
            } catch (e) { };
        }

    } else {
        li.removeClass('dd-collapsed');
    }

}
function findByDepth(parent, child, depth) {
    var children = $();
    $(child, $(parent)).each(function () {
        if ($(this).parentsUntil(parent, child).length == (depth - 1)) {
            children = $(children).add($(this));
        }
    });
    return children;
}
function findUntilDepth(parent, child, depth) {
    var children = $();
    $(child, $(parent)).each(function () {
        if ($(this).parentsUntil(parent, child).length <= (depth - 1)) {
            children = $(children).add($(this));
        }
    });
    return children;
}
jQuery.fn.highlight = function (pat) {
    function innerHighlight(node, pat) {
        var skip = 0;
        if (node.nodeType == 3) {
            var pos = node.data.toUpperCase().indexOf(pat);
            if (pos >= 0) {
                var spannode = document.createElement('span');
                spannode.className = 'highlight';
                var middlebit = node.splitText(pos);
                var endbit = middlebit.splitText(pat.length);
                var middleclone = middlebit.cloneNode(true);
                spannode.appendChild(middleclone);
                middlebit.parentNode.replaceChild(spannode, middlebit);
                skip = 1;
            }
        }
        else if (node.nodeType == 1 && node.childNodes && !/(script|style)/i.test(node.tagName)) {
            for (var i = 0; i < node.childNodes.length; ++i) {
                i += innerHighlight(node.childNodes[i], pat);
            }
        }
        return skip;
    }
    return this.length && pat && pat.length ? this.each(function () {
        innerHighlight(this, pat.toUpperCase());
    }) : this;
};
jQuery.fn.removeHighlight = function () {
    return this.find("span.highlight").each(function () {
        this.parentNode.firstChild.nodeName;
        with (this.parentNode) {
            replaceChild(this.firstChild, this);
            normalize();
        }
    }).end();
};