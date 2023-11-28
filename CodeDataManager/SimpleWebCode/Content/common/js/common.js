/**
 * 通用方法封装处理
 * Copyright (c) 2020 commonservice
 */
$(document).on("input propertychange", "textarea", function (e) {
	textAreaAutoHeight(e.target);
});

function textAreaAutoHeight(dom)
{
	var target = $(dom)[0];
	if(!target)return;
	// 保存初始高度，之后需要重新设置一下初始高度，避免只能增高不能减低。
	var dh = $(dom).attr('defaultHeight') || 0;
	if (!dh) {
		dh = target.clientHeight;
		$(dom).attr('defaultHeight', dh);
	}
	target.style.height = dh +'px';
	var clientHeight = target.clientHeight;
	var scrollHeight = target.scrollHeight;
	if (clientHeight !== scrollHeight) { target.style.height = scrollHeight* (1.025) + "px";
	}
}

$.fn.textAreaAutoHeight=function()
{
	textAreaAutoHeight(this)
}

$(function() {
	// select2复选框事件绑定
	if ($.fn.select2 !== undefined) {
        $.fn.select2.defaults.set( "theme", "bootstrap" );
		$("select.form-control:not(.noselect2)").each(function () {
			$(this).select2().on("change", function () {
				$(this).valid();
			})
		})
	}
	
	// iCheck单选框及复选框事件绑定
	if ($.fn.iCheck !== undefined) {
		$(".check-box:not(.noicheck),.radio-box:not(.noicheck)").each(function() {
            $(this).iCheck({
                checkboxClass: 'icheckbox-blue',
                radioClass: 'iradio-blue',
            })
        })
	}

	// $("select[name$='ID'],select[name$='CODE']").each(function () {
	// 	var hidval = $($(this).nextAll('input:hidden')[0]).val();
	// 	if (!hidval) {
	// 		if (!$(this).find("option:selected").text())
	// 		{
	// 			$(this).get(0).selectedIndex = 0;
	// 		}
	// 		$($(this).nextAll('input:hidden')[0]).val($(this).find("option:selected").text());
	// 	}
	// });

	$("select[name$='ID'],select[name$='CODE'],select[name$='id'],select[name$='Id'],select[name$='code'],select[name$='Code']").change(function () {
			$($(this).nextAll('input:hidden')[0]).val($(this).find("option:selected").text());
		});

	  $('.distpicker select').change(function () {
		  var obj = $(this).closest('.distpicker');
		  var zonecodeStr = "";
		  if (obj.find('#COUNTY').val()) {
			  zonecodeStr = "COUNTY";
		  }
		  if (obj.find('#county').val() ) {
			  zonecodeStr = "county";
		  } else if (obj.find('#CITY').val()) {
			  zonecodeStr = "CITY";
		  } else if (obj.find('#city').val()) {
			  zonecodeStr = "city";
		  } else if (obj.find('#PROVINCE').val()) {
			  zonecodeStr = "PROVINCE";
		  } else if (obj.find('#province').val()) {
			  zonecodeStr = "province";
		  }
		  var name;
		  var obj1 = obj.next();
		  if (obj1 && obj1.is(':hidden')) {
			  name = obj1.attr('name');
		  }
		  if (name && zonecodeStr) {
			  obj1.val(obj.find('#' + zonecodeStr).find("option:selected").attr('data-code'));
			  var obj2 = obj1.next();
			  if (obj2 && obj2.is(':hidden')) {
				  var name2 = obj2.attr('name');
				  obj2.val(obj.find('#' + zonecodeStr).find("option:selected").text());
			  }
		  }
	  });
		// $(":radio[name$='ID'],:radio[id$='CODE']").on('ifChecked', function () {
		// 	$($(this).nextAll('input:hidden')[0]).val($(this).closest('div').next('label').html());
		// });

	 
	// laydate 时间控件绑定
	if ($(".select-time").length > 0) {
		layui.use('laydate', function() {
		    var laydate = layui.laydate;
		    var startDate = laydate.render({
		        elem: '#startTime',
		        max: $('#endTime').val(),
		        theme: 'molv',
		        trigger: 'click',
		        done: function(value, date) {
		            // 结束时间大于开始时间
		            if (value !== '') {
		                endDate.config.min.year = date.year;
		                endDate.config.min.month = date.month - 1;
		                endDate.config.min.date = date.date;
		            } else {
		                endDate.config.min.year = '';
		                endDate.config.min.month = '';
		                endDate.config.min.date = '';
		            }
		        }
		    });
		    var endDate = laydate.render({
		        elem: '#endTime',
		        min: $('#startTime').val(),
		        theme: 'molv',
		        trigger: 'click',
		        done: function(value, date) {
		            // 开始时间小于结束时间
		            if (value !== '') {
		                startDate.config.max.year = date.year;
		                startDate.config.max.month = date.month - 1;
		                startDate.config.max.date = date.date;
		            } else {
		                startDate.config.max.year = '';
		                startDate.config.max.month = '';
		                startDate.config.max.date = '';
		            }
		        }
		    });
		});
	}
	// laydate time-input 时间控件绑定
	if ($(".time-input").length > 0) {
		layui.use('laydate', function () {
			var com = layui.laydate;
			$(".time-input").each(function (index, item) {
				var time = $(item);
				// 控制控件外观
				var type = time.attr("data-type") || 'date';
				// 控制回显格式
				var format = time.attr("data-format") || 'yyyy-MM-dd';
				// 控制日期控件按钮
				var buttons = time.attr("data-btn") || 'clear|now|confirm', newBtnArr = [];
				// 日期控件选择完成后回调处理
				var callback = time.attr("data-callback") || {};
				if (buttons) {
					if (buttons.indexOf("|") > 0) {
						var btnArr = buttons.split("|"), btnLen = btnArr.length;
						for (var j = 0; j < btnLen; j++) {
							if ("clear" === btnArr[j] || "now" === btnArr[j] || "confirm" === btnArr[j]) {
								newBtnArr.push(btnArr[j]);
							}
						}
					} else {
						if ("clear" === buttons || "now" === buttons || "confirm" === buttons) {
							newBtnArr.push(buttons);
						}
					}
				} else {
					newBtnArr = ['clear', 'now', 'confirm'];
				}
				com.render({
					elem: item,
					theme: 'molv',
					trigger: 'click',
					type: type,
					format: format,
					btns: newBtnArr,
					done: function (value, data) {
						if (typeof window[callback] != 'undefined'
							&& window[callback] instanceof Function) {
							window[callback](value, data);
						}
					}
				});
			});
		});
	}
	// tree 关键字搜索绑定
	if ($("#keyword").length > 0) {
		$("#keyword").bind("focus", function focusKey(e) {
		    if ($("#keyword").hasClass("empty")) {
		        $("#keyword").removeClass("empty");
		    }
		}).bind("blur", function blurKey(e) {
		    if ($("#keyword").val() === "") {
		        $("#keyword").addClass("empty");
		    }
		    $.tree.searchNode(e);
		}).bind("input propertychange", $.tree.searchNode);
	}
	// tree表格树 展开/折叠
	var expandFlag;
	$("#expandAllBtn").click(function() {
		var dataExpand = $.common.isEmpty($.table._option.expandAll) ? true : $.table._option.expandAll;
		expandFlag = $.common.isEmpty(expandFlag) ? dataExpand : expandFlag;
	    if (!expandFlag) {
	    	$.bttTable.bootstrapTreeTable('expandAll');
	    } else {
	    	$.bttTable.bootstrapTreeTable('collapseAll');
	    }
	    expandFlag = expandFlag ? false: true;
	})
	// 按下ESC按钮关闭弹层
	$('body', document).on('keyup', function(e) {
	    if (e.which === 27) {
	        $.modal.closeAll();
	    }
	});

	$.each($("form").find("[validate]"),function(i,obj)
	{
		var rules=eval('({'+$(obj).attr('validate')+'})');
		if($(obj).attr('validate-message'))
		{
			rules.messages=eval('({'+$(obj).attr('validate-message')+'})');
		}
		$(obj).rules("add",rules);
	});
});

