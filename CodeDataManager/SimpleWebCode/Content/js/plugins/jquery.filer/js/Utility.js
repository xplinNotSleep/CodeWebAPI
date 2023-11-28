/*
@@版权所有: 版权所有(C) 2010，广东省建设信息中心
@@功能描述: 项目公共有用类库
@@创建标识: Soul 2014 年02 月17 日（V1.0）
@@备注说明: 
*/

/*产生随机地址参数*/
function GetRandomQueryStr() {
    var p = (Math.random() * 10000000000000).toString() + "=" + (Math.random() * 10000000000000).toString();
    return p;
}

/*
@@版权所有: 版权所有(C) 2013，广东省建设信息中心
@@功能描述: 获取地址中的参数值
@@创建标识: Soul 2013 年11 月19 日（V1.0）
@@备注说明: 
*/
function GetRequest() {
    var Url = window.location.href; //如果想获取框架顶部的url可以用 top.window.location.href
    var u, g, StrBack = '';
    Url = Url.replace("#", "");
    if (arguments[arguments.length - 1] == "#")
        u = Url.split("#");
    else
        u = Url.split("?");
    if (u.length == 1) g = '';
    else g = u[1];

    if (g != '') {
        gg = g.split("&");
        var MaxI = gg.length;
        str = arguments[0] + "=";
        for (xm = 0; xm < MaxI; xm++) {
            if (gg[xm].indexOf(str) == 0) {
                StrBack = gg[xm].replace(str, "");
                break;
            }
        }
    }
    return StrBack;
}

/*extjs用于新建或打开子标签的管理*/
function opentab(pid, title, url) {
    var tab = window.parent.tab;

    if (pid == null || pid.length == 0) {
        var d = new Date();
        pid = d.getTime();
    }
    var n = tab.getComponent(pid);
    if (!n) { //判断是否已经打开该面板 
        n = tab.add({
            'id': pid.toString(),
            'title': title,
            closable: true,  //通过html载入目标页    
            html: "<iframe scrolling='auto' frameborder='0' width='100%' height='100%' src='" + url + "'></iframe>"
        });
        top.AlertUserWhenOpenMoreThan10Tabs(tab.items.length);
    }

    tab.setActiveTab(n);
}

/*打开子标签超过10个时提醒*/
function AlertUserWhenOpenMoreThan10Tabs(count) {
    var never_show = Ext.state.Manager.get('ck_never_show_tabs_msg', false);
    if (!never_show && count > 5) {
        Ext.MessageBox.show({
            title: '信息提示',
            msg: '你已经打开了' + count + '个窗口，打开太多窗口会影响运行效率，建议保留5个窗口以内！<br /><input type="checkbox" id="ck_never_show_tabs_msg" />不再提示',
            minWidth: 200,
            closable: true,
            buttons: { ok: "确定" },
            icon: Ext.MessageBox.INFO,
            fn: AfterAlertUserWhenOpenMoreThan5Tabs
        });
    }
}

function AfterAlertUserWhenOpenMoreThan5Tabs() {
    var ck = Ext.get('ck_never_show_tabs_msg');
    if (ck != null && ck.dom.checked) {
        Ext.state.Manager.set('ck_never_show_tabs_msg', true);
    }
}

/*提示框*/
function AlertWindow(msg, callback) {
    alert(decodeURIComponent(msg));
    if (typeof callback != 'undefined' && callback != null)
        callback();
}

/*确认框*/
function ConfirmWindow(msg, OKcallback, Cancelcallback) {
    if (confirm(msg)) {
        if (typeof OKcallback != 'undefined' && OKcallback != null)
            OKcallback();
    } else {
        if (typeof Cancelcallback != 'undefined' && Cancelcallback != null)
            Cancelcallback();
    }
}

/*打开窗口*/
function OpenWindow(url) {
    window.open(url);
}

/*打开Ext窗口*/
function OpenExtWindow(url, winid, width, height, title, isModal, refresh, refreshWin) {
    if (isModal == null) isModal = true;
    var tmpWin = new Ext.Window
        (
            {
                id: winid,
                layout: 'fit',
                width: width,
                height: height,
                maximizable: true,
                closable: true,
                plain: true,
                constrain: true,
                modal: isModal,
                title: title,
                html: '<iframe scrolling="auto" frameborder="0" width="100%" height="100%" src="' + url + '" id="' + winid + '"></iframe>'
            }
        )
    tmpWin.show();
    if (refresh) {
        tmpWin.on('close', function () {
            refreshWin.refresh();
        });
    }
    return false;
}

