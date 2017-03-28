<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Easyui.aspx.cs" Inherits="Pactera.Easyui" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title></title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="Scripts/jquery-easyui-1.4.1/themes/gray/easyui.css" rel="stylesheet" />
    <link href="Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>

    <script type="text/javascript">

        // 包括这个ready不也是嘛jquery的ready再页面加载完成后你传了一个function，来执行你的代码，至于你在代码里写什么，和$(document).ready没关系
        $(document).ready(function () { 
            // 1. Json对象定义
            // 定义一个空对象
            var person = {};

            // 对象的属性在JavaScript里可以直接写，不用双引号也可以
            person = { Name: "张三", Age: 24, Say: function () { alert("我叫张三，我是个男人！"); }, IsMan: true };

            // 1.1 定义对象并直接给对象赋值
            // 这里就不用var了，因为上面已经定义了
            person = { "Name": "张三", "Age": 24, "Say": function () { alert("我叫张三，我是个男人！"); },IsMan:true };

            // 2. Json对象的使用
            // 给Json对象赋【字符串类型】值
            person["Name"] = "张三";
            // 给Json对象赋【数字类型】值
            person["Age"] = 24;
            // 给Json对象赋【function类型】值
            person["Say"] = function () {
                alert("我叫张三，我是个男人！");
            };
            // 给Json对象赋【布尔类型】值
            person["IsMan"] = true;

            // 可以直接访问对象的属性
            //console.info(person.Name);

            // 也可以通过 ["属性名"] 访问
            //console.info(person["Name"]);

            // 3. Easyui对象的创建规则
            //$("#指定的元素").combobox("这里需要传递一个对象，对象来声明这个下拉框的各各属性");

            // 3.1 原始（基础）形式
            var option = { iconCls: "icon-search", onClick: function () { alert("点击了按钮") } };
            $("#EasyuiLinkbutton").linkbutton(option);

            // 3.2 简化写法
            $("#EasyuiLinkbuttonA").linkbutton({ iconCls: "icon-search", onClick: function () { alert("点击了按钮") } });

            // 3.3 为了方便阅读和书写，每一行显示一个属性
            $("#EasyuiLinkbuttonB").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    // 就像这里，你写什么和这个button没什么关系，所以你想想你在这里写了个url，算这么回事呢？
                    alert("点击了按钮")
                }
            });
            
            // 看到了吧？就是这么一步一步来的

            // Easyui 三大类使用规则，我自己理解并记忆的，你自己选择一个方式记忆和理解也行

            // 一、初始化时
            // 初始化的时候都是调用控件的名称并传递一个初始化的对象，告诉Easyui你的这个控件需要如何创建（什么样子，做什么事）
            // 二、调用方法时
            // 调用方法时都是调用控件的名称并传递一个字符串（方法的名称），如果需要传递参数，就在后面在加参数，
            // 三、设置属性时
            // 设置属性时基本都是通过调用方法来设置控件的属性，来达到UI的显示效果。


            // 其他的我也没什么要说的了，也不知道再咋说了。你自己好好理解透了，再做程序把
            //怎么跟要高考似的呢
            //老师对成绩不好的同学说的话
        });
    </script>
</head>
<body>
    <a id="EasyuiLinkbutton">这是一个按钮初始化示例</a>
    <a id="EasyuiLinkbuttonA">这是一个按钮初始化示例</a>
    <a id="EasyuiLinkbuttonB">这是一个按钮初始化示例</a>
</body>
</html>
