<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SMSMessage.aspx.cs" Inherits="Pactera.SendMessage.SMSMessage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>短信发送管理功能</title>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    <script src="../Scripts/public.js"></script>
    <script type="text/javascript">
        var args = { "Action": "CurrentSigninUserHasPermission" };
        $(document).ready(function () {
            $("#tbMessage").datagrid({
                columns: [[
                    { field: "Sender", title: "发送工程师", width: "20%" },
                    { field: "Receiver", title: "接收工程师", width: "20%" },
                    { field: "MessageContent", title: "短信内容", width: "38%" },
                    { field: "SendingTime",title:"发送时间",width:"20%" }
                ]],
                toolbar: "#ToolBar",
                method: "post",
                url: "/Handler/Email/PublicMessage.ashx",
                pagination: true,
                rownumbers: true,
                pageNumber: 1,
                pageSize: 20,
                queryParams: { Action: "SMSList" },
                height:500

            });

            //////////删除客户//////
            $("#SMSDelte").linkbutton({
                iconCls: "icon-remove",
                disabled: true,
                onClick: function () {
                    var row = $("#tbMessage").datagrid("getSelected");
                    if (row == null) {
                        parent.$.messager.alert("提示", "请选择要删除的数据！", "info");
                        return;
                    }

                    parent.$.messager.confirm("警告", "删除后不可恢复，确认删除？", function (c) {
                        if (!c) return;
                        $.post("/Handler/Email/PublicMessage.ashx", { Action: "delete_SMS", Id: row.Id }, function (data) {
                            parent.$.messager.alert("提示", data, 'info');
                            $("#tbMessage").datagrid("load", { Action: "SMSList" });
                        });
                    });
                }
            });


            // 根据权限设置按钮是否启用
            args["Permission"] = "SMS_Delete";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#SMSDelte").linkbutton("enable");
            });
        });
    </script>

</head>
<body>
    <div id="ToolBar" style="padding:15px;">
        <a id="SMSDelte">删除</a>
    </div>
   <table id="tbMessage"></table>
</body>
</html>
