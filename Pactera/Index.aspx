<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Pactera.Index" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>知识库管理系统</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta http-equiv="X-UA-Compatible" content="IE=8" />

    <script type="text/javascript">
        //存储打开的多个模态窗口
        var $frameModalWindows = [];
        var $frameModalWindowsRequestData = [];
    </script>

    <link href="Scripts/jquery-easyui-1.4.1/themes/default/easyui.css" rel="stylesheet" />
    <link href="Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <link href="css/common.css" rel="stylesheet" />
    <link href="css/default.css" rel="stylesheet" />

    <script src="Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script src="Scripts/public.js"></script>

    <%--<script src="Scripts/onmove.js"></script>--%>
    <script type="text/javascript">

        function SignOut() {
            $.messager.confirm('系统提示', '您确定要退出本次登录吗?', function (r) {
                if (r) {
                    $.ajax({
                        url: "/Handler/PublicUser.ashx?action=safe_quit",
                        async: false,
                        dataType: 'json',
                        success: function (r) {
                            window.location.href = '/login.aspx';
                        }
                    });
                }
            });
        }

        // 创建并打开模态窗口
        function createAndOpenModalWindow(winTitle, winWidth, winHeight, url, winCallback) {
            var iframeWidth = winWidth - 15;
            var iframeHeight = winHeight - 45;
            var iframeStyle = 'style="border:none; width: ' + iframeWidth + 'px; height: ' + iframeHeight + 'px; margin: 0px 0px -7px 0px; padding: 0px;"';
            var windowContentIframe = '<iframe src="' + url + '" ' + iframeStyle + ' border="0" frameborder="0"></iframe>';
            var newModalWindow = $("<div>" + windowContentIframe + "</div>").appendTo(document.body).dialog({
                title: winTitle,
                width: winWidth,
                height: winHeight,
                //top: 105,
                modal: true,
                collapsible: false,
                minimizable: false,
                maximizable: false,
                resizable: false,
                //iconCls: winIconCls || void (0),
                onClose: function () {
                    var $win = $frameModalWindows.pop();
                    winCallback && winCallback($win.data("callbackData"));
                    $win.window("destroy");
                }
            });
            //}).window("center");

            $frameModalWindows.push(newModalWindow);
            //newModalWindow.dialog('dragInWindow');//控制窗口不能拖出页面
        }

        // 创建并打开模态窗口并且传递对象
        function createAndOpenModalWindowWithData(winTitle, winWidth, winHeight, url, data, winCallback) {
            var iframeWidth = winWidth - 15;
            var iframeHeight = winHeight - 45;

            var iframeStyle = 'style="border:none; width: ' + iframeWidth + 'px; height: ' + iframeHeight + 'px; margin: 0px 0px -7px 0px; padding: 0px;"';
            var iframeName = "Iframe_" + parseInt(Math.random() * 1000000);
            var formName = "Form_" + parseInt(Math.random() * 1000000);
            var windowContentIframe = '<iframe name="' + iframeName + '" ' + iframeStyle + ' border="0" frameborder="0"></iframe>';
            var windowContentFormData = '<input type="hidden" name="HiddenIframeRequestDataJson" />';
            var windowContentForm = '<form name="' + formName + '" method="post" action="' + url + '" target="' + iframeName + '">' + windowContentFormData + '</form>';

            var newModalWindow = $("<div>" + windowContentIframe + windowContentForm + "</div>").appendTo(document.body).dialog({
                title: winTitle,
                width: winWidth,
                height: winHeight,
                //top: 105,
                modal: true,
                collapsible: false,
                minimizable: false,
                maximizable: false,
                resizable: false,
                //iconCls: winIconCls || void (0),
                onClose: function () {
                    var $win = $frameModalWindows.pop();
                    winCallback && winCallback($win.data("callbackData"));
                    $win.window("destroy");
                }
            });

            // 设置值，提交From
            $("input[name='HiddenIframeRequestDataJson']").val(JSON.stringify(data));
            $("form[name='" + formName + "']").submit();
            //}).window("center");

            $frameModalWindows.push(newModalWindow);
            //newModalWindow.dialog('dragInWindow');//控制窗口不能拖出页面
        }

        // 关闭并销毁模态窗口
        function closeAndDestroyModalWindow(data) {
            var $win = $frameModalWindows[$frameModalWindows.length - 1];
            $win.data("callbackData", data);
            $win.window("close");
        }

        // 弹出窗口和对话框
        function openBaseWindowOrDialog(windowName, innerHtml) {
            //$("#" + windowName).html(innerHtml);
            $("#" + windowName).window('open');
        }
        // 关闭窗口和对话框<a href="Properties/">Properties/</a>
        function closeBaseWindowOrDialog(windowName) {
            $("#" + windowName).window('close');
        }
        function getJquery() {
            return jQuery;
        }
        // 弹出对话框
        function getMessager() {
            return $.messager;
        }

        $(document).ready(function () {

            $("#indexTabs").tabs({
                width: '100%',
                fit: true,
                border: false,
                tabHeight: 30
            });

        });

        // 当前用户修改自己信息
        function UpdateUserInfoWindow(){
            var url = "/UserRole/User_Update_Window.aspx";
            // 你不会也不能瞎写啊。你这样写的结果是什么你知道吗？要命吗
            var row = <%=CurrentSigninUser%>;

            createAndOpenModalWindowWithData("修改工程师", 700, 500, url, row, function (data) {
                
            });
        }

        function iFrameHeight(iframe) {
            //var ifm = document.getElementsByTagName("iframe");
            /*$("iframe").each(function (index) {
                //console.info($(this).contents().height());
                $(this).height = this.body.scrollHeight;
            });*/
            //console.info($(iframe).contents().find("body").height());
            $(iframe).height($(iframe).contents().find("body").height() + 20);
            /*var subWeb = document.frames ? document.frames["iframepage"].document : ifm.contentDocument;
            if (ifm != null && subWeb != null) {
                ifm.height = subWeb.body.scrollHeight;
            }
            setTimeout(iFrameHeight, 5000);
            //console.info(ifm.length);*/
        }

        // 关闭tab页
        function closeTab(subtitle) {
            if ($('#indexTabs').tabs('exists', subtitle)) {
                parent.$('#indexTabs').tabs('close', subtitle);
            }
        }

        // 添加tab页
        function addTab(subtitle, url) {
            if (!$('#indexTabs').tabs('exists', subtitle)) {
                $('#indexTabs').tabs('add', {
                    title: subtitle,
                    content: createFrame(url),
                    closable: true
                });
                // 给新增的选项卡增加颜色
                $("#indexTabs").find(".panel-body").css("background-color", "#F7F9F8");
            } else {
                $('#indexTabs').tabs('select', subtitle);
            }
        }

        //创建iframe标签
        function createFrame(url) {
            var frame = '<iframe scrolling="no" frameborder="0"  src="' + url + '" style="width:100%;height:450px;" onload=\"iFrameHeight(this);\"></iframe>';
            return frame;
        }


        //动态时间
        $(function () {
            setInterval("GetTime()", 1000);
        });

        function Appendzero(obj) {
            if (obj < 10) return "0" + "" + obj;
            else return obj;

        }

        function GetTime() {
            var mon, day, now, hour, min, ampm, time, str, tz, end, beg, sec;
            /*
            mon = new Array("Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug",
                    "Sep", "Oct", "Nov", "Dec");
            */
            mon = new Array("01", "02", "03", "04", "05", "06", "07", "08",
                    "09", "10", "11", "12");
            /*
            day = new Array("Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat");
            */
            day = new Array("周日", "周一", "周二", "周三", "周四", "周五", "周六");
            now = new Date();
            hour = now.getHours();
            min = now.getMinutes();
            sec = now.getSeconds();
            if (hour < 10) {
                hour = "0" + hour;
            }
            if (min < 10) {
                min = "0" + min;
            }
            if (sec < 10) {
                sec = "0" + sec;
            }
            $("#Timer").html(
                    "<nobr>" + day[now.getDay()] + "    " + now.getFullYear() + "-"
                            + mon[now.getMonth()] + "-" + Appendzero(now.getDate()) + " " + hour
                            + ":" + min + ":" + sec + "</nobr>");

        }

    </script>