/** 刷新选项卡 */
var refreshItem = function(){
    var topWindow = $(window.parent.document);
	var currentId = $('.page-tabs-content', topWindow).find('.active').attr('data-id');
	var target = $('.common_iframe[data-id="' + currentId + '"]', topWindow);
    var url = target.attr('src');
    target.attr('src', url).ready();
}

/** 关闭选项卡 */
var closeItem = function(dataId){
	var topWindow = $(window.parent.document);
	if($.common.isNotEmpty(dataId)){
		window.parent.$.modal.closeLoading();
		// 根据dataId关闭指定选项卡
		$('.menuTab[data-id="' + dataId + '"]', topWindow).remove();
		// 移除相应tab对应的内容区
		$('.mainContent .common_iframe[data-id="' + dataId + '"]', topWindow).remove();
		return;
	}
	var panelUrl = window.frameElement.getAttribute('data-panel');
	$('.page-tabs-content .active i', topWindow).click();
	if($.common.isNotEmpty(panelUrl)){
		$('.menuTab[data-id="' + panelUrl + '"]', topWindow).addClass('active').siblings('.menuTab').removeClass('active');
		$('.mainContent .common_iframe', topWindow).each(function() {
            if ($(this).data('id') == panelUrl) {
                $(this).show().siblings('.common_iframe').hide();
                return false;
            }
		});
	}
}

