<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PiShiXT.aspx.cs" Inherits="Pactera.MarketInfo.PiShiXT" %>
<%@ OutputCache Duration="1" VaryByParam="None"%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>工程信息</title>
    <link href="../Scripts/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" />
    <script src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/pactera-workflow-description.css" rel="stylesheet" />
    <script src="../Scripts/public.js"></script>
    <%--列表省略号js--%>
    <link href="/Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />
    <style>
    /*显示日期的textbox的样式--灰色*/
    .kgGary{
	    border:#d9d9d9 1px solid;
        border-radius:3px 3px 3px 3px;
	    height:17px;
        font-size:12px; padding-left:4px; padding-top:2px;color:gray;
    }
    </style>
    <script charset="utf-8">
        //页面初始化
        $(function () {          
            //1、弹出框
            $('#mapdiv').dialog({
                title: '地图查询',  //标题
                width: 820,
                height: 430,
                modal: true,
                //draggable: false,
                //collapsible: false,
                //minimizable: false,
                //maximizable: false,
                //resizable: false,
                //left: '15%',
                //top: 10,
                closed: true
            });

            getDefaultSetp();
        });

        //地图查询坐标--批示form
        function selectPoint1() {
            var lngx = $('#longitude1').val();
            var latx = $('#latitude1').val();

            var strUrl = '/MarketInfo/MapHtml.html?lng=' + lngx + '&lat=' + latx + '&isuse=0&time=' + new Date().getTime();

            var v = window.showModalDialog(strUrl, "", "Dialogwidth:800px; Dialogheight:360px;help:no;scroll:no;status:no;");
            //alert("v[0]:" + v[0]);
        }

        // 获取默认流程
        function getDefaultSetp() {
            // 默认显示造价5000万，竞争者为空
            var formKeys = "BuildCost,ContestantCorpName";
            var buildCost = "5000";  //造价
            var contestantCorpName = "123";  //竞争公司

            var param = { action: "get_default_step_text", flow_name: "ProjectChanceSubmit", form_keys: formKeys, BuildCost: buildCost, ContestantCorpName: contestantCorpName };
            $.post("/Handler/PublicWorkflow.ashx", param, function (data) {
                $("#divWorkflowStep").html(data);
            });
        }

        //获取传递的操作参数，
        function loadForm() {
            //1.获取传递的操作参数，
            var actionx = getQueryStringByName("action");
            if ($.trim(actionx) == 'pishi') {
                Action.Pishi();
            } else if ($.trim(actionx) == 'updpishi') {
                Action.UpdPishi();
            }
        }

        //操作
        var Action = {
            //1.批示按钮
            Pishi: function () {
                var Id = getQueryStringByName("keys");
                Id = $.trim(Id);
                $('#Id').val(Id);

                //1、获取步骤StepId，InstanceId，跳转到审批页面
                var a = getQueryStringByName("a");  //StepId
                var b = getQueryStringByName("b");  //InstanceId
                //alert("a:"+a+"//b:"+b);

                //2.隐藏域保存实例及步骤编号审批时使用
                $('#step_idx').val(a);
                $('#instance_idx').val(b);

                //3、数据绑定到编辑form上，只读
                $("#mainForm .easyui-textbox").textbox({ disabled: true });  //禁用所有textbox
                $("#mainForm .easyui-numberbox").numberbox({ disabled: true });//禁用编辑form数值输入框
                $("#ApproveMessagex").textbox({ disabled: false });  //只有审批意不禁用

                $('#mainForm').form('load', '/Handler/MaketOperate.ashx?flag=approveinfo&instance_id=' + b + "&time=" + new Date().getTime());
                //4.审批信息加载成功后,加载经纬度
                $('#mainForm').form({
                    onLoadSuccess: function (data1) {
                        if (data1.spCoordinationResult) {
                            $('spCoordinationResult').combobox('select', data1.spCoordinationResult);
                        }
                        if (data1.ApproveMessagex) {
                            $('ApproveMessagex').textbox('setValue', data1.ApproveMessagex);
                        }
                        if (data1.longitude == '' || data1.longitude == '0' || data1.latitude == '' || data1.latitude == '0') {
                            $("#longitude1").val('0');
                            $("#latitude1").val('0');
                        }
                        else {
                            $("#longitude1").val(data1.longitude);
                            $("#latitude1").val(data1.latitude);

                            $("#jwd1").textbox('setValue', data1.longitude + ',' + data1.latitude);
                        }
                    }
                });

                //5.流程说明
                var param = { action: Workflow.Action.GetWorkflowDisplay, instance_id: b, time: new Date().getTime() };
                $.post(Workflow.Handler.Url, param, function (data) {
                    $("#divWorkflowStep").html(data);
                });

                //6、加载审批信息
                if (a && b && a != '' && b != '') {
                    $("#dgownerlist").datagrid('load', { action: 'approve_list', step_id: a, instance_id: b, time: new Date().getTime() });
                }
            },

            //2.修改批示按钮--已完成/退回的，改为退回状态
            UpdPishi: function () {
                var Id = getQueryStringByName("keys");
                Id = $.trim(Id);
                $('#Id').val(Id);

                $('#updps').val(1);  //表示是否修改批示，在保存审核批示意见时使用
                //1、后台修改状态
                var a = getQueryStringByName("a");  //StepId
                var b = getQueryStringByName("b");  //InstanceId
                //alert("a:"+a+"//b:"+b);

                //2.隐藏域保存实例及步骤编号审批时使用
                $('#step_idx').val(a);
                $('#instance_idx').val(b);

                //3、数据绑定到编辑form上，只读
                $("#mainForm .easyui-textbox").textbox({ disabled: true });  //禁用所有textbox
                $("#mainForm .easyui-numberbox").numberbox({ disabled: true });//禁用编辑form数值输入框
                $("#ApproveMessagex").textbox({ disabled: false });  //只有审批意不禁用
                $('#mainForm').form('load', '/Handler/MaketOperate.ashx?flag=approveinfo&instance_id=' + b + "&time=" + new Date().getTime());

                //4.审批信息加载成功后,加载经纬度
                $('#mainForm').form({
                    onLoadSuccess: function (data1) {
                        if (data1.spCoordinationResult) {
                            $('spCoordinationResult').combobox('select', data1.spCoordinationResult);
                        }
                        if (data1.ApproveMessagex) {
                            $('ApproveMessagex').textbox('setValue', data1.ApproveMessagex);
                        }
                        if (data1.longitude == '' || data1.longitude == '0' || data1.latitude == '' || data1.latitude == '0') {
                            $("#longitude1").val('0');
                            $("#latitude1").val('0');
                        }
                        else {
                            $("#longitude1").val(data1.longitude);
                            $("#latitude1").val(data1.latitude);

                            $("#jwd1").textbox('setValue', data1.longitude + ',' + data1.latitude);
                        }
                    }
                });

                //5.流程说明
                var param = { action: Workflow.Action.GetWorkflowDisplay, instance_id: b, time: new Date().getTime() };
                $.post(Workflow.Handler.Url, param, function (data) {
                    $("#divWorkflowStep").html(data);
                });
                
                //6、加载审批信息(历史数据无待办表，即无步骤编号，所以令a=='0')
                if (a && b && a != '' && b != '' & a!= '0') {
                    $("#dgownerlist").datagrid('load', { action: 'approve_list', step_id: a, instance_id: b, time: new Date().getTime() });
                }
                    
            },

            //4.冲突协调审批按钮
            SubmitApprove: function () {
                var keys = $('#Id').val();
                if ($.trim(keys) == '') {
                    parent.$.messager.alert('提示', '获取的信息编Id无效', 'info');
                    return;
                }
                var updps = $('#updps').val();  //是否修改批示
                var xt = $('#spCoordinationResult').combobox('getValue');   //协调结果
                var mess = $('#ApproveMessagex').textbox('getValue');  //审批意见
                var stepid = $('#step_idx').val();
                var instanid = $('#instance_idx').val();
                //alert("stepid:" + stepid + "///instanid:" + instanid);

                //验证是否输入审批意见,选择协调结果
                var flagx = $('#mainForm').form('enableValidation').form('validate');
                if (!flagx) {
                    //parent.$.messager.alert('提示', '请输入审批意见,选择协调结果', 'info');
                    return;
                }

                if ($.trim(stepid) != '' && $.trim(instanid)) {
                    if ($.trim(updps) == '1') {  //修改
                        /*$.ajax({
                            type: "post",
                            url: "/Handler/MaketOperate.ashx",
                            data: { keys: keys, flag: 'updpishi', message: mess, xt: xt, step_id: stepid, instance_id: instanid, time: new Date().getTime() },
                            dataType: "json",
                            async: false,
                            success: function (data) {
                                if (data && data.Message) {
                                    parent.$.messager.alert('提示', data.Message, 'info');
                                }
                                else {
                                    parent.$.messager.alert('提示', data.success, 'info');
                                    $('#updps').val(0); //
                                    Action.CancelUpd();  //关闭弹窗
                                }
                            }
                        });*/
                        $.post("/Handler/MaketOperate.ashx",
                            { keys: keys, flag: 'updpishi', message: mess, xt: xt, step_id: stepid, instance_id: instanid, time: new Date().getTime() },
                            function (data) {
                                if (data && data.Message) {
                                    parent.$.messager.alert('提示', data.Message, 'info');
                                }
                                else {
                                    parent.$.messager.alert('提示', data.success, 'info');
                                    $('#updps').val(0); //
                                    Action.CancelUpd();  //关闭弹窗
                                }
                            }, "json");
                    }
                    else {  //正常审批
                        if ($.trim(stepid) != '' && $.trim(instanid)) {
                            $("#mainForm").form("submit", {
                                onSubmit: function (param) {
                                    param.flag = "approve";
                                    param.step_id = stepid;
                                    param.instance_id = instanid;
                                    param.keys = keys;
                                },
                                success: function (data) {
                                    var data = $.parseJSON(data);
                                    if (data && data.State) {
                                        parent.$.messager.alert('提示', data.Message, 'info');
                                        Action.CancelUpd();  //关闭弹窗
                                    }
                                }
                            });
                        }

                    }
                }
                else {
                    parent.$.messager.alert('提示', '获取批示步骤及实例编号失败！', 'info');
                    return;
                }
                $('#spCoordinationResult').combobox('reset');   //协调结果
                $('#ApproveMessagex').textbox('setValue', '');  //审批意见
            },

            //5.重置冲突审批
            ClearApprove: function () {
                $('#spCoordinationResult').combobox('select', '');
                $('#ApproveMessagex').textbox('setValue', '');
            },

            //6.取消编辑按钮
            CancelUpd: function () {
                parent.closeAndDestroyModalWindow();  //关闭弹窗
            },

            //7.根据枚举value，获取pvalue,使得施工领域默认选中获取pvalue，然后再根据pvalue，加载工程类型选项，默认选择id项
            GetPid: function (id) {
                $.ajax({
                    type: 'post',
                    url: '/Handler/PublicEnum.ashx?action=enum_getPid&value=' + id,
                    dataType: 'text',
                    success: function (data) {
                        $('#ProjectType').combobox('setValue', ''); //工程类型
                        $('#ConstructionDoman').combobox('setValue', '');
                        $('#ProjectType').combobox('disableValidation');  //不验证是否必填
                        $('#ConstructionDoman').combobox('disableValidation');

                        if (data && data.length == 36) {
                            var data1 = $('#ConstructionDoman').combobox('getData');
                            if (data1.length > 0) {
                                for (var i = 0; i < data1.length; i++) {
                                    if (data == data1[i].Key) {  //data是有效的
                                        $('#ConstructionDoman').combobox('select', data);  //施工领域

                                        //工程类型，根据所选的施工领域，重新加载
                                        $('#ProjectType').combobox('clear');
                                        var url = '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=' + data;  //escape(name) :转换中文参数
                                        $('#ProjectType').combobox('reload', url);
                                        $('#ProjectType').combobox('select', id); //工程类型
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            },

            //8.根据枚举value，获取pvalue,使得使用资质默认选中获取pvalue，然后再根据pvalue，加载上班公司选项，默认选择id项
            GetPid1: function (id) {
                $.ajax({
                    type: 'post',
                    url: '/Handler/PublicEnum.ashx?action=enum_getPid&value=' + id,
                    dataType: 'text',
                    success: function (data) {
                        $('#UseQualifications').combobox('setValue', ''); //使用资质
                        $('#ReportCorp').combobox('setValue', '');  //上报公司
                        $('#UseQualifications').combobox('disableValidation');  //不验证是否必填
                        $('#ReportCorp').combobox('disableValidation');

                        if (data && data.length == 36) {
                            var data1 = $('#UseQualifications').combobox('getData');
                            if (data1.length > 0) {
                                for (var i = 0; i < data1.length; i++) {
                                    if (data == data1[i].Key) {  //data是有效的
                                        $('#UseQualifications').combobox('select', data);  //施工领域

                                        //上报公司，根据所选的使用资质，重新加载
                                        $('#ReportCorp').combobox('clear');
                                        var url = '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=' + data;  //escape(name) :转换中文参数
                                        $('#ReportCorp').combobox('reload', url);
                                        $('#ReportCorp').combobox('select', id); //上报公司
                                        break;
                                    }
                                }
                            }
                        }
                    }
                });
            }
        }
    </script>
</head>
<body style=" width:915px; " onload="loadForm()">
    <div id="ctxtdiv" style="padding: 10px; overflow-y: auto;">
        <form id="mainForm" method="post" data-options="novalidate:false" action="/Handler/MaketOperate.ashx">
            <input id="Id" name="Id" type="hidden" value="0" />
            <%--标识主键id--%>
            <input id="instance_idx" name="instance_idx" type="hidden" />
            <input id="step_idx" name="step_idx" type="hidden" />
            <%--隐藏域保存实例及步骤编号审批时使用--%>
            <input id="updps" name="updps" type="hidden" value="0" />
            <%--是否修改批示，在保存审核批示意见时使用--%>

            <!--基本信息-->
            <div class="easyui-panel panel_border">
                <div class="content-1">
                    <dl>
                        <dt>
                            <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                        <dd>基本信息</dd>
                    </dl>
                </div>
                <table cellpadding="5" style="margin-top: -10px;">
                    <tr>
                        <td>工程名称:</td>
                        <td colspan="3">

                            <input id="Text1" name="ProjectName" class="easyui-textbox" readonly="readonly" style="width: 446px" />
                        </td>
                        <td>登记编号:</td>
                        <td>
                            <input id="Text2" name="RegNo" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px">建设单位名称:</td>
                        <td colspan="3">
                            <input id="Text3" name="BuildCopeName" class="easyui-textbox" readonly="readonly" style="width: 446px" />
                        </td>
                        <td>公司简称:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="CreateCorp1" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px">使用资质:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="UseQualifications1" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 70px;">上报公司:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="ReportCorp1" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 80px;">拟开工时间:</td>
                        <td style="width: 210px">
                            <input id="Text7" name="nkgsj" class="easyui-textbox" readonly="readonly" style="width: 149px;" />
                            <%--<input id="Text17" name="nkgsj" class="Wdate" type="text" onclick="WdatePicker();" maxlength="10" readonly="readonly"  style="width: 146px;" />--%>
                        </td>
                    </tr>
                    <tr>
                        <td>招标方式:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="TenderMode1" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>项目用途:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="ConstructionDoman1" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>项目类型:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="ProjectType1" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>建筑面积(m²):</td>
                        <td>
                            <input id="Text8" name="BuildArea" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>造价(万元):</td>
                        <td>
                            <input id="Text10" name="BuildCost" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>登记日期:</td>
                        <td>
                            <input id="Text5" name="CreateTime" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <%--<tr>
                        <td>工程规模:</td>
                        <td colspan="7">
                            <input id="Text9" name="ProjectScope" class="easyui-textbox" readonly="readonly" style="width: 740px;" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td>工程描述:</td>
                        <td colspan="7">
                            <input id="Text11" name="ProjectDetail" class="easyui-textbox" readonly="readonly" data-options="multiline:true"
                                style="width: 740px; height: 60px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注:</td>
                        <td colspan="7">
                            <input id="Text16" name="Memo" class="easyui-textbox" readonly="readonly" data-options="multiline:true"
                                style="width: 740px; height: 60px;" />
                        </td>
                    </tr>
                </table>
            </div>
            <!--联系及跟踪人信息-->
            <div class="easyui-panel  panel_border" style="margin-top: 10px; margin-bottom: 10px;">
                <div class="content-1">
                    <dl>
                        <dt>
                            <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                        <dd>联系人信息</dd>
                    </dl>
                </div>
                <table cellpadding="5" style="margin-top: -10px;">
                    <tr>
                        <td style="width: 77px;">联系人:</td>
                        <td style="width: 202px">
                            <input id="Text12" name="ContactMan" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td style="width: 70px">联系电话:</td>
                        <td style="width: 210px">
                            <input id="Text13" name="ContantPhone" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td>跟踪联系人:</td>
                        <td>
                            <input id="Text14" name="LinkMan" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>跟踪人电话:</td>
                        <td>
                            <input id="Text15" name="LinkManPhone" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td></td>
                        <td></td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>
            <!--位置信息-->
            <div class="easyui-panel panel_border">
                <div class="content-1">
                    <dl>
                        <dt>
                            <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                        <dd>位置信息</dd>
                    </dl>
                </div>
                <table cellpadding="5" style="margin-top: -10px;">
                    <tr>
                        <td style="width: 77px">所属国家:</td>
                        <td style="width: 202px">
                            <input class="easyui-textbox" type="text" name="CountryName" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 70px">所属省:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="ProvinceName" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 60px;">所属市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="CityName" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>所属区:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="DistrictName" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>所属区域:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="Region1" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>经纬度:</td>
                        <td>
                            <input id="jwd1" name="jwd1" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'"
                                style="width: 150px;" />
                            <input id="longitude1" name="longitude1" type="hidden" value="0" />
                            <input id="latitude1" name="latitude1" type="hidden" value="0" />
                        </td>
                    </tr>
                    <tr>
                        <td>工程地点:</td>
                        <td colspan="3" style="">
                            <input id="Text4" name="ProjectLocation" class="easyui-textbox"
                                readonly="readonly" style="width: 446px;" />
                        </td>
                        <td colspan="2" align="right">
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-world'" onclick="selectPoint1();">&nbsp;在地图中选择工程位置</a>
                        </td>
                    </tr>
                </table>
            </div>
            <!--竞争者信息-->
            <div class="easyui-panel panel_border" style="margin-top: 10px; margin-bottom: 10px;">
                <div class="content-1">
                    <dl>
                        <dt>
                            <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                        <dd>竞争者信息</dd>
                    </dl>
                </div>
                <table cellpadding="5" style="margin-top: -10px;">
                    <tr>
                        <td style="width: 77px;">公司名称:</td>
                        <td style="width: 202px">
                            <input id="Text21" name="ContestantCorpName" class="easyui-textbox" readonly="readonly" style="width: 150px;" /></td>

                        <td style="width: 70px;">资质状况:</td>
                        <td style="width: 210px">
                            <input id="Text22" name="ContestantAptitude" class="easyui-textbox" readonly="readonly" style="width: 150px;" /></td>

                        <td style="width: 60px;">联系人:</td>
                        <td>
                            <input id="Text23" name="ContestantContactMan" class="easyui-textbox" readonly="readonly" style="width: 150px;" /></td>
                    </tr>
                    <tr>
                        <td>职务:</td>
                        <td>
                            <input id="Text24" name="ContestantPosition" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td>联系电话:</td>
                        <td>
                            <input id="Text25" name="ContestantTel" class="easyui-textbox" readonly="readonly" style="width: 150px;" /></td>

                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>

            <!--审批信息-->
            <div>
                <div class="easyui-panel panel_border">
                    <div class="content-1">
                        <dl>
                            <dt>
                                <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                            <dd>批示信息</dd>
                        </dl>
                    </div>
                    <table cellpadding="5" style="margin-top: -10px;">
                        <tr>
                            <td style="width: 77px;">协调结果:</td>
                            <td>
                                <select id="spCoordinationResult" name="spCoordinationResult" class="easyui-combobox" style="width: 180px;"
                                    data-options="
                                        panelHeight: 'auto',
                                        panelMinHeight: 20,
                                        panelMaxHeight: 200,
                                        editable:false,
                                        required:true,
                                        url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=F1A978E4-B0CC-41B2-B1FA-23BC917C03D1',
                                        valueField: 'Key',
                                        textField: 'Value'">
                                </select>
                            </td>
                        </tr>
                        <tr>
                            <td>审批意见:</td>
                            <td>
                                <input class="easyui-textbox" id="ApproveMessagex" name="ApproveMessagex" data-options="multiline:true,required:true,validType:'maxLength[500]'"
                                    style="height: 80px; width: 740px;" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div style="text-align: right; margin-top: 15px; margin-right: 20px; height: 40px;">
                <a class="easyui-linkbutton linkbutton" style="margin-left: 10px;" onclick="Action.SubmitApprove();">提交</a>
                <a class="easyui-linkbutton linkbutton" style="margin-left: 10px;" onclick="Action.ClearApprove();">重置</a>
            </div>
            <div class="easyui-panel panel_border">
                <div class="content-1">
                    <dl>
                        <dt>
                            <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                        <dd>审批流程信息</dd>
                    </dl>
                </div>
                <div id="divWorkflowStep" style="padding: 5px;">读取中...</div>
                <%--<script type="text/javascript">
                    $.post("/Handler/PublicWorkflow.ashx", { action: "get_instance_step_text", instance_id: getQueryStringByName("instance_id") }, function (data) {
                        $("#divWorkflowStepDescription").html(data);
                    });
                </script>--%>
                <table id="dgownerlist" class="easyui-datagrid" style="width: 100%; height: 200px;"
                    url="/Handler/Enroll.ashx"
                    nowrap="false"
                    rownumbers="true" fitcolumns="true" singleselect="true">
                    <thead>
                        <tr>
                            <th field="StepDescription" halign="center" style="width: 16%;">节点</th>
                            <th field="Persion" halign="center" align="center" style="width: 8%;">处理人</th>
                            <%--<th field="ActionType" halign="center" align="center" style="width: 5%;">动作</th>--%>
                            <th field="ApproveResult" halign="center" align="center" style="width: 7%;">状态</th>
                            <th field="Message" halign="center" style="width: 19%;" data-options="formatter: function (value, row, index) {return '<span title=' + value + '>' + value + '</span>';}">审批意见</th>
                            <th field="StartDate" halign="center" align="center" style="width: 17%;">接收时间</th>
                            <th field="Date" halign="center" align="center" style="width: 17%;">操作时间</th>
                            <th field="TimeConsuming" halign="center" align="center" style="width: 13%;">操作耗时</th>
                        </tr>
                    </thead>
                </table>
            </div>
        </form>
    </div>
</body>
</html>
