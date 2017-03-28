<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WorkflowDemo.aspx.cs" Inherits="Pactera.Workflow.WorkflowDemo" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>

    <title>流程示例</title>

    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="/Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="/Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="/Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <script src="/Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>
    
    <script src="/Scripts/json2.js"></script>
    <script src="/Scripts/public.js"></script>

    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/pactera-workflow-description.css" rel="stylesheet" />

    <script type="text/javascript">
        var flowRequestData = {
            FlowCode: Workflow.FlowCode.OwnerSubmit,
            FormValues: [{ Field: "RegisteredCapital", Value: 1.99 }],
            FlowPersons: {}
        };

        // 验证节点是否选择了审批人
        function hasWorkflowPersons() {
            var has = false;
            for (var prop in flowRequestData.FlowPersons) {
                has = true;
                break;
            }
            if (!has) {
                parent.$.messager.alert("提示", "请选择审批人", 'info');
            }
            return hasWorkflowPersons;
        }

        function getWorkflowDisplay() {
            // 显示审批步骤人员
            var param = {
                action: Workflow.Action.GetWorkflowDisplay,
                flow_code: Workflow.FlowCode.OwnerSubmit,
                form_values: JSON.stringify(flowRequestData["FormValues"])
            };
            $.post(Workflow.Handler.Url, param, function (data) {
                $("#divWorkflowStep").html(data);
            });
        }

    </script>
</head>
<body>

    <a id="lbtnChoiceApprovePersion" class="linkbutton">选择审批人</a>
    <div id="divWorkflowStep">请先选择审批人</div>
    <script type="text/javascript">
        var workflowSelectPersonData;
        $("#lbtnChoiceApprovePersion").linkbutton({
            onClick: function () {
                var url = "/Workflow/SelectApprovePersonWindow.aspx?r=" + Math.random();
                parent.createAndOpenModalWindowWithData('选择人员', '600', '400', url, flowRequestData, function (r) {
                    flowRequestData["FlowPersons"] = r["FlowPersons"];
                    if (r != null) {
                        // 显示审批步骤人员
                        var param = {
                            action: Workflow.Action.GetWorkflowDisplay,
                            flow_code: Workflow.FlowCode.OwnerSubmit,
                            //flow_persons: JSON.stringify(data)
                            flow_persons: JSON.stringify(flowRequestData["FlowPersons"]),
                            form_values: JSON.stringify(flowRequestData["FormValues"])
                        };
                        $.post(Workflow.Handler.Url, param, function (html) {
                            $("#divWorkflowStep").html(html);
                        });
                    }
                });
            }
        });
    </script>

</body>
</html>
