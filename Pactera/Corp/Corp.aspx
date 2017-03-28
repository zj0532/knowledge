<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Corp.aspx.cs" Inherits="Pactera.Corp.Corp" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script type="text/javascript">

        var editRow = undefined;
        var idIndex = 10000;
        var toolbar = [{
            text: '添加',
            iconCls: 'icon-add',
            handler: function () {
                if (idIndex != 10000 || editRow != undefined) {
                    // 执行相应的修改或添加
                    //updateOrInsertRole();
                    return;
                }
                var node = $('#CorpGrid').treegrid('getSelected');
                if (node == undefined) {
                    parent.$.messager.alert("提示", "请先选择上级公司！", 'info');
                    return;
                }
                if ($("#CorpGrid").treegrid('getLevel', node.Id) == 4) {
                    parent.$.messager.alert("提示", "已经是最低一层公司，不能在其下再添加新公司！", 'info');
                    return;
                }
                $('#CorpGrid').treegrid("expandAll", node.Id);
                idIndex++;
                $("#hiddenCorpId").val(node.Id);
                var childNodeArr = $('#CorpGrid').treegrid('getChildren', node.Id);
                if (childNodeArr.length == 0) {
                    $('#CorpGrid').treegrid("append", {
                        parent: node.Id,
                        data: [{
                            Id: idIndex
                        }]
                    });

                }
                else {
                    $('#CorpGrid').treegrid("insert", {
                        before: childNodeArr[0].Id,
                        data: {
                            Id: idIndex
                        }
                    });
                }

                $("#CorpGrid").treegrid('select', idIndex)
                    .treegrid('beginEdit', idIndex);

            }
        }, {
            text: '编辑',
            iconCls: 'icon-edit',
            handler: function () {
                if (editRow != undefined) {
                    $('#CorpGrid').treegrid('select', editRow);
                    return;
                }
                if (idIndex == 10001) {
                    return;
                }
                var row = $("#CorpGrid").treegrid('getSelected');
                if (row == undefined) {
                    parent.$.messager.alert("提示", "请先选择要编辑的项！", 'info');
                    return;
                }
                if (row) {
                    editRow = row.Id;
                    $("#CorpGrid").treegrid('beginEdit', editRow);
                    //$("#CorpGrid").treegrid('unselectAll');
                }
            }
        }, {
            text: '保存',
            iconCls: 'icon-save',
            handler: function () {
                if (editRow != undefined || idIndex != 10000) {
                    // 执行相应的修改或添加
                    updateOrInsertRole();
                }
            }
        }, {
            text: '撤销',
            iconCls: 'icon-undo',
            handler: function () {
                if (editRow != undefined) {
                    $("#CorpGrid").treegrid("cancelEdit", editRow);
                    editRow = undefined;
                }
                if (idIndex > 10000) {
                    $("#CorpGrid").treegrid("cancelEdit", idIndex);
                    $("#CorpGrid").treegrid("remove", idIndex);
                    idIndex = 10000;
                }
            }
        }, {
            text: '删除',
            iconCls: 'icon-remove',
            handler: function () {
                var row = $("#CorpGrid").treegrid('getSelected');
                if (row == undefined) {
                    // 执行相应的修改或添加
                    //updateOrInsertRole();
                    parent.$.messager.alert("提示", "请先选择要删除的项！", 'info');
                    return;
                }


                if (row == null) return;

                if ($("#CorpGrid").treegrid('getChildren', row.Id) != "") {
                    parent.$.messager.alert("提示", "该公司下存有子公司不能进行删除！", 'info');
                    return;
                }
                parent.$.messager.confirm(
                    "确认操作",
                    "您是否确定要删除该公司？",
                    function (r) {
                        if (!r) return;

                        var result = $.ajax({
                            type: "post",
                            async: false,
                            cache: false,
                            url: "/Handler/PublicCorp.ashx?action=delete_corp&id=" + row.Id
                        });

                        // 刷新表格
                        $("#CorpGrid").treegrid('reload');

                        parent.parent.$.messager.alert("提示", result.responseText, 'info');
                    });

            }
        }];

        function updateOrInsertRole() {

            if (idIndex > 10000) {
                $("#CorpGrid").treegrid("select", idIndex);
                $("#CorpGrid").treegrid('endEdit', idIndex);
            } else {
                $("#CorpGrid").treegrid("select", editRow);
                $("#CorpGrid").treegrid('endEdit', editRow);
            }
            var rows = $("#CorpGrid").treegrid('getChanges');
            $("#CorpGrid").treegrid("acceptChanges");

            if (rows.length > 0 && idIndex == 10000) {
                // 执行更新
                editRow = undefined;
                $.post("/Handler/PublicCorp.ashx", { action: "update_corp", id: rows[0].Id, name: rows[0].Name, shortName: rows[0].ShortName }, function (data) {
                    parent.parent.$.messager.alert("提示", data, 'info');
                    $("#CorpGrid").treegrid("clearSelections");
                    $("#CorpGrid").treegrid('reload');
                });
            } else if (idIndex > 10000) {
                // 执行新增
                var fatherId = $('#hiddenCorpId').val();
                $.post("/Handler/PublicCorp.ashx", { action: "insert_corp", parentId: fatherId, name: rows[0].Name, shortName: rows[0].ShortName }, function (data) {
                    idIndex = 10000;
                    parent.parent.$.messager.alert("提示", data, 'info');
                    $("#CorpGrid").treegrid("clearSelections");
                    $("#CorpGrid").treegrid('reload');
                });
            }
            else { editRow = undefined; $("#CorpGrid").treegrid('reload'); }
        }


        $(document).ready(function () {
            $('#CorpGrid').treegrid({
                url: '/Handler/PublicCorp.ashx?action=corp_list',
                iconCls: 'icon-ok',
                rownumbers: true,
                idField: 'Id',
                treeField: 'Name',
                toolbar: toolbar,
                columns: [[
                    { field: 'Name', title: '公司全称', editor: 'text', halign: 'center', width: 350 },
                    { field: 'ShortName', title: '公司简称', editor: 'text', halign: 'center', editor: 'text', width: 305 },
                    { field: 'ShowOut', title: "级别信息", editor: 'text', halign: 'center', editor: 'text', width: 200 }
                ]]
            });
            $("#CorpGrid").treegrid("clearSelections");
            //$('.datagrid-header-rownumber').text('序号');
        });
    </script>
