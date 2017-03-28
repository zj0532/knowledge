<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KnowList.aspx.cs" Inherits="Pactera.Knowledge.KnowList" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>知识库列表</title>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    <script src="../Scripts/public.js"></script>
    <style type="text/css">
    #KnowToolbar .know{line-height:40px;display:block;}
    </style>

    <script type="text/javascript">
        var args = { "Action": "CurrentSigninUserHasPermission" };
        function Knowledge(value) {
            var Param = { Id: value };
            var url = "/Knowledge/KnowLedgeDetail.aspx";
            parent.createAndOpenModalWindowWithData("查看详细内容", 800, 550, url, Param, function (data) {

            });
        }
        function getOneCaseType() {
            $("#OneValue").combobox({
                url: "/Handler/PublicCaseType.ashx?Action=GetCaseType&ParentId=0",
                editable: false,
                panelHeight: "auto",
                valueField: "Id",
                textField: "Text",
                onSelect: function (record) {
                    getTwoCaseType(record["Id"]);
                    $("#ThreeValue").combobox("clear");
                    $("#FourValue").combobox("clear");
                }
            });
        }

        function getTwoCaseType(parentId) {
            $("#TwoValue").combobox({
                url: "/Handler/PublicCaseType.ashx?Action=GetCaseType&ParentId=" + parentId,
                editable: false,
                panelHeight: "auto",
                valueField: "Id",
                textField: "Text",
                onSelect: function (record) {
                    getThreeCaseType(record["Id"]);
                    $("#FourValue").combobox("clear");
                }
            });
        }

        function getThreeCaseType(parentId) {
            $("#ThreeValue").combobox({
                url: "/Handler/PublicCaseType.ashx?Action=GetCaseType&ParentId=" + parentId,
                editable: false,
                panelHeight: "auto",
                valueField: "Id",
                textField: "Text",
                onSelect: function (record) {
                    getFourCaseType(record["Id"]);
                }
            });
        }

        function getFourCaseType(parentId) {
            $("#FourValue").combobox({
                url: "/Handler/PublicCaseType.ashx?Action=GetCaseType&ParentId=" + parentId,
                editable: false,
                panelHeight: "auto",
                valueField: "Id",
                textField: "Text"
            });
        }
        $(document).ready(function () {
            getOneCaseType();
            $("#KnowLedgeList").datagrid({
                url: "/Handler/Knowledge/PublicKnow.ashx",
                queryParams: { action: "getKnowlist" },
                method: "post",
                height: 520,
                singleSelect: true,
                pagination: true,
                rownumbers: true,
                pageSize: 12,
                toolbar: "#KnowToolbar",
                columns: [[
                    { field: "OneCaseType", title: "Case类型一级", width: "9%", align: "center" },
                    { field: "TwoCaseType", title: "Case类型二级", width: "9%", align: "center" },
                    { field: "ThreeCaseType", title: "Case类型三级", width: "9%", align: "center" },
                    { field: "FourCaseType", title: "Case类型四级", width: "9%", align: "center" },
                    { field: "MalDescription", title: "故障描述", width: "19%", align: "center" },
                    { field: "Solution", title: "解决方案", width: "19%", align: "center" },
                    { field: "Created", title: "创建人", width: "8%", align: "center" },
                    { field: "CreateDate", title: "创建时间", width: "12%", align: "center" },
                    {
                        field: "Id", title: "操作", width: "4%", align: "center", formatter: function (value, row, index) {
                            return "<a href=\"javascript:void(0)\" onClick=\"Knowledge(" + value + ")\" >查看</a>";
                        }
                    }
                ]]

            });

            $("#knowledgeAdd").linkbutton({
                iconCls: "icon-add",
                disabled: true,
                onClick: function () {
                    var url = "/Knowledge/AddKnowledge.aspx";
                    parent.createAndOpenModalWindow("添加知识点", "810", "630", url, function (data) {
                        if (data != null && data == 1) { $("#KnowLedgeList").datagrid("load", { Action: "getKnowlist" }); }

                    });
                }
            });


            // 根据权限设置按钮是否启用
            args["Permission"] = "Know_Add";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#knowledgeAdd").linkbutton("enable");
            });

            $("#knowledgeEdit").linkbutton({
                iconCls: "icon-edit",
                disabled: true,
                onClick: function () {
                    var row = $("#KnowLedgeList").datagrid("getSelected");
                    if (row == null) {
                        parent.$.messager.alert("提示", "请选择要更改的数据！", "info");
                        return;
                    }
                    var url = "/Knowledge/EditKnowledge.aspx";
                    parent.createAndOpenModalWindowWithData("修改知识点", 800, 460, url, row, function (data) {
                        btnSearch();
                    });
                }
            });

            // 根据权限设置按钮是否启用
            args["Permission"] = "Know_Edit";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#knowledgeEdit").linkbutton("enable");
            });

            $("#knowledgeDelete").linkbutton({
                iconCls: "icon-delete",
                disabled: true,
                onClick: function () {
                    var getSelected = $("#KnowLedgeList").datagrid("getSelected");
                    if (getSelected == null) {
                        parent.$.messager.alert("提示", "请选择要删除的数据！", "info");
                        return;
                    }
                    $.messager.confirm("警告", "删除后不可恢复，确认删除？", function (c) {
                        if (!c) return;
                        $.post("/Handler/Knowledge/PublicKnow.ashx", { action: "DeleteKnow", Id: getSelected.Id }, function (data) {
                            parent.$.messager.alert("提示", "删除成功！", 'info');
                            btnSearch();
                        });
                    });

                }
            });


            // 根据权限设置按钮是否启用
            args["Permission"] = "Know_Delete";
            $.post("/Handler/PublicPermission.ashx", args, function (data) {
                if (data == "0") $("#knowledgeDelete").linkbutton("enable");
            });


            //查询JS
            $("#btnSearch").linkbutton({
                iconCls: "icon-search",
                onClick: function () {
                    btnSearch();
                }
            });
        });

        function btnSearch() {
            var OneValue = $("#OneValue").combobox("getValue");
            var TwoValue = $("#TwoValue").combobox("getValue");
            var ThreeValue = $("#ThreeValue").combobox("getValue");
            var FourValue = $("#FourValue").combobox("getValue");
            var MalDescription = $("#MalDescription").val();
            var Created = $("#created").textbox("getValue");
            var CreateDateStart = $("#CreateDateStart").datetimebox("getValue");
            var CreateDateEnd = $("#CreateDateEnd").datetimebox("getValue");
            var url = "/Handler/Knowledge/PublicKnow.ashx";
            $("#KnowLedgeList").datagrid("load", { action: "getKnowlist", OneValue: OneValue, TwoValue: TwoValue, ThreeValue: ThreeValue, FourValue: FourValue, MalDescription: MalDescription, Created: Created, CreateDateStart: CreateDateStart, CreateDateEnd: CreateDateEnd });

        }
    </script>
