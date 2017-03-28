<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectProjectChanceWindow.aspx.cs" Inherits="Pactera.MarketInfo.SelectProjectChanceWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>选择建设单位</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />
    
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        var pkCorp = getQueryStringByName("pk_corp");

        $(document).ready(function () {
            $("#Datagrid").datagrid({
                url: "/Handler/PublicProjectChance.ashx",
                queryParams: { action: "select_project_chance_datagrid", pk_corp: pkCorp },
                idField: "Id",
                pagination: true,
                nowrap: true,
                singleSelect: true,
                autoRowHeight: true,
                rownumbers: true,
                width: 660,
                height: 312,
                columns: [[{
                    field: "VNAME",
                    title: "项目名称",
                    width: 620,
                    halign: "center",
                    formatter: function (value, row, index) {
                        return "<span title=" + value + ">" + value + "</span>";
                    }
                }]]
            });

            $("#LinkbtnConfirm").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    var row = $("#Datagrid").datagrid("getSelected");
                    if (row) {
                        parent.closeAndDestroyModalWindow(row);
                    } else {
                        parent.$.messager.alert("提示", "请选择项目！", "info");
                    }
                }
            });

            $("#LinkbtnCancel").linkbutton({
                iconCls: "icon-cancel",
                onClick: function () {
                    parent.closeAndDestroyModalWindow(null);
                }
            });

            $("#LinkbtnSearch").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    var serProjectName = $("#SerProjectName").textbox("getValue");
                    $("#Datagrid").datagrid("load", { action: "select_project_chance_datagrid", pk_corp: pkCorp, SerProjectName: serProjectName });
                }
            });

            $("#LinkbtnReset").linkbutton({
                iconCls: "icon-reset",
                onClick: function () {
                    $("#SerProjectName").textbox("clear");
                    $("#Datagrid").datagrid("load", { action: "select_project_chance_datagrid", pk_corp: pkCorp });
                }
            });
        });

    </script>
</head>
<body style="padding:0px 10px 0px 10px; margin:0px; background-color:#F7F9F8;">
    <div class="toubu">
        <p>查询条件</p>
    </div>
    <div class="easyui-panel" style="width: 100%; background-color: #fff;">
        <form id="sleownerform" method="post">
            <table class="form-search-table">
                <tr>
                    <th>项目名称：</th>
                    <td><input type="text" id="SerProjectName" class="easyui-textbox" /></td>
                    <td>
                        <a id="LinkbtnSearch" class="easyui-linkbutton linkbutton">查询</a>
                        <a id="LinkbtnReset" class="easyui-linkbutton linkbutton">重置</a>
                    </td>
                </tr>
            </table>
        </form>
    </div>
    <div class="toubu">
        <p>项目信息</p>
    </div>
    <table id="Datagrid"></table>
    <div style="text-align:right; padding:10px 0px 10px 0px;">
        <a id="LinkbtnConfirm" class="easyui-linkbutton linkbutton">确定</a>
        <a id="LinkbtnCancel" class="easyui-linkbutton linkbutton" style="margin-left: 10px;">取消</a>
    </div>
</body>
</html>
