<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="KnowLedgeDetail.aspx.cs" Inherits="Pactera.Knowledge.KnowLedgeDetail" %>


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
        
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
        $(document).ready(function () {
            $("#mainForm").form("load", "/Handler/Knowledge/PublicKnow.ashx?action=getKnowDetailInfo&Id=" + requestData["Id"] + "&r=" + Math.random());
        });

    </script>
</head>
<body>
    <form id="mainForm" method="post" action ="/Handler/Knowledge/PublicKnow.ashx">
        <div style="padding:10px;">
            <div style="margin-bottom:10px;">
                <span>&nbsp;&nbsp;&nbsp;创建人：</span>
                <input class="easyui-textbox" name="Created" />
            </div>
            <div style="margin-bottom:10px;">
                <span>创建时间：</span>
                <input class="easyui-textbox" name="CreateDate" />
            </div>
            <div style="margin-bottom:10px;">
                <span>Case类型：</span>
                <input class="easyui-textbox" name="OneValue" />
                <input class="easyui-textbox" name="TwoValue" />
                <input class="easyui-textbox" name="ThreeValue" />
                <input class="easyui-textbox" name="FourValue" />
            </div>
            <div style="margin-bottom:10px;"><span>故障描述：</span><textarea id="tMalDescription" cols="80" rows="4" name="MalDescription"></textarea></div>
            <div style="margin-bottom:10px;"><span>解决方案：</span><textarea id="Solution" cols="80" rows="4" name="Solution"></textarea></div>
            <div style="margin-bottom:10px;"><span>根本原因：</span><textarea id="MainReason" cols="80" rows="4" name="MainReason"></textarea></div>
           
        </div>
    </form>
</body>
</html>
