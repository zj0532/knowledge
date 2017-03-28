<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DetailInfo.aspx.cs" Inherits="Pactera.MarketInfo.DetailInfo" %>
<%@ OutputCache Duration="1" VaryByParam="None"%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>详细信息</title>
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
            //1.添加时国省市区
            $('#Country').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                required: true,
                valueField: 'Key',
                textField: 'Value',
                url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=07BB50F6-80F7-4776-999D-3DD61E617AD6',
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#Country').combobox('getData');
                    if (data.length == 1) {
                        $('#Country').combobox('select', data[0].Key);
                    }
                },
                onSelect: function (record) {
                    if (record.Value != '中国') {
                        $("#Province").combobox({ disabled: true });
                        $("#City").combobox({ disabled: true });
                        $("#District").combobox({ disabled: true });
                        $("#Region").combobox({ disabled: true });  //区域
                    } else {
                        $("#Province").combobox({ disabled: false });
                        $("#City").combobox({ disabled: false });
                        $("#District").combobox({ disabled: false });
                        $("#Region").combobox({ disabled: false });//区域
                        $('#Province').combobox('clear');
                        $('#City').combobox('clear');
                        $('#District').combobox('clear');
                        $('#Province').combobox('disableValidation');  //不验证必填
                        $('#City').combobox('disableValidation');
                        $('#District').combobox('disableValidation');
                        $('#Region').combobox('disableValidation');
                        var url = '/Handler/PublicDict.ashx?action=area&AreaType=Province';
                        $('#Province').combobox('reload', url);
                    }
                }
            });
            $('#Province').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                required: true,
                onSelect: function (record) {
                    //1).市重新加载
                    $('#City').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=area&AreaType=City&Parent=' + record.Key;
                    $('#City').combobox('reload', url);
                    $('#City').combobox('disableValidation');  //不验证是否必填

                    //2).区重新加载
                    $('#District').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=0';
                    $('#District').combobox('reload', url);
                    $('#District').combobox('disableValidation');  //不验证是否必填

                    if (record.Value)  //区域还没加载并且市有值
                    {
                        Action.SeQYBySheng(record.Value);
                    }
                }
            });
            $('#City').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                //required: true,
                onSelect: function (record) {
                    //alert("11");
                    $('#District').combobox('clear');
                    $('#District').combobox('disableValidation');  //不验证是否必填
                    var url = '/Handler/PublicDict.ashx?action=area&AreaType=District&Parent=' + record.Key;
                    $('#District').combobox('reload', url);
                    //alert("22");
                    if (record.Value && $('#hdldqy').val() == '0')  //区域还没加载并且市有值
                    {
                        Action.SeQYByShi(record.Value);
                    }
                },
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#City').combobox('getData');
                    if (data.length < 1) {
                        $('#City').combobox('disableValidation');
                        $('#District').combobox('disableValidation');
                        $("#City").combobox('disable');
                        $("#District").combobox('disable');
                        $('.textbox-focused').removeClass('textbox-focused');
                    }
                    else {
                        if ($('#ProjectName').attr('disabled') == null) {  //查看详细时只读
                            $("#City").combobox('enable');
                            $("#District").combobox('enable');
                        }
                    }
                }
            });
            $('#District').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                //required: true,
                valueField: 'Key',
                textField: 'Value'
            });

            //2.使用资质、上报公司
            $('#UseQualifications').combobox({
                url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=419B0C04-E58A-41AD-90A9-C4FEAF924622',
                required: true,
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                onSelect: function (record) {
                    $('#ReportCorp').combobox('clear');
                    $('#ReportCorp').combobox('disableValidation');  //不验证是否必填
                    var url = '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=' + record.Key;
                    $('#ReportCorp').combobox('reload', url);
                }
            });
            //上报公司
            $('#ReportCorp').combobox({
                required: true,
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#ReportCorp').combobox('getData');
                    if (data.length == 1) {
                        $('#ReportCorp').combobox('select', data[0].Key);  //上报公司
                    }
                }
            });

            //3.CreateCorp-公司简称--编辑弹窗
            $('#CreateCorp').combobox({
                url: '/Handler/PublicBPM_Corp.ashx?action=info2',
                required: true,
                mode: 'remote',
                method: 'post',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'PK_CORP',
                textField: 'UNITSHORTNAME',
                onSelect: function (record) {
                    $("#WorkflowPersion").val('');  //清空审批人
                    getDefaultSetp();   // 显示默认流程
                },
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#CreateCorp').combobox('getData');
                    if (data.length == 1) {
                        $('#CreateCorp').combobox('select', data[0].PK_CORP);  //上报公司
                    }
                }
            });

            //4.加载项目用途，
            $('#ConstructionDoman').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                required: true,
                valueField: 'Key',
                textField: 'Value',
                url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=CE0E9ECC-7C1B-7674-AA54-B0F7CB0370B8',
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#ConstructionDoman').combobox('getData');
                    //alert("s1:" + data[0].Key);
                    if (data.length == 1) {
                        $('#ConstructionDoman').combobox('select', data[0].Key);
                    }
                }
            });
            //4.加载项目类型，
            $('#ProjectType').combobox({
                required: true,
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                valueField: 'Key',
                textField: 'Value',
                url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=4B75FBE3-BF4B-22EF-27B8-F46518563AAB',
                onLoadSuccess: function () {
                    //设置默认选择第一项
                    var data = $('#ProjectType').combobox('getData');
                    //alert("s1:" + data[0].Key);
                    if (data.length == 1) {
                        $('#ProjectType').combobox('select', data[0].Key);
                    }
                }
            });

            //6.初始化编辑表单区域、下拉表信息
            $('#updMarkfm').form({
                onLoadSuccess: function (data) {
                    //加载区域-国省市区
                    if (data.CountryName == '中国') {
                        var url1 = '/Handler/PublicDict.ashx?action=area&AreaType=Province';
                        $('#Province').combobox('reload', url1);
                        var url2 = '/Handler/PublicDict.ashx?action=area&AreaType=City&Parent=' + data.Province;
                        $('#City').combobox('reload', url2);
                        //处理市
                        //$('#City').combobox('setValue', '');
                        //if (data.City && data.City != '') {
                        //    $('#City').combobox({
                        //        onLoadSuccess: function () {
                        //            //设置默认选择第一项
                        //            var data1 = $('#City').combobox('getData');
                        //            if (data1.length >= 1) {
                        //                for (var i = 0; i < data1.length; i++) {
                        //                    //alert("data.City:" + data.City + "///data1[i].Key:" + data1[i].Key);
                        //                    if (data.City == data1[i].Key) {  //data是有效的
                        //                        $('#City').combobox('select', data.City);
                        //                        //alert("00");
                        //                        break;
                        //                    }
                        //                }
                        //            }
                        //        }
                        //    });
                        //}

                        var url3 = '/Handler/PublicDict.ashx?action=area&AreaType=District&Parent=' + data.City;
                        $('#District').combobox('reload', url3);
                        //处理区
                        $('#District').combobox('setValue', '');
                        if (data.District && data.District != '') {
                            $('#District').combobox({
                                onLoadSuccess: function () {
                                    //设置默认选择第一项
                                    var data1 = $('#District').combobox('getData');
                                    //alert("n:" + data1.length);
                                    if (data1.length >= 1) {
                                        for (var i = 0; i < data1.length; i++) {
                                            //alert("data.District:" + data.District + "///data1[i].Key:" + data1[i].Key);
                                            if (data.District == data1[i].Key) {  //data是有效的
                                                $('#District').combobox('select', data.District);
                                                break;
                                            }
                                        }
                                    }
                                }
                            });
                        }
                    } else {
                        $("#Province").combobox({ disabled: true });
                        $("#City").combobox({ disabled: true });
                        $("#District").combobox({ disabled: true });
                    }

                    $('#hdtszj').val(1);  //提示造价

                    //加载下拉表默认值
                    //1).公司简称
                    $('#CreateCorp').combobox('disableValidation');  //不验证是否必填
                    $('#CreateCorp').combobox('setValue', '');
                    if (data.CreateCorp && data.CreateCorp != '') {
                        var data1 = $('#CreateCorp').combobox('getData');
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                if (data.CreateCorp == data1[i].PK_CORP) {  //data是有效的
                                    $('#CreateCorp').combobox('select', data.CreateCorp);
                                    break;
                                }
                            }
                        }
                    }

                    //2).招标方式
                    $('#TenderMode').combobox('disableValidation');  //不验证是否必填
                    $('#TenderMode').combobox('setValue', '');
                    if (data.TenderMode && data.TenderMode != '') {
                        var data1 = $('#TenderMode').combobox('getData');
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                if (data.TenderMode == data1[i].Key) {  //data是有效的
                                    $('#TenderMode').combobox('select', data.TenderMode);
                                    break;
                                }
                            }
                        }
                    }

                    //3).所属区域
                    //$('#Region').combobox('disableValidation');  //不验证是否必填
                    //$('#Region').combobox('setValue', '');
                    //if (data.Region && data.Region != '') {
                    //    var data1 = $('#Region').combobox('getData');
                    //    if (data1.length > 0) {
                    //        for (var i = 0; i < data1.length; i++) {
                    //            if (data.Region == data1[i].Key) {  //data是有效的
                    //                $('#Region').combobox('select', data.Region);
                    //                break;
                    //            }
                    //        }
                    //    }
                    //}

                    //4).项目用途
                    $('#ConstructionDoman').combobox('disableValidation');  //不验证是否必填
                    $('#ConstructionDoman').combobox('setValue', '');
                    if (data.ConstructionDoman && data.ConstructionDoman != '') {
                        var data1 = $('#ConstructionDoman').combobox('getData');
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                if (data.ConstructionDoman == data1[i].Key) {  //data是有效的
                                    $('#ConstructionDoman').combobox('select', data.ConstructionDoman);
                                    break;
                                }
                            }
                        }
                    }

                    //5).项目类型
                    $('#ProjectType').combobox('disableValidation');  //不验证是否必填
                    $('#ProjectType').combobox('setValue', '');
                    if (data.ProjectType && data.ProjectType != '') {
                        var data1 = $('#ProjectType').combobox('getData');
                        if (data1.length > 0) {
                            for (var i = 0; i < data1.length; i++) {
                                if (data.ProjectType == data1[i].Key) {  //data是有效的
                                    $('#ProjectType').combobox('select', data.ProjectType);
                                    break;
                                }
                            }
                        }
                    }
                    Action.GetPid1(data.UseQualifications);  //设置使用资质、上报公司

                    //根据省/市加载区域
                    if (data.ProvinceName)  //区域还没加载并且市有值
                    {
                        Action.SeQYBySheng(data.ProvinceName);
                    }
                    if (data.CityName && $('#hdldqy').val() == '0')  //区域还没加载并且市有值
                    {
                        Action.SeQYByShi(data.CityName);
                    }

                    // 保存标注的坐标到隐藏域
                    if (data.longitude == '' || data.longitude == '0' || data.latitude == '' || data.latitude == '0') {
                        //setMarker(data.longitude, data.latitude);
                        $("#longitude").val('0');
                        $("#latitude").val('0');
                    }
                    else {
                        $("#longitude").val(data.longitude);
                        $("#latitude").val(data.latitude);

                        $("#jwd").textbox('setValue', data.longitude + ',' + data.latitude);
                    }
                    //$('#jwd').textbox({ disabled: true });  //经纬度始终禁用

                }
            });

            //7、弹出框
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

        //地图查询坐标--编辑form-查看
        function selectPoint() {
            var lngx = $('#longitude').val();
            var latx = $('#latitude').val();

            var strUrl = '/MarketInfo/MapHtml.html?lng=' + lngx + '&lat=' + latx + '&isuse=0&time=' + new Date().getTime();

            var v = window.showModalDialog(strUrl, "", "Dialogwidth:800px; Dialogheight:360px;help:no;scroll:no;status:no;");
            //alert("v[0]:" + v[0]);
        }

        // 获取默认流程--暂时无用
        function getDefaultSetp() {
            //// 默认显示造价5000万，竞争者为空
            //var formKeys = "BuildCost,ContestantCorpName";
            //var buildCost = "5000";  //造价
            //var contestantCorpName = "123";  //竞争公司

            //var param = { action: "get_default_step_text", flow_name: "ProjectChanceSubmit", form_keys: formKeys, BuildCost: buildCost, ContestantCorpName: contestantCorpName };
            //$.post("/Handler/PublicWorkflow.ashx", param, function (data) {
            //    $("#divWorkflowStep").html(data);
            //});
        }

        //获取传递的操作参数，
        function loadForm() {
            //5.获取传递的操作参数，
            var actionx = getQueryStringByName("action");
            if ($.trim(actionx) == 'xtdetail') {
                Action.Detail();
            }
        }

        //操作
        var Action = {
            //1.详细按钮
            Detail: function () {
                var Id = getQueryStringByName("keys");
                Id = $.trim(Id); 
                $('#Id').val(Id);
                //$("#updMarkfm .easyui-textbox").textbox({ disabled: true });  //禁用编辑form输入框
                //$("#updMarkfm .easyui-combobox").combobox({ disabled: true });//禁用编辑form下拉列表
                //$("#updMarkfm .easyui-datebox").datebox({ disabled: true });//禁用编辑form日期
                //$("#updMarkfm .easyui-numberbox").numberbox({ disabled: true });//禁用编辑form数值输入框

                //1、根据id，获取步骤StepId，InstanceId，跳转到审批页面
                $.ajax({
                    type: "post",
                    url: "/Handler/MaketOperate.ashx?time=" + new Date().getTime(),
                    data: { keys: Id, flag: 'getinstanid'},
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        if (data && data.Message) {
                            $('#updMarkfm').form('load', '/Handler/MaketOperate.ashx?flag=modl&keys=' + Id + '&time=' + new Date().getTime());
                            $("#divWorkflowStepDescription").html('没找到该信息对应流程的实例编号！');
                            //parent.$.messager.alert('提示', data.Message, 'info');
                            return;
                            
                        }
                        var b = data.Id;
                        //alert("b:"+b);

                        //2.隐藏域保存实例及步骤编号审批时使用
                        //$('#step_idx').val(a);
                        $('#instance_idx').val(b);

                        //3、数据绑定到编辑form上，
                        $('#updMarkfm').form('load', '/Handler/MaketOperate.ashx?flag=modl&keys=' + Id + '&time=' + new Date().getTime());

                        //4、加载审批信息--有实例编号时
                        if (b && b != '') {
                            $("#dglistx").datagrid('load', { action: 'approve_list', instance_id: b });
                        }
                        //5.步骤说明
                        var param = { action: Workflow.Action.GetWorkflowDisplay, instance_id: b, time: new Date().getTime() };
                        $.post(Workflow.Handler.Url, param, function (data) {
                            $("#divWorkflowStepDescription").html(data);
                        });
                    },
                    error: function (msg) {
                        parent.$.messager.alert('提示', '获取实例编号失败，请重新选择数据查看详细或确认是否有实例！', 'info');
                        return;
                    }
                });

            },

            //2.取消编辑按钮
            CancelUpd: function () {
                parent.closeAndDestroyModalWindow();  //关闭弹窗
            },

            //3.根据枚举value，获取pvalue,使得使用资质默认选中获取pvalue，然后再根据pvalue，加载上班公司选项，默认选择id项
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
            },

            //4.根据省加载区域列表
            SeQYBySheng: function (value) {
                //区域
            $('#hdldqy').val(0); //区域未加载
            $('#Region').combobox('setValue', 'AD19D170-8266-49BF-B81A-F70B546A6CAC');  //默认其他区域
            if (value.indexOf('北京') != -1 || value.indexOf('河北') != -1 || value.indexOf('辽宁') != -1
                || value.indexOf('吉林') != -1 || value.indexOf('黑龙江') != -1) {
                $('#Region').combobox('setValue', '387D2D88-A7E4-4D5D-B959-0567C478EB55');
                $('#hdldqy').val(1);  //区域已加载
            }
            else if (value.indexOf('上海') != -1 || value.indexOf('江苏') != -1 || value.indexOf('安徽') != -1
                || value.indexOf('浙江') != -1 || value.indexOf('江西') != -1) {
                $('#Region').combobox('setValue', 'A04A0106-8A87-4356-AA77-99D0D82DC24B');
                $('#hdldqy').val(1);
            }
            else if (value.indexOf('天津') != -1) {
                $('#Region').combobox('setValue', 'E0DF24B9-C93B-427E-9853-62688F5BAFBF');
                $('#hdldqy').val(1);
            }
            else if (value.indexOf('河南') != -1 || value.indexOf('山西') != -1 || value.indexOf('陕西') != -1) {
                $('#Region').combobox('setValue', '6A6C5828-B1A1-445A-B8F0-5DAC54A19934');
                $('#hdldqy').val(1);
            }
            else if (value.indexOf('四川') != -1 || value.indexOf('重庆') != -1 || value.indexOf('湖北') != -1
                || value.indexOf('湖南') != -1 || value.indexOf('云南') != -1 || value.indexOf('贵州') != -1) {
                $('#Region').combobox('setValue', '3A2D06AE-66D5-456C-BAED-5F2B972C76F8');
                $('#hdldqy').val(1);
            }
            else if (value.indexOf('广东') != -1 || value.indexOf('海南') != -1) {
                $('#Region').combobox('setValue', 'E86778D9-E511-4A1E-BA2D-407D109067B5');
                $('#hdldqy').val(1);
            }
            else if (value.indexOf('福建') != -1) {
                $('#Region').combobox('setValue', '1913882B-79A0-4066-870E-1AA3C753D166');
                $('#hdldqy').val(1);
            }
        },

            //5.根据市加载区域列表
            SeQYByShi: function (value) {
                if (value.indexOf('青岛') != -1) {
                    $('#Region').combobox('setValue', '1A99F0DC-A234-48B9-83D7-E28C39B9711A');
                }
                else if (value.indexOf('济南') != -1 || value.indexOf('泰安') != -1 || value.indexOf('德州') != -1 || value.indexOf('莱芜') != -1) {
                    $('#Region').combobox('setValue', '648C8C55-E36D-4F06-B359-1383A66DF986');
                }
                else if (value.indexOf('东营') != -1) {
                    $('#Region').combobox('setValue', 'E511A628-3202-43F9-8419-5229C4CB41F6');
                }
                else if (value.indexOf('日照') != -1 || value.indexOf('潍坊') != -1) {
                    $('#Region').combobox('setValue', 'C29F6C7F-9B8A-4749-B373-FBF6D5F45C59');
                }
                else if (value.indexOf('济宁') != -1) {
                    $('#Region').combobox('setValue', 'BD805862-F8EC-427A-8CCE-83480BB818F8');
                }
                else if (value.indexOf('淄博') != -1 || value.indexOf('滨州') != -1) {
                    $('#Region').combobox('setValue', 'CF071599-BF95-445B-AFEE-F07D675B15E5');
                }
                else if (value.indexOf('烟台') != -1 || value.indexOf('威海') != -1) {
                    $('#Region').combobox('setValue', '595B0A9F-A319-48EF-A3B2-F5503FDA39D1');
                }
                else
                    $('#Region').combobox('setValue', 'AD19D170-8266-49BF-B81A-F70B546A6CAC');  //默认其他区域
            }
        }
    </script>