function lowerJSONKey(jsonObj){
	for (var key in jsonObj){
		if(key!=key.toLowerCase())
		{
			jsonObj["\""+key.toLowerCase()+"\""] = jsonObj[key];
			delete(jsonObj[key]);
		}
	}
	return jsonObj;
}

$.fn.AddYearOptions= function() {
	if(!$(this).is('select'))return;
	var count = arguments[0] ? arguments[0] : 5;
	var from=arguments[1] ? arguments[1] : new Date().getFullYear();
	for(var i=from;i>from-count;i--)
	{
		$(this).append($("<option></option>").val(i).text(i));
	}
}

function upperPropertyNames(obj) {
	if (obj == null) {
		return;
	}
	if (typeof obj != 'object') {
		return;
	}
	if (isArray(obj) && obj.length > 0) {
		// iterate over array obj
		for (var index in obj) {
			upperPropertyNames(obj[index]);
		}
	} else {
		// iterate over object obj
		var props = Object.keys(obj);
		var propNums = props.length;
		if (propNums == 0) {
			return;
		}
		for (var index = 0; index < propNums; index++) {
			var prop = props[index];
			var prop_uppercase = prop.toUpperCase();
			if (prop_uppercase !== prop) {
				obj[prop_uppercase] = obj[prop];
				delete obj[prop];
			}
			if (typeof obj[prop_uppercase] == 'object') {
				upperPropertyNames(obj[prop_uppercase]);
			}
		}
	}
	function isArray(o) {
		return typeof o === "object" &&
			Object.prototype.toString.call(o) === "[object Array]";
	};
}