</head>
<body>
    <div class="easyui-panel">
        <div id="KnowToolbar">
            <div class="know">
                <span class="spanwidth">Case类型:</span>
                <input class="easyui-combobox" id="OneValue" />
                <input class="easyui-combobox" id="TwoValue" />
                <input class="easyui-combobox" id="ThreeValue" />
                <input class="easyui-combobox" id="FourValue" /> <span class="spanwidth">创&nbsp;&nbsp;建&nbsp;&nbsp;人:&nbsp;</span><input class="easyui-textbox" id="created" /> &nbsp; 
           </div>
            <div style="clear:both;"></div>
            <div  class="know">
                <span class="spanwidth">故障描述:&nbsp;</span><input class="easyui-textbox" id="MalDescription" /> <a style="color:red;">（*模糊查询）</a>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; 
                <span class="spanwidth">创建时间（开始时间）:&nbsp;</span><input class="easyui-datetimebox" id="CreateDateStart" /> &nbsp; 
                <span class="spanwidth">创建时间（结束时间）:&nbsp;</span><input class="easyui-datetimebox" id="CreateDateEnd" /> &nbsp; 
               
            </div> 
            <div style="clear:both;"></div>
            <div style="height:10px;"></div>
            <a id="knowledgeAdd" class="linkbutton">添加</a>
            <a id="knowledgeEdit" class="linkbutton">修改</a>
            <a id="knowledgeDelete" class="linkbutton">删除</a>
            <a id="btnSearch" class="linkbutton">搜索</a>
        </div>
        <div style="clear:both;"></div>
        <div id="KnowLedgeList"></div>
    </div>
</body>
</html>
