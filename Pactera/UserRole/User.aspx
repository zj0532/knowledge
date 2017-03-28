<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="User.aspx.cs" Inherits="Pactera.UserRole.User" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head>

    <title>工程师管理</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            // DataTable总会把这个值默认设置，很奇怪，只好先在这里首次加载时重置下
            $("#hiddenRoleId").val("0");
        });

        function selectRole(userId, roleName, userName) {
            // 打开窗口
            var url = "/UserRole/UserRoleWindow.aspx?user_id=" + userId;
            parent.createAndOpenModalWindow("为【" + userName + "】分配角色", 300, 400, url, function (data) {
                $("#UserDatagrid").datagrid("reload");
            });
        }
    </script>
    <script type="text/javascript">
        var editRow = undefined;
        var args = { "Action": "CurrentSigninUserHasPermission" };

        function oneArea() {
            $("#oneArea").combobox({
                panelHeight: "auto",
                width: "120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=0",
                onSelect: function (record) {
                    twoArea(record["Id"]);
                    $("#twoArea").combobox("clear");
                    $("#threeArea").combobox("clear");
                }
            });
        }
        function twoArea(parentId) {
            $("#twoArea").combobox({
                panelHeight: "auto",
                width: "120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=" + parentId,
                onSelect: function (record) {
                    threeArea(record["Id"]);
                }
            });
        }
        function threeArea(parentId) {
            $("#threeArea").combobox({
                panelHeight: "auto",
                width: "120px",
                editable: false,
                valueField: "Id",
                textField: "Text",
                url: "/Handler/Area/PublicArea.ashx?Action=AreaJilian&ParentId=" + parentId
            });
        }
        $(document).ready(function () {
            oneArea();
            $("#twoArea").combobox({
                width: "120px",
            })
            $("#threeArea").combobox({
                width: "120px",
            })
            //添加工程师JS
            $("#linkbtnCreateUser").linkbutton({
                iconCls: "icon-add",
                disabled: true,
                onClick: function () {
                    // 打开窗口
                    var url = "/UserRole/UserWindow.aspx";
                    parent.createAndOpenModalWindow("新增工程师", 600, 500, url, function (data) {
                        if (data != null && data == 1) {
                            RefreshUserDatagrid();
                        }
                    });
                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "User_Insert";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#linkbtnCreateUser").linkbutton("enable");
            });

            //修改工程师js
            $("#linkbtnUpdateUser").linkbutton({
                iconCls: "icon-edit",
                disabled: true,
                onClick: function () {
                    var row = $("#UserDatagrid").datagrid("getSelected");
                    if (row == null) {
                        parent.$.messager.alert("提示", "请选择要更改的数据！", "info");
                        return;
                    }

                    // 打开修改窗口并传递参数
                    var url = "/UserRole/User_Update_Window.aspx";
                    parent.createAndOpenModalWindowWithData("修改工程师", 700, 500, url, row, function (data) {
                        if (data != null && data == 1) {
                            RefreshUserDatagrid();
                        }
                    });
                }
            });
            // 根据权限设置按钮是否启用
            args["Permission"] = "User_Update";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#linkbtnUpdateUser").linkbutton("enable");
            });

            //删除工程师JS
            $("#linkbtnDeleteUser").linkbutton({
                iconCls: "icon-remove",
                disabled: true,
                onClick: function () {
                    var row = $("#UserDatagrid").datagrid("getSelected");
                    if (row == null) {
                        parent.$.messager.alert("提示", "请选择要删除的数据！", "info");
                        return;
                    }

                    $.messager.confirm("警告", "删除后不可恢复，确认删除？", function (c) {
                        if (!c) return;
                        $.post("/Handler/PublicUser.ashx", { action: "delete_user", id: row.Id }, function (data) {
                            parent.$.messager.alert("提示", data, 'info');
                            RefreshUserDatagrid();
                        });
                    });
                }
            });
            //根据权限设置按钮是否启用
            args["Permission"] = "User_Delete";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#linkbtnDeleteUser").linkbutton("enable");
            });

            //查询JS
            $("#linkbtnSearchUser").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    RefreshUserDatagrid();
                }
            });
            function RefreshUserDatagrid() {
                var vUserName = $("#TxbUserName").textbox("getValue");
                var vName = $("#TxbName").textbox("getValue");
                var vRole = $("#TxbRole").textbox("getValue");
                var oneArea = $("#oneArea").textbox("getValue");
                var twoArea = $("#twoArea").textbox("getValue");
                var threeArea = $("#threeArea").textbox("getValue");
                $("#UserDatagrid").datagrid("load", { action: "get_user_list", user_name: vUserName, name: vName, role: vRole, oneArea: oneArea, twoArea: twoArea, threeArea: threeArea });
            }
            $("#linkbtnResetSearch").linkbutton({
                iconCls: "icon-reset",
                onClick: function () {
                    $("#TxbUserName").textbox("clear");
                    $("#TxbName").textbox("clear");
                    $("#TxbRole").textbox("clear");
                    // 刷新员工表
                    $("#UserDatagrid").datagrid("load", { action: "get_user_list" });
                }
            });

            //获取工程师列表JS
            $("#UserDatagrid").datagrid({
                url: "/Handler/PublicUser.ashx",
                queryParams: { action: "get_user_list", Permission: "User_List" },
                method: "post",
                height: 520,
                width: 1000,
                singleSelect: true,
                pagination: true,
                rownumbers: true,
                pageSize: 10,//每页显示的记录条数，默认为10
                toolbar: "#UserToolbar",
                columns: [[
                    { field: "UserName", title: "登录名", align: "center", width: '8%' },
                    { field: "Name", title: "姓名", align: "center", width: '8%' },
                    { field: "Phone", title: "联系方式（固话/手机）", align: "center", width: '14%' },
                    { field: "Email", title: "邮箱", align: "center", width: '20%' },
                    { field: "Address", title: "地址", align: "center", width: '28%' },
                    { field: "Enable", title: "工程师状态", align: "center", width: '8%' },
                    {
                        field: "RoleNames", title: "选择角色", align: "center", width: '13%',
                        formatter: function (value, row, index) {
                            var url = "javascript:void(0)";
                            var onclick = "javascript:selectRole('" + row.Id + "', '" + row.RoleNames + "', '" + row.Name + "')";
                            var tooltip = row.RoleNames;
                            var text = row.ShortRoleName;
                            if (row.RoleNames == "") text = "选择";
                            return "<a href=\"" + url + "\" onclick=\"" + onclick + "\" title=\"" + tooltip + "\">" + text + "</a>";
                        }
                    }
                ]]
            });

        });
    </script>

