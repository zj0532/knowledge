<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddSMS.aspx.cs" Inherits="Pactera.SendMessage.AddSMS" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>短信发送功能</title>
    <link href="../Css/common.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    <script src="../Scripts/public.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#cboEngineer").combobox({
                valueField: "Id",
                textField: "Name",
                url: "/Handler/Email/PublicMessage.ashx?Action=GetEngineer",
                method: "post",
                panelHeight: "auto"

            });
            $("#senderMessage").linkbutton({
                iconCls: "icon-save",
                onClick: function () {
                    var cboValue = $("#cboEngineer").combobox("getValue");
                    var txtMessage = $("#txtContext").val();
                    var url = "/Handler/Email/PublicMessage.ashx";
                    var Param = { Action: "SenderMessage", Id: cboValue,txtContext:txtMessage};
                    $.post(url, Param, function (data) {
                        if (data["StateCode"] == 0) {
                            // 这里的状态码是自己在后台定义的，就统一0是成功，其他是失败
                            parent.$.messager.alert("提示", "操作成功", "info", function () {
                                // 这里的function是指点了Easyui对话框的确定时执行
                            });
                        } else {
                            // 如果没有操作成功，提示对应的错误信息，但是不关闭窗口
                            parent.$.messager.alert("提示", data["Message"], "info");
                        }
                    }, "json");
                }
            });
        });
    </script>
</head>
<body>
    <!-----------鼠标放再环节名称上要显示该处理人的相关信息---------->
    <div style="height: 10px;"></div>
    <div style="margin-left: 15px;">
        <table width="620" border="1" cellpadding="0" cellspacing="0" style="border-color: #ccc">
            <tr>
                <td style="text-align: right; width: 124px; height: 35px;">选择收件人:&nbsp;</td>
                <td>&nbsp;<input id="cboEngineer" class="easyui-combobox" /><a style="color: red"> &nbsp;&nbsp; （*一次只能给一个人发送）</a></td>
            </tr>
            <tr>
                <td style="text-align: right">短信内容:&nbsp;</td>
                <td>&nbsp;<textarea id="txtContext" rows="18" cols="75"></textarea></td>
            </tr>

        </table>
        <div style="height: 10px;"></div>
        <a class="linkbutton" id="senderMessage">发送</a>
    </div>
    <div style="height: 20px;"></div>

</body>

</html>
