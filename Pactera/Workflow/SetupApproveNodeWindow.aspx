﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetupApproveNodeWindow.aspx.cs" Inherits="Pactera.Workflow.SetupApproveNodeWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>审批节点选项窗口</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    
    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
        $(document).ready(function () {
            var data = [
                { name: "步骤号码", width: 120, value: requestData.NodeNumber, editor: { type: "numberbox", "options": { "disabled": true } } },
                { name: "步骤名称", value: requestData.Display.Description, editor: { type: "text" } },
                {
                    field: "AutoSkip", name: "无审批人自动跳过", value: requestData.AutoSkip + "",
                    editor: {
                        type: "combobox",
                        options: {
                            data: [{ text: "允许", value: "true" }, { text: "不允许", value: "false" }],
                            panelHeight: "auto",
                            editable: false
                        }
                    }
                }
            ];

            $.extend($.fn.propertygrid.defaults.columns[0][1], {
                formatter: function (value, rowData, rowIndex) {
                    if (rowData.field == "AutoSkip") {
                        return value == "true" ? "允许" : "不允许";
                    } else {
                        return value;
                    }
                }
            });

            $("#StepOptionPropertygrid").propertygrid({
                data: data,
                border: false,
                showGroup: false,
                showHeader: false
            });

            //
            //var userId = getQueryStringByName("user_id");
            var userId = 5003;
            $("#StepRoleDatagrid").datagrid({
                idField: "Id",
                url: "/Handler/RolePermission.ashx",
                queryParams: { action: "role_list", user_id: userId },
                method: "post",
                border: false,
                singleSelect: true,
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
                                $("#StepRoleDatagrid").datagrid("checkRow", index);
                            }
                        });

                        $("#StepRoleDatagrid").datagrid("selectRecord", requestData.Action.Approve.Content);
                    }
                }
            });

            // 窗口 - 确认按钮
            $("#linkConfimation").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    // 获取选中的角色
                    var roleRow = $("#StepRoleDatagrid").datagrid("getSelected");
                    if (!roleRow) {
                        parent.$.messager.alert("提示", "请选择一个角色", "info");
                        return;
                    }

                    var rows = $("#StepOptionPropertygrid").propertygrid("getRows");
                    var responseData = {
                        Description: rows[1].value,
                        AutoSkip: rows[2].value == "true" ? true : false,
                        RoleIds: roleRow.Id
                    };

                    //requestData
                    parent.closeAndDestroyModalWindow(responseData);
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
<body style="padding:0px; margin:0px;">

    <table id="StepOptionPropertygrid"></table>
    <table id="StepRoleDatagrid"></table>
    
    <div style="text-align: right; padding: 5px 10px 0px 0px;">
        <a id="linkConfimation" class="linkbutton">确认</a>
        <a id="linkCancel" class="linkbutton">取消</a>
    </div>
</body>
</html>