</head>

<body>
    <input type="hidden" id="hiddenUserId" value="-1" />
    <table id="UserDatagrid"></table>
    <div id="UserToolbar">
        <div style="padding: 5px; margin-top: 7px; margin-left: 15px;">
            登录名：<input type="text" id="TxbUserName" class="easyui-textbox" style="width: 130px;" />
            姓名：<input type="text" id="TxbName" class="easyui-textbox" style="width: 130px;" />
            角色：<input type="text" id="TxbRole" class="easyui-textbox" style="width: 130px;" />
            &nbsp;&nbsp;
            <span>区域：</span>
            <input class="easyui-combobox" id="oneArea" />&nbsp;
            <input class="easyui-combobox" id="twoArea" />&nbsp;
            <input class="easyui-combobox" id="threeArea" />
        </div>
        <div style="padding: 5px; margin-bottom: 8px; margin-left: 15px;">
            <a id="linkbtnCreateUser" class="linkbutton">添加</a>
            <a id="linkbtnDeleteUser" class="linkbutton">删除</a>
            <a id="linkbtnUpdateUser" class="linkbutton">修改</a>
            <a id="linkbtnSearchUser" class="linkbutton">查询</a>
            <a id="linkbtnResetSearch" class="linkbutton">重置</a>
        </div>
    </div>


</body>
</html>
