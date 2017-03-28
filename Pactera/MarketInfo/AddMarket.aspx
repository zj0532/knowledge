<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddMarket.aspx.cs" Inherits="Pactera.MarketInfo.AddMarket" %>

<%@ OutputCache Duration="1" VaryByParam="None" %>
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
    <script src="../Scripts/json2.js"></script>
    <%--列表省略号js--%>
    <link href="/Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />
    <style>
        /*显示日期的textbox的样式--灰色*/
        .kgGary {
            border: #d9d9d9 1px solid;
            border-radius: 3px 3px 3px 3px;
            height: 17px;
            font-size: 12px;
            padding-left: 4px;
            padding-top: 2px;
            color: gray;
        }
    </style>
    <script charset="utf-8">
        //将form表单元素值序列化成对象--由于IE下放在公共js中一直不起作用，暂时放在页面
        function serializeObject(form) {
            //alert("开始序列化");
            var o = {};
            $.each(form.serializeArray(), function (index) {
                if (o[this['name']]) {
                    o[this['name']] = o[this['name']] + "," + this['value'];
                    //alert("a:"+this['value']);
                } else {
                    o[this['name']] = this['value'];
                    //alert("b:"+this['value']);
                }
            });
            //alert("序列化结束");
            return o;
        }

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
                        $('#mapbtn').hide();
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
                        $('#mapbtn').show();
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
                required: true,
                onSelect: function (record) {
                    $('#District').combobox('clear');
                    $('#District').combobox('disableValidation');  //不验证是否必填
                    var url = '/Handler/PublicDict.ashx?action=area&AreaType=District&Parent=' + record.Key;
                    $('#District').combobox('reload', url);

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
                required: true,
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
                url: '/Handler/PublicBPM_Corp.ashx?action=info',
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
                    getDefaultSetp();   // 显示默认流程(含有清空审批人)
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
                        $('#City').combobox('setValue', '');
                        if (data.City && data.City != '') {
                            $('#City').combobox({
                                onLoadSuccess: function () {
                                    //设置默认选择第一项
                                    var data1 = $('#City').combobox('getData');
                                    if (data1.length > 0) {
                                        for (var i = 0; i < data1.length; i++) {
                                            //alert("data.City:" + data.City + "///data1[i].Key:" + data1[i].Key);
                                            if (data.City == data1[i].Key) {  //data是有效的
                                                $('#City').combobox('select', data.City);
                                                break;
                                            }
                                        }
                                    }
                                }
                            });
                        }

                        var url3 = '/Handler/PublicDict.ashx?action=area&AreaType=District&Parent=' + data.City;
                        $('#District').combobox('reload', url3);
                        //处理区
                        $('#District').combobox('setValue', '');
                        if (data.District && data.District != '') {
                            $('#District').combobox({
                                onLoadSuccess: function () {
                                    //设置默认选择第一项
                                    var data1 = $('#District').combobox('getData');
                                    if (data1.length > 0) {
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
                    //if (data.Region && data.Region!='') {
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
                        if (data.ProjectType.length == 36) {
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
                    $('#jwd').textbox({ disabled: true });  //经纬度始终禁用

                }
            });

            //7.开始时间变化时，截至时间选择范围变化--现在用WdatePicker日期控件，不用这里限制截至时间的最小日期
            //$('#begintime1').datebox({
            //    onSelect: function (date) {
            //        var stime = $('#begintime1').datebox('getValue');
            //        stime = stime.replace("-", "/");//替换字符，变成标准格式    
            //        var stime1 = new Date(Date.parse(stime));
            //        //alert("begin:" + stime);

            //        $('#endtime1').datebox().datebox('calendar').calendar({
            //            validator: function (date) {
            //                var now = new Date();
            //                var d1 = new Date(now.getFullYear(), now.getMonth(), now.getDate());
            //                var d2 = new Date(now.getFullYear(), now.getMonth(), now.getDate() + 10);
            //                return stime1 <= date;
            //            }
            //        });
            //    }
            //});


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

            //8.造价变化，审批人置空，重新选择
            $('#BuildCost').textbox({
                onChange: function (newValue, oldValue) {
                    //显示默认流程步骤--造价>5000 && 竞争公司!='' ,审批显示3步审批，否则显示2步审批
                    var jz = $('#ContestantCorpName').textbox('getValue');  //竞争公司
                    newValue = $.trim(newValue);
                    var zj = parseFloat(newValue);
                    getDefaultSetp();   // 显示默认流程(含有清空审批人)

                    //造价大于100玩提醒
                    if (zj >= 1000000 && $('#hdtszj').val() == '1') {
                        parent.$.messager.alert('提示', '请注意：造价单位万元！', 'info');
                        return;
                    }
                }
            });

            //9.竞争公司变化，审批人置空，重新选择
            $('#ContestantCorpName').textbox({
                onChange: function (newValue, oldValue) {
                    //显示默认流程步骤--造价>5000 && 竞争公司!='' ,审批显示3步审批，否则显示2步审批
                    var zj1 = $('#BuildCost').textbox('getValue');  //造价
                    var zj = parseFloat(zj1);
                    getDefaultSetp();   // 显示默认流程(含有清空审批人)
                }
            });

            //10.登记编号变化时验证唯一
            $('#RegNo').textbox({
                onChange: function (newValue, oldValue) {
                    if ($('#hdtszj').val() == '1' && $.trim(newValue) != '') {  //编辑验证时验证
                        $.ajax({
                            type: 'post',
                            url: '/Handler/MaketOperate.ashx',
                            data: { newvalue: newValue, flag: 'checkbh', id: $('#Id').val(), RegNo: $('#RegNo').textbox('getValue') },
                            dataType: 'json',
                            success: function (data) {
                                if (data && data.Message) {
                                    parent.$.messager.alert("提示", data.Message, 'info', function () {
                                        $('#RegNo').textbox('clear');
                                        $('#RegNo').focus();
                                    });

                                    return;
                                }
                            }
                        });
                    }
                }
            });

            ////11.工程名称变化时验证唯一
            //$('#ProjectName').textbox({
            //    onChange: function (newValue, oldValue) {
            //        //parent.$.messager.alert("提示", $('#hdtszj').val(), 'info');
            //        if ($('#hdtszj').val() == '1' && $.trim(newValue) != '') {  //编辑验证时验证
            //            $.ajax({
            //                type: 'post',
            //                url: '/Handler/MaketOperate.ashx',
            //                data: { newvalue: newValue, flag: 'checkgcmc', id: $('#Id').val(), ProjectName: $('#ProjectName').textbox('getValue') },
            //                dataType: 'json',
            //                success: function (data) {
            //                    if (data && data.Message) {
            //                        parent.$.messager.alert("提示", data.Message, 'info', function () {
            //                            $('#ProjectName').textbox('clear');
            //                            $('#ProjectName').focus();
            //                        });

            //                        return;
            //                    }
            //                }
            //            });
            //        }
            //    }
            //});
            getDefaultSetp();
        });

        //地图查询坐标-编辑时
        function selectPoint() {
            var lngx = $('#longitude').val();
            var latx = $('#latitude').val();
            var isuse = $('#isuse').val(); //是否可用
            var strUrl = '/MarketInfo/MapHtml.html?lng=' + lngx + '&lat=' + latx + '&isuse=1&time=' + new Date().getTime();
            if ($.trim(isuse) == '0') {
                strUrl = '/MarketInfo/MapHtml.html?lng=' + lngx + '&lat=' + latx + '&isuse=0&time=' + new Date().getTime();
            }
            //var v = $('#mapdiv').window({
            //    href: strUrl,
            //});
            //$('#ifm').attr('src',strUrl);
            //var v=$('#mapdiv').window('open');
            var v = window.showModalDialog(strUrl, "", "Dialogwidth:800px; Dialogheight:360px;help:no;scroll:no;status:no;");
            //alert("v[0]:" + v[0]);
            if ($.trim(isuse) == '1') {   //添加或编辑时，经纬度回填
                if (v[0] && v[0] != '' && v[0] != '0' && v[1] && v[1] != '' && v[1] != '0') {
                    $('#longitude').val(v[0]);
                    $('#latitude').val(v[1]);
                    $('#jwd').textbox('setValue', v[0] + ',' + v[1]);
                    $('#jwd').textbox({ disabled: true });  //经纬度始终禁用
                }
            }
        }

        // 获取默认流程
        function getDefaultSetp() {
            // 默认显示造价5000万，竞争者为空
            var formKeys = "BuildCost,ContestantCorpName";
            var buildCost = "5000";  //造价
            var contestantCorpName = "123";  //竞争公司

            workflowSelectPersonData = undefined;  //清空审批人

            //[{"Field": "字段A的名称", "Value": "字段A的值"}, ...]
            var formValues = [{ Field: "BuildCost", Value: buildCost }] ;
            var param = {
                action: Workflow.Action.GetWorkflowDisplay,
                flow_code: Workflow.FlowCode.ProjectChanceSubmit,
                form_values: JSON.stringify(formValues)
            };
            //alert("url:" + Workflow.Handler.Url + "//action:" + Workflow.Action.GetWorkflowDisplay);
            $.post(Workflow.Handler.Url, param, function (data) {
                $("#divWorkflowStep").html(data);
            });
        }

        //获取传递的操作参数，
        function loadForm() {
            var actionx = getQueryStringByName("action");
            if ($.trim(actionx) == 'add') {
                Action.Add();
            } else if ($.trim(actionx) == 'update') {
                Action.Update();
            } else if ($.trim(actionx) == 'detail') {
                Action.Detail();
            }
        }

        //操作
        var Action = {
            //1.添加操作
            Add: function () {
                $('#thstatex').val(0); //不是退回
                $("#Province").combobox({ disabled: true });//添加时，省市区默认不启用
                $("#City").combobox({ disabled: true });
                $("#District").combobox({ disabled: true });
                $('#Operation').show();  //启用操作div
                $('#Approve').show();    //启用审批div
                $('#ProjectType').combobox('clear');  //工程类型清除

                $('#data1').hide();  //拟开工input隐藏
                $('#data2').show();  //拟开工日期控件显示
                //$('#ditu').linkbutton('enable');  //地图按钮启用

                //下面：默认选择第一项，启用编辑form输入框，下拉列表，及编辑form禁用验证。
                //位置不能颠倒，否则在查看详细后，添加时，上报公司是禁用的
                $('#Country').combobox({  //国家就一项，所以设置默认选择第一项
                    onLoadSuccess: function () {
                        //设置默认选择第一项
                        var data = $('#Country').combobox('getData');
                        if (data.length == 1) {
                            $('#Country').combobox('select', data[0].Key);  //国家
                        }
                    }
                });
                $('#CreateCorp').combobox({  //公司简称就一项时，所以设置默认选择第一项
                    onLoadSuccess: function () {
                        //设置默认选择第一项
                        var data = $('#CreateCorp').combobox('getData');
                        if (data.length == 1) {
                            $('#CreateCorp').combobox('select', data[0].PK_CORP);  //国家
                        }
                    }
                });

                //这4项数值在加载编辑form时，有退回的信息，不可以编辑，所以现在启用
                $('#UseQualifications').combobox({ disabled: false }); //使用资质
                $('#ReportCorp').combobox({ disabled: false }); //上报公司
                $('#CreateCorp').combobox({ disabled: false }); //公司简称
                $('#RegNo').textbox({ disabled: false });  //登记编号

                $("#updMarkfm .easyui-textbox").textbox({ disabled: false });  //启用编辑form输入框
                $("#updMarkfm .easyui-combobox").combobox({ disabled: false });//启用编辑form下拉列表
                $("#updMarkfm .easyui-datebox").datebox({ disabled: false });//启用编辑form日期
                $("#updMarkfm .easyui-numberbox").numberbox({ disabled: false });//启用编辑form数值输入框
                $('#updMarkfm').form('clear');  //重置表单数据
                $('#updMarkfm').form('disableValidation');
                $('#jwd').textbox({ disabled: true });  //经纬度始终禁用
                $('#Id').val('0');  //id
                $('#isuse').val(1); //地图是否可用

                $('#hdtszj').val(1);  //提示造价
                $('#thstatex').val(0);

                //开工时间默认当前日期
                var date = new Date()
                var y = date.getFullYear();
                var m = date.getMonth() + 1;
                var d = date.getDate();
                var rq = y + '-' + (m < 10 ? ('0' + m) : m) + '-' + (d < 10 ? ('0' + d) : d);
                $('#CreateTime').textbox('setValue', rq);

                //显示默认流程步骤
                getDefaultSetp();
            },

            //2.修改操作
            Update: function () {
                $("#updMarkfm .easyui-textbox").textbox({ disabled: false });  //启用编辑form输入框
                $("#updMarkfm .easyui-combobox").combobox({ disabled: false });//启用编辑form下拉列表
                $("#updMarkfm .easyui-datebox").datebox({ disabled: false });//启用编辑form日期
                $("#updMarkfm .easyui-numberbox").numberbox({ disabled: false });//启用编辑form数值输入框

                $('#Operation').show();  //启用操作div
                $('#Approve').show();  //启用审批div

                //显示默认流程步骤
                getDefaultSetp();

                var thstate = 0;  //退回状态，默认没退回
                var state = getQueryStringByName("state");
                if ($.trim(state) == '5') {  //退回信息编辑时4项禁在编辑
                    thstate = 1;
                    $('#UseQualifications').combobox({ disabled: true }); //使用资质
                    $('#ReportCorp').combobox({ disabled: true }); //上报公司
                    $('#CreateCorp').combobox({ disabled: true }); //公司简称
                    $('#RegNo').textbox({ disabled: true });  //登记编号
                }

                //数据绑定到编辑form上
                $('#jwd').textbox({ disabled: true });  //经纬度始终禁用
                $('#isuse').val(1); //地图是否可用
                $('#Id').val(getQueryStringByName("keys")); //给隐藏域主键赋值
                $('#uid').val(getQueryStringByName("uid")); //登记人
                $('#data1').hide();  //拟开工input隐藏
                $('#data2').show();  //拟开工日期控件显示
                //$('#ditu').linkbutton('enable');  //地图按钮启用

                if (thstate == '1')  //隐藏域保存是否是退回信息标识
                    $('#thstatex').val(1);
                else
                    $('#thstatex').val(0);
                $('#hdtszj').val(0); //注意：不能删除，标识是否提示造价及验证工程名称、登记编号，
                $('#updMarkfm').form('load', '/Handler/MaketOperate.ashx?flag=modl&keys=' + getQueryStringByName("keys") + '&datetime=' + new Date().getTime());
                $('#updMarkfm').form('disableValidation');  //加载后默认先不验证，点击保存/提交时在验证
            },

            //3.保存操作
            Save: function () {
                $('#updMarkfm').form('submit', {
                    onSubmit: function (param) {
                        param.flag = 'save';
                        param.thstate = $('#thstatex').val();  //是否退回状态
                        //返回easyui 验证信息
                        //var isValid = $(this).form('enableValidation').form('validate');
                        $(this).form('enableValidation');
                        $('#WorkflowMessage').textbox('disableValidation');  //审批意见：保存时不验证
                        var isValid = $(this).form('validate');
                        var rets = '';
                        //1.验证拟开工日期
                        if ($.trim($('#nkgsj').val()) == '') {
                            rets += '请选择拟开工日期！';
                            isValid = false;
                        }
                        //2、验证经纬度
                        if ($('#Country').combobox('getText') == "中国") {
                            if ($("#longitude").val() == '' || $("#latitude").val() == '' || $("#longitude").val() == '0' || $("#latitude").val() == '0') {
                                rets += '请在地图中选择位置！';
                                isValid = false;
                            }
                        }
                        if (isValid) {
                            $.messager.progress();// 显示进度条
                        }
                        else {
                            if (rets != '') {
                                parent.$.messager.alert("提示", rets, 'info');
                                return false;
                            }
                        }
                        return isValid;
                    },
                    success: function (msg) {
                        $.messager.progress('close');//表单提交成功隐藏进度条
                        parent.$.messager.alert("提示", msg, "info");
                        //var msg = eval('(' + msg + ')');  //json对象转换成js对象
                        //验证登记编号、工程名称,提示并
                        if (msg.indexOf("编号") > 0) {
                            //$("#RegNo").textbox('setText', '');
                            $("#RegNo").focus();
                            return;
                        }
                        if (msg.indexOf("名称") > 0) {
                            //$("#ProjectName").textbox('setText', '');
                            $("#ProjectName").focus();
                            return;
                        }
                        if (msg.indexOf("登记人") > 0) {
                            //$("#ProjectName").textbox('setText', '');
                            //$("#ProjectName").focus();
                            return;
                        }

                        parent.closeAndDestroyModalWindow();  //关闭弹窗
                    }
                });
            },

            //5.取消编辑按钮
            CancelUpd: function () {
                parent.closeAndDestroyModalWindow();  //关闭弹窗
            },

            //6.详细按钮
            Detail: function () {
                $('#Id').val(getQueryStringByName("keys"));
                $('#thstatex').val(0); //不是退回
                $('#Operation').hide();  //禁用操作div
                $('#Approve').hide();  //禁用审批div

                $('#updMarkfm').form('clear');  //重置表单数据
                $("#updMarkfm .easyui-textbox").textbox({ disabled: true });  //禁用编辑form输入框
                $("#updMarkfm .easyui-combobox").combobox({ disabled: true });  //禁用编辑form下拉列表
                $("#updMarkfm .easyui-datebox").datebox({ disabled: true });//禁用编辑form日期
                $("#updMarkfm .easyui-numberbox").numberbox({ disabled: true });//禁用编辑form数值输入框
                $('#isuse').val(0); //地图是否可用
                $('#data2').hide();  //拟开工input隐藏
                $('#data1').show();  //拟开工日期控件显示

                //3.数据绑定到编辑form上
                $('#updMarkfm').form('load', '/Handler/MaketOperate.ashx?flag=modl&keys=' + getQueryStringByName("keys") + '&datetime=' + new Date().getTime());
                $('#Province').combobox('disable');
                $('#Province').combobox({ disabled: true });
            },

            //7.根据枚举value，获取pvalue,使得使用资质默认选中获取pvalue，然后再根据pvalue，加载上班公司选项，默认选择id项
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

            //8.根据省加载区域列表
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

            //9.根据市加载区域列表
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
<body style="width: 915px;" onload="loadForm()">
    <div id="updMarkDiv" style="padding: 0px; overflow-y: auto;">
        <form id="updMarkfm" class="easyui-form" method="post" data-options="novalidate:true" action="/Handler/MaketOperate.ashx" style="width: 100%;">
            <input id="thstatex" name="thstatex" type="hidden" value="0" />
            <%--标识是否是退回--%>
            <input id="hdtszj" type="hidden" value="0" />
            <%--是否提示造价--%>
            <input id="Id" name="Id" type="hidden" value="0" />
            <%--标识主键id--%>
            <input id="uid" name="uid" type="hidden" />
            <%--表示该信息的登记人，用于在保存时验证是否是当前人是否有权修改--%>
            <input id="hdldqy" name="hdldqy" type="hidden" value="0" />
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

                            <input id="ProjectName" name="ProjectName" class="easyui-textbox" data-options="required:true,validType:'length[1,150]'" style="width: 447px" />
                        </td>
                        <td>登记编号:</td>
                        <td>
                            <input id="RegNo" name="RegNo" class="easyui-textbox" data-options="required:true,missingMessage:'请输入登记编号',validType:['intx','length[1,50]']" style="width: 150px;" />
                            <input id="Text1" name="RegNoxx" type="hidden" />
                        </td>
                    </tr>
                    <tr>
                        <td>建设单位名称:</td>
                        <td colspan="3">
                            <input id="BuildCopeName" name="BuildCopeName" class="easyui-textbox" data-options="required:true,validType:'length[1,100]'" style="width: 447px" />
                        </td>
                        <td>公司简称:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="CreateCorp" id="CreateCorp" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 120px">使用资质:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="UseQualifications" id="UseQualifications" style="width: 150px;" />
                        </td>
                        <td style="width: 70px;">上报公司:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="ReportCorp" id="ReportCorp" style="width: 150px;" />
                        </td>
                        <td style="width: 80px;">拟开工时间:</td>
                        <td style="width: 210px">
                            <div id="data1">
                                <input id="nkgsj1" name="nkgsj1" class="kgGary" readonly="readonly" style="width: 144px; display: block" />
                            </div>
                            <div id="data2">
                                <input id="nkgsj" name="nkgsj" class="Wdate" type="text" onclick="WdatePicker({ minDate: '#F{$dp.$D(\'CreateTime\',{d:1})}' });"
                                    maxlength="10" style="width: 143px;" />
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td>招标方式:</td>
                        <td>
                            <select id="TenderMode" name="TenderMode" class="easyui-combobox" style="width: 150px;"
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
                            <input class="easyui-textbox" type="text" name="ConstructionDoman" id="ConstructionDoman" style="width: 150px;" />
                        </td>
                        <td>项目类型:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="ProjectType" id="ProjectType" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>建筑面积(m²):</td>
                        <td>
                            <input id="BuildArea" name="BuildArea" class="easyui-textbox" data-options="required:true,missingMessage:'请输入面积数值',validType:['number','length[1,30]']" style="width: 150px;" />
                        </td>
                        <td>造价(万元):</td>
                        <td>
                            <input id="BuildCost" name="BuildCost" class="easyui-textbox" data-options="required:true,missingMessage:'请输入造价数值',validType:['number','length[1,30]']" style="width: 150px;" />
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
                                style="width: 746px; height: 60px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>备注:</td>
                        <td colspan="7">
                            <input id="Memo" name="Memo" class="easyui-textbox" data-options="required:true,multiline:true,validType:'length[1,500]'"
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
                            <input id="ContactMan" name="ContactMan" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'" style="width: 150px;" />
                        </td>

                        <td style="width: 70px">联系电话:</td>
                        <td style="width: 210px">
                            <input id="ContantPhone" name="ContantPhone" class="easyui-textbox" data-options="required:true,validType:['tel','length[1,30]']" style="width: 150px;" />
                        </td>

                        <td>跟踪联系人:</td>
                        <td>
                            <input id="LinkMan" name="LinkMan" class="easyui-textbox" data-options="required:true,validType:'length[1,30]'" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>跟踪人电话:</td>
                        <td>
                            <input id="LinkManPhone" name="LinkManPhone" class="easyui-textbox" data-options="required:true,validType:['tel','length[1,30]']" style="width: 150px;" />
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
                            <input class="easyui-textbox" type="text" name="Country" id="Country" style="width: 150px;" />
                        </td>
                        <td style="width: 70px">所属省:</td>
                        <td style="width: 210px">
                            <input class="easyui-textbox" type="text" name="Province" id="Province" style="width: 150px;" />
                        </td>
                        <td style="width: 60px;">所属市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="City" id="City" style="width: 150px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>所属区:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="District" id="District" style="width: 150px;" />
                        </td>
                        <td>所属区域:</td>
                        <td>
                            <select id="Region" name="Region" class="easyui-combobox" style="width: 150px; color: gray;" readonly="readonly"
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
                                style="width: 150px; color: gray;" />
                            <input id="longitude" name="longitude" type="hidden" value="0" />
                            <input id="latitude" name="latitude" type="hidden" value="0" />
                            <input id="isuse" type="hidden" value="0" />
                        </td>
                    </tr>
                    <tr>
                        <td>工程地点:</td>
                        <td colspan="3" style="">
                            <input id="ProjectLocation" name="ProjectLocation" class="easyui-textbox"
                                data-options="required:true,validType:'length[1,150]'" style="width: 445px;" />
                        </td>
                        <td colspan="2" align="right">
                            <a id="mapbtn" href="javascript:void(0)" class="easyui-linkbutton" data-options="iconCls:'icon-world'" onclick="selectPoint();">&nbsp;在地图中选择工程位置</a>
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
                            <input id="ContestantCorpName" name="ContestantCorpName" class="easyui-textbox" data-options="validType:'length[1,200]'" style="width: 150px;" /></td>

                        <td style="width: 70px;">资质状况:</td>
                        <td style="width: 210px">
                            <input id="ContestantAptitude" name="ContestantAptitude" class="easyui-textbox" data-options="validType:'length[1,50]'" style="width: 150px;" /></td>

                        <td style="width: 60px;">联系人:</td>
                        <td>
                            <input id="ContestantContactMan" name="ContestantContactMan" class="easyui-textbox" data-options="validType:'length[1,10]'" style="width: 150px;" value="" /></td>
                    </tr>
                    <tr>
                        <td>职务:</td>
                        <td>
                            <input id="ContestantPosition" name="ContestantPosition" class="easyui-textbox" data-options="validType:'length[1,20]'" style="width: 150px;" />
                        </td>

                        <td>联系电话:</td>
                        <td>
                            <input id="ContestantTel" name="ContestantTel" class="easyui-textbox" data-options="validType:['tel','length[1,30]']" style="width: 150px;" />
                        </td>
                        <td></td>
                        <td></td>
                    </tr>
                </table>
            </div>
            <!--审批信息-->
            <div id="Approve">
                <input type="hidden" id="WorkflowPersion" name="WorkflowPersion" />
                <div id="divSelectWorkflowPersion"></div>
                <%--弹窗--%>
                <div class="easyui-panel panel_border">
                    <div class="content-1">
                        <dl>
                            <dt>
                                <img src="../Scripts/jquery-easyui-1.4.1/themes/pactera/images/xiaoicon.png" /></dt>
                            <dd>审批信息</dd>
                        </dl>
                    </div>
                    <table cellpadding="5" style="margin-top: -10px;">
                        <tr>
                            <td style="width: 87px">请选择审批人:</td>
                            <td>
                                <a id="lbtnChoiceApprovePersion" class="linkbutton">选择审批人</a>
                            </td>
                        </tr>
                        <tr>
                            <td>审批流程说明:</td>
                            <td>
                                <div id="divWorkflowStep">...</div>
                            </td>
                        </tr>
                        <tr>
                            <td>审批意见:</td>
                            <td>
                                <input class="easyui-textbox" id="WorkflowMessage" name="WorkflowMessage" data-options="required:true,multiline:true,validType:'length[1,500]'" style="height: 80px; width: 750px;" />
                            </td>
                        </tr>
                    </table>
                </div>
            </div>
            <div id="Operation" style="float: right; margin-top: 15px; margin-right: 20px; height: 40px;">
                <a class="easyui-linkbutton linkbutton" style="margin-left: 10px;" id="lbtnSubmit">提交</a>
                <a class="easyui-linkbutton linkbutton" style="margin-left: 10px;" onclick="Action.Save()">暂存</a>
                <a class="easyui-linkbutton linkbutton" style="margin-left: 10px;" onclick="Action.CancelUpd()">取消</a>
            </div>
            <script type="text/javascript">
                var workflowSelectPersonData;  //变量：审批人

                $("#lbtnChoiceApprovePersion").linkbutton({
                    onClick: function () {
                        var formKeys = "BuildCost,ContestantCorpName";
                        var buildCost = $('#BuildCost').val();  //造价
                        var contestantCorpName = $('#ContestantCorpName').val();  //竞争公司
                        buildCost = $.trim(buildCost);
                        contestantCorpName = $.trim(contestantCorpName);

                        var CreateCorp = $('#CreateCorp').combobox('getValue');  //公司简称
                        CreateCorp = $.trim(CreateCorp);

                        if (buildCost == '') {
                            parent.$.messager.alert('提示', '请输入工程造价！', 'info');
                            return;
                        }
                        else if (CreateCorp == '') {
                            parent.$.messager.alert('提示', '请先选择公司简称！', 'info');
                            return;
                        }
                        else {
                            var url = '/Workflow/SelectApprovePersonWindow.aspx?flow_name=ProjectChanceSubmit&pk_corp=' + CreateCorp;
                            url += '&form_keys=' + formKeys;
                            url += '&BuildCost=' + buildCost;
                            url += '&ContestantCorpName=' + contestantCorpName + '&persons=' + workflowSelectPersonData + '&r=' + Math.random();
                            parent.createAndOpenModalWindow('选择人员', '600', '400', url, function (data) {
                                if (data != null && data != '') {
                                    //保存审批人
                                    workflowSelectPersonData = undefined;
                                    for (var prop in data) {
                                        if (workflowSelectPersonData == undefined) {
                                            workflowSelectPersonData = prop + "_" + data[prop];
                                            continue;
                                        }
                                        workflowSelectPersonData += ";" + prop + "_" + data[prop];
                                    }
                                    //alert("workflowSelectPersonData:" + workflowSelectPersonData);

                                    // 显示审批步骤人员
                                    var formValues = [{ Field: "BuildCost", Value: buildCost }, { Field: "ContestantCorpName", Value: contestantCorpName }];
                                    var param = {
                                        action: Workflow.Action.GetWorkflowDisplay,
                                        flow_code: Workflow.FlowCode.ProjectChanceSubmit,
                                        flow_persons: JSON.stringify(data),
                                        form_values: JSON.stringify(formValues)
                                    };
                                    $.post(Workflow.Handler.Url, param, function (data) {
                                        $("#divWorkflowStep").html(data);
                                    });
                                }
                            });
                        }
                    }
                });
            </script>
            <script type="text/javascript">
                //提交按钮--开始进入审批流程
                $("#lbtnSubmit").linkbutton({
                    onClick: function () {
                        $('#updMarkfm').form('submit', {
                            onSubmit: function (param) {
                                param.flag = 'submit';  //添加参数
                                param.thstate = $('#thstatex').val();  //是否退回状态
                                //返回easyui 验证信息
                                var isValid = $(this).form('enableValidation').form('validate');
                                var rets = '';
                                if (isValid) {
                                    //1.验证拟开工日期
                                    if ($.trim($('#nkgsj').val()) == '') {
                                        rets += '请选择拟开工日期！';
                                        isValid = false;
                                    }

                                    //2、验证选择审批人
                                    if (workflowSelectPersonData == null || workflowSelectPersonData == '') {
                                        rets += '请选择审批人！';
                                        isValid = false;
                                    }
                                    //3、验证经纬度
                                    if ($('#Country').combobox('getText') == "中国") {
                                        if ($("#longitude").val() == '' || $("#latitude").val() == '' || $("#longitude").val() == '0' || $("#latitude").val() == '0') {
                                            rets += '请在地图中选择位置！';
                                            isValid = false;
                                        }
                                    }
                                }
                                if (isValid) {
                                    $.messager.progress();// 显示进度条
                                    param.flow_persons = workflowSelectPersonData;  //验证通过，保存审批人
                                }
                                else {
                                    if (rets != '') {
                                        parent.$.messager.alert("提示", rets, 'info');
                                        return false;
                                    }
                                }
                                return isValid;
                            },
                            success: function (msg) {
                                $.messager.progress('close');//表单提交成功隐藏进度条
                                parent.$.messager.alert("提示", msg, "info");
                                //var msg = eval('(' + msg + ')');  //json对象转换成js对象
                                //验证登记编号、工程名称,提示并
                                if (msg.indexOf("编号") > 0) {
                                    //$("#RegNo").textbox('setText', '');
                                    $("#RegNo").focus();
                                    return;
                                }
                                if (msg.indexOf("名称") > 0) {
                                    //$("#ProjectName").textbox('setText', '');
                                    $("#ProjectName").focus();
                                    return;
                                }
                                if (msg.indexOf("登记人") > 0) {
                                    //$("#ProjectName").textbox('setText', '');
                                    //$("#ProjectName").focus();
                                    return;
                                }
                                parent.closeAndDestroyModalWindow();  //关闭弹窗
                            }
                        });
                    }
                });
            </script>
        </form>
    </div>
</body>
</html>
