<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SelectApproveEnrollWindow.aspx.cs" Inherits="Pactera.Workflow.SelectApproveEnrollWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        var stepRolePersonMap = {};
        var flowName = getQueryStringByName("flow_name");
        var pkCorp = getQueryStringByName("pk_corp");
        var form_keys = getQueryStringByName("form_keys");
        var BuildCost = getQueryStringByName("BuildCost");  //造价
        var ContestantCorpName = getQueryStringByName("ContestantCorpName");  //竞争公司
        var WorkflowNumber = getQueryStringByName("WorkflowNumber");  //封标流程编号

        // 把选择的人传回来
        var persons = getQueryStringByName("persons");
        var seldocument = getQueryStringByName("selborrow");
        function selectPerson(rowIndex, stepNumber, roleId, action) {
            var singleSelect = action == "审批" ? 1 : 0;

            var url = "/Workflow/SelectPersonWindow.aspx?pk_corp=" + pkCorp + "&role_id=" + roleId + "&single_select=" + singleSelect + "&r=" + Math.random();
            parent.createAndOpenModalWindow("选择人员", "520", "410", url, function (data) {
                if (data != null) {
                    var person;
                    var personIds;
                    for (var i = 0; i < data.length; i++) {
                        // 存储步骤角色人员
                        if (i == 0) {
                            person = data[i].Name;
                            personIds = data[i].Id;
                            continue;
                        }
                        person += "，" + data[i].Name;
                        personIds += "," + data[i].Id;
                    }
                    stepRolePersonMap[stepNumber + "_" + roleId] = personIds + "|" + person;
                    var id = "#SpanWorkflowPerson_" + stepNumber + "_" + roleId;
                    $(id).text(person);

                    // 选择完成后检查按钮状态
                    $("#linkbtnClearSelected").linkbutton("disable");
                    for (var prop in stepRolePersonMap) {
                        if (prop == stepNumber + "_" + roleId) {
                            $("#linkbtnClearSelected").linkbutton("enable");
                        }
                    }

                }
            });
        }
        $(document).ready(function () {
            //加载审批人datagrid(根据审批流程名称及参数，如市场及封标里的form_keys，BuildCost，ContestantCorpName，WorkflowNumber)
            $("#ApprovePersonDatagrid").datagrid({
                url: "/Handler/PublicWorkflow.ashx",
                queryParams: { action: "get_step_persion_content", pk_corp: pkCorp, flow_code: flowName, form_keys: form_keys, BuildCost: BuildCost, ContestantCorpName: ContestantCorpName, WorkflowNumber: WorkflowNumber },
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
                    var stepNumber = rowData.StepNumber;
                    var roleId = rowData.RoleId;

                    $("#linkbtnClearSelected").linkbutton("disable");
                    for (var prop in stepRolePersonMap) {
                        if (prop == stepNumber + "_" + roleId) {
                            $("#linkbtnClearSelected").linkbutton("enable");
                        }
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
                    stepRolePersonMap = {};
                    if (persons != null && persons != 'undefined') {
                        var tempPerson = persons.split(";");
                        for (var i = 0; i < tempPerson.length; i++) {
                            var items = tempPerson[i].split("_");
                            var personName = decodeURI(items[2].split("|")[1]);

                            // 将名字显示出来
                            $("#SpanWorkflowPerson_" + items[0] + "_" + items[1]).text(personName);

                            // 将值放回去
                            stepRolePersonMap[items[0] + "_" + items[1]] = items[2].split("|")[0] + "|" + personName;
                        }
                    }
                }
            });

            // 是否存在步骤审批人
            function hasStepRolePerson(stepNumber) {
                for (var prop in stepRolePersonMap) {
                    // 这个会签节点选择了人
                    if (prop.indexOf(stepNumber) == 0) return true;
                }

                return false;
            }

            $("#linkbtnConfirm").linkbutton({
                iconCls: "icon-ok",
                onClick: function () {
                    var rows = $("#ApprovePersonDatagrid").datagrid("getRows");
                    // 判断不允许跳过的审批节点是否选择了审批人
                    for (var i = 0; i < rows.length; i++) {
                        // 忽略掉允许跳过的节点
                        if (rows[i].Action == "审批" && rows[i].AutoSkip != "True") {
                            if (!hasStepRolePerson(rows[i].StepNumber)) {
                                parent.$.messager.alert("提示", "请为【" + rows[i].RoleName + "】选择审批人！", "info");
                                return;
                            }
                        }
                    }

                    // 找到所有会签节点
                    var counterSignStep = {};
                    for (var i = 0; i < rows.length; i++) {
                        if (rows[i].Action == "会签") {
                            counterSignStep[rows[i].StepNumber] = true;
                        }
                    }

                    // 判断会签节点是否选择了人员
                    var allSelected = true;
                    for (var stepNumber in counterSignStep) {
                        var thisStepIsSelected = false;
                        for (var prop in stepRolePersonMap) {
                            // 这个会签节点选择了人
                            if (prop.indexOf(stepNumber) == 0) {
                                thisStepIsSelected = true;
                                break;
                            }
                        }

                        // 只要有一个步骤没选择，不允许提交
                        if (!thisStepIsSelected) {
                            allSelected = false;
                            break;
                        }
                    }

                    if (!allSelected) {
                        parent.$.messager.alert("提示", "会签人员不能为空！", "info");
                        return;
                    }
                    var seldocuments = seldocument.split(',');
                    var temp = undefined;
                    for (var prop in stepRolePersonMap) {
                        if (temp == undefined) {
                            temp = prop + "_" + stepRolePersonMap[prop];
                            continue;
                        }
                        temp += ";" + prop + "_" + stepRolePersonMap[prop];
                    }
                    for (var i = 0; i < seldocuments.length - 1; i++) {

                        if (temp.indexOf(seldocuments[i]) < 0) {
                            var selectInfo;
                            switch (seldocuments[i]) {
                                case '35':
                                    selectInfo = '请选择用印业务审批人！';
                                    break;
                                case '36':
                                    selectInfo = '请选择借证业务审批人！';
                                    break;
                                case '37':
                                    selectInfo = '请选择开户许可证审批人！';
                                    break;
                                case '38':
                                    selectInfo = '请选择税务登记证审批人！';
                                    break;
                                case '39':
                                    selectInfo = '请选择财务审计报告人！';
                                    break;
                                default:
                                    selectInfo = '请选择正确的会签人！';
                                    break;
                            }
                            parent.$.messager.alert("提示", selectInfo, "info");
                            return;
                        }
                    }
                    parent.closeAndDestroyModalWindow(stepRolePersonMap);
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
                        if (stepRolePersonMap.hasOwnProperty(stepNumber + "_" + roleId)) {
                            delete stepRolePersonMap[stepNumber + "_" + roleId];
                            // 移除这个属性，然后清空显示
                            var id = "#SpanWorkflowPerson_" + stepNumber + "_" + roleId;
                            $(id).text("");
                        }
                    }
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
