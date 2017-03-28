<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditKnowledge.aspx.cs" Inherits="Pactera.Knowledge.EditKnowledge" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>修改知识点</title>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js"></script>

    <script type="text/javascript">
        
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>
        //console.info(requestData);

        //requestData中包含了1、2、3、4级的值吗恩，包含

        function getOneCaseType() {
            $("#OneValue").combobox({
                url: "/Handler/PublicCaseType.ashx?Action=GetCaseType&ParentId=0",
                editable: false,
                panelHeight: "auto",
                valueField: "Id",
                textField: "Text",
                onLoadSuccess:function(){
                    
                    if(requestData && requestData["OneValue"]){
                        $("#OneValue").combobox("select",requestData["OneValue"]);
                        delete requestData["OneValue"];
                    }else{
                        $("#OneValue").combobox("setText","请选择");
                    }
                },
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
                onLoadSuccess:function(){
                    
                    if(requestData && requestData["TwoValue"]){
                        $("#TwoValue").combobox("select",requestData["TwoValue"]);
                        delete requestData["TwoValue"];
                    }else{
                        $("#TwoValue").combobox("setText","请选择");
                    }
                },
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
                onLoadSuccess:function(){
                    if(requestData && requestData["ThreeValue"]){
                        $("#ThreeValue").combobox("select",requestData["ThreeValue"]);
                        delete requestData["ThreeValue"];
                    }
                },
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
                textField: "Text",
                onLoadSuccess:function(){
                    if(requestData && requestData["FourValue"]){
                        $("#FourValue").combobox("select",requestData["FourValue"]);
                        delete requestData["FourValue"];
                    }else{
                        $("#FourValue").combobox("select",requestData["FourValue"]);
                    }
                }
            });
        }

        function submitUserForm() {
            $("#mainForm").form("submit", {
                onSubmit: function (param) {
                    param.action = "UpdateKnow";
                    param.Id = requestData["Id"];
                    
                    // 这里是获取表单验证，检查表单验证是否通过
                    var isValid = $(this).form("enableValidation").form("validate");
                    if (isValid) {
                        parent.$.messager.progress();// 显示进度条
                    }

                    // 如果返回True就会提交表单，否则不提交表单
                    return isValid;
                },
                success: function (data) {
                    // 表单提交成功隐藏进度条
                    parent.$.messager.progress("close");
                    var result = eval("(" + data + ")");
                    if(result.StateCode != 0){
                        parent.$.messager.alert("提示", result.Message, "info");
                        return;
                    }

                    parent.$.messager.alert("提示", result.Message, "info");
                    parent.closeAndDestroyModalWindow(1);
                }
            });
        }

        $(document).ready(function () {
            //console.info(requestData);
            getOneCaseType();
            $("#tMalDescription").val(requestData["MalDescription"]);
            $("#Solution").val(requestData["Solution"]);
            $("#MainReason").val(requestData["MainReason"]);
        });

    </script>
</head>
<body>
    <form id="mainForm" action="/Handler/Knowledge/PublicKnow.ashx" method="post">

        <div style="padding: 10px;">
            <div style="margin-bottom: 10px;">
                <span>Case类型：</span>
                <input class="easyui-combobox" id="OneValue" name="OneValue" />
                <input class="easyui-combobox" id="TwoValue" name="TwoValue" />
                <input class="easyui-combobox" id="ThreeValue" name="ThreeValue" />
                <input class="easyui-combobox" id="FourValue" name="FourValue" />
            </div>
            <div style="margin-bottom: 10px;"><span>故障描述：</span><textarea id="tMalDescription" cols="80" rows="4" name="MalDescription"></textarea></div>
            <div style="margin-bottom: 10px;"><span>解决方案：</span><textarea id="Solution" cols="80" rows="4" name="Solution"></textarea></div>
            <div style="margin-bottom: 10px;"><span>根本原因：</span><textarea id="MainReason" cols="80" rows="4" name="MainReason"></textarea></div>
            <a id="UpdateKnowledge" class="linkbutton">修改</a>
        </div>
    </form>
    <script type="text/javascript">
        
        // 窗口 - 确认按钮
        $("#UpdateKnowledge").linkbutton({
            iconCls: "icon-ok",
            onClick: function () {
                submitUserForm();
            }
        });
    </script>
</body>
</html>
