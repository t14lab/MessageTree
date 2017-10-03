t14Lab.MessageTree.ItemEditor = {
    init: function (itemId, itemType, itemLi, itemDiv, textShortcut, panel, response, wyEditor, isStructure) {
        //$(".nightMode").fadeIn('fast');
        itemDiv.unbind('click');


        itemDiv.addClass('editMode');
        //itemDiv.css('margin-top', '-10px');
        //itemDiv.css('margin-left', '-4px');
        itemLi.find(textShortcut + itemId).html(MT.itemSmallEditorTemplate(response.data));
        var count = response.data.messageText.match(/<br/g);
        var heightCorrection = itemDiv.height();
        if (count) {
            var faktor = count.length;
            if (faktor > 8) {
                faktor = 8;
            }
            heightCorrection = ((faktor + 1) * 21);
        } else {

        }

        heightCorrection = heightCorrection + 150;


        itemLi.find('#editTextArea' + itemId).css('height', heightCorrection + 'px');
        itemLi.find('#editTextArea' + itemId).css('width', '100%');
        itemLi.find('.messageAvatar,.messageDate, .messageNickName').hide();

        t14Lab.MessageTree.Item.operationsEvents($('#mbHandle' + itemId), false);
        if (wyEditor && !isStructure) {
            $.each(panel.options, function (key, value) {
                if (value.name == 'editor') {
                    if (value.mode == 'plain' || value.mode == 'standardJson') {
                        wyEditor = false;
                    }
                }
            });
        }

        if (wyEditor) {
            $('#editTextArea' + itemId).wysihtml5({
                "stylesheets": ["../css/editor.css"],
                "html": true, //Button which allows you to edit the generated HTML. Default false
                "view": "textarea",
                "color": true, //Button to change color of font  
                "events": {
                    "change_view": function () {
                        $('#editTextArea' + itemId).resize();
                        $('#editTextArea' + itemId).css('height', '300px');
                    },
                    "load": function () {
                        $(this).focus();
                        itemLi.find('.wysihtml5-toolbar').hide();

                        $($('.wysihtml5-sandbox').get(0).contentWindow.document).keydown(
                            function (e) {
                                if (e.ctrlKey && e.keyCode == 13)
                                    $('.buttonSave').click();
                                if (e.keyCode == 27)
                                    $('.buttonCancel').click();
                                if (e.ctrlKey && e.keyCode == 38)
                                    $('.buttonBigger').click();
                                if (e.ctrlKey && e.keyCode == 72) {
                                    $('[data-wysihtml5-action="change_view"]').click();
                                }
                            }
                        );
                        $(document).delegate('#editTextArea' + itemId, 'keydown', function (e) {
                            var keyCode = e.keyCode || e.which;
                            if (keyCode == 9) {
                                e.preventDefault();
                                var start = $(this).get(0).selectionStart;
                                var end = $(this).get(0).selectionEnd;

                                // set textarea value to: text before caret + tab + text after caret
                                $(this).val($(this).val().substring(0, start)
                                    + "\t"
                                    + $(this).val().substring(end));

                                // put caret at right position again
                                $(this).get(0).selectionStart =
                                $(this).get(0).selectionEnd = start + 1;
                            }
                        });



                    },
                    "blur": function () {
                    }
                }
            });
        }

        //console.log($('#editTextArea'+itemId).data("wysihtml5"));
        //$('[data-wysihtml5-action="change_view"]').click();
        //var wysihtml5Editor = $('#editTextArea'+itemId).data("wysihtml5").toolbar;
        //var wysihtml5Editor = $('#editTextArea'+itemId).data("wysihtml5").editor;
        //console.log(wysihtml5Editor.composer);
        //wysihtml5Editor.composer.commands.exec("bold");
        //wysihtml5Editor.execCommand("formatBlock", "blockquote");
        //wysihtml5Editor.execAction("change_view");
        //console.log(wysihtml5Editor);
        //wysihtml5Editor.composer.commands.exec("bold");

        if (wyEditor) {
            itemLi.find('.editMode').resizable({
                resize: function (event, ui) {
                    if (ui.size.height < 250) {
                        itemLi.find('.wysihtml5-toolbar').hide();
                        itemLi.find('.wysihtml5-sandbox').css('height', (ui.size.height - 50) + 'px');
//                        itemLi.find('.wysihtml5-sandbox').css('width', (ui.size.width - 28) + 'px');
                    } else {
                        itemLi.find('.wysihtml5-toolbar').show();
                        itemLi.find('.wysihtml5-sandbox').css('height', (ui.size.height - $('.wysihtml5-toolbar').height() - 50) + 'px');
//                        itemLi.find('.wysihtml5-sandbox').css('width', (ui.size.width - 28) + 'px');
                    }
                }
            });
            itemLi.find(".buttonBigger").click(function () {
                itemLi.find('.wysihtml5-sandbox').css('height', '350px');
                itemLi.find('.wysihtml5-toolbar').show();
                itemLi.find('.wysihtml5-sandbox').css('height', (itemLi.find('.editMode').size.height - $('.wysihtml5-toolbar').height() - 50) + 'px');
//                itemLi.find('.wysihtml5-sandbox').css('width', (itemLi.find('.editMode').size.width - 28) + 'px');
            });
        } else {
            var textArea1 = itemLi.find('#editTextArea' + itemId);
            heightCorrection += 100;
            if (heightCorrection < 1) {
                heightCorrection = 20;
            }
            textArea1.css('height', heightCorrection + 'px');
            textArea1.css('width', (itemDiv.width() - 28) + 'px');
            itemLi.find('.editMode').resizable({
                resize: function (event, ui) {
                    textArea1.css('height', (ui.size.height - 50) + 'px');
                    textArea1.css('width', (ui.size.width - 28) + 'px');
                }
            });
            itemLi.find(".buttonBigger").click(function () {
                itemLi.find('.editMode').css('height', '350px');
                textArea1.css('height', '300px');
            });
        }



        itemLi.find('.editMode').css('z-index', 10);

        itemLi.find(".buttonCancel").click(function () {

            MT.API.loadItem(itemId, itemLi.attr('data-itemType'));
            itemDiv.removeClass('editMode');
            $(".nightMode").fadeOut('fast');
            itemDiv.removeClass('nightModeMessage');
            itemLi.find('.messageAvatar,.messageDate, .messageNickName').show();

            // wtf
            itemDiv.on("taphold", function (e) {
                var messageItemOuter = $(this).parent();
                var messageId = messageItemOuter.attr('data-id');
                t14Lab.MessageTree.Item.editMode(messageId, true, messageItemOuter.attr('data-itemType'));
            });
        });



        itemLi.find(".buttonSave").click(function () {
            // itemId
            // messageItemOuter.find('#editTextArea'+itemId).text()
            itemDiv.removeClass('editMode');
            $(".nightMode").fadeOut('fast');
            itemDiv.removeClass('nightModeMessage');
            var newText = itemLi.find('#editTextArea' + itemId).val();
            if (newText.length < 1) {
                newText = "[empty]";
            }
            MT.API.editItem(itemLi.attr('data-itemType'), itemId, newText);
            // reload message
            MT.API.loadItem(itemId, itemLi.attr('data-itemType'));
            itemLi.find('.messageAvatar,.messageDate, .messageNickName').show();
            // wtf
            itemDiv.on("taphold", function (e) {
                var messageItemOuter = $(this).parent();
                var messageId = messageItemOuter.attr('data-id');
                t14Lab.MessageTree.Item.editMode(messageId, true, messageItemOuter.attr('data-itemType'));
            });
        });


        itemLi.find(".buttonFullMode").toggle(
            function () {
                $(".nightMode").fadeIn('fast');
                itemDiv.addClass('nightModeMessage');
            },
            function () {
                $(".nightMode").fadeOut('fast');
                itemDiv.removeClass('nightModeMessage');
            });


    }
}