</head>
<body>
    <form method="post">
        <input class="easyui-textbox" type="text" name="Query" id="Query" style="width: 180px;" />
        <a class="easyui-linkbutton linkbutton" iconcls="icon-search" onclick="SearchGorp()">搜索</a>
    </form>
    <input type="hidden" id="hiddenCorpId" value="-1" />
    <table id="CorpGrid" style="width: 900px; height: 400px"></table>
    <script>
        var oldfindkey; //点点亮旧关键字
        var currLen = 0; //当前位置
        var array = new Array();
        var data;
        function SearchGorp() {
            var query = $('#Query').val();//获取关键字
            if (query != '') {
                if (oldfindkey == query && array.length > 0) {
                    if (currLen < array.length) {
                        $('#CorpGrid').treegrid('select', array[currLen++]);
                    } else {
                        currLen = 0;
                        $('#CorpGrid').treegrid('select', array[currLen++]);
                    }
                } else {
                    data = $('#CorpGrid').treegrid('getData');
                    array = new Array();
                    RecursiveText(data, query);
                    currLen = 0;
                    oldfindkey = query;
                    if (array.length > 0) {
                        $('#CorpGrid').treegrid('select', array[currLen++]);
                    }
                }
            }
        }
        function RecursiveText(data, text) {
            if (data.length == 0) {
                return;
            }
            for (var i = 0; i < data.length; i++) {
                if (data[i].Name.indexOf(text) > -1 || data[i].ShortName.indexOf(text) > -1) {
                    array.push(data[i].Id);
                }
                RecursiveText(data[i].children, text);
            }
        }
    </script>
</body>
</html>
