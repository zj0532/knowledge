<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dict.aspx.cs" Inherits="Pactera.Dict.Dict" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>枚举管理</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

</head>

<body>
    <input type="hidden" id="hiddenEnumId" value="-1" />
    <table id="EnumTreegrid" style="width: 900px; height: 400px"></table>
    <script type="text/javascript">
        var editRow = undefined;
        var toolbar = [{
            iconCls: "icon-add",
            text: "添加",
            handler: function () {
                if (editRow != undefined) return;

                var row = $("#EnumTreegrid").treegrid("getSelected");
                if (row == null || row.ParentValue != "0") {
                    parent.$.messager.alert("提示", "请先选择上级枚举！", "info");
                    return;
                }

                $("#EnumTreegrid").treegrid("expandAll", row.Id);

                //console.info(row);
                editRow = -1;
                $("#EnumTreegrid").treegrid("append", {
                    parent: row.Id,
                    data: [{
                        Id: editRow,
                        ParentValue: row.EnumValue,
                        _parentId: row.Id
                    }]
                });

                $("#EnumTreegrid").treegrid("beginEdit", editRow);
            }
        }, {
            text: "编辑",
            iconCls: "icon-edit",
            handler: function () {
                if (editRow != undefined) return;

                var row = $("#EnumTreegrid").treegrid('getSelected');
                if (row == null) {
                    parent.$.messager.alert("提示", "请先选择要编辑的项！", 'info');
                    return;
                }

                editRow = row.Id;
                $("#EnumTreegrid").treegrid("beginEdit", editRow);
            }
        }, {
            text: "保存",
            iconCls: "icon-save",
            handler: function () {
                if (editRow != undefined) {
                    // 执行相应的修改或添加
                    updateOrInsertRole();
                }
            }
        }, {
            text: "撤销",
            iconCls: "icon-undo",
            handler: function () {
                if (editRow != undefined) {
                    $("#EnumTreegrid").treegrid("cancelEdit", editRow);
                    editRow = undefined;
                    $("#EnumTreegrid").treegrid("reload");
                }
            }
        }, {
            text: "删除",
            iconCls: "icon-remove",
            handler: function () {
                var row = $("#EnumTreegrid").treegrid("getSelected");
                if (row == undefined) {
                    // 执行相应的修改或添加
                    //updateOrInsertRole();
                    parent.$.messager.alert("提示", "请先选择要删除的项！", "info");
                    return;
                }

                if (row == null) return;

                if ($("#EnumTreegrid").treegrid("getChildren", row.Id) != "") {
                    parent.$.messager.alert("提示", "该枚举下存有下级数据不能进行删除！", "info");
                    return;
                }

                parent.$.messager.confirm(
                    "确认操作",
                    "您是否确定要删除该枚举？",
                    function (r) {
                        if (!r) return;

                        var result = $.ajax({
                            type: "post",
                            async: false,
                            cache: false,
                            url: "/Handler/PublicDict.ashx?action=delete_enum&id=" + row.Id
                        });

                        // 刷新表格
                        $("#EnumTreegrid").treegrid("reload");

                        parent.parent.$.messager.alert("提示", result.responseText, "info");
                    });

            }
        }];

        function updateOrInsertRole() {

            $("#EnumTreegrid").datagrid("endEdit", editRow);

            // 取得变更的行
            var rows = $("#EnumTreegrid").treegrid("getChanges");
            //console.info(rows);
            //getChildren

            var isExists = false;
            var validRows = $("#EnumTreegrid").treegrid("getChildren", rows[0]._parentId);
            $(validRows).each(function () {
                if (this.Id != editRow && this.EnumText == rows[0].EnumText) {
                    isExists = true;
                    return false;
                }
            });
            if (isExists) {
                $("#EnumTreegrid").datagrid("beginEdit", editRow);
                parent.parent.$.messager.alert("提示", "枚举名称已存在！", 'info');
                return;
            }

            $("#EnumTreegrid").treegrid("acceptChanges");

            // 执行新增
            if (rows.length > 0 && rows[0].Id == -1) {
                $.post("/Handler/PublicDict.ashx", { action: "insert_enum", parent_value: rows[0].ParentValue, name: rows[0].EnumText, desc: rows[0].EnumDesc, order:rows[0].EnumOrder }, function (data) {
                    parent.parent.$.messager.alert("提示", data, 'info');
                    $("#EnumTreegrid").treegrid("clearSelections");
                    $("#EnumTreegrid").treegrid("reload");
                });
            }

            // 执行更新
            if (rows.length > 0 && rows[0].Id > 0) {
                $.post("/Handler/PublicDict.ashx", { action: "update_enum", id: rows[0].Id, name: rows[0].EnumText, desc: rows[0].EnumDesc, order: rows[0].EnumOrder }, function (data) {
                    parent.parent.$.messager.alert("提示", data, 'info');
                    $("#EnumTreegrid").treegrid("clearSelections");
                    $("#EnumTreegrid").treegrid("reload");
                });
            }

            editRow = undefined;
        }

        $(document).ready(function () {
            $("#EnumTreegrid").treegrid({
                url: "/Handler/PublicDict.ashx?action=enum_list",
                method: "get",
                iconCls: 'icon-ok',
                rownumbers: true,
                minHeight: 400,
                idField: "Id",
                nowrap: false,
                treeField: "EnumText",
                toolbar: toolbar,
                columns: [[
                    { field: "EnumText", title: "名称", editor: "text", halign: "center", width: 400 },
                    { field: "EnumDesc", title: "描述", editor: 'text', width: 300 },
                    { field: "EnumOrder", title: "排序", editor: "numberbox", width: 150 }
                ]]
            });
            $("#EnumTreegrid").treegrid("clearSelections");
        })

    </script>
</body>
</html>
