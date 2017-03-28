 <%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Role.aspx.cs" Inherits="Pactera.RolePermission.Role" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>角色管理</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script type="text/javascript">

        //设置按钮显示/隐藏
        var args = { "Action": "CurrentSigninUserHasPermission" };
        $(document).ready(function () {

            //添加角色JS
            // 根据权限设置按钮是否启用
            args["Permission"] = "Role_Add";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#addRole").linkbutton("enable");
            });

            //修改角色JS
            // 根据权限设置按钮是否启用
            args["Permission"] = "Role_Edit";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#editRole").linkbutton("enable");
            });

            //删除角色JS
            // 根据权限设置按钮是否启用
            args["Permission"] = "Role_Delete";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#deleteRole").linkbutton("enable");
            });

            //给角色分配权限JS
            //根据权限设置按钮是否启用
            args["Permission"] = "Role_Permission";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#linkSaveRolePermission").linkbutton("enable");
            });

        });

        var editRow = undefined;
        var toolbar = [{
            text: "添加角色",
            disabled: "true",
            id: "addRole",
            iconCls: "icon-add",
            handler: function () {
                if (editRow != undefined) {
                    return;
                }

                $("#RoleDatagrid").datagrid("insertRow", { index: 0, row: { IsEnable: "True" } });
                $("#RoleDatagrid").datagrid("beginEdit", 0);
                editRow = 0;
            }
        }, {
            text: "编辑角色",
            disabled: "true",
            id: "editRole",
            iconCls: "icon-edit",
            handler: function () {

                var row = $("#RoleDatagrid").datagrid("getSelected");
                if (editRow != undefined) {
                    // 执行相应的修改或添加
                    //updateOrInsertRole();
                    $("#RoleDatagrid").treegrid("select", editRow);
                    return;
                }

                if (row == null) return;

                var index = $("#RoleDatagrid").datagrid("getRowIndex", row);
                $("#RoleDatagrid").datagrid("beginEdit", index);
                $("#RoleDatagrid").datagrid("unselectAll");
                editRow = index;

            }
        }, {
            text: "删除角色",
            iconCls: "icon-remove",
            disabled: "true",
            id: "deleteRole",
            handler: function () {
                if (editRow != undefined) {
                    // 执行相应的修改或添加
                    //updateOrInsertRole();
                    return;
                }

                var row = $("#RoleDatagrid").datagrid("getSelected");
                if (row == null) return;

                parent.$.messager.confirm(
                    "确认操作",
                    "删除角色后拥有此角色的用户将失效，是否继续？",
                    function (r) {
                        if (!r) return;

                        var result = $.ajax({
                            type: "post",
                            async: false,
                            cache: false,
                            url: "/Handler/RolePermission.ashx?action=delete_role&role_id=" + row.Id
                        });

                        // 刷新表格
                        $("#RoleDatagrid").datagrid("reload");

                        parent.parent.$.messager.alert("提示", result.responseText, "info");
                    });

            }
        }, {
            text: "保存角色",
            iconCls: "icon-save",
            handler: function () {
                if (editRow != undefined) {
                    // 执行相应的修改或添加
                    updateOrInsertRole();
                }
            }
        }, {
            text: "撤销角色变更",
            iconCls: "icon-undo",
            handler: function () {
                editRow = undefined;
                $("#RoleDatagrid").datagrid("rejectChanges");
            }
        }];


        function updateOrInsertRole() {
            $("#RoleDatagrid").datagrid("endEdit", editRow);
            var rows = $("#RoleDatagrid").datagrid("getChanges");

            $("#RoleDatagrid").datagrid("acceptChanges");
            $("#RoleDatagrid").datagrid("selectRow", editRow);

            editRow = undefined;

            if (rows.length > 0 && rows[0].Id) {
                // 执行更新
                $.post("/Handler/RolePermission.ashx", { action: "update_role", role_id: rows[0].Id, role_name: rows[0].RoleName }, function (data) {
                    parent.parent.$.messager.alert("提示", data, "info");
                });
            } else if (rows.length > 0) {
                // 执行新增
                $.post("/Handler/RolePermission.ashx", { action: "insert_role", role_name: rows[0].RoleName }, function (data) {
                    parent.parent.$.messager.alert("提示", data, "info");
                });
            }

            // 刷新表格
            $("#RoleDatagrid").datagrid("reload");
        }


        $(document).ready(function () {
            $("#RoleDatagrid").datagrid({
                url: "/Handler/RolePermission.ashx?action=role_list",
                method: "get",
                border: false,
                singleSelect: true,
                fitColumns: true,
                width: 590,
                height: 500,
                toolbar: toolbar,
                columns: [[
                    { field: "RoleName", title: "角色", width: 200, editor: { type: "text", options: { required: true } } }
                    /*{
                        field: 'IsEnable', title: '是否启用', align: 'center',
                        formatter: function (value, row, index) {
                            if (row.IsEnable) return "是";
                            return "否";
                        }
                    }*/
                ]],
                onClickRow: function (rowIndex, rowData) {
                    $("#hiddenRoleId").val(rowData.Id);
                    $("#permissionTree").html("<li>读取中...</li>");
                    $("#permissionTree").tree({
                        url: "/Handler/Menu.ashx?action=permission_tree&role_id=" + rowData.Id,
                        //onlyLeafCheck: true,
                        animate: false,
                        checkbox: true
                    });
                    if (editRow != undefined) {
                        // 执行相应的修改或添加
                        //updateOrInsertRole();
                        return;
                    }
                },
                onAfterEdit: function (rowIndex, rowData, changes) {
                    editRow = undefined;
                },
                onDblClickRow: function (rowIndex, rowData) {
                    if (editRow != undefined) {
                        // 执行相应的修改或添加
                        //updateOrInsertRole();
                        return;
                    }

                    $("#RoleDatagrid").datagrid("beginEdit", rowIndex);
                    editRow = rowIndex;
                }
            });

            $("#RoleDatagrid").datagrid("autoSizeColumn");

            // DataTable总会把这个值默认设置，很奇怪，只好先在这里首次加载时重置下
            $("#hiddenRoleId").val("0");
        });
    </script>
</head>
<body>
    <input type="hidden" id="hiddenRoleId" value="-1" />
    <div id="content" style="height:500px;">
        <div id="divRoleDatagrid" style="float:left;height:500px;"><table id="RoleDatagrid"></table></div>
        <div id="divPermissionTree" style="width: 300px; height:500px; float: left; margin-left: 5px;">
            <div style="height:34px;">
                <a id="linkSaveRolePermission">保存权限</a>
            </div>
            <div style="width: 300px; height:466px; border: 1px solid #d0d0d0; overflow-y:scroll;">
                <ul id="permissionTree" class="easyui-tree"></ul>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        $("#linkSaveRolePermission").linkbutton({
            iconCls: "icon-save",
            disabled: "true",
            onClick: function () {
                // 执行修改权限
                var nodes = $("#permissionTree").tree("getChecked");
                var items = "";
                for (var i = 0; i < nodes.length; i++) {
                    if (nodes[i].Code == undefined) continue;
                    if (items != "") items += ",";
                    items += nodes[i].Code
                }
                var roleId = $("#hiddenRoleId").val();

                $.post("/Handler/RolePermission.ashx", { action: "save_permission", role_id: roleId, permission: items }, function (data) {
                    parent.parent.$.messager.alert("提示", data, "info");
                });
            }
        });
    </script>
</body>
</html>
