/**
 * 通用js方法封装处理
 * Copyright (c) 2021 commonservice
 */
(function ($) {
    $.extend({
    	_tree: {},
    	btTable: {},
    	bttTable: {},
		apitable: {
			_option: {},
			// 初始化表格参数
			init: function(options) {
				var defaults = {
					id: "bootstrap-table",
					type: 0, // 0 代表bootstrapTable 1代表bootstrapTreeTable
					height: undefined,
					sidePagination: "server",
					sortName: "",
					sortOrder: "desc",
					pagination: true,
					pageSize: 10,
					pageList: [10, 25, 50],
					toolbar: "toolbar",
					striped: true,
					escape: false,
					firstLoad: true,
					showFooter: false,
					search: false,
					showSearch: true,
					showPageGo: false,
					showRefresh: true,
					showColumns: true,
					showToggle: true,
					showExport: false,
					singleSelect : false,
					clickToSelect: false,
					rememberSelected: false,
					fixedColumns: false,
					fixedNumber: 0,
					rightFixedColumns: false,
					rightFixedNumber: 0,
					queryParams: $.apitable.queryParams,
					rowStyle: {},
				};
				var options = $.extend(defaults, options);
				$.apitable._option = options;
				$.btTable = $('#' + options.id);
				$.apitable.initEvent();
				$('#' + options.id).bootstrapTable({
					url: options.url,                                   // 请求后台的URL（*）
					contentType: options.contentType,   // 编码类型
					method:options.method,                                     // 请求方式（*）
					cache: false,                                       // 是否使用缓存
					height: options.height,                             // 表格的高度
					striped: options.striped,                           // 是否显示行间隔色
					sortable: true,                                     // 是否启用排序
					sortStable: true,                                   // 设置为 true 将获得稳定的排序
					sortName: options.sortName,                         // 排序列名称
					sortOrder: options.sortOrder,                       // 排序方式  asc 或者 desc
					pagination: options.pagination,                     // 是否显示分页（*）
					pageNumber: 1,                                      // 初始化加载第一页，默认第一页
					pageSize: options.pageSize,                         // 每页的记录行数（*）
					pageList: options.pageList,                         // 可供选择的每页的行数（*）
					firstLoad: options.firstLoad,                       // 是否首次请求加载数据，对于数据较大可以配置false
					escape: options.escape,                             // 转义HTML字符串
					showFooter: options.showFooter,                     // 是否显示表尾
					iconSize: 'outline',                                // 图标大小：undefined默认的按钮尺寸 xs超小按钮sm小按钮lg大按钮
					toolbar: '#' + options.toolbar,                     // 指定工作栏
					sidePagination: options.sidePagination,             // server启用服务端分页client客户端分页
					search: options.search,                             // 是否显示搜索框功能
					searchText: options.searchText,                     // 搜索框初始显示的内容，默认为空
					showSearch: options.showSearch,                     // 是否显示检索信息
					showPageGo: options.showPageGo,               		// 是否显示跳转页
					showRefresh: options.showRefresh,                   // 是否显示刷新按钮
					showColumns: options.showColumns,                   // 是否显示隐藏某列下拉框
					showToggle: options.showToggle,                     // 是否显示详细视图和列表视图的切换按钮
					showExport: options.showExport,                     // 是否支持导出文件
					singleSelect:options.singleSelect,					//是否单选
					uniqueId: options.uniqueId,                         // 唯一的标识符
					clickToSelect: options.clickToSelect,				// 是否启用点击选中行
					detailView: options.detailView,                     // 是否启用显示细节视图
					onClickRow: options.onClickRow,                     // 点击某行触发的事件
					onDblClickRow: options.onDblClickRow,               // 双击某行触发的事件
					onClickCell: options.onClickCell,                   // 单击某格触发的事件
					onDblClickCell: options.onDblClickCell,             // 双击某格触发的事件
					rememberSelected: options.rememberSelected,         // 启用翻页记住前面的选择
					fixedColumns: options.fixedColumns,                 // 是否启用冻结列（左侧）
					fixedNumber: options.fixedNumber,                   // 列冻结的个数（左侧）
					rightFixedColumns: options.rightFixedColumns,       // 是否启用冻结列（右侧）
					rightFixedNumber: options.rightFixedNumber,         // 列冻结的个数（右侧）
					onReorderRow: options.onReorderRow,                 // 当拖拽结束后处理函数
					queryParams: options.queryParams,                   // 传递参数（*）
					rowStyle: options.rowStyle,                         // 通过自定义函数设置行样式
					columns: options.columns,                           // 显示列信息（*）
					responseHandler: $.apitable.responseHandler,           // 在加载服务器发送来的数据之前处理函数
					onLoadSuccess: $.apitable.onLoadSuccess,               // 当所有数据被加载时触发处理函数
					exportOptions: options.exportOptions,               // 前端导出忽略列索引
					detailFormatter: options.detailFormatter          // 在行下面展示其他数据列表
				});
			},
			// 查询条件
			queryParams: function(params) {
				var curParams = {
					// 传递参数查询参数
					pagesize:       params.limit,
					pageindex:      params.offset / params.limit + 1,
					keyword:        params.search,
					sort:  			params.sort,
					order:          params.order
				};
				var currentId = $.common.isEmpty($.apitable._option.formId) ? $('form').attr('id') : $.apitable._option.formId;
				var data=$.common.formToJSON(currentId);
				for ( var key in data ){
					if ( data[key] === '' ){
						delete data[key]
					}
				}
				return $.extend(curParams, data);
			},
			// 请求获取数据后处理回调函数
			responseHandler: function (res) {
				if (typeof $.apitable._option.responseHandler == "function") {
					$.apitable._option.responseHandler(res);
				}
				if (res.Status == rpcresponse_status.SUCCESS) {
					if ($.common.isNotEmpty($.apitable._option.sidePagination) && $.apitable._option.sidePagination == 'client') {
						return res.Item;
					} else {
						if ($.common.isNotEmpty($.apitable._option.rememberSelected) && $.apitable._option.rememberSelected) {
							var column = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable._option.columns[1].field : $.apitable._option.uniqueId;
							$.each(res.Item, function(i, row) {
								row.state = $.inArray(row[column], selectionIds) !== -1;
							})
						}
						if (!res.Item)
							return {rows:null,total:0};
						return { rows: res.Item  , total: res.Total };
					}
				} else {
					$.modal.alertWarning(res.Message);
					return { rows: [], total: 0 };
				}
			},
			// 初始化事件
			initEvent: function (data) {
				// 触发行点击事件 加载成功事件
				$.btTable.on("check.bs.table uncheck.bs.table check-all.bs.table uncheck-all.bs.table load-success.bs.table", function () {
					// 工具栏按钮控制
					var rows = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
					// 非多个禁用
					$('#' + $.apitable._option.toolbar + ' .multiple').toggleClass('disabled', !rows.length);
					// 非单个禁用
					$('#' + $.apitable._option.toolbar + ' .single').toggleClass('disabled', rows.length!=1);
				});
				// 绑定选中事件、取消事件、全部选中、全部取消
				$.btTable.on("check.bs.table check-all.bs.table uncheck.bs.table uncheck-all.bs.table", function (e, rows) {
					// 复选框分页保留保存选中数组
					var rowIds = $.apitable.affectedRowIds(rows);
					if ($.common.isNotEmpty($.apitable._option.rememberSelected) && $.apitable._option.rememberSelected) {
						func = $.inArray(e.type, ['check', 'check-all']) > -1 ? 'union' : 'difference';
						selectionIds = _[func](selectionIds, rowIds);
					}
				});
				// 图片预览事件
				$.btTable.on('click', '.img-circle', function() {
					var src = $(this).attr('src');
					var target = $(this).data('target');
					var height = $(this).data('height');
					var width = $(this).data('width');
					if($.common.equals("self", target)) {
						layer.open({
							title: false,
							type: 1,
							closeBtn: true,
							shadeClose: true,
							area: ['auto', 'auto'],
							content: "<img src='" + src + "' height='" + height + "' width='" + width + "'/>"
						});
					} else if ($.common.equals("blank", target)) {
						window.open(src);
					}
				});
				// 单击tooltip复制文本
				$.btTable.on('click', '.tooltip-show', function() {
					var input = $(this).prev();
					input.select();
					document.execCommand("copy");
				});
			},
			// 当所有数据被加载时触发
			onLoadSuccess: function(data) {
				if (typeof $.apitable._option.onLoadSuccess == "function") {
					$.apitable._option.onLoadSuccess(data);
				}
				// 浮动提示框特效
				$("[data-toggle='tooltip']").tooltip();
			},
			// 表格销毁
			destroy: function (tableId) {
				var currentId = $.common.isEmpty(tableId) ? $.apitable._option.id : tableId;
				$("#" + currentId).bootstrapTable('destroy');
			},
			// 序列号生成
			serialNumber: function (index) {
				var table = $.btTable.bootstrapTable('getOptions');
				var pageSize = table.pageSize;
				var pageNumber = table.pageNumber;
				return pageSize * (pageNumber - 1) + index + 1;
			},
			// 列超出指定长度浮动提示（单击文本复制）
			tooltip: function (value, length) {
				var _length = $.common.isEmpty(length) ? 20 : length;
				var _text = "";
				var _value = $.common.nullToStr(value);
				if (_value.length > _length) {
					_text = _value.substr(0, _length) + "...";
					_value = _value.replace(/\'/g,"’");
					var actions = [];
					actions.push($.common.sprintf('<input id="tooltip-show" style="opacity: 0;position: absolute;z-index:-1" type="text" value="%s"/>', _value));
					actions.push($.common.sprintf("<a href='###' class='tooltip-show' data-toggle='tooltip' title='%s'>%s</a>", _value, _text));
					return actions.join('');
				} else {
					_text = _value;
					return _text;
				}
			},
			// 下拉按钮切换
			dropdownToggle: function (value) {
				var actions = [];
				actions.push('<div class="btn-group">');
				actions.push('<button type="button" class="btn btn-xs dropdown-toggle" data-toggle="dropdown" aria-expanded="false">');
				actions.push('<i class="fa fa-cog"></i>&nbsp;<span class="fa fa-chevron-down"></span></button>');
				actions.push('<ul class="dropdown-menu">');
				actions.push(value.replace(/<a/g,"<li><a").replace(/<\/a>/g,"</a></li>"));
				actions.push('</ul>');
				actions.push('</div>');
				return actions.join('');
			},
			// 图片预览
			imageView: function (value, height, width, target) {
				if ($.common.isEmpty(width)) {
					width = 'auto';
				}
				if ($.common.isEmpty(height)) {
					height = 'auto';
				}
				// blank or self
				var _target = $.common.isEmpty(target) ? 'self' : target;
				if ($.common.isNotEmpty(value)) {
					return $.common.sprintf("<img class='img-circle img-xs' data-height='%s' data-width='%s' data-target='%s' src='%s'/>", width, height, _target, value);
				} else {
					return $.common.nullToStr(value);
				}
			},
			// 搜索-默认第一个form
            search: function (formId, tableId, data) {
				var currentId = $.common.isEmpty(formId) ? $('form').attr('id') : formId;
				var params = $.common.isEmpty(tableId) ? $.btTable.bootstrapTable('getOptions') : $("#" + tableId).bootstrapTable('getOptions');
				params.queryParams = function(params) {
					var search = $.common.formToJSON(currentId);
					if($.common.isNotEmpty(data)){
						$.each(data, function(key) {
							if(data[key])
							{
								search[key] = data[key];
							}
						});
					}
					if($.common.isNotEmpty(search)){
						$.each(search, function(key) {
							if(!search[key])
							{
								delete search[key];
							}
						});
					}
					search.pagesize = params.limit;
					search.pageindex = params.offset / params.limit + 1;
					search.keyword = params.search;
					search.order = params.order;
					search.sort = params.sort;
					return search;
				}
				if($.common.isNotEmpty(tableId)){
					$("#" + tableId).bootstrapTable('refresh', params);
				} else{
					$.btTable.bootstrapTable('refresh', params);
				}
			},
			// 导出数据
			exportExcel: function(columns) {
				$.modal.confirm("确定导出所有" + $.apitable._option.modalName + "吗？", function() {
					$.modal.loading("正在导出数据，请稍后...");
					window.location.href = $.apitable._option.exportUrl+(columns?'?columns='+columns:'');
					$.modal.closeLoading();
				});
			},
			// 下载模板
			importTemplate: function() {
				window.location.href =$.apitable._option.importTemplateUrl;
			},
			// 导入数据
			importExcel: function(formId) {
				var importHtml='<form enctype="multipart/form-data" class="mt20 mb10">\n' +
					'        <div class="col-xs-offset-1">\n' +
					'            <input type="file" id="file" name="file"/>\n' +
					'            <div class="mt10 pt5">\n' +
					'                <a onclick="$.apitable.importTemplate()" class="btn btn-default btn-xs"><i class="fa fa-file-excel-o"></i> 下载模板</a>\n' +
					'            </div>\n';
				if(formId && $('#'+formId).html().length>0)importHtml+=$('#'+formId).html();
				importHtml+='<font color="red" class="pull-left mt10">\n' +
					'                提示：仅允许导入“xls”或“xlsx”格式文件！\n' +
					'         </font>\n' +
					'        </div>\n' +
					'    </form>';
				layer.open({
					type: 1,
					area: ['400px', '230px'],
					fix: false,
					//不固定
					maxmin: true,
					shade: 0.3,
					title: '导入' + $.apitable._option.modalName + '数据',
					content: importHtml,
					btn: ['<i class="fa fa-check"></i> 导入', '<i class="fa fa-remove"></i> 取消'],
					// 弹层外区域关闭
					shadeClose: true,
					btn1: function(index, layero){
						var file = layero.find('#file').val();
						if (file == '' || (!$.common.endWith(file, '.xls') && !$.common.endWith(file, '.xlsx'))){
							$.modal.msgWarning("请选择后缀为 “xls”或“xlsx”的文件。");
							return false;
						}
						var index = layer.load(2, {shade: false});
						$.modal.disable();
						var formData = new FormData();
						$.each($('#file')[0].files, function (i, obj) {
							formData.append("file", obj); //加入文件对象
						});
						formData.append("updateSupport", $("input[name='updateSupport']").is(':checked'));
						$.ajax({
							url: $.apitable._option.importUrl,
							data: formData,
							cache: false,
							contentType: false,
							processData: false,
							type: 'POST',
							beforeSend: function (request){
								request.setRequestHeader("Authorization", $.cookie('token_type')+" "+$.cookie('access_token'));
							},
							success: function (result) {
								if (result.Status == rpcresponse_status.SUCCESS) {
									$.modal.closeAll();
									$.modal.alertSuccess(result.Message);
									$.apitable.refresh();
								} else if (result.Status == rpcresponse_status.WARNING) {
									layer.close(index);
									$.modal.enable();
									$.modal.alertWarning(result.Message)
								} else {
									layer.close(index);
									$.modal.enable();
									$.modal.alertError(result.Message);
								}
							}
						});
					}
				});
			},
			// 刷新表格
			refresh: function() {
				$.btTable.bootstrapTable('refresh', {
					silent: true
				});
			},
			// 查询表格指定列值
			selectColumns: function(column) {
				var rows = $.map($.btTable.bootstrapTable('getSelections'), function (row) {
					return row[column];
				});
				if ($.common.isNotEmpty($.apitable._option.rememberSelected) && $.apitable._option.rememberSelected) {
					rows = rows.concat(selectionIds);
				}
				return $.common.uniqueFn(rows);
			},
			//查询表格选中的列
			getSelectColumns:function()
			{
				return $.btTable.bootstrapTable('getSelections');
			},
			// 获取当前页选中或者取消的行ID
			affectedRowIds: function(rows) {
				var column = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable._option.columns[1].field : $.apitable._option.uniqueId;
				var rowIds;
				if ($.isArray(rows)) {
					rowIds = $.map(rows, function(row) {
						return row[column];
					});
				} else {
					rowIds = [rows[column]];
				}
				return rowIds;
			},
			// 查询表格首列值
			selectFirstColumns: function() {
				var rows = $.map($.btTable.bootstrapTable('getSelections'), function (row) {
					return row[$.apitable._option.columns[1].field];
				});
				if ($.common.isNotEmpty($.apitable._option.rememberSelected) && $.apitable._option.rememberSelected) {
					rows = rows.concat(selectionIds);
				}
				return $.common.uniqueFn(rows);
			},
			// 回显数据字典
			selectDictLabel: function(datas, value) {
				var actions = [];
				$.each(datas, function(index, dict) {
					if (dict.dictValue == ('' + value)) {
						var listClass = $.common.equals("default", dict.listClass) || $.common.isEmpty(dict.listClass) ? "" : "badge badge-" + dict.listClass;
					actions.push($.common.sprintf("<span class='%s'>%s</span>", listClass, dict.dictLabel));
					return false;
				}
				});
				return actions.join('');
			},
			// 显示表格指定列
			showColumn: function(column) {
				$.btTable.bootstrapTable('showColumn', column);
			},
			// 隐藏表格指定列
			hideColumn: function(column) {
				$.btTable.bootstrapTable('hideColumn', column);
			},
			// 查询表格指定列值
			gettableDate: function() {
				var rows =$.btTable.bootstrapTable('getData');

				return rows;
			},
			getRowByUnitqueId: function (id) {
				return $.btTable.bootstrapTable('getRowByUniqueId',id);
            }

		},
        apitreeTable: {
			_option: {},
            // 初始化表格
            init: function(options) {
                var defaults = {
                    id: "bootstrap-tree-table",
                    type: 1, // 0 代表bootstrapTable 1代表bootstrapTreeTable
                    height: 0,
                    pageSize: 10,
                    pageList: [10, 25, 50],
                    rootIdValue: null,
                    ajaxParams: {},
                    toolbar: "toolbar",
                    striped: false,
                    expandColumn: 1,
                    showSearch: true,
                    showRefresh: true,
                    showColumns: true,
                    expandAll: true,
                    expandFirst: true
                };
                var options = $.extend(defaults, options);
                $.apitable._option = options;
                $.bttTable = $('#' + options.id).bootstrapTreeTable({
                    code: options.code,                                 // 用于设置父子关系
                    parentCode: options.parentCode,                     // 用于设置父子关系
					method:options.method,                                     // 请求方式（*）
					cache: false,
					url: options.url,                                   // 请求后台的URL（*）
					contentType: options.contentType,   // 编码类型
                    data: options.data,                                 // 无url时用于渲染的数据
                    ajaxParams: options.ajaxParams,                     // 请求数据的ajax的data属性
                    rootIdValue: options.rootIdValue,                   // 设置指定根节点id值
                    height: options.height,                             // 表格树的高度
                    expandColumn: options.expandColumn,                 // 在哪一列上面显示展开按钮
                    striped: options.striped,                           // 是否显示行间隔色
                    bordered: true,                                     // 是否显示边框
                    toolbar: '#' + options.toolbar,                     // 指定工作栏
                    showSearch: options.showSearch,                     // 是否显示检索信息
                    showRefresh: options.showRefresh,                   // 是否显示刷新按钮
                    showColumns: options.showColumns,                   // 是否显示隐藏某列下拉框
                    expandAll: options.expandAll,                       // 是否全部展开
                    expandFirst: options.expandFirst,                   // 是否默认第一级展开--expandAll为false时生效
                    columns: options.columns,                           // 显示列信息（*）
                    responseHandler: $.apitreeTable.responseHandler        // 当所有数据被加载时触发处理函数
                });
            },
            // 条件查询
            search: function(formId) {
                var currentId = $.common.isEmpty(formId) ? $('form').attr('id') : formId;
                var params = $.common.formToJSON(currentId);
				for ( var key in params ){
					if ( params[key] === '' ){
						delete params[key]
					}
				}
                $.bttTable.bootstrapTreeTable('refresh', params);
            },
            // 刷新
            refresh: function() {
                $.bttTable.bootstrapTreeTable('refresh');
            },
            // 查询表格树指定列值
            selectColumns: function(column) {
                var rows = $.map($.bttTable.bootstrapTreeTable('getSelections'), function (row) {
                    return row[column];
                });
                return $.common.uniqueFn(rows);
            },
            // 请求获取数据后处理回调函数，校验异常状态提醒
            responseHandler: function(res) {
				if (typeof $.apitreeTable._option.responseHandler == "function") {
					$.apitreeTable._option.responseHandler(res);
				}
                if (res.Status == rpcresponse_status.FAIL) {
                    $.modal.alertWarning(res.Message);
                    return [];
                } else {
                    return res.Item;
                }
            },
        },
		apiform: {
			// 表单重置
			reset: function(formId, tableId) {
				var currentId = $.common.isEmpty(formId) ? $('form').attr('id') : formId;
				$("#" + currentId)[0].reset();
				if ($.apitable._option.type == table_type.bootstrapTable) {
					if($.common.isEmpty(tableId)){
						$.btTable.bootstrapTable('refresh');
					} else{
						$("#" + tableId).bootstrapTable('refresh');
					}
				}
			},
			// 获取选中复选框项
			selectCheckeds: function(name) {
				var checkeds = "";
				$('input:checkbox[name="' + name + '"]:checked').each(function(i) {
					if (0 == i) {
						checkeds = $(this).val();
					} else {
						checkeds += ("," + $(this).val());
					}
				});
				return checkeds;
			},
			// 获取选中下拉框项
			selectSelects: function(name) {
				var selects = "";
				$('#' + name + ' option:selected').each(function (i) {
					if (0 == i) {
						selects = $(this).val();
					} else {
						selects += ("," + $(this).val());
					}
				});
				return selects;
			}
		},
        // 弹出层封装处理
    	modal: {
    		// 显示图标
    		icon: function(type) {
            	var icon = "";
        	    if (type == modal_status.WARNING) {
        	        icon = 0;
        	    } else if (type == modal_status.SUCCESS) {
        	        icon = 1;
        	    } else if (type == modal_status.FAIL) {
        	        icon = 2;
        	    } else {
        	        icon = 3;
        	    }
        	    return icon;
            },
    		// 消息提示
            msg: function(content, type) {
            	if (type != undefined) {
                    layer.msg(content, { icon: $.modal.icon(type), time: 1000, shift: 5 });
                } else {
                    layer.msg(content);
                }
            },
            // 错误消息
            msgError: function(content) {
            	$.modal.msg(content, modal_status.FAIL);
            },
            // 成功消息
            msgSuccess: function(content) {
            	$.modal.msg(content, modal_status.SUCCESS);
            },
            // 警告消息
            msgWarning: function(content) {
            	$.modal.msg(content, modal_status.WARNING);
            },
    		// 弹出提示
            alert: function(content, type) {
        	    layer.alert(content, {
        	        icon: $.modal.icon(type),
        	        title: "系统提示",
        	        btn: ['确认'],
        	        btnclass: ['btn btn-primary'],
        	    });
            },
            // 消息提示并刷新父窗体
            msgReload: function(msg, type) {
            	layer.msg(msg, {
            	    icon: $.modal.icon(type),
            	    time: 500,
            	    shade: [0.1, '#8F8F8F']
            	},
            	function() {
            	    $.modal.reload();
            	});
            },
            // 错误提示
            alertError: function(content) {
            	$.modal.alert(content, modal_status.FAIL);
            },
            // 成功提示
            alertSuccess: function(content) {
            	$.modal.alert(content, modal_status.SUCCESS);
            },
            // 警告提示
            alertWarning: function(content) {
            	$.modal.alert(content, modal_status.WARNING);
            },
            // 关闭窗体
            close: function () {
            	var index = parent.layer.getFrameIndex(window.name);
                parent.layer.close(index);
            },
            // 关闭全部窗体
            closeAll: function () {
                layer.closeAll();
            },
            // 确认窗体
            confirm: function (content, callBack) {
            	layer.confirm(content, {
        	        icon: 3,
        	        title: "系统提示",
        	        btn: ['确认', '取消']
        	    }, function (index) {
        	    	layer.close(index);
        	        callBack(true);
        	    });
            },
            // 弹出层指定宽度
            open: function (title, url, width, height, showType, yesCallback, cancelCallback,endCallback, successCallback,isfull) {
            	//如果是移动端，就使用自适应大小弹窗
            	if (navigator.userAgent.match(/(iPhone|iPod|Android|ios)/i)) {
            	    width = 'auto';
            	    height = 'auto';
            	}
            	if ($.common.isEmpty(title)) {
                    title = false;
                }
                if ($.common.isEmpty(url)) {
                    url = "/404.html";
                }
				if(typeof width === 'number')
				{
					width=width+"px";
				}
				if(typeof height === 'number')
				{
					height=height+"px";
				}
                if ($.common.isEmpty(width)) {
                	width = '800px';
                }
                if ($.common.isEmpty(height)) {
                	height = ($(window).height() - 50)+'px';
                }

                if ($.common.isEmpty(yesCallback)) {
					yesCallback = function (index, layero) {
                        var iframeWin = layero.find('iframe')[0];
                        iframeWin.contentWindow.submitHandler(index, layero);
                    }
                }

				var btnss = [];
				if (showType == 2) {
					btnss = ['确认', '关闭'];
				}
				else if (showType == 3) {
					btnss = [];
				}
				else
				{
					btnss = ['保存', '关闭'];
				}
				var type = 2;
				if (url instanceof jQuery) {
					type = 1;
				}
				if (url.indexOf('<')>-1&&url.indexOf('>')>-1) {
					type = 1;
				}
            	var nindex= layer.open({
            		type: type,
            		area: [width , height],
            		fix: false,
            		//不固定
            		maxmin: true,
            		shade: 0.8,
					btnAlign: 'c',
            		title: title,
            		content: url,
            		btn: btnss,
            	    // 弹层外区域关闭
            		shadeClose: true,
					success: successCallback,
					end: endCallback,
            		yes: yesCallback,
					cancel: cancelCallback
            	});
				if(isfull)
				{
					layer.full(nindex);
				}
            },
            // 弹出层指定参数选项
            openOptions: function (options) {
            	var _url = $.common.isEmpty(options.url) ? "/404.html" : options.url;
            	var _title = $.common.isEmpty(options.title) ? "系统窗口" : options.title;
                var _width = $.common.isEmpty(options.width) ? "800" : options.width;
                var _height = $.common.isEmpty(options.height) ? ($(window).height() - 50) : options.height;
                var _btn = ['<i class="fa fa-check"></i> 确认', '<i class="fa fa-close"></i> 关闭'];
                if ($.common.isEmpty(options.yes)&&$.common.isNotEmpty(options.callBack)) {
                	options.yes = function(index, layero) {
                     	 options.callBack(index, layero);
                     	 return false;
                    }
                }
				$.modal.open(_title, _url, _width, _height, 2, options.yes, options.cancel);
            },
            // 弹出层全屏
            openFull: function (title, url, width, height, showType, yesCallback, cancelCallback,endCallback, successCallback) {
    			$.modal.open(title, url, width, height, showType, yesCallback, cancelCallback,endCallback, successCallback,true);
            },
            // 选卡页方式打开
            openTab: function (title, url) {
            	createMenuItem(url, title);
            },
            // 选卡页同一页签打开
            parentTab: function (title, url) {
            	var dataId = window.frameElement.getAttribute('data-id');
            	createMenuItem(url, title);
            	closeItem(dataId);
            },
            // 关闭选项卡
            closeTab: function (dataId) {
            	closeItem(dataId);
            },
            // 禁用按钮
            disable: function() {
            	var doc = window.top == window.parent ? window.document : window.parent.document;
	        	$("a[class*=layui-layer-btn]", doc).addClass("layer-disabled");
            },
            // 启用按钮
            enable: function() {
            	var doc = window.top == window.parent ? window.document : window.parent.document;
            	$("a[class*=layui-layer-btn]", doc).removeClass("layer-disabled");
            },
            // 打开遮罩层
            loading: function (message) {
            	$.blockUI({ message: '<div class="loaderbox"><div class="loading-activity"></div> ' + message + '</div>' });
            },
            // 关闭遮罩层
            closeLoading: function () {
            	setTimeout(function(){
            		$.unblockUI();
            	}, 50);
            },
            // 重新加载
            reload: function () {
            	parent.location.reload();
			}
        },
		apioperate: {
			// 提交数据
			submit: function(url, type, dataType, data, callback,contentType,beforesend) {
				var config = {
					url: url,
					type: type,
					dataType: dataType,
					data: data,
					beforeSend: function (request){
						request.setRequestHeader("Authorization", $.cookie('token_type')+" "+$.cookie('access_token'));
						$.modal.loading("正在处理中，请稍后...");
						if (typeof beforesend == "function") {
							beforesend(request);
						}
					},
					success: function (result) {
						if (typeof callback == "function") {
							callback(result);
							$.modal.closeLoading();
						}
						else
						{
							$.apioperate.ajaxSuccess(result);
						}
					}
				};
				if(contentType)
					config.contentType=contentType;
				$.ajax(config)
			},
			// post请求传输,传递键值对
			postkv: function (url, data, callback) {
				if (callback == undefined)
					callback = function () { };
				$.apioperate.submit(url, "post", "json", data, callback);
			},
			// post请求传输，传递json
			post: function (url, data, callback) {
				if (callback == undefined)
					callback = function () { };
				$.apioperate.submit(url, "post", "json", data? JSON.stringify(data) : null, callback, "application/json");
			},
			// get请求传输
			get: function(url, callback) {
				$.apioperate.submit(url, "get", "json", "", callback,"application/json");
			},
			// 详细访问地址
			detailUrl: function(id) {
				var url = "/404.html";
				if ($.common.isNotEmpty(id)) {
					url = $.apitable._option.detailUrl.replace("{id}", id);
				} else {
					var id = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
					if (id.length == 0) {
						$.modal.alertWarning("请至少选择一条记录");
						return;
					}
					url = $.apitable._option.detailUrl.replace("{id}", id);
				}
				return url;
			},
			// 删除信息
			remove: function(id) {
				$.modal.confirm("确定删除该条" + $.apitable._option.modalName + "信息吗？", function() {
					var url = $.common.isEmpty(id) ? $.apitable._option.removeUrl : $.apitable._option.removeUrl.replace("{id}", id);
					var data = { "id": id };
					$.apioperate.submit(url, "delete", "json", data);
				});

			},
			// 批量删除信息
			removeAll: function() {

				var rows = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
				if (rows.length == 0) {
					$.modal.alertWarning("请至少选择一条记录");
					return;
				}
				var ids= rows.join();
				$.modal.confirm("确认要删除选中的" + rows.length + "条数据吗?", function() {
					var url = $.common.isEmpty(ids) ? $.apitable._option.removeAllUrl : $.apitable._option.removeAllUrl.replace("{ids}",  ids);
					var data = { "ids": ids };
					$.apioperate.submit(url, "delete", "json", data);
				});
			},
			// 清空信息
			clean: function() {
				$.modal.confirm("确定清空所有" + $.apitable._option.modalName + "吗？", function() {
					var url = $.apitable._option.cleanUrl;
					$.apioperate.submit(url, "post", "json", "");
				});
			},
			// 添加信息
			add: function (id, width, height) {
				$.modal.open("添加" + $.apitable._option.modalName, $.apioperate.addUrl(id),width,height);
			},
			// 添加信息，以tab页展现
			addTab: function (id) {
				$.modal.openTab("添加" + $.apitable._option.modalName, $.apioperate.addUrl(id));
			},
			// 添加信息 全屏
			addFull: function(id) {
				var url = $.common.isEmpty(id) ? $.apitable._option.createUrl : $.apitable._option.createUrl.replace("{id}", id);
				$.modal.openFull("添加" + $.apitable._option.modalName, url);
			},
			// 添加访问地址
			addUrl: function(id) {
				var url = $.common.isEmpty(id) ? $.apitable._option.createUrl.replace("{id}", "") : $.apitable._option.createUrl.replace("{id}", id);
				return url;
			},
			// 修改信息
			edit: function(id,width,height) {
				if ($.common.isEmpty(id) && $.apitable._option.type == table_type.bootstrapTreeTable) {
					var row = $.bttTable.bootstrapTreeTable('getSelections')[0];
					if ($.common.isEmpty(row)) {
						$.modal.alertWarning("请至少选择一条记录");
						return;
					}
					id = row[$.apitable._option.uniqueId];
					// var url = $.apitable._option.updateUrl.replace("{id}", row[$.apitable._option.uniqueId]);
				}
				$.modal.open("修改" + $.apitable._option.modalName, $.apioperate.editUrl(id),width,height);
			},
			treeedit: function(id,width,height) {
				if ($.common.isEmpty(id) && $.apitreeTable._option.type == table_type.bootstrapTreeTable) {
					var row = $.bttTable.bootstrapTreeTable('getSelections')[0];
					if ($.common.isEmpty(row)) {
						$.modal.alertWarning("请至少选择一条记录");
						return;
					}
					id = row[$.apitreeTable._option.uniqueId];
					// var url = $.apitable._option.updateUrl.replace("{id}", row[$.apitable._option.uniqueId]);
				}
				$.modal.open("修改" + $.apitreeTable._option.modalName, $.apitreeTable.editUrl(id),width,height);
			},
			// 查看信息
			detail: function(id,width,height) {
				$.modal.open("查看" + $.apitable._option.modalName+"详情", $.apioperate.detailUrl(id),width,height,3);
			},
			// 修改信息，以tab页展现
			editTab: function(id) {
				$.modal.openTab("修改" + $.apitable._option.modalName, $.apioperate.editUrl(id));
			},
			// 修改信息 全屏
			editFull: function(id) {
				var url = "/404.html";
				if ($.common.isNotEmpty(id)) {
					url = $.apitable._option.updateUrl.replace("{id}", id);
				} else {
					var row = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
					url = $.apitable._option.updateUrl.replace("{id}", row);
				}
				$.modal.openFull("修改" + $.apitable._option.modalName, url);
			},
			// 修改访问地址
			editUrl: function(id) {
				var url = "/404.html";
				if ($.common.isNotEmpty(id)) {
					url = $.apitable._option.updateUrl.replace("{id}", id);
				} else {
					var id = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
					if (id.length == 0) {
						$.modal.alertWarning("请至少选择一条记录");
						return;
					}
					url = $.apitable._option.updateUrl.replace("{id}", id);
				}
				return url;
			},
			// 修改访问地址
            detailUrl: function (id) {
				var url = "/404.html";
				if ($.common.isNotEmpty(id)) {
					url = $.apitable._option.detailUrl.replace("{id}", id);
				} else {
					var id = $.common.isEmpty($.apitable._option.uniqueId) ? $.apitable.selectFirstColumns() : $.apitable.selectColumns($.apitable._option.uniqueId);
					if (id.length == 0) {
						$.modal.alertWarning("请至少选择一条记录");
						return;
					}
					url = $.apitable._option.detailUrl.replace("{id}", id);
				}
				return url;
			},
			// 保存信息 刷新表格
			save: function(url, data, callback) {
				$.apioperate.submit(url, "post", "json", data ? JSON.stringify(data) : null, function (result) {
					if (typeof callback == "function") {
						callback(result);
						$.modal.closeLoading();
					}
					else
					{
						$.apioperate.successCallback(result);
					}
				},"application/json",function(){$.modal.disable();});
				},
			// 保存信息 不刷新表格
			savenoback: function(url, data, callback) {
				$.apioperate.submit(url, "post", "json", data ? JSON.stringify(data) : null, function (result) {
					if (typeof callback == "function") {
						callback(result);
						$.modal.closeLoading();
					}
					else
					{
						//$.apioperate.successCallback(result);
					}
				},"application/json",function(){$.modal.disable();});
			},
			// 保存信息 刷新表格
			update: function(url, data, callback) {
				$.apioperate.submit(url, "put", "json", data ? JSON.stringify(data) : null,function (result) {
					if (typeof callback == "function") {
						callback(result);
						$.modal.closeLoading();
					}
					else
					{
						$.apioperate.successCallback(result);
					}
				},"application/json",function(){$.modal.disable();});
			},
			// 保存信息 刷新表格
			patch: function(url, data, callback) {
                $.apioperate.submit(url, "patch", "json", data ? JSON.stringify(data) : null,function (result) {
                    if (typeof callback == "function") {
                        callback(result);
						$.modal.closeLoading();
                    }
                    else
                    {
                        $.apioperate.successCallback(result);
                    }
                },"application/json",function(){$.modal.disable();});
            },
            // 保存信息 刷新表格 不刷单
            patchnoback: function(url, data, callback) {
				$.apioperate.submit(url, "patch", "json", data ? JSON.stringify(data) : null,function (result) {
                    if (typeof callback == "function") {
                        callback(result);
						$.modal.closeLoading();
                    }
                    else
                    {
                        //$.apioperate.successCallback(result);
                    }
                },"application/json",function(){$.modal.disable();});
            },
			// 保存信息 刷新表格
			patchupdate: function(url, data, callback) {
				var params=[];
				for ( var key in data ) {
					params.push({"op": "replace", "path": key, "value": data[key]});
				}
				$.apioperate.patch(url, params,callback);
			},
			// 保存信息 刷新表格
			savefile: function(url, data, callback) {
				$.ajax({
					url:url,
					dataType:'json',
					type:'POST',
					async: false,
					data: data,
					processData : false, // 使数据不做处理
					contentType : false, // 不要设置Content-Type请求头
					beforeSend: function (request){
						request.setRequestHeader("Authorization", $.cookie('token_type')+" "+$.cookie('access_token'));
						$.modal.loading("正在处理中，请稍后...");
					},
					success: function(result) {
						if (typeof callback == "function") {
							callback(result);
							$.modal.closeLoading();
						}
					},
					error:function(response){
						console.log(response);
					}
				});
			},
			// 保存信息 弹出提示框
			saveModal: function(url, data, callback) {
				$.apioperate.submit(url, "post", "json", data ? JSON.stringify(data) : null, function (result) {
					if (typeof callback == "function") {
						callback(result);
						$.modal.closeLoading();
					}
					else {
						if (result.Status == rpcresponse_status.SUCCESS) {
							$.modal.alertSuccess(result.Message)
						} else if (result.Status == rpcresponse_status.WARNING) {
							$.modal.alertWarning(result.Message)
						} else {
							$.modal.alertError(result.Message);
						}
						$.modal.closeLoading();
					}
				}, "application/json");
			},
			// 保存选项卡信息
			saveTab: function(url, data, callback) {
				$.apioperate.submit(url, "post", "json", data ? JSON.stringify(data) : null, function (result) {
					if (typeof callback == "function") {
						callback(result);
						$.modal.closeLoading();
					}
					else
					{
						$.apioperate.successTabCallback(result);
					}
				},"application/json");

		},
			// 保存结果弹出msg刷新table表格
			ajaxSuccess: function (result) {
				if (result.Status == rpcresponse_status.SUCCESS && $.apitable._option.type == table_type.bootstrapTable) {
					$.modal.msgSuccess(result.Message);
					$.apitable.refresh();
				} else if (result.Status == rpcresponse_status.SUCCESS && $.apitable._option.type == table_type.bootstrapTreeTable) {
					$.modal.msgSuccess(result.Message);
					$.treeTable.refresh();
				} else if (result.Status == rpcresponse_status.WARNING) {
					$.modal.alertWarning(result.Message)
				}
				else if (result.Status == rpcresponse_status.SUCCESS) {

				}
				else {
					$.modal.alertError(result.Message);
				}
				$.modal.closeLoading();
			},
			// 成功结果提示msg（父窗体全局更新）
			saveSuccess: function (result) {
				if (result.Status == rpcresponse_status.SUCCESS) {
					$.modal.msgReload("保存成功,正在刷新数据请稍后……", modal_status.SUCCESS);
				} else if (result.Status == rpcresponse_status.WARNING) {
					$.modal.alertWarning(result.Message)
				}  else {
					$.modal.alertError(result.Message);
				}
				$.modal.closeLoading();
			},
			// 成功回调执行事件（父窗体静默更新）
			successCallback: function(result) {
				if (result.Status == rpcresponse_status.SUCCESS) {
					var parent = window.parent;
					if (parent.$.apitable._option.type == table_type.bootstrapTable) {
						$.modal.close();
						parent.$.modal.msgSuccess(result.Message);
						parent.$.apitable.refresh();
					} else if (parent.$.apitable._option.type == table_type.bootstrapTreeTable) {
						$.modal.close();
						parent.$.modal.msgSuccess(result.Message);
						parent.$.treeTable.refresh();
					} else {
						$.modal.msgReload("保存成功,正在刷新数据请稍后……", rpcresponse_status.SUCCESS);
					}
				} else if (result.Status == rpcresponse_status.WARNING) {
					$.modal.alertWarning(result.Message)
				}  else {
					$.modal.alertError(result.Message);
				}
				$.modal.closeLoading();
				$.modal.enable();
			},
			// 选项卡成功回调执行事件（父窗体静默更新）
			successTabCallback: function(result) {
				if (result.Status == rpcresponse_status.SUCCESS) {
					var topWindow = $(window.parent.document);
					var currentId = $('.page-tabs-content', topWindow).find('.active').attr('data-panel');
					var $contentWindow = $('.common_iframe[data-id="' + currentId + '"]', topWindow)[0].contentWindow;
					$.modal.close();
					$contentWindow.$.modal.msgSuccess(result.Message);
					$contentWindow.$(".layui-layer-padding").removeAttr("style");
					if ($contentWindow.$.apitable._option.type == table_type.bootstrapTable) {
						$contentWindow.$.apitable.refresh();
					} else if ($contentWindow.$.apitable._option.type == table_type.bootstrapTreeTable) {
						$contentWindow.$.treeTable.refresh();
					}
					$.modal.closeTab();
				} else if (result.Status == rpcresponse_status.WARNING) {
					$.modal.alertWarning(result.Message)
				} else {
					$.modal.alertError(result.Message);
				}
				$.modal.closeLoading();
			}
		},
        // 校验封装处理
        validate: {
        	// 判断返回标识是否唯一 false 不存在 true 存在
        	unique: function (value) {
            	if (value == "0") {
                    return true;
                }
                return false;
            },
            // 表单验证
            form: function (formId) {
            	var currentId = $.common.isEmpty(formId) ? $('form').attr('id') : formId;
            	var dom=$("#" + currentId);
            	var errorcount=0;
				$.each($('div[contenteditable="true"][required]'),function() {
					if ($(this).html().length == 0) {
						if (!$(this).hasClass('error')) {
							$(this).addClass('error');
							var tips = $(this).parent().prev().html();
							if ($.common.endWith(tips, '：') || $.common.endWith(tips, ':')) {
								tips = tips.substring(0, tips.length - 1);
							}
							$(this).after("<label class='error1'><i class='fa fa-times-circle'></i>" + tips + "不能为空" + '</label>');
						}
						errorcount++;
					}
				});
                var ret= dom.validate().form();
                return ret&&errorcount==0;
            }
        },
        // 树插件封装处理
        tree: {
        	_option: {},
        	_lastValue: {},
        	// 初始化树结构
        	init: function(options) {
        		var defaults = {
            		id: "tree",                    // 属性ID
            		expandLevel: 0,                // 展开等级节点
            		view: {
    			        selectedMulti: false,      // 设置是否允许同时选中多个节点
    			        nameIsHTML: true           // 设置 name 属性是否支持 HTML 脚本
    			    },
            		check: {
    				    enable: false,             // 置 zTree 的节点上是否显示 checkbox / radio
    				    nocheckInherit: true,      // 设置子节点是否自动继承
    				},
    				data: {
    			        key: {
    			            title: "title"         // 节点数据保存节点提示信息的属性名称
    			        },
    			        simpleData: {
    			            enable: true           // true / false 分别表示 使用 / 不使用 简单数据模式
    			        }
    			    },
        		};
            	var options = $.extend(defaults, options);
        		$.tree._option = options;
        		// 树结构初始化加载
        		var setting = {
    				callback: {
    			        onClick: options.onClick,                      // 用于捕获节点被点击的事件回调函数
    			        onCheck: options.onCheck,                      // 用于捕获 checkbox / radio 被勾选 或 取消勾选的事件回调函数
    			        onDblClick: options.onDblClick                 // 用于捕获鼠标双击之后的事件回调函数
    			    },
    				check: options.check,
    			    view: options.view,
    			    data: options.data
    			};
        	    $.get(options.url, function(data) {
        			var treeId = $("#treeId").val();
        			tree = $.fn.zTree.init($("#" + options.id), setting, data);
        			$._tree = tree;
        			var nodes = tree.getNodesByParam("level", options.expandLevel - 1);
        			for (var i = 0; i < nodes.length; i++) {
        				tree.expandNode(nodes[i], true, false, false);
        			}
        			var node = tree.getNodesByParam("id", treeId, null)[0];
        			$.tree.selectByIdName(treeId, node);
        		});
        	},
        	// 搜索节点
        	searchNode: function() {
        		// 取得输入的关键字的值
        		var value = $.common.trim($("#keyword").val());
        		if ($.tree._lastValue == value) {
        		    return;
        		}
        		// 保存最后一次搜索名称
        		$.tree._lastValue = value;
        		var nodes = $._tree.getNodes();
        		// 如果要查空字串，就退出不查了。
        		if (value == "") {
        		    $.tree.showAllNode(nodes);
        		    return;
        		}
        		$.tree.hideAllNode(nodes);
        		// 根据搜索值模糊匹配
        		$.tree.updateNodes($._tree.getNodesByParamFuzzy("name", value));
        	},
        	// 根据Id和Name选中指定节点
        	selectByIdName: function(treeId, node) {
        		if ($.common.isNotEmpty(treeId) && treeId == node.id) {
        			$._tree.selectNode(node, true);
        		}
        	},
        	// 显示所有节点
        	showAllNode: function(nodes) {
        		nodes = $._tree.transformToArray(nodes);
        		for (var i = nodes.length - 1; i >= 0; i--) {
        		    if (nodes[i].getParentNode() != null) {
        		    	$._tree.expandNode(nodes[i], true, false, false, false);
        		    } else {
        		    	$._tree.expandNode(nodes[i], true, true, false, false);
        		    }
        		    $._tree.showNode(nodes[i]);
        		    $.tree.showAllNode(nodes[i].children);
        		}
        	},
        	// 隐藏所有节点
        	hideAllNode: function(nodes) {
        	    var tree = $.fn.zTree.getZTreeObj("tree");
        	    var nodes = $._tree.transformToArray(nodes);
        	    for (var i = nodes.length - 1; i >= 0; i--) {
        	    	$._tree.hideNode(nodes[i]);
        	    }
        	},
        	// 显示所有父节点
        	showParent: function(treeNode) {
        		var parentNode;
        		while ((parentNode = treeNode.getParentNode()) != null) {
        			$._tree.showNode(parentNode);
        			$._tree.expandNode(parentNode, true, false, false);
        		    treeNode = parentNode;
        		}
        	},
        	// 显示所有孩子节点
        	showChildren: function(treeNode) {
        		if (treeNode.isParent) {
        		    for (var idx in treeNode.children) {
        		        var node = treeNode.children[idx];
        		        $._tree.showNode(node);
        		        $.tree.showChildren(node);
        		    }
        		}
        	},
        	// 更新节点状态
        	updateNodes: function(nodeList) {
        		$._tree.showNodes(nodeList);
        		for (var i = 0, l = nodeList.length; i < l; i++) {
        		    var treeNode = nodeList[i];
        		    $.tree.showChildren(treeNode);
        		    $.tree.showParent(treeNode)
        		}
        	},
        	// 获取当前被勾选集合
        	getCheckedNodes: function(column) {
        		var _column = $.common.isEmpty(column) ? "id" : column;
        		var nodes = $._tree.getCheckedNodes(true);
        		return $.map(nodes, function (row) {
        	        return row[_column];
        	    }).join();
        	},
        	// 不允许根父节点选择
        	notAllowParents: function(_tree) {
    		    var nodes = _tree.getSelectedNodes();
    		    if(nodes.length == 0){
                    $.modal.msgError("请选择节点后提交");
                    return false;
				}
    		    for (var i = 0; i < nodes.length; i++) {
    		        if (nodes[i].level == 0) {
    		            $.modal.msgError("不能选择根节点（" + nodes[i].name + "）");
    		            return false;
    		        }
    		        if (nodes[i].isParent) {
    		            $.modal.msgError("不能选择父节点（" + nodes[i].name + "）");
    		            return false;
    		        }
    		    }
        		return true;
        	},
        	// 不允许最后层级节点选择
        	notAllowLastLevel: function(_tree) {
    		    var nodes = _tree.getSelectedNodes();
    		    for (var i = 0; i < nodes.length; i++) {
                    if (!nodes[i].isParent) {
    		    		$.modal.msgError("不能选择最后层级节点（" + nodes[i].name + "）");
    		            return false;
    		        }
    		    }
        		return true;
        	},
        	// 隐藏/显示搜索栏
        	toggleSearch: function() {
        		$('#search').slideToggle(200);
        		$('#btnShow').toggle();
        		$('#btnHide').toggle();
        		$('#keyword').focus();
        	},
        	// 折叠
        	collapse: function() {
        		$._tree.expandAll(false);
        	},
        	// 展开
        	expand: function() {
        		$._tree.expandAll(true);
        	}
        },
        // 通用方法封装处理
    	common: {
    		// 判断字符串是否为空
            isEmpty: function (value) {
                if (value == null || this.trim(value) == "") {
                    return true;
                }
                return false;
            },
            // 判断一个字符串是否为非空串
            isNotEmpty: function (value) {
            	return !$.common.isEmpty(value);
            },
            // 空对象转字符串
            nullToStr: function(value) {
                if ($.common.isEmpty(value)) {
                    return "-";
                }
                return value;
            },
            // 是否显示数据 为空默认为显示
            visible: function (value) {
                if ($.common.isEmpty(value) || value == true) {
                    return true;
                }
                return false;
            },
            // 空格截取
            trim: function (value) {
                if (value == null) {
                    return "";
                }
                return value.toString().replace(/(^\s*)|(\s*$)|\r|\n/g, "");
            },
            // 比较两个字符串（大小写敏感）
            equals: function (str, that) {
            	return str == that;
            },
            // 比较两个字符串（大小写不敏感）
            equalsIgnoreCase: function (str, that) {
            	return String(str).toUpperCase() === String(that).toUpperCase();
            },
            // 将字符串按指定字符分割
            split: function (str, sep, maxLen) {
            	if ($.common.isEmpty(str)) {
            	    return null;
            	}
            	var value = String(str).split(sep);
            	return maxLen ? value.slice(0, maxLen - 1) : value;
            },
            // 字符串格式化(%s )
            sprintf: function (str) {
                var args = arguments, flag = true, i = 1;
                str = str.replace(/%s/g, function () {
                    var arg = args[i++];
                    if (typeof arg === 'undefined') {
                        flag = false;
                        return '';
                    }
                    return arg;
                });
                return flag ? str : '';
            },
            // 指定随机数返回
            random: function (min, max) {
                return Math.floor((Math.random() * max) + min);
            },
            // 判断字符串是否是以start开头
            startWith: function(value, start) {
                var reg = new RegExp("^" + start);
                return reg.test(value)
            },
            // 判断字符串是否是以end结尾
            endWith: function(value, end) {
                var reg = new RegExp(end + "$");
                return reg.test(value)
            },
            // 数组去重
            uniqueFn: function(array) {
                var result = [];
                var hashObj = {};
                for (var i = 0; i < array.length; i++) {
                    if (!hashObj[array[i]]) {
                        hashObj[array[i]] = true;
                        result.push(array[i]);
                    }
                }
                return result;
            },
            // 数组中的所有元素放入一个字符串
            join: function(array, separator) {
            	if ($.common.isEmpty(array)) {
            	    return null;
            	}
                return array.join(separator);
            },
            // 获取form下所有的字段并转换为json对象
            formToJSON: function(formId) {
            	 var json = {};
                 $.each($("#" + formId).serializeArray(), function(i, field) {
                	 json[field.name] = field.value;
                 });
            	return json;
            }
        }
    });
})(jQuery);

/** 表格类型 */
table_type = {
    bootstrapTable: 0,
    bootstrapTreeTable: 1
};

/** 消息状态码 */
web_status = {
    SUCCESS: 0,
    FAIL: 500,
    WARNING: 301
};

/** 消息状态码 */
rpcresponse_status = {
	SUCCESS: 1,
	FAIL: 0,
	WARNING: 301
};

/** 弹窗状态码 */
modal_status = {
    SUCCESS: "success",
    FAIL: "error",
    WARNING: "warning"
};