/*打开模态窗口*/
function OpenModalDialogWindow(url, width, height) {
    return window.showModalDialog(url, '_blank', 'dialogWidth=' + width + 'px;dialogHeight=' + height + 'px;help:no;status:no;maximize:yes;minimize:no;');
}

/*显示提示框*/
function showToolTip(event, msg) {
}

/*隐藏提示框*/
function hideToolTip() {
}

/*提示输入的字符数量*/
function checkLength(which, maxChars) {
    var tipid = "txtRemarktip_" + which.id;
    if ($('#' + tipid).length == 0)
        $(which).after('<div id="' + tipid + '" class="txt-tip">');
    var length = which.value.length;
    if (length > maxChars) {
        which.value = which.value.substring(0, maxChars);
        length = maxChars;
    }
    var curr = maxChars - length;
    var $obj = $("#" + tipid);
    $obj.html("已输入" + length + "/" + maxChars + "个字");
    $obj.show();
}
/*隐藏提示输入的字符数量*/
function hideCheckLength(which) {
    $("#txtRemarktip_" + which.id).hide();
}

/*调整图片显示，等比例缩放*/
function DrawImage(id, FitWidth, FitHeight) {
    var ImgD = document.getElementById(id);
    var image = new Image();
    image.src = ImgD.src;
    if (image.width > 0 && image.height > 0) {
        if (image.width / image.height >= FitWidth / FitHeight) {
            if (image.width > FitWidth) {
                ImgD.width = FitWidth;
                ImgD.height = (image.height * FitWidth) / image.width;
            } else {
                ImgD.width = image.width;
                ImgD.height = image.height;
            }
        } else {
            if (image.height > FitHeight) {
                ImgD.height = FitHeight;
                ImgD.width = (image.width * FitHeight) / image.height;
            } else {
                ImgD.width = image.width;
                ImgD.height = image.height;
            }
        }
    }
    ImgD.style.display = "";
}

/*输入框只能输入整数*/
function OnlyInteger(obj) {
    var v = obj.value.replace(/[^0-9]/g, '');
    if (v == null || v == "") {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = v;
        obj.style.backgroundColor = "";
    }

}

/*输入框只能输入数字*/
function OnlyNum(obj) {
    var v = obj.value.replace(/[^0-9.]/g, '');
    if (v == null || v == "") {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = v;
        obj.style.backgroundColor = "";
    }

}

/*输入框只能输入组织机构代码*/
function OnlyOrgCode(obj) {
    var v = obj.value;
    v = v.replace(/[^0-9]{9}/g, '');
    v = v.replace(/[^0-9X]/g, '');
    v = v.length > 9 ? v.substring(0, 9) : v;
    if (v == null || v == "") {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = v;
        obj.style.backgroundColor = "";
    }
}

/*输入框只能输入网址*/
function OnlyHttpUrl(obj) {
    var reg = new RegExp('(http|https|ftp):(\/\/)');
    var result = reg.exec(obj.value);
    if (result == null) {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = result.input;
        obj.style.backgroundColor = "";
    }
}

/*输入框只能输入email地址*/
function OnlyEmail(obj) {
    var reg = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
    var result = reg.exec(obj.value);
    if (result == null) {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = result.input;
        obj.style.backgroundColor = "";
    }
}

/*输入框只能输入手机号码*/
function OnlyMobile(obj) {
    var v = obj.value.replace(/[^0-9.]/g, '');
    if (v == null || v == "" || v.length != 11) {
        obj.value = "";
        obj.style.backgroundColor = "#fbd48f";
    } else {
        obj.value = v;
        obj.style.backgroundColor = "";
    }
}

/*是否为空*/
function isEmptyStr(value) {
    if (value == null || value == "" || value.legnth == 0)
        return true;
    else
        return false;
}

