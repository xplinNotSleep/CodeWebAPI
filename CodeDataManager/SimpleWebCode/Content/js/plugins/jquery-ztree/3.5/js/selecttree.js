(function ($) {
    "use strict";
    //define some global dom
    var SelectTree = function (element, options) {
        this.options = options;
        this.$element = $(element);
        this.$containner = $('<div></div>');
        this.$ztree = $('<ul class="ztree"></ul>');
        this.$pInput = $('<input type="hidden"/>'); // 这里为隐藏域数据 id='nodes',选中的数都存储在这。
    }
    //prototype method
    SelectTree.prototype = {
        constructor: SelectTree,
        init: function () {
            var that = this,
                st = this.$element;
            // style
            that.$containner.css({ 'position': 'relative'});
            that.$ztree.css({
                'position': 'absolute',
                'z-index': 10000,
                'border-radius': '3px',
                'border': '1px #ccc solid',
                'overflow': 'auto',
                'background': '#fff',
                'width':'-webkit-fill-available'
            });

            that.$ztree.attr('id', 'selectTree-ztree');
            that.$ztree.css('display', 'none');
            that.$element.attr('readonly', 'readonly');

            if(that.$element.next().is(':hidden'))
            {
                that.$pInput=that.$element.next();
            }
            else
            {
                st.after(that.$pInput);
            }
            st.wrap(that.$containner);
            st.after(that.$ztree);

            that.$element.closest('body').bind('click', function (event) {
                var evt = event.srcElement ? event.srcElement : event.target;
                if(evt.classList.contains('selecttree'))
                {
                    that.$ztree.toggle();
                }
                else if(evt.id.indexOf('selectTree-ztree')==-1)
                {
                    that.$ztree.hide();
                }
            });

            //ztree
            //listener
            this.options.ztree.setting.callback = {
                onClick: function (event, treeId, treeNode) {
                    return that._onSelectTreeCheck(event, treeId, treeNode);
                }
            }
            $.fn.zTree.init(that.$ztree, this.options.ztree.setting, this.options.ztree.data);
        },
        _onSelectTreeCheck: function (event, treeId, treeNode) {
            var that = this;
            //获得所有选中节点
            var treeObj = $.fn.zTree.getZTreeObj(that.$ztree.attr('id'));
            if (treeObj) {
                var nodes = treeObj.getSelectedNodes();
                if(nodes.length==1)
                {
                    var text=nodes[0][that.options.ztree.setting.data.key.name];
                    var val=nodes[0][that.options.ztree.setting.data.simpleData.idKey];
                    // nodes数据 从这赋给隐藏域input(id='nodes')
                    that.$pInput.val(val);
                    that.$element.val(text);
                    that.$element.attr('title', text);
                    that.$ztree.hide();
                }
            }
        }
    }
    var common = {
        //这里组织默认参数，用户传过来的参数,ztree的一些固定参数
        _getSelectTreeOptions: function (options) {
            options = options ? options : {};
            if(options.addtop)
            {
                var topdata={};
                topdata[options.key]=0;
                topdata[options.text]=options.toptext?options.toptext:'无';
                options.data.push(topdata);
            }
            return {
                pIcon: options.pIcon || '',
                cIcon: options.cIcon || '',
                ztree: {
                    data: options.data,
                    setting: {
                        view: { selectedMulti: false },
                        data: {
                            key: {
                                name: options.text,
                                title: options.text
                            },
                            simpleData: {
                                enable: true,
                                idKey: options.key,
                                pIdKey: options.pkey,
                                rootPId: 'null'
                            }
                        }
                    }
                }
            }
        }
    }
    $.fn.selectTree = function (options) {
        var data = new SelectTree(this, common._getSelectTreeOptions(options));
        return data.init();
    }
    $.fn.selectTree.Constructor = SelectTree;
})(jQuery)
