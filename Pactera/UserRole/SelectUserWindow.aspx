<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectUserWindow.aspx.cs" Inherits="Pactera.UserRole.SelectUserWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>选择用户</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        var action = getQueryStringByName("action");

        $(document).ready(function () {
            $("#UserDatagrid").datagrid({
                url: "/Handler/PublicUser.ashx",
                queryParams: { action: "get_user_list", Permission: "User_List" },
                method: "post",
                height: 485,
                width: 600,
                singleSelect: true,
                pagination: true,
                rownumbers: true,
                fitColumns: true,
                pageSize: 15,//每页显示的记录条数，默认为10
                toolbar: "#Toolbar",
                columns: [[
                    { field: "Name", title: "姓名", align: "center", width: 220 }
                ]]
            });

            $("#LinkbtnConfirm").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    var row = $("#UserDatagrid").datagrid("getSelected");
                    if (row) {
                        parent.closeAndDestroyModalWindow(row);
                    } else {
                        parent.$.messager.alert("提示", "请选一个用户！", "info");
                    }
                }
            });

            $("#LinkbtnCancel").linkbutton({
                iconCls: "icon-cancel",
                onClick: function () {
                    parent.closeAndDestroyModalWindow(null);
                }
            });
            oneArea();
            $("#twoArea").combobox({
                width: "120px",
            })
            $("#threeArea").combobox({
                width: "120px",
            })
            $("#linksearch").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    var oneArea = $("#oneArea").textbox("getValue");
                    var twoArea = $("#twoArea").textbox("getValue");
                    var threeArea = $("#threeArea").textbox("getValue");
                    $("#UserDatagrid").datagrid("load", { action: "get_user_list", oneArea: oneArea, twoArea: twoArea, threeArea: threeArea });
                }

            });
        });

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
    </script>
</head>
<body style="padding:10px; margin:0px;">
    <div id="Toolbar" style="padding:5px;">
        <span style="width:40px;height:30px;line-height:30px;text-align:left;display:block;float:left;">区域：</span>
        <input class="easyui-combobox" id="oneArea" />&nbsp;
        <input class="easyui-combobox" id="twoArea" />&nbsp;
        <input class="easyui-combobox" id="threeArea" />
        <a id="linksearch" class="linkbutton">搜索</a>
    </div>
    <table id="UserDatagrid"></table>
    <div style="text-align:center; padding:10px 0px 10px 0px;">
        <a id="LinkbtnConfirm">确定</a>
        <a id="LinkbtnCancel" style="margin-left: 10px;">取消</a>
    </div>
</body>
</html>