/*身份证检查 var idnum = new ChinaIDCard(stridnum)*/
function ChinaIDCard(idCard) {
    this.__IDCard = idCard;
    this.__IDLength = idCard.length;
    this.__IDCardObj = { idcard: idCard, length: idCard.length, isvalid: true, birthday: '', sex: '' };
    this.Validate = function () {
        this.__isValidate();
        if (!this.__IDCardObj.isvalid) {
            return this.__IDCardObj;
        }

        //地区码

        //出生日期
        this.__validateBirthday();
        if (!this.__IDCardObj.isvalid) {
            return this.__IDCardObj;
        }

        //性别
        this.__validateSex();
        if (!this.__IDCardObj.isvalid) {
            return this.__IDCardObj;
        }

        //18位身份证的校验码
        this.__validateCheckNumber();
        if (!this.__IDCardObj.isvalid) {
            return this.__IDCardObj;
        }

        this.__setValidateSuccess();

        return this.__IDCardObj;
    };
    this.__isValidate = function () {
        if (this.__IDLength != 15 && this.__IDLength != 18) {
            this.__setValidateFail();
        }
        else if (this.__IDLength == 15) {
            var regExp = /^[1-9][0-9]{14}$/gi;
            if (!regExp.test(this.__IDCard)) {
                this.__setValidateFail();
            }
        }
        else if (this.__IDLength == 18) {
            var regExp = /^[1-9][0-9]{16}[0-9,X]$/gi;
            if (!regExp.test(this.__IDCard)) {
                this.__setValidateFail();
            }
        }
    };
    this.__validateBirthday = function () {
        try {
            var _temps2 = new String();
            if (this.__IDLength == 15) {
                _temps2 = "19" + this.__IDCard.substr(6, 6);
            }
            else {
                _temps2 = this.__IDCard.substr(6, 8);
            }
            var d = Date.parse(_temps2);
            this.__IDCardObj.birthday = _temps2.substr(0, 4) + "-" + _temps2.substr(4, 2) + "-" + _temps2.substr(6, 2);
        }
        catch (e) {
            this.__setValidateFail();
        }
    };
    this.__validateSex = function () {
        try {
            var _temps3 = new String();
            if (this.__IDLength == 15) {
                _temps3 = this.__IDCard.substr(14, 1);
            }
            else {
                _temps3 = this.__IDCard.substr(16, 1);
            }
            var t = parseInt(_temps3);
            if ((t % 2) == 0) {
                this.__IDCardObj.sex = "女";
            }
            else {
                this.__IDCardObj.sex = "男";
            }
        }
        catch (e) {
            this.__setValidateFail();
        }
    };
    this.__validateCheckNumber = function () {
        if (this.__IDLength == 18) {
            var _temparray1 = new Array(7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2);
            var _temparray2 = new Array("1", "0", "X", "9", "8", "7", "6", "5", "4", "3", "2");
            var _temps4 = this.__IDCard.substr(17, 1);
            var sum = 0;
            for (var i = 0; i < 17; i++) {
                sum += parseInt(this.__IDCard.substr(i, 1)) * _temparray1[i];
            }
            var t = sum % 11;
            if (_temparray2[t] != _temps4) {
                this.__setValidateFail();
            }
        }
    };
    this.__setValidateFail = function () {
        this.__IDCardObj.isvalid = false;
        this.__IDCardObj.msg = "无效的中国大陆身份证!";
    };
    this.__setValidateSuccess = function () {
        this.__IDCardObj.isvalid = true;
        this.__IDCardObj.msg = "有效的中国大陆身份证!";
    };
}

//查看图片
function ViewImage(t, path) {
    var src = $(t).attr("src");
    if (!isEmptyStr(src) && src != NoPictureWelUrl) {
        zoom(t, null, path);
    }
    return false;
}

//日期
function FrontEndWdatePicker(config) {
    //if ((typeof (IsViewModel) != 'undefined' && !IsViewModel) || (typeof (IsPatch) != 'undefined' && IsPatch)) {
    WdatePicker(config);
    // }
}

//上传图片
function SelectUpdateImageFile(Obj, url, width, height) {
    if (typeof url == 'undefined' || url == null || url == "")
        return;
    var value = window.parent.showModalDialog(url, '_blank', 'dialogWidth=' + width + 'px;dialogHeight=' + height + 'px;help:no;status:no;maximize:yes;minimize:no;');
    if (!isEmptyStr(value)) {
        var value = value.split(',');
        var $span = $(Obj).parent().siblings("span.ImageControl");
        var $img = $span.children("img");
        var $hid = $span.children("input:hidden[name$='hidPHOTO']");
        var $hidname = $span.children("input:hidden[name$='hidImageName']");
        $img.attr("src", value[1]);
        $hid.val(value[1]);
        $hidname.val(value[0]);
    }
}
//隐藏添加和选择按钮
function HideToolBar() {
    $(document).ready(function () {
        $("div.tool-bar").hide();
        $("input.btnSave").hide();
        $("a.select").hide();
        $(".DivUploadFile").hide();
    });
}
//是否编辑模式
function IsEditAction() {
    if (typeof ActionStatus != 'undefined' && ActionStatus != null && (ActionStatus == 1 || ActionStatus == 2))
        return true;
    else
        return false;
}

