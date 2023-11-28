///自定义Lookup控件
function Lookup(Opt) {
    var Option = {
        showEvent: null
    };

    var params = {};
    if (Opt) {
        $.extend(params, Option, Opt);
    }
    var openel;
    var ob = new Object();
    ob.Option = params;

    ob.BindTextLookup = function () {
        $('.text-lookup').parent().css("position", "relative");
        $('.text-lookup').parent().find(".text-lookup-label").remove();
        $('.text-lookup').after(function () {
            return '<i class="fa fa-ellipsis-h text-lookup-label"></i>'
        });
        $("div[contenteditable='true'][required]").after('<span style="color:red;position: absolute;top:50%;right:3%;">*</span>');
        $('.text-lookup-label').click(function () {
            openel = $(this);
            var otext = openel.parent().find('.text-lookup');
            var text = '';
            if (ob.IsHtml(this)) {
                text = otext.html();
            }
            else {
                text = otext.val();
            }
            if (params.showEvent) {
                params.showEvent(text, this);
            }
        });

        $('.text-lookup').dblclick(function () {
            $(this).parent().find('.text-lookup-label').click();
        });
    };

    ob.CloseBindText = function (index, el) {
        if (ob.IsHtml(el)) {
            openel.parent().find('.text-lookup').html(top.window["layui-layer-iframe" + index].getValue());
        }
        else {
            openel.parent().find('.text-lookup').val(top.window["layui-layer-iframe" + index].getValue());
        }
    }

    ob.IsReadonly = function (el) {
        var otext = $(el).parent().find('.text-lookup');
        return otext.attr('readonly') == 'readonly' || otext.attr('disabled') == 'disabled' || (otext.is('.html-lookup') && otext.attr('contenteditable') != 'true');
    }

    ob.IsHtml = function (el) {
        var otext = $(el).parent().find('.text-lookup');
        return otext.is('.html-lookup');
    }

    return ob;
}

function checkEnter(e) {
    var et = e || window.event;
    var keycode = et.charCode || et.keyCode;
    if (keycode == 13) {
        if (window.event)
            window.event.returnValue = false;
        else
            e.preventDefault();//for firefox
    }
}

function ValidTextLookup() {
    var flag = true;
    var div;
    $.each($('div[contenteditable="true"]'), function () {
        var required = $(this).attr("lookup-required") == "true";
        if (required) {
            if ($(this).html().length == 0) {
                flag = false;
                div = this;
                return false;
            }
        }
    });
    if (!flag) {
        $(div).focus().html('');
        var tips = $(div).parent().prev().html();
        tips = tips.trim(':');
        return tips + '不能为空';
    }

    return "";
}

function BindTextLookup() {
    var mylookup = Lookup({
        showEvent: function (text, el) {
            if (mylookup.IsHtml(el)) {
                top.$.modal.open(mylookup.IsReadonly(el) ? "查看" : "编辑", '/UEditor', '90%', '90%', mylookup.IsReadonly(el) ? 3 : 2, function (index) {
                    if (!mylookup.IsReadonly(el)) {
                        mylookup.CloseBindText(index, el);
                    }
                    top.layer.close(index);
                    return true;
                }, function (index) {
                    if (!mylookup.IsReadonly(el)) {
                        mylookup.CloseBindText(index, el);
                    }
                    top.layer.close(index);
                    return true;
                }, null, function (aaa, index) {
                    top.window["layui-layer-iframe" + index].setValue(text, mylookup.IsReadonly(el));
                });
            }
            else {
                var language = $(el).parent().find('.text-lookup').attr("language");
                if (language) {
                    language = "?language=" + language;
                }
                top.$.modal.open(mylookup.IsReadonly(el) ? "查看" : "编辑", '/MonacoEditor' + language, '90%', '90%', mylookup.IsReadonly(el) ? 3 : 2, function (index) {
                    if (!mylookup.IsReadonly(el)) {
                        mylookup.CloseBindText(index, el);
                    }
                    top.layer.close(index);
                    return true;
                }, function (index) {
                    if (!mylookup.IsReadonly(el)) {
                        mylookup.CloseBindText(index, el);
                    }
                    top.layer.close(index);
                    return true;
                }, null, function (aaa, index) {
                    top.window["layui-layer-iframe" + index].setValue(text, mylookup.IsReadonly(el));
                });
            }
        }
    });
    mylookup.BindTextLookup();
}

$(function () {
    if ($('.text-lookup').length > 0) {
        BindTextLookup();
        // ValidTextLookup($('form'));
        $('div[contenteditable="true"]').keyup(function () {
            if ($(this).html().length > 0 && $(this).hasClass('error')) {
                $(this).removeClass('error');
                $(this).parent().find('.error1').remove();
            }
        });
    }
});