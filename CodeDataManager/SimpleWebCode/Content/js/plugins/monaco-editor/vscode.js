var defaultLang = localStorage["vscode-lang"] || 'csharp',
    defaultContent = localStorage["vscode-content"] || 'console.log("Hello world!");',
    defaultTheme = localStorage["vscode-theme"] || 'vs';

require(['vs/editor/editor.main'], function () {
    var modesIds = monaco.languages.getLanguages().map(function (lang) { return lang.id }).sort();

    var te = $("#editor"), selang = $('#selanguage'),
        languagehtm = [];
    for (var i = 0; i < modesIds.length; i++) {
        var mo = modesIds[i];
        languagehtm.push('<option>' + mo + '</option>');
    }
    selang.children()[0].innerHTML = languagehtm.join('');

   
    editor = monaco.editor.create(te[0], {
        value: defaultContent,
        language: defaultLang,
        automaticLayout: true,
		wordWrap: "on",   //自动换行，注意大小写
		wrappingIndent: "indent",
        theme: defaultTheme,
        scrollbar: {
            verticalScrollbarSize: 6,
            horizontalScrollbarSize: 6
        },
        minimap: {
            _enabled: false
        }
    });

    console.log("finish init editor.");

    selang.change(function () {
        var oldModel = editor.getModel();
        var newModel = monaco.editor.createModel(editor.getValue(), this.value);
        editor.setModel(newModel);
        if (oldModel) {
            oldModel.dispose();
        }
        localStorage["vscode-lang"] = this.value;
    }).val(defaultLang);

    $('#setheme').change(function () {
        monaco.editor.setTheme(this.value);
        localStorage["vscode-theme"] = this.value;
    }).val(defaultTheme);

    editor.onDidChangeModelContent(function (e) {
        clearTimeout(window.defer1);
        window.defer1 = setTimeout(function () {
            localStorage["vscode-content"] = editor.getValue();
        }, 1000 * 1)
    });
});
 
 
$(window).resize(AutoHeight);


function setEditorLang(lang) { 
    console.log("select language:" + lang);
    var oldModel = editor.getModel();
    var newModel = monaco.editor.createModel(editor.getValue(), lang);
    editor.setModel(newModel);
    if (oldModel) {
        oldModel.dispose();
    }
    localStorage["vscode-lang"] = lang;
     
}




/*
 https://www.netnr.com/home/list/111

Monaco Editor
https://microsoft.github.io/monaco-editor/

非常强大的一款在线代码编辑器，vscode就是最好的证明

使用
<div id="editor"></div>
<link href="https://lib.baomitu.com/monaco-editor/0.17.0/min/vs/editor/editor.main.css" rel="stylesheet">

<script>
    var require = { paths: { 'vs': 'https://lib.baomitu.com/monaco-editor/0.17.0/min/vs' } };
</script>

<script src="https://lib.baomitu.com/monaco-editor/0.17.0/min/vs/loader.js"></script>

//载入Monaco
var editor = null;
require(['vs/editor/editor.main'], function () {
    //得到支持的语言
    var modesIds = monaco.languages.getLanguages().map(function (lang) { return lang.id }).sort();

    //创建编辑器
    editor = monaco.editor.create(document.getElementById("editor"), {
        //内容
        value: 'console.log("Hello world!");',
        //语言
        language: 'javascript',
        //自适应调整
        automaticLayout: true,
        //主题，三款：vs、vs-dark、hc-black
        theme: 'vs',
        //代码略缩图
        minimap: {
            enabled: false
        }
    });
});
常用的方法
//设置值
editor.setValue('console.log("Hello world!");');

//获取值
editor.getValue();

//获取选中的行信息
editor.getSelection();

//获取某一行的内容
editor.getModel().getLineContent(1);

//获取每一行的内容
editor.getModel().getLinesContent();

//内容改变事件
editor.onDidChangeModelContent(function(e){
　　console.log(e);
　　console.log(editor.getValue());
});

//添加按键监听
editor.addCommand(monaco.KeyMod.CtrlCmd | monaco.KeyCode.KEY_S, function () {
　　console.log('Ctrl + S 保存')
})

//设置主题
monaco.editor.setTheme('vs-dark');

//改变属性
editor.updateOptions({
　　//关闭行号
　　lineNumbers: "off"
});

//渲染代码得到HTML
monaco.editor.colorize('console.log("Hello world!");', 'javascript').then(function (data) {
    console.log(data);
});

//渲染节点代码
<pre id="code" data-lang="javascript" style="width:500px;">console.log("Hello world!");</pre>
monaco.editor.colorizeElement(document.getElementById('code'));
属性配置
https://microsoft.github.io/monaco-editor/api/interfaces/monaco.editor.ieditoroptions.html

注意
谷歌火狐支持优秀，IE11+
Monaco很多方法得到是一个Promise

 
 */