$.fn.serializeJson = function () {
	var serializeObj = {};
	var that=this;
	var checkboxobj=$(this).find(':checkbox');
	var array = this.serializeArray();
	$(array).each(function () {
		var c_val = this.value;
		if (serializeObj[this.name]) {
			if ($.isArray(serializeObj[c_val])) {
				serializeObj[this.name].push(c_val);
			} else {
				serializeObj[this.name] = [serializeObj[this.name], c_val];
			}
		} else {
			serializeObj[this.name] = c_val;
		}
	});
	$(checkboxobj).each(function () {
		serializeObj[this.name] = this.checked;
	});
	$.each(this.find(".html-lookup"), function () {
		var c_val ='';
		if ($(this).html().length > 0) {
			c_val = escape($(this).html());
		}
		else
		{
			c_val='';
		}
		var name = $(this).attr('name');
		if (serializeObj[name]) {
			if ($.isArray(serializeObj[c_val])) {
				serializeObj[name].push(c_val);
			} else {
				serializeObj[name] = [serializeObj[name], c_val];
			}
		} else {
			serializeObj[name] = c_val;
		}
	});
	$.each(that.find(".distpicker"), function () {
		var obj = $(this);
		var zonecodeStr = "";
		if (obj.find('#COUNTY').val()) {
			zonecodeStr = "COUNTY";
		}
		if (obj.find('#county').val()) {
			zonecodeStr = "county";
		}
		else if (obj.find('#CITY').val()) {
			zonecodeStr = "CITY";
		}
		else if (obj.find('#city').val()) {
			zonecodeStr = "city";
		}
		else if ($('#PROVINCE').val()) {
			zonecodeStr = "PROVINCE";
		}
		else if ($('#province').val()) {
			zonecodeStr = "province";
		}
		var name;
		var obj1=obj.next();
		if(obj1&&obj1.is(':hidden'))
		{
			name=obj1.attr('name');
		}
		if (name&&zonecodeStr) {
			serializeObj[name] = obj.find('#' + zonecodeStr).find("option:selected").attr('data-code');
			var obj2 = obj1.next();
			if (obj2 && obj2.is(':hidden')) {
				var name2 = obj2.attr('name');
				serializeObj[name2] = obj.find('#' + zonecodeStr).find("option:selected").text();
			}
		}
	});
	return serializeObj;
}

$.fn.bindForm = function (jsonValue) {
	var obj = this;
	$.each(jsonValue, function (name, ival) {
		var $oinput = obj.find("[name=" + name + "]");
		if ($oinput.attr("type") == "checkbox") {
			if (ival != null) {
				$oinput.attr("checked", ival);
			}
		}
		else if ($oinput.attr("type") == "radio") {
			var aival = '';
			if (ival !== null) {
				aival = ival + '';
			}
			if (ival !== null) {
				var radioObj = $(":radio[name='" + name + "']");
				$.each(radioObj, function (i, obj) {
					if ($(this).val()== aival) {
						$(this).iCheck('check');
					}
				});
			}
		}
		else if ($oinput.is(".html-lookup")) {
			if (ival != null) {
				obj.find("[name=" + name + "]").html(unescape(ival));
			}
		}
		else if ($oinput.prev().is(".distpicker")) {
			$oinput.prev().distpicker({
				autoSelect: false,
				code: ival
			});
			 $oinput.val(ival);
		}
		else if ($oinput.is("label") || $oinput.is("span")|| $oinput.is("textarea")) {
			$oinput.html(ival);
			$oinput.textAreaAutoHeight();
		}
		else if ($oinput.is("select")) {
				$oinput.val(ival);}
		else {
			if(ival&&(ival+'').indexOf(' 00:00:00')!=-1)
			{
				ival=(ival+'').replace(' 00:00:00','');
			}
			$oinput.val(ival);
		}
	});
}

// 查找指定的元素在数组中的位置
Array.prototype.indexOf = function (val) {
	if (this.length == 0) return -1;
	for (var i = 0; i < this.length; i++) {
		if (this[i] == val) {
			return i;
		}
	}
	return -1;
};
// 通过索引删除数组元素
Array.prototype.remove = function (val) {
	var index = this.indexOf(val);
	if (index > -1) {
		this.splice(index, 1);
	}
};