</head>
<body class="easyui-layout">
    <div data-options="region:'north',split:true" class="header">
        联通海尔项目组统知识库系统
        <span class="head">欢迎，<a href="javascript:void(0);" onclick="UpdateUserInfoWindow()"><%=CurrentSigninUser.Name%></a>
            &nbsp;&nbsp;&nbsp;&nbsp; <span id="Timer"></span>&nbsp;&nbsp;
            <span class="button-exit" onclick="SignOut()" style="color: #ffffff; cursor: pointer">退出系统</span>&nbsp;&nbsp;
        </span>
    </div>
    <div data-options="region:'west',split:true" style="width: 200px;">
        <div style="width: 100%;" data-options="border:false,fit:true" class="easyui-accordion">
            <%=GenerateMenu()%>
        </div>
    </div>
    <div data-options="region:'center'">
        <div id="indexTabs" class="easyui-tabs">
            <div title="工作台">
                <iframe id="frameContent" scrolling="aoto" style="width:100%;height:530px;" frameborder="0" src="Welcome.aspx" onloadeddata="iFrameHeight()"></iframe>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function addPanel() {
            var region = $('#region').val();
            var options = {
                region: region
            };
            if (region == 'north' || region == 'south') {
                options.height = 50;
            } else {
                options.width = 100;
                options.split = true;
                options.title = $('#region option:selected').text();
            }
            $('#cc').layout('add', options);
        }
        function removePanel() {
            $('#cc').layout('remove', $('#region').val());
        }
    </script>
</body>
</html>
