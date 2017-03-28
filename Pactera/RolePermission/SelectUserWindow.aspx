<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectUserWindow.aspx.cs" Inherits="Pactera.RolePermission.SelectUserWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>选择用户</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script src="../Scripts/public.js"></script>
    <script src="../Scripts/json2.js"></script>

</head>
<body style="padding:0px; margin:0px;">
    <div style="padding: 5px;">
        登录名：<input type="text" id="TxbUserName" class="easyui-textbox" style="width: 130px;" />&nbsp;&nbsp;
        姓名：<input type="text" id="TxbName" class="easyui-textbox" style="width: 130px;" />
        <a id="linkbtnSearchUser" class="linkbutton">查询</a>
        <a id="linkbtnResetSearch" class="linkbutton">重置</a>
    </div>
    <table id="UserDatagrid"></table>
    <div style="text-align: right; padding: 10px 10px 0px 0px;">
        <a id="linkConfimation" class="linkbutton">确认</a>
        <a id="linkCancel" class="linkbutton">取消</a>
    </div>

    <script type="text/javascript">
        var roleId = getQueryStringByName("role_id");

        $(document).ready(function () {
            $("#linkConfimation").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    // 执行更新
                    var rows = $("#UserDatagrid").datagrid("getSelections");
                    if (rows.length < 1) {
                        parent.$.messager.alert("提示", "请选择要授权的用户！", "info");
                        return;
                    }
                    var requestData = { RoleId: roleId, UserIds: [] };
                    requestData.RoleId = roleId;
                    $.each(rows, function (i, n) {
                        requestData.UserIds.push({ Id: n.Id });
                    });

                    $.post("/Handler/RolePermission.ashx", { action: "insert_role_users", data: JSON.stringify(requestData) }, function (data) {
                        parent.parent.$.messager.alert("提示", data, "info", function () {
                            // 关闭窗口
                            parent.closeAndDestroyModalWindow(1);
                        });
                    });
                }
            });
            $("#linkCancel").linkbutton({
                iconCls: "icon-cancel",
                onClick: function () {
                    parent.closeAndDestroyModalWindow(1);
                }
            });

            $("#UserDatagrid").datagrid({
                url: "/Handler/PublicUser.ashx",
                queryParams: { action: "get_simple_user_list" },
                method: "post",
                height: 310,
                width: 550,
                fitColumns: true,
                selectOnCheck: true,
                checkOnSelect: true,
                pagination: true,
                pageSize: 10,//每页显示的记录条数，默认为10
                toolbar: "#UserToolbar",
                columns: [[
                    { field: "checked", checkbox: true },
                    { field: "UserName", title: "登录名", align: "center", width: 130 },
                    { field: "Name", title: "姓名", align: "center", width: 90 }
                ]]
            });

            $("#linkbtnSearchUser").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    var vUserName = $("#TxbUserName").textbox("getValue");
                    var vName = $("#TxbName").textbox("getValue");
                    $("#UserDatagrid").datagrid("load", { action: "get_simple_user_list", user_name: vUserName, name: vName });
                }
            });
            $("#linkbtnResetSearch").linkbutton({
                iconCls: "icon-reset",
                onClick: function () {
                    $("#TxbUserName").textbox("clear");
                    $("#TxbName").textbox("clear");
                    // 刷新员工表
                    $("#UserDatagrid").datagrid("load", { action: "get_user_list" });
                }
            });
        });
    </script>
</body>
</html>
