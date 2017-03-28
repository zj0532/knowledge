<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectApprovePersonWindow.aspx.cs" Inherits="Pactera.Workflow.SelectApprovePersonWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>选择审批人</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="/Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="/Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    <link href="/Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />

    <script src="/Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>

    <script src="../Scripts/public.js"></script>

    <script type="text/javascript">
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
        var pkCorp = getQueryStringByName("pk_corp");
        var WorkflowNumber = getQueryStringByName("WorkflowNumber");  //封标流程编号

        function selectPerson(rowIndex, nodeNumber, roleId, action) {
            var singleSelect = action == "审批" ? 1 : 0;

            var url = "/Workflow/SelectPersonWindow.aspx?pk_corp=" + pkCorp + "&role_id=" + roleId + "&single_select=" + singleSelect + "&r=" + Math.random();
            parent.createAndOpenModalWindow("选择人员", "520", "410", url, function (r) {
                if (r != null) {
                    var h = requestData["FlowPersons"];
                    if(!h) h = {};

                    var userDisplayText = "";
                    for (var i = 0; i < r.length; i++) {
                        if(!h[nodeNumber]) h[nodeNumber] = {};
                        if(!h[nodeNumber][roleId]) h[nodeNumber][roleId] = {};
                        if(!h[nodeNumber][roleId][r[i].Id]) h[nodeNumber][roleId][r[i].Id] = {};

                        // 存储选择的人
                        h[nodeNumber][roleId][r[i].Id]["Name"] = r[i].Name;
                        if(userDisplayText == "") {
                            userDisplayText = r[i].Name;
                            continue;
                        }

                        userDisplayText += "，" + r[i].Name;
                    }

                    // 显示选择的人
                    $("#SpanWorkflowPerson_" + nodeNumber + "_" + roleId).text(userDisplayText);
                    
                    // 选择完成后检查按钮状态
                    $("#linkbtnClearSelected").linkbutton("disable");
                    if(h[nodeNumber] && h[nodeNumber][roleId]){
                        $("#linkbtnClearSelected").linkbutton("enable");
                    }
                    
                }
            });
        }

        $(document).ready(function () {
            //加载审批人datagrid(根据审批流程名称及参数，如市场及封标里的form_keys，BuildCost，ContestantCorpName，WorkflowNumber)
            $("#ApprovePersonDatagrid").datagrid({
                url: "/Handler/PublicWorkflow.ashx",
                queryParams: { 
                    action: "get_step_persion_content",
                    pk_corp: pkCorp,
                    flow_code: requestData["FlowCode"],
                    FormValues: JSON.stringify(requestData["FormValues"]),
                    WorkflowNumber: WorkflowNumber
                },
                singleSelect: true,
                nowrap: false,
                width: 580,
                height: 300,
                columns: [[
                    {
                        field: "Action", title: "节点", width: 50, align: "center",
                        styler: function (value, rowData, rowIndex) {
                            return "background-color:#ffffff;";
                        }
                    },
                    { field: "RoleName", title: "角色", width: 170 },
                    {
                        field: "Person", title: "审批人", width: 340,
                        formatter: function (value, rowData, rowIndex) {
                            //console.info(rowData);
                            var id = "id=\"SpanWorkflowPerson_" + rowData.StepNumber + "_" + rowData.RoleId + "\"";
                            var click = "onclick=\"selectPerson(" + rowIndex + ",'" + rowData.StepNumber + "','" + rowData.RoleId + "','" + rowData.Action + "')\"";
                            var alink = "<a href=\"javascript:void(0)\" class=\"datagrid-linkbutton easyui-linkbutton\" " + click + " style=\"float:right;\"></a>";
                            return "<span " + id + " style=\"line-height:22px;\"></span>" + alink;
                        }
                    }
                ]],
                onClickRow: function (rowIndex, rowData) {
                    var nodeNumber = rowData.StepNumber;
                    var roleId = rowData.RoleId;

                    $("#linkbtnClearSelected").linkbutton("disable");
                    var h = requestData["FlowPersons"];
                    if(h[nodeNumber] && h[nodeNumber][roleId]){
                        $("#linkbtnClearSelected").linkbutton("enable");
                    }
                },
                onLoadSuccess: function (data) {
                    var rowIndex = 0;
                    var rowSpan = 0;
                    var stepNumber;

                    for (var i = 0; i < data.rows.length; i++) {
                        if (i == 0) stepNumber = data.rows[i].StepNumber;
                        if (data.rows[i].StepNumber == stepNumber) {
                            rowSpan++;
                        } else {
                            if (rowSpan > 1) {
                                $("#ApprovePersonDatagrid").datagrid("mergeCells", {
                                    index: rowIndex,
                                    field: "Action",
                                    rowspan: rowSpan,
                                    colspan: null
                                });
                            }

                            stepNumber = data.rows[i].StepNumber;

                            rowIndex = i;
                            rowSpan = 1;
                        }
                    }
                    if (rowSpan > 1) {
                        $("#ApprovePersonDatagrid").datagrid("mergeCells", {
                            index: rowIndex,
                            field: "Action",
                            rowspan: rowSpan,
                            colspan: null
                        });
                    }

                    $(".datagrid-linkbutton").linkbutton({ height: 24, plain: true, iconCls: "icon-search" });

                    // 把选中的人显示到列表中
                    for (var nodeNumber in requestData["FlowPersons"]) {
                        for(var roleId in requestData["FlowPersons"][nodeNumber]){
                            var userDisplayText = "";
                            for(var userId in requestData["FlowPersons"][nodeNumber][roleId]){
                                // 将名字显示出来
                                var personName = requestData["FlowPersons"][nodeNumber][roleId][userId]["Name"];
                                if(userDisplayText.length == "") {
                                    userDisplayText = personName;
                                    continue;
                                }

                                userDisplayText += "，" + personName;
                            }
                            $("#SpanWorkflowPerson_" + nodeNumber + "_" + roleId).text(userDisplayText);
                        }
                    }
                }
            });

            $("#linkbtnConfirm").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    var rows = $("#ApprovePersonDatagrid").datagrid("getRows");

                    // 判断不允许跳过的审批节点是否选择了审批人
                    for (var i = 0; i < rows.length; i++) {
                        // 忽略掉允许跳过的节点
                        if (rows[i].Action == "审批" && rows[i].AutoSkip != "True") {
                            if(!requestData["FlowPersons"]){
                                parent.$.messager.alert("提示", "请选择审批人！", "info");
                                return;
                            }
                            if (requestData["FlowPersons"] && !requestData["FlowPersons"][rows[i].StepNumber]) {
                                parent.$.messager.alert("提示", "请为【" + rows[i].RoleName + "】选择审批人！", "info");
                                return;
                            }
                        }
                    }
                    
                    // 判断会签节点是否选择了人员
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].Action == "会签" && !requestData["FlowPersons"][rows[i].StepNumber]) {
                            parent.$.messager.alert("提示", "会签人员不能为空！", "info");
                            return;
                        }
                    }

                    parent.closeAndDestroyModalWindow(requestData);
                }
            });

            $("#linkbtnCancel").linkbutton({
                iconCls: "icon-cancel",
                onClick: function () {
                    parent.closeAndDestroyModalWindow(null);
                }
            });
            $("#linkbtnClearSelected").linkbutton({
                disabled: true,
                iconCls: "icon-reset",
                onClick: function () {
                    var row = $("#ApprovePersonDatagrid").datagrid("getSelected");
                    if (row != null) {
                        var stepNumber = row.StepNumber;
                        var roleId = row.RoleId;

                        delete requestData["FlowPersons"][stepNumber][roleId];
                        $("#SpanWorkflowPerson_" + stepNumber + "_" + roleId).text("");
                    }

                    $("#linkbtnClearSelected").linkbutton("disable");
                }
            });
        });
    </script>
</head>
<body>
    <div>
        <table id="ApprovePersonDatagrid"></table>
    </div>
    <div style="text-align: right; padding-top: 10px;">
        <a id="linkbtnConfirm" class="linkbutton">确认</a>
        <a id="linkbtnCancel" class="linkbutton" style="margin-left: 10px;">取消</a>
        <a id="linkbtnClearSelected" class="linkbutton" style="margin-left: 10px;">重置</a>
    </div>
</body>
</html>
