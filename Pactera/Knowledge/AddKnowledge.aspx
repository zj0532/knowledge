<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddKnowledge.aspx.cs" Inherits="Pactera.Knowledge.AddKnowledge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>添加知识</title>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>
    <script type="text/javascript">

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
            $("#AddKnowledge").linkbutton({
                iconCls: "icon-add",
                onClick: function () {
                    var OneValue = $("#OneValue").combobox("getValue");
                    var TwoValue = $("#TwoValue").combobox("getValue");
                    var ThreeValue = $("#ThreeValue").combobox("getValue");
                    var FourValue = $("#FourValue").combobox("getValue");
                    var Solution = $("#Solution").val();
                    var MalDescription = $("#tMalDescription").val();
                    var MainReason = $("#MainReason").val();
                    var url = "/Handler/Knowledge/PublicKnow.ashx";
                    var Param = { action: "AddKnow", OneValue: OneValue, TwoValue: TwoValue, ThreeValue: ThreeValue, FourValue: FourValue, Solution: Solution, MalDescription: MalDescription, MainReason: MainReason };
                    $.post(url, Param, function (data) {
                        if (data["StateCode"] == 0) {
                            // 这里的状态码是自己在后台定义的，就统一0是成功，其他是失败
                            parent.$.messager.alert("提示", data["Message"], "info", function () {
                                // 这里的function是指点了Easyui对话框的确定时执行
                                parent.closeAndDestroyModalWindow(1);
                            });
                        } else {
                            // 如果没有操作成功，提示对应的错误信息，但是不关闭窗口
                            parent.$.messager.alert("提示", data["Message"], "info");
                        }
                    }, "json")
                }
            });
        });
        
    </script>
</head>
<body>
<div style="padding:10px;">
    <div style="margin-bottom:10px;">
        <span>Case类型：</span>
        <input class="easyui-combobox" id="OneValue" />
        <input class="easyui-combobox" id="TwoValue" />
        <input class="easyui-combobox" id="ThreeValue" />
        <input class="easyui-combobox" id="FourValue" />
    </div>
    <div style="margin-bottom:10px;"><span>故障描述：</span><textarea id="tMalDescription" cols="80" rows="4"></textarea></div>
    <div style="margin-bottom:10px;"><span>解决方案：</span><textarea id="Solution" cols="80" rows="4"></textarea></div>
    <div style="margin-bottom:10px;"><span>根本原因：</span><textarea id="MainReason" cols="80" rows="4"></textarea></div>
    <a id="AddKnowledge" class="linkbutton">添加</a>
</div>
</body>
</html>