//设置所有表单元素只读
$.fn.setReadonly=function() {
	$(this).find('input,select,textarea').attr('readonly', true);
	$(this).find("select").attr("disabled", "disabled");
	$(this).find("input[type='radio']").attr("disabled", "disabled");
	$(this).find("input[type='checkbox']").attr("disabled", "disabled");
}

function getRequestValue(name,defaultval) {

	var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
	var r = window.location.search.substr(1).match(reg);
	if (r != null) return unescape(r[2]);
	return defaultval;
}

function  getLastIndexRequestValue() {
	var url =window.location.href;
	return url.substring(url.lastIndexOf('/')+1, url.length)
}

/** 创建选项卡 */
function createMenuItem(dataUrl, menuName) {
	var panelUrl = window.frameElement.getAttribute('data-id');
    dataIndex = $.common.random(1,100),
    flag = true;
    if (dataUrl == undefined || $.trim(dataUrl).length == 0) return false;
    var topWindow = $(window.parent.document);
    // 选项卡菜单已存在
    $('.menuTab', topWindow).each(function() {
        if ($(this).data('id') == dataUrl) {
            if (!$(this).hasClass('active')) {
                $(this).addClass('active').siblings('.menuTab').removeClass('active');
                $('.page-tabs-content').animate({ marginLeft: ""}, "fast");
                // 显示tab对应的内容区
                $('.mainContent .common_iframe', topWindow).each(function() {
                    if ($(this).data('id') == dataUrl) {
                        $(this).show().siblings('.common_iframe').hide();
                        return false;
                    }
                });
            }
            flag = false;
            return false;
        }
    });
    // 选项卡菜单不存在
    if (flag) {
        var str = '<a href="javascript:;" class="active menuTab" data-id="' + dataUrl + '" data-panel="' + panelUrl + '">' + menuName + ' <i class="fa fa-times-circle"></i></a>';
        $('.menuTab', topWindow).removeClass('active');

        // 添加选项卡对应的iframe
        var str1 = '<iframe class="common_iframe" name="iframe' + dataIndex + '" width="100%" height="100%" src="' + dataUrl + '" frameborder="0" data-id="' + dataUrl + '" data-panel="' + panelUrl + '" seamless></iframe>';
        $('.mainContent', topWindow).find('iframe.common_iframe').hide().parents('.mainContent').append(str1);
        
        window.parent.$.modal.loading("数据加载中，请稍后...");
        $('.mainContent iframe:visible', topWindow).load(function () {
        	window.parent.$.modal.closeLoading();
        });

        // 添加选项卡
        $('.menuTabs .page-tabs-content', topWindow).append(str);
    }
    return false;
}

//日志打印封装处理
var log = {
    log: function (msg) {
    	console.log(msg);
    },
    info: function(msg) {
    	console.info(msg);
    },
    warn: function(msg) {
    	console.warn(msg);
    },
    error: function(msg) {
    	console.error(msg);
    }
};

/** 设置全局ajax处理 */
$.ajaxSetup({
	complete: function (XMLHttpRequest, textStatus) {
		if (textStatus == 'timeout') {
			$.modal.alertWarning("服务器超时，请稍后再试！");
			$.modal.enable();
			$.modal.closeLoading();
		} else if (textStatus == "parsererror") {
			$.modal.alertWarning("服务器错误，请联系管理员！");
			$.modal.enable();
			$.modal.closeLoading();
		} else if (textStatus == "error") {
			var message = "服务器错误，请联系管理员！";
			var error = XMLHttpRequest.responseJSON;
			if (error&&error.Status!=undefined&&error.Status == 0 && error.Message) {
				message = error.Message;
			} else if (error&&error.message) {
				message = error.message;
			}
			else if(XMLHttpRequest.responseText)
			{
				message=XMLHttpRequest.responseText;
			}
			$.modal.alertWarning(message);
			$.modal.enable();
			$.modal.closeLoading();
		}
	}
});
layer.config({
    extend: 'moon/style.css',
    skin: 'layer-ext-moon'
});
