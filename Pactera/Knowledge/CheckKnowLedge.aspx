<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckKnowLedge.aspx.cs" Inherits="Pactera.Knowledge.CheckKnowLedge" %>

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
        function CheckKnowLedge(Id) {
            var url = "/Handler/Knowledge/PublicKnow.ashx";
            var Param = {action:"CheckKnowledge",Id:Id};
            $.post(url, Param, function (data) {
                if (data["StateCode"] == 0) { alert("审核通过"); location.reload();}
            }, "json");
        }

        function NoCheckKnowLedge(Id) {
            var url = "/Handler/Knowledge/PublicKnow.ashx";
            var Param = { action: "NoCheckKnowledge", Id: Id };
            $.post(url, Param, function (data) {
                if (data["StateCode"] == 0) { alert("审核未通过"); location.reload(); }
            }, "json");
        }
        $(document).ready(function () {
            $("#KnowLedgeList").datagrid({
                url: "/Handler/Knowledge/PublicKnow.ashx",
                queryParams: { action: "getCheckKnowlist" },
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
                    { field: "MalDescription", title: "故障描述", width: "13%", align: "center" },
                    { field: "Solution", title: "解决方案", width: "14%", align: "center" },
                    { field: "Created", title: "创建人", width: "8%", align: "center" },
                    { field: "CreateDate", title: "创建时间", width: "12%", align: "center" },
                    {
                        field: "Id", title: "操作", width: "15%", align: "center", formatter: function (value, row, index) {
                        return "<a href=\"javascript:void(0)\" onClick=\"Knowledge(" + value + ")\" >查看 </a>| <a href=\"javascript:void(0)\" onClick=\"CheckKnowLedge(" + value + ")\">通过</a>| <a href=\"javascript:void(0)\" onClick=\"NoCheckKnowLedge("+value+")\">未通过</a>";
                            
                           
                        }
                    }
                ]]

            });




        });
    </script>
</head>
<body>
    <div class="easyui-panel">
        <div id="KnowLedgeList"></div>
    </div>
</body>
</html>
