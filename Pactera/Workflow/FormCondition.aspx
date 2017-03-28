<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FormCondition.aspx.cs" Inherits="Pactera.Workflow.FormCondition" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>nex</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="/Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="/Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="/Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        $(document).ready(function () {
            var toolbar = [{
                text: "添加",
                iconCls: "icon-add",
                handler: function () {
                    var rowCount = $("#FormConditionDatagrid").datagrid("getRows");
                    var conditionStr = "";
                    if (rowCount.length > 0) conditionStr = "并且";
                    $("#FormConditionDatagrid").datagrid("insertRow", {
                        index: rowCount.length + 1,
                        row: {
                            Condition: conditionStr,
                            Field: "工程造价",
                            Operator: "4",
                            Value: "5000",
                            Option: "删除"
                        }
                    });
                    $("#FormConditionDatagrid").datagrid("beginEdit", rowCount.length - 1);
                }
            }, {
                text: "编辑",
                iconCls: "icon-edit",
                handler: function () {
                }
            }, {
                text: "保存",
                iconCls: "icon-save",
                handler: function () {

                }
            }, {
                text: "撤销",
                iconCls: "icon-undo",
                handler: function () {
                    
                }
            }];

            var conditionArr = [
                { Text: "并且", Value: "1" },
                { Text: "或者", Value: "2" }
            ];
            var fields = [
                { Text: "工程造价", Value: "1" },
                { Text: "竞争者", Value: "2" }
            ];
            var operators = [
                { Text: "大于", Value: "1" },
                { Text: "大于等于", Value: "2" },
                { Text: "小于", Value: "3" },
                { Text: "小于等于", Value: "4" },
                { Text: "等于", Value: "5" },
                { Text: "不等于", Value: "6" }
            ];

            $("#FormConditionDatagrid").datagrid({
                showHeader: false,
                toolbar: toolbar,
                singleSelect: true,
                height: 400,
                width: 600,
                columns: [[
                    {
                        field: "Condition", title: "条件", width: 80, halign: "center", align: "center", editor: {
                            type: "combobox",
                            options: { data: conditionArr, panelHeight: "auto", valueField: "Value", textField: "Text" }
                        }
                    },
                    {
                        field: "Field", title: "表单字段", width: 150, editor: {
                            type: "combobox",
                            options: { data: fields, panelHeight: "auto", valueField: "Value", textField: "Text" }
                        }
                    },
                    {
                        field: "Operator", title: "比较运算符", width: 90, align: "center", editor: {
                            type: "combobox",
                            options: { data: operators, panelHeight: "auto", valueField: "Value", textField: "Text" }
                        }
                    },
                    { field: "Value", title: "值", width: 150, align: "center", editor: { type: "text", options: { required: true } } },
                    { field: "Option", title: "Option", width: 50, align: "center" }
                ]]
            });
        });
    </script>
</head>
<body>
    <table id="FormConditionDatagrid"></table>
</body>
</html>
