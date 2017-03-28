<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectPersonWindow.aspx.cs" Inherits="Pactera.Workflow.SelectPersonWindow" %>

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

    <script type="text/javascript">

        var single = getQueryStringByName("single_select");
        var roleId = getQueryStringByName("role_id");
        var pkCorp = getQueryStringByName("pk_corp");
        var isSingleSelect = single == "1" ? true : false;

        $(document).ready(function () {
            $("#UserDatagrid").datagrid({
                url: "/Handler/PublicUser.ashx",
                queryParams: { action: "get_approve_user_list_json", pk_corp: pkCorp, role_id: roleId },
                singleSelect: isSingleSelect,
                border: false,
                height: 310,
                width: 500,
                columns: [[
                    { field: "Name", title: "姓名", width: 180, align: "center" },
                    { field: "RoleName", title: "角色", width: 300, align: "center" }
                ]]
            });

            // 窗口 - 确认按钮
            $("#linkConfimation").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    var rows = $("#UserDatagrid").datagrid("getRows");
                    if (rows.length < 1) {
                        parent.$.messager.alert("提示", "没有可用的用户", "info", function () {
                            parent.closeAndDestroyModalWindow(null);
                        });
                        return;
                    }

                    rows = $("#UserDatagrid").datagrid("getSelections");
                    if (rows.length < 1) {
                        parent.$.messager.alert("提示", "请选择要添加的用户", "info");
                        return;
                    }

                    parent.closeAndDestroyModalWindow(rows);
                }
            });

            // 窗口 - 取消按钮
            $("#linkCancel").linkbutton({
                iconCls: "icon-cancel",
                onClick: function () {
                    parent.closeAndDestroyModalWindow(null);
                }
            });
        });
    </script>
</head>
<body>
    <!-- 屏蔽搜索，应该用不到，因为搜索后无法同时选择多个人
    <div style="padding: 5px;">
        用户名：<input type="text" class="easyui-textbox" id="searchEmployeeUserName" style="width: 100px;" />&nbsp;&nbsp;
        姓名：<input type="text" class="easyui-textbox" id="searchEmployeeName" style="width: 100px;" />
        <a id="linkSearchEmployee" href="javascript:void(0)" class="linkbutton" style="margin-left:5px;">查询</a>
        <a id="linkResetEmployee" href="javascript:void(0)" class="linkbutton" style="margin-left:5px;">重置</a>
    </div>-->
    <table id="UserDatagrid"></table>
    <div style="text-align: right; padding: 10px 0px 0px 0px;">
        <a id="linkConfimation" class="linkbutton">确认</a>
        <a id="linkCancel" class="linkbutton">取消</a>
    </div>

    <script type="text/javascript">
        /* 屏蔽搜索，应该用不到，因为搜索后无法同时选择多个人
        $("#linkSearchEmployee").linkbutton({
            iconCls: "icon-search",
            onClick: function () {
                var userName = $("#searchEmployeeUserName").val();
                var name = $("#searchEmployeeName").val();
                $("#EmployeeGrid").datagrid("reload", { action: "get_employee_list", user_name: userName, name: name });
            }
        });
        $("#linkResetEmployee").linkbutton({
            iconCls: "icon-reset",
            onClick: function () {
                $("#searchEmployeeUserName").textbox("clear");
                $("#searchEmployeeName").textbox("clear");
                $("#EmployeeGrid").datagrid("reload", { action: "get_employee_list" });
            }
        });*/
    </script>
</body>
</html>