</head>
<body style=" width:915px; " onload="loadForm()">
    <div id="updMarkDiv" style=" padding: 0px; overflow-y: auto;">
        <form id="updMarkfm" class="easyui-form" method="post" data-options="novalidate:true" style="width: 100%;">
            <input id="Id" name="Id" type="hidden" value="0" />
            <%--标识主键id--%>
            <input id="instance_idx" name="instance_idx" type="hidden" />
            <input id="step_idx" name="step_idx" type="hidden" />
            <%--隐藏域保存实例及步骤编号审批时使用--%>
            <input id="updps" name="updps" type="hidden" value="0" />
            <%--是否修改批示，在保存审核批示意见时使用--%>
            <input id="hdldqy" name="hdldqy" type="hidden" value="0"/>
            <%--标识区域是否加载过了--%>
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
                        <td style="width: 91px">工程名称:</td>
                        <td colspan="3">
                            
                            <input id="ProjectName" name="ProjectName" class="easyui-textbox" data-options="required:true,validType:'length[1,150]'" readonly="readonly" style="width: 447px" />
                        </td>
                        <td>登记编号:</td>
                        <td>
                            <input id="RegNo" name="RegNo" class="easyui-textbox" data-options="required:true,missingMessage:'请输入登记编号',validType:['intx','length[1,50]']" readonly="readonly" style="width: 150px;" />
                            <input id="Text1" name="RegNoxx" type="hidden" />
                        </td>
                    </tr>
                    <tr>
                        <td>建设单位名称:</td>
                        <td colspan="3">
                            <input id="BuildCopeName" name="BuildCopeName" class="easyui-textbox" data-options="required:true,validType:'length[1,100]'" readonly="readonly" style="width: 447px" />
                        </td>
                        <td>公司简称:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="CreateCorp" id="CreateCorp" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px">使用资质:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="UseQualifications" id="UseQualifications" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 70px;">上报公司:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="ReportCorp" id="ReportCorp" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 80px;">拟开工时间:</td>
                        <td style="width: 210px">
                            <input id="nkgsj" name="nkgsj" class="easyui-textbox" readonly="readonly" style="width: 149px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>招标方式:</td>
                        <td>
                            <select id="TenderMode" name="TenderMode" class="easyui-combobox" readonly="readonly" style="width: 150px;"
                                data-options="
                                required:true,
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200,
                                editable:false,
                                 url: '/Handler/PublicEnum.ashx?action=enum_auto&pvalue=3D2F9EE2-8096-403C-A32C-B01B49B29D66',
                                valueField: 'Key',
                                textField: 'Value'">
                            </select>
                            <%--<input class="easyui-textbox" type="text" name="TenderMode" id="TenderMode" style="width: 150px;" />--%>
                        </td>
                        <td>项目用途:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="ConstructionDoman" id="ConstructionDoman" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>项目类型:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="ProjectType" id="ProjectType" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>建筑面积(m²):</td>
                        <td>
                            <input id="BuildArea" name="BuildArea" class="easyui-textbox" data-options="required:true,missingMessage:'请输入面积数值',validType:['number','length[1,30]']" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>造价(万元):</td>
                        <td>
                            <input id="BuildCost" name="BuildCost" class="easyui-textbox" data-options="required:true,missingMessage:'请输入造价数值',validType:['number','length[1,30]']" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>登记日期:</td>
                        <td>
                            <input id="CreateTime" name="CreateTime" class="easyui-textbox" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <%--<tr>
                        <td>工程规模:</td>
                        <td colspan="7">
                            <input id="ProjectScope" name="ProjectScope" class="easyui-textbox" title="fsfsf" data-options="required:true,validType:'length[1,250]'" style="width: 746px;" />
                        </td>
                    </tr>--%>
                    <tr>
                        <td>工程描述:</td>
                        <td colspan="7">
                            <input id="ProjectDetail" name="ProjectDetail" class="easyui-textbox" data-options="required:true,multiline:true,validType:'length[1,500]'"
                                readonly="readonly" style="width: 746px; height: 60px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注:</td>
                        <td colspan="7">
                            <input id="Memo" name="Memo" class="easyui-textbox" data-options="required:true,multiline:true,validType:'length[1,500]'" readonly="readonly"
                                style="width: 745px; height: 60px;" />
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
                        <td style="width: 87px;">联系人:</td>
                        <td style="width: 202px">
                            <input id="ContactMan" name="ContactMan" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td style="width: 70px">联系电话:</td>
                        <td style="width: 210px">
                            <input id="ContantPhone" name="ContantPhone" class="easyui-textbox" data-options="required:true,validType:['tel','length[1,30]']" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td>跟踪联系人:</td>
                        <td>
                            <input id="LinkMan" name="LinkMan" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>跟踪人电话:</td>
                        <td>
                            <input id="LinkManPhone" name="LinkManPhone" class="easyui-textbox" data-options="required:true,validType:['tel','length[1,30]']" readonly="readonly" style="width: 150px;" />
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
                        <td style="width: 87px">所属国家:</td>
                        <td style="width: 202px">
                            <input class="easyui-textbox" type="text" name="Country" id="Country" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 70px">所属省:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="Province" id="Province" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td style="width: 60px;">所属市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="City" id="City" readonly="readonly" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>所属区:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="District" id="District" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td>所属区域:</td>
                        <td>
                            <select id="Region" name="Region" class="easyui-combobox" readonly="readonly" style="width: 150px;"
                                data-options="
                                required:true,
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200,
                                editable:false,
                                url: '/Handler/PublicArea.ashx?action=enum_auto',
                                valueField: 'Key',
                                textField: 'Value'">
                            </select>
                        </td>
                        <td>经纬度:</td>
                        <td>
                            <input id="jwd" name="jwd" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'" readonly="readonly"
                                style="width: 150px; " />
                            <input id="longitude" name="longitude" type="hidden" value="0" />
                            <input id="latitude" name="latitude" type="hidden" value="0" />
                            <input id="isuse"  type="hidden" value="0" />
                        </td>
                    </tr>
                    <tr>
                        <td>工程地点:</td>
                        <td colspan="3" style="">
                            <input id="ProjectLocation" name="ProjectLocation" class="easyui-textbox"
                                data-options="required:true,validType:'length[1,150]'" readonly="readonly" style="width: 445px;" />
                        </td>
                        <td colspan="2" align="right">
                            <a href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-world'" onclick="selectPoint();">&nbsp;在地图中选择工程位置</a>
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
                        <td style="width: 87px;">公司名称:</td>
                        <td style="width: 202px">
                            <input id="ContestantCorpName" name="ContestantCorpName" class="easyui-textbox" data-options="validType:'length[1,200]'" readonly="readonly" style="width: 150px;" /></td>

                        <td style="width: 70px;">资质状况:</td>
                        <td style="width: 210px">
                            <input id="ContestantAptitude" name="ContestantAptitude" class="easyui-textbox" data-options="validType:'length[1,50]'" readonly="readonly" style="width: 150px;" /></td>

                        <td style="width: 60px;">联系人:</td>
                        <td>
                            <input id="ContestantContactMan" name="ContestantContactMan" class="easyui-textbox" data-options="validType:'length[1,10]'" readonly="readonly" style="width: 150px;" value="" /></td>
                    </tr>
                    <tr>
                        <td>职务:</td>
                        <td>
                            <input id="ContestantPosition" name="ContestantPosition" class="easyui-textbox" data-options="validType:'length[1,20]'" readonly="readonly" style="width: 150px;" />
                        </td>

                        <td>联系电话:</td>
                        <td>
                            <input id="ContestantTel" name="ContestantTel" class="easyui-textbox" data-options="validType:['tel','length[1,30]']" readonly="readonly" style="width: 150px;" />
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>
            <div class="easyui-panel panel_border">
                <div id="divWorkflowStepDescription" style="padding: 5px;">读取中...</div>
                <%--<script type="text/javascript">
                    $.post("/Handler/PublicWorkflow.ashx", { action: "get_instance_step_text", instance_id: getQueryStringByName("instance_id") }, function (data) {
                        $("#divWorkflowStepDescription").html(data);
                    });
                </script>--%>
                <table id="dglistx" class="easyui-datagrid" style="width: 100%; height: 200px;"
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
