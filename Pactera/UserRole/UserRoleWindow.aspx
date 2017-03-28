<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UserRoleWindow.aspx.cs" Inherits="Pactera.UserRole.UserRoleWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>给用户分配角色</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script src="../Scripts/public.js"></script>

</head>
<body>
    
    <div id="winRoleSelect" style="width: 280px">
        <table id="RoleDatagrid"></table>
        <div style="text-align:right; padding:10px;">
            <a id="linkbtnConfirm" class="linkbutton">确认</a>
            <a id="linkbtnCancel" class="linkbutton" style="margin-left:10px;">取消</a>
        </div>
    </div>
    <script type="text/javascript">
        var args = { "Action": "CurrentSigninUserHasPermission" };

        var userId = getQueryStringByName("user_id");
        $("#RoleDatagrid").datagrid({
            url: "/Handler/RolePermission.ashx",
            queryParams: { action: "role_list", user_id: userId },
            method: "post",
            border: false,
            singleSelect: false,
            fitColumns: true,
            height: 290,
            selectOnCheck: true,
            checkOnSelect: true,
            columns: [[
                { field: "checked", checkbox: true },
                { field: "RoleName", title: "角色", width: 240 }
            ]],
            onLoadSuccess: function (data) {
                if (data) {
                    $.each(data.rows, function (index, item) {
                        if (item.checked) {
                            $("#RoleDatagrid").datagrid("checkRow", index);
                        }
                    });
                }
            }
        });

        $("#linkbtnConfirm").linkbutton({
            iconCls: "icon-ok",
            disabled: "true",
            onClick: function () {
                var row = $("#RoleDatagrid").datagrid("getSelections");

                /* 如果不选中任何，则清除用户的所有角色
                if (row == null || row.length < 1) {
                    parent.$.messager.alert("提示", "请选择要分配的角色。", "info");
                    return;
                }*/

                var ids = "";
                $(row).each(function () {
                    if (ids != "") ids += ",";
                    ids += $(this).attr("Id");
                });

                $.post("/Handler/PublicUser.ashx", { action: "update_role", user_id: userId, role_ids: ids }, function (data) {
                    parent.$.messager.alert("提示", data, "info");
                    parent.closeAndDestroyModalWindow(1);
                });
            }
        });


        // 根据权限设置按钮是否启用
        args["Permission"] = "User_SelectRole";
        $.post("/Handler/PublicPermission.ashx", args, function (data) {
            if (data == "0") $("#linkbtnConfirm").linkbutton("enable");
        });

        $("#linkbtnCancel").linkbutton({
            iconCls: "icon-cancel",
            onClick: function () {
                parent.closeAndDestroyModalWindow(0);
            }
        });
    </script>
</body>
</html>
