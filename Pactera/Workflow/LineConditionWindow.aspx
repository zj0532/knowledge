<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="LineConditionWindow.aspx.cs" Inherits="Pactera.Workflow.LineConditionWindow" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <title>设置节点连接线</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />

    <link href="../Scripts/jquery-easyui-1.4.1/themes/metro/easyui.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />

    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js"></script>

    <script type="text/javascript">
        var requestData = <%=Request.Form["HiddenIframeRequestDataJson"]%>;
    </script>
</head>
<body>
    <table>
        <tr>
            <td>连接线类型：</td>
            <td>
                <select id="ToNodeType">
                    <option value="Normal">无条件跳转</option>
                    <option value="ApproveAgree">审批通过时</option>
                    <option value="ApproveNotAgree">审批不通过时</option>
                    <option value="Condition">自定义条件</option>
                </select>
            </td>
        </tr>
        <tr data-condition="toggle">
            <td>自定义条件：</td>
            <td><input type="text" id="Content" class="easyui-textbox" data-options="width:400" /></td>
        </tr>
        <tr data-condition="toggle">
            <td>条件说明：</td>
            <td>数据库字段用英文中括号 []，如 [Field]
                <h4>运算符</h4>
                <ul>
                    <li>等于： ==</li>
                    <li>大于： &gt;</li>
                    <li>小于： &lt;</li>
                    <li>大于等于： &gt;=</li>
                    <li>小于等于： &lt;=</li>
                    <li>不等于： !=</li>
                </ul>
                例：<br />
                [BuildCost]&nbsp;&gt;&nbsp;5000<br />
                [ContestantCorpName]&nbsp;!=&nbsp;""
            </td>
        </tr>
    </table>
    <script type="text/javascript">
        function toggleCondition(){
            var val = $("#ToNodeType").combobox("getValue");
            if(val == "Condition"){
                $("[data-condition='toggle']").show(300);
            }else{
                $("[data-condition='toggle']").hide(300);
            }
        }

        $(document).ready(function(){
            $("#ToNodeType").combobox({
                width: 200,
                editable: false,
                panelHeight: "auto",
                onChange:function(newValue, oldValue){
                    toggleCondition();
                }
            });

            var line = requestData["Line"];
            for(var tn in requestData["Node"]["NextNodes"]){
                if(requestData["Node"]["NextNodes"][tn]["ToNodeNumber"] == line["To"].substring(9)){
                    var conditionType = requestData["Node"]["NextNodes"][tn]["Type"];
                    //console.info(conditionType);
                    $("#ToNodeType").combobox("setValue", conditionType);
                }
            }
            //$("#ToNodeType").combobox("select", "Condition");
            //console.info($("#ToNodeType").combobox("getValue"));
            toggleCondition();
            
        });
    </script>
</body>
</html>
