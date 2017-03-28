<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StepCounterSignOptionsWindow.aspx.cs" Inherits="Pactera.Workflow.StepCounterSignOptionsWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>会签步骤选项窗口</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    
    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        var requestData = parent.$frameModalWindowsRequestData;
        //console.info(requestData);

        $(document).ready(function () {
            /*
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
            });*/

            // 窗口 - 确认按钮
            $("#linkConfimation").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
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
    <table id="UserDatagrid"></table>
    <div style="text-align: right; padding: 10px 0px 0px 0px;">
        <a id="linkConfimation" class="linkbutton">确认</a>
        <a id="linkCancel" class="linkbutton">取消</a>
    </div>
</body>
</html>