function SearchContentExpand() {
    var d = document.getElementById('class1content');
    var h = d.offsetHeight;
    var maxh = 100;
    function dmove() {
        h += 50; //设置层展开的速度
        if (h >= maxh) {
            clearInterval(iIntervalId);
        } else {
            d.style.display = 'block';
        }
    }
    iIntervalId = setInterval(dmove, 2);
}
function SearchContentHide() {
    var d = document.getElementById('class1content');
    var h = d.offsetHeight;
    var maxh = 100;
    function dmove() {
        h -= 50;//设置层收缩的速度
        if (h <= 0) {
            d.style.display = 'none';
            clearInterval(iIntervalId);
        }
    }
    iIntervalId = setInterval(dmove, 2);
}
function SearchTitleBarClick() {
    var d = document.getElementById('class1content');
    var sb = document.getElementById('stateBut');
    if (d.style.display == 'none') {
        SearchContentExpand();
        sb.innerHTML = '搜索条件（点击此处收起）';
    } else {
        SearchContentHide();
        sb.innerHTML = '搜索条件（点击此处展开）';
    }
}



//打开面板窗口
function OpenJQAeaoWindow(title, url, cloaseAction) {
    var tempID = url;
    if (tempID.indexOf('RandomQueryStr') > -1) {
        tempID = tempID.substr(0, tempID.indexOf('RandomQueryStr'));
    }
    var targetIframe = document.getElementById(tempID);
    if (targetIframe == null) {
        var randomid = "AeaoWindow" + (Math.random() * 10000000000000).toString().replace('.', '');
        var html = '<div id="' + randomid + '" style="display: none;"><iframe id="' + tempID + '" aeaowindow="' + randomid + '" src="' + url + '" width="100%" height="100%" style="border: 0px;" frameborder="0"></iframe><div class="iframeHelper"></div></div>';
        $(document.body).append(html);
        targetIframe = document.getElementById(tempID);
    }
    var divID = $(targetIframe).attr('aeaowindow');
    $('#' + divID).AeroWindow({
        WindowTitle: title,
        WindowPositionTop: 'center',
        WindowPositionLeft: 'center',
        WindowWidth: GDCIC.util.GetWidth(0.9, 1400),
        WindowHeight: GDCIC.util.GetHeight(0.9, 900),
        WindowResizable: false,
        WindowAnimation: 'easeOutCubic',
        WindowCloseAction: cloaseAction
    });
}

//是否登录
function UserHasLogin() {
    if (typeof (_CurrentUserObj) != 'undefined' && _CurrentUserObj != null && _CurrentUserObj.UserID != null) {
        return true;
    }
    return false;
}

//将后台序列化出来的日期时间格式转换成文本格式
function ShowCustomDateTime(dtStr, dtFormat) {
    if (typeof (dtStr) == 'undefined' || dtStr == null || dtStr == "") {
        return null;
    }
    if (typeof (dtFormat) == 'undefined' || dtFormat == null) {
        dtFormat = 'Y-m-d';
    }
    var dt = Date.parseDate(dtStr, 'Y-m-d H:i:s');

    return Ext.util.Format.date(dt, dtFormat);
}

//打开窗口
function OpenWin(url, winid, width, height, title, refresh, parent, isModal) {
    if (isModal == null) isModal = true;
    var tmpWin = new Ext.Window
        (
            {
                id: winid,
                layout: 'fit',
                width: width,
                height: height,
                maximizable: true,
                closable: true,
                plain: true,
                constrain: true,              
                modal: isModal,
                title: title,
                html: '<iframe scrolling="auto" frameborder="0" width="100%" height="100%" src="' + url + '" id="' + winid + '"></iframe>'
            }
        )
    tmpWin.show();
    if (refresh && parent) {
        tmpWin.on('close', function () {
            parent.refresh();
        });
    }
    return false;
}

//是否中国移动号码
function IsCMMCMobile(mobile) {
    if (mobile == null || mobile == "" || mobile.length != 11)
        return false;
    var exp2 = /^(134|135|136|137|138|139|147|150|151|152|154|157|158|159|182|183|184|187|188)[0-9]{8}$/;
    return exp2.test(mobile);
}

function IsEmail(emial) {
    if (emial == null || emial == "")
        return false;
    var exp2 = /^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/;
    return exp2.test(emial);
}
