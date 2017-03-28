<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConflictCoordination.aspx.cs" Inherits="Pactera.MarketInfo.ConflictCoordination" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>冲突协调</title>
    <link href="../Scripts/My97DatePicker/skin/WdatePicker.css" rel="stylesheet" />
    <script src="../Scripts/My97DatePicker/WdatePicker.js"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.min.js" charset="utf-8"></script>
    <script src="../Scripts/jquery-easyui-1.4.1/jquery.easyui.min.js" charset="utf-8"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/easyui-pactera.css" rel="stylesheet" />
    <link href="../Scripts/jquery-easyui-1.4.1/themes/icon.css" rel="stylesheet" />
    <script src="../Scripts/jquery-easyui-1.4.1/locale/easyui-lang-zh_CN.js" charset="utf-8"></script>
    <script src="../Scripts/public.js"></script>
    <link href="../Scripts/jquery-easyui-1.4.1/themes/default/pactera-workflow-description.css" rel="stylesheet" />
    <%--列表省略号js--%>
    <link href="/Scripts/jquery-easyui-1.4.1/themes/diy_style.css" rel="stylesheet" />
    <script charset="utf-8">
        //下面3个全局变量，用于键盘快捷键操作
        var vTmpField = '';      //对应的字段
        var vTmpFieldValue = ''; //复制的值
        var myhash = new Array();  //复制的值的数组


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
            //1.选择登记人
            $('#CreateUser').combobox({
                url: '/Handler/BPM_User.ashx?action=auto_complete1',
                mode: 'remote',  //用户输入将被发送到名为'q'的HTTP请求参数到服务器检索新数据。
                method: 'post',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                textField: 'Value',
                valueField: 'Key',
                required: true,//验证非空
                novalidate: true,//提交表单时在验证
                missingMessage: '该输入项为必选项',//验证提示
                onSelect: function (record) {
                    $('#CreateUserId').val(record.Key);
                    //alert("用户id：" + $('#CreateUserId').val());
                }
            });

            //2.查找时帅选省市区
            $('#comsheng').combobox({
                url: '/Handler/PublicDict.ashx?action=queryarea&AreaType=Province',
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                onSelect: function (record) {
                    //1).市重新加载
                    $('#comshi').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=City&Parent=' + record.Key;
                    $('#comshi').combobox('reload', url);
                    //2).区重新加载
                    $('#comv').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=0';
                    $('#comv').combobox('reload', url);
                    //3).省-全部：区清空
                    //if (record.Key == '0') {  
                    //    $('#comv').combobox('clear');
                    //    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=0';
                    //    $('#comv').combobox('reload', url);
                    //}
                }
            });
            $('#comshi').combobox({
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                onSelect: function (record) {
                    $('#comv').combobox('clear');
                    var url = '/Handler/PublicDict.ashx?action=queryarea&AreaType=District&Parent=' + record.Key;
                    $('#comv').combobox('reload', url);
                }
            });
            $('#comv').combobox({
                valueField: 'Key',
                textField: 'Value',
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false
            });

            //3.gsjc-公司简称-查询
            $('#gsjc').combobox({
                panelHeight: 'auto',
                panelMinHeight: 20,
                panelMaxHeight: 200,
                editable: false,
                url: '/Handler/PublicBPM_Corp.ashx?action=info2',
                mode: 'remote',
                method: 'post',
                valueField: 'PK_CORP',
                textField: 'UNITSHORTNAME',
                onSelect: function (record) {
                    if (record.PK_CORP) {
                        $('#gsjc1').val(record.PK_CORP);
                    }
                }
            });

            //4.加载列表table
            $('#tblist').datagrid({
                url: '/Handler/MaketOperate.ashx',
                //toolbar: '#tooldiv',
                idField: 'Id',  //主键
                pagination: true, //是否分页-yes
                nowrap: true,  //是否换行-no
                fitcolumns: true, //列自动展开/收缩到合适的DataGrid宽度。
                singleSelect: true, //是否单选一行-yes
                width: '100%',
                rownumbers: true,
                height: 315,
                columns: [[{  //halign:设置表头对齐方式，align：设置列对齐方式，默认left
                    field: 'CreateCorp1', title: '公司简称', width: '8%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';   //value用""引起来，在value包含空格时，能提示全部内容，否则就提示空格前的部分内容
                    }
                }, {
                    field: 'RegNo', title: '登记编号', width: '6%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'BuildCopeName', title: '建设单位', width: '8%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'ProjectName',
                    title: '工程名称', width: '10%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'ProvinceName',
                    title: '所属省市', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'CityName',
                    title: '所属地市', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'DistrictName',
                    title: '所属区县', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'ProjectLocation',
                    title: '工程地点', width: '7%', halign: 'center', sortable: true,
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'CreateTime',
                    title: '登记时间', width: '6%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'EndTime',
                    title: '上报时间', width: '10%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                //{
                //    field: 'BuildArea',
                //    title: '建筑面积(m²)', width: '6%', halign: 'center', sortable: true, align: 'right',
                //    formatter: function (value, row, index) {
                //        return '<span title=' + value + '>' + value + '</span>';
                //    }
                //}, 
                {
                    field: 'BuildCost',
                    title: '造价(万元)', width: '6%', halign: 'right', sortable: true, align: 'right',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'UserName',
                    title: '登记人', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }, {
                    field: 'ApprovalStatus',
                    title: '审批状态',
                    width: '6%',
                    halign: 'center',
                    sortable: true,
                    align: 'left',
                    formatter: function (value, row, index) {
                        if (value == '1') {
                            return '审批中';
                        } else if (value == '4') {
                            return '审批通过';
                        } else if (value == '5') {
                            return '审批未通过';
                        }
                    }
                },
                {
                    field: 'CoordinationResult1',
                    title: '协调结果', width: '5%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                },
                {
                    field: 'CoordinationMessage',
                    title: '批示意见', width: '6%', halign: 'center', sortable: true, align: 'center',
                    formatter: function (value, row, index) {
                        return '<span title="' + value + '">' + value + '</span>';
                    }
                }
                //{
                //    field: 'longitude',
                //    title: '经度', width: '5%', halign: 'center', sortable: true, align: 'center',
                //    formatter: function (value, row, index) {
                //        return '<span title=' + value + '>' + value + '</span>';
                //    }
                //},
                //{
                //    field: 'latitude',
                //    title: '纬度', width: '5%', halign: 'center', sortable: true, align: 'center',
                //    formatter: function (value, row, index) {
                //        return '<span title=' + value + '>' + value + '</span>';
                //    }
                //}
                ]],
                //表格加载成功
                onLoadSuccess: function (data) {
                    ////1、左侧分页table
                    //var table = $("#dataPagerButtonSpace").parent().parent().parent().parent();
                    //var options = $('#tblist').datagrid('getPager').data("pagination").options;
                    //var total = options.total;
                    //var pagecount = Math.ceil(total / options.pageSize);
                    //$(table).find("td").each(function (index) {
                    //    if (index == 7) {
                    //        $(this).html('页&nbsp;&nbsp;共 ' + pagecount + ' 页，共 ' + total + ' 条&nbsp;&nbsp;');
                    //    }
                    //});

                    //2、右侧命令table
                    var table1 = $("#btn_export").parent().parent().parent().parent();
                    $(table1).find("td").each(function (index) {
                        if (index == 0) {
                            //alert("a:" + $('#hdbj').val());
                            var bjx = $('#hdbj').val();  //筛选的半径

                            //半径：在筛选后，显示筛选的半径，否则，默认2
                            if ($.trim(bjx) != '') {
                                $(this).html('半径:<input class="pagination-num" id="sxbjid" name="sxbjid" type="text" size="2" value=' + bjx + '>km');
                            } else {
                                $(this).html('半径:<input class="pagination-num" id="sxbjid" name="sxbjid" type="text" size="2" value="2">km');
                            }
                        }
                    });
                },
                //点击行，根据选中行的审批状态，设置修改和删除按钮是否可用
                onClickRow: function (rowIndex, rowData) {
                    //1)、批示
                    if (rowData.ApprovalStatus != '1') {
                        $('#btn_pishi').linkbutton('disable'); //禁用
                    }
                    else
                        $('#btn_pishi').linkbutton('enable');  //启用

                    //2)、取消批示
                    //if (rowData.ApprovalStatus != '4') {
                    //    $('#btn_calps').linkbutton('disable'); //禁用
                    //}
                    //else
                    //    $('#btn_calps').linkbutton('enable');

                    //3)、修改批示/取消批示
                    if (rowData.ApprovalStatus != '5' && rowData.ApprovalStatus != '4') {
                        $('#btn_updps').linkbutton('disable');//禁用
                        $('#btn_calps').linkbutton('disable');
                    }
                    else {
                        $('#btn_updps').linkbutton('enable');
                        $('#btn_calps').linkbutton('enable');
                    }

                },
                //双击，显示详细
                onDblClickRow: function (rowIndex, rowData) {
                    var url = '/MarketInfo/DetailInfo.aspx?action=xtdetail&keys=' + rowData.Id + '&time=' + new Date().getTime();
                    parent.createAndOpenModalWindow('查看工程信息', 950, 500, url, function (data) {

                    });
                },
                //列单机事件--行索引、列名、单元格的值
                onClickCell: function (rowIndex, field, value) {
                    vTmpField = field;
                    vTmpFieldValue = document.all ? document.selection.createRange().text : document.getSelection();
                    //vTmpFieldValue = document.selection.createRange().text;  //运用IE滤镜获取数据,目前测试，IE支持，但火狐好像不支持，有这行代码，表格不能选中
                }
            });

            //5.Grid分页
            //$.fn.pagination.defaults.displayMsg = '';  //显示数据条数：不显示
            //var pager = $('#tblist').datagrid().datagrid('getPager');	// get the pager of datagrid

            //设置分页控件
            var pager = $("#tblist").datagrid("getPager");
            $(pager).pagination({
                beforePageText: "第",//页数文本框前显示的汉字 
                afterPageText: "页 共 {pages} 页",
                displayMsg: "当前显示 {from} - {to} 条记录   共 {total} 条记录"
            });

            //6.给Grid添加按钮
            AddGridBtn(pager);

            //7、验证造价范围
            $('#zj2').textbox({
                onChange: function (newValue, oldValue) {
                    var zj1 = $('#zj1').textbox('getValue');  //竞争公司
                    zj1 = $.trim(zj1);

                    newValue = $.trim(newValue);
                    if (zj1 != '' && newValue != '') {
                        var zj1x = parseFloat(zj1);
                        var zj2x = parseFloat(newValue);
                        if (zj1x >= zj2x) {
                            parent.$.messager.alert('提示', '造价的截至数值必须大于开始数值', 'info');
                            return;
                        }
                    }
                }
            });
        });

        //表格命令按钮
        function AddGridBtn(pager) {
            pager.pagination({
                buttons: [{
                    //id: 'dataPagerButtonSpace'
                },
                {
                    id: 'btn_shaixuan',
                    iconCls: 'icon-edit-filter',
                    text: '系统筛选',
                    handler: function () {
                        Action.Shaixuan();
                    }
                },
                {
                    id: 'btn_pishi',
                    iconCls: 'icon-edit',
                    text: '批示',
                    handler: function () {
                        Action.Pishi();
                    }
                },
                {
                    id: 'btn_calps',
                    iconCls: 'icon-remove',
                    text: '取消批示',
                    handler: function () {
                        Action.cancelPishi();
                    }
                }, {
                    id: 'btn_updps',
                    iconCls: 'icon-edit',
                    text: '修改批示',
                    handler: function () {
                        Action.UpdPishi();
                    }
                }, {
                    id: 'btn_info',
                    iconCls: 'icon-info',
                    text: '详细信息',
                    handler: function () {
                        Action.Detail();
                    }
                }, {
                    id: 'btn_export',
                    iconCls: 'icon-export',
                    text: '导出',
                    handler: function () {
                        ExportInfo();
                    }
                }]
            });
            //$("#dataPagerButtonSpace").removeAttr("class").removeAttr("href").removeAttr("group");
            //var table = $("#dataPagerButtonSpace").parent().parent().parent().parent();
            //var datagridPager = $("#dataPagerButtonSpace").parent().parent().parent().parent().parent();
            //$(datagridPager).css("position", "relative");
            //var newTable = $(table).clone().css("position", "absolute").css("top", "0px").css("right", "10px");
            //// 移除旧TD
            //$(table).find("td").each(function (index) {
            //    if (index > 14) $(this).remove();
            //});
            //// 移除多余的分页TD
            //$(newTable).find("td").each(function (index) {
            //    if (index <= 14) $(this).remove();
            //});
            //datagridPager.append(newTable);
            //// 这里添加事件，需要重新添加，克隆后对象的属性关联不过来
            //$("#btn_shaixuan").click(function () {
            //    Action.Shaixuan();
            //});
            //$("#btn_pishi").click(function () {
            //    if ($('#hdused1').val() == "1") {
            //        Action.Pishi();
            //    }
            //});
            //$("#btn_calps").click(function () {
            //    if ($('#hdused2').val() == "1") {
            //        Action.cancelPishi();
            //    }
            //});
            //$("#btn_updps").click(function () {
            //    if ($('#hdused3').val() == "1") {
            //        Action.UpdPishi();
            //    }
            //});
            //$("#btn_info").click(function () {
            //    Action.Detail();
            //});
            //$("#btn_export").click(function () {
            //    ExportInfo();
            //});
            //$('.datagrid-header-rownumber').text('序号');
        }

        //以下键盘事件
        $(document).keyup(function (e) {
            var key = e.which;
            //shift + f 组合查询条件
            if (event.shiftKey == true && key == 70) {
                myhash[vTmpField] = vTmpFieldValue;
                showTiaoJianFun();
            }
            //shift + c 清空查询条件
            if (event.shiftKey == true && key == 67) {
                for (key in myhash) {
                    delete myhash[key];
                    try {
                        //$('#' + key + 'Input').textbox('setValue', '');
                        $('#' + key + 'Input').val('');
                    }
                    catch (e) { }
                }
                vTmpField = '';
                vTmpFieldValue = '';
                showTiaoJianFun();
            }
            //shift + enter 执行查询
            if (event.shiftKey == true && key == 13) {
                //Action.DeailSerch();
                Action.Search();
            }
        });
        //public func 绑定查询条件
        function showTiaoJianFun() {
            var v = '';
            for (key in myhash) {
                //alert(myhash[key]);
                v += key + '=' + myhash[key] + '';
                try {
                    //$('#' + key + 'Input').textbox('setValue', myhash[key]);
                    $('#' + key + 'Input').val( myhash[key]);
                }
                catch (e) {
                }
            };
        }
        //以上键盘事件


        //导出
        function ExportInfo() {
            var issel = $('#hdsel').val();

            if ($.trim(issel) == '1') {
                //alert("查询导出");
                $('#serMarkfm').submit();
            }
            else if ($.trim(issel) == '0') {
                //alert("经纬度导出");
                $('#jwdfm').submit();
            }
        }

        //编辑操作
        var Action = {
            //1.查询
            Search: function () {
                //alert("Search");
                //1、验证开始结束时间是否都输入
                var st = $('#begintime1').val();
                var et = $('#endtime1').val();
                st = $.trim(st);
                et = $.trim(et);
                //alert("st:"+st+"///et:"+et);
                if ((st != '' && et == '') || (st == '' && et != '')) {
                    parent.$.messager.alert('提示', '请输入开始和截止时间！', 'info');
                    return;
                }
                    //2、验证批示开始结束时间是否都输入
                    //st = $('#psbegintime').val();
                    //et = $('#psendtime').val();
                    //st = $.trim(st);
                    //et = $.trim(et);
                    ////alert("st:"+st+"///et:"+et);
                    //if ((st != '' && et == '') || (st == '' && et != '')) {
                    //    parent.$.messager.alert('提示', '请输入批示开始和截止时间！', 'info');
                    //    return;
                    //}

                    //3、验证造价范围
                    //var zj1 = $("zj1").textbox('getValue');
                    //var zj2 = $("zj2").textbox('getValue');
                    //zj1 = $.trim(zj1);
                    //zj2 = $.trim(zj2);
                    //if ((zj1 != '' && zj2 == '') || (zj1 == '' && zj2 != '')) {
                    //    parent.$.messager.alert('提示', '请输入造价的开始和截止数值！', 'info');
                    //    return;
                    //}
                else {
                    $('#tblist').datagrid('load', serializeObject($('#serMarkfm')));
                    $('#hdsel').val(1);  //查询
                }
            },

            //2.筛选--根据选择行的经纬度查询
            Shaixuan: function () {
                //1、验证半径输入是否有效
                var bj = $('#sxbjid').val();
                if (!$.isNumeric($.trim(bj))) {
                    parent.$.messager.alert('提示', '输入的半径R：0<=R<=100km ！', 'info');
                    return;
                }
                //2.验证半径范围
                var bj1 = parseFloat(bj);
                if (bj1 < 0 || bj1 > 100) {
                    parent.$.messager.alert('提示', '输入的半径R：0<=R<=100km ！', 'info');
                    return;
                }

                //3、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要参照筛选的数据！', 'info');
                    return;
                }
                //alert("a:" + row.longitude);
                //alert("b:" + row.latitude);

                //2、检测行的经纬度是否有效
                if (row.longitude == '' || row.latitude == '' || row.longitude == '0' || row.latitude == '0') {
                    parent.$.messager.alert('提示', '请选择经纬度有效的数据进行筛选！', 'info');
                    return;
                }

                $('#tblist').datagrid('load', { flag: 'shaixuan', keys: row.Id, bj: bj1 });
                $('#hdcode').val(row.Id); //保存id
                $('#hdsel').val(0);  //筛选
                $('#hdbj').val(bj1);
                //$.ajax({
                //    type: "post",
                //    url: "/Handler/MaketOperate.ashx",
                //    data: { flag: 'shaixuan', keys: row.Id, bj: bj1 },
                //    dataType: "json",
                //    async: false,
                //    success: function (data) {
                //        if (data && data.Message) {
                //            parent.$.messager.alert('提示', data.Message, 'info');
                //            return;
                //        }
                //        $('#hdcode').val(row.Id); //保存id
                //        $('#hdsel').val(0);  //筛选
                //        $('#hdbj').val(bj1);

                //    }
                //});
                
            },

            //3.查询重置
            ClearSer: function () {
                $('#serMarkfm').form('clear');  //重置表单数据
                //清空快捷键保存的value
                for (key in myhash) {
                    delete myhash[key];
                    try {
                        //$('#' + key + 'Input').textbox('setValue', '');
                        $('#' + key + 'Input').val('');
                    }
                    catch (e) { }
                }

                // 指定了排序顺序的列
                $('#tblist').datagrid('sort', {	        
                    sortName: 'EndTime',
                    sortOrder: 'desc'
                });

                $('#tblist').datagrid('load', { flag: 'ctxt' });

                $('#hdsel').val(1);  //查询
                $('#filename').val('冲突协调列表.xls');
                $('#sheetname').val('冲突协调列表');
            },

            //4.详细按钮
            Detail: function () {
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要查看的数据！', 'info');
                    return;
                }
                var url = '/MarketInfo/DetailInfo.aspx?action=xtdetail&keys=' + row.Id + '&time=' + new Date().getTime();
                parent.createAndOpenModalWindow('查看工程信息', 950, 500, url, function (data) {

                });
            },

            //5.批示按钮
            Pishi: function () {
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要批示的数据！', 'info');
                    return;
                }

                //2、判断审批状态
                if (row.ApprovalStatus != '1') {
                    parent.$.messager.alert('提示', '只有状态是“审批中”的数据可以批示！', 'info');
                    return;
                }
                $.ajax({
                    type: "post",
                    url: "/Handler/MaketOperate.ashx",
                    data: { keys: row.Id, flag: 'getshenpi', isps: '1', time: new Date().getTime() },
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        if (data && data.Message) {
                            parent.$.messager.alert('提示', data.Message, 'info');
                            return;
                        }

                        var url = '/MarketInfo/PiShiXT.aspx?action=pishi&keys=' + row.Id + '&a=' + data.StepId + '&b=' + data.InstanceId + '&time=' + new Date().getTime();
                        parent.createAndOpenModalWindow('冲突协调', 950, 500, url, function (data) {
                            $('#tblist').datagrid('reload');  //重新加载列表
                        });
                    },
                    error: function (msg) {
                        parent.$.messager.alert('提示', '获取批示步骤及实例编号失败，请重新选择数据进行批示或确认是否有实例及批示步骤！', 'info');
                        return;
                    }
                });
            },

            //2.修改批示按钮--已完成/退回的，改为退回状态
            UpdPishi: function () {
                $('#updps').val(1);  //表示是否修改批示，在保存审核批示意见时使用
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要修改批示的数据！', 'info');
                    return;
                }

                //2、判断审批状态
                if (row.ApprovalStatus != '4' && row.ApprovalStatus != '5') {
                    parent.$.messager.alert('提示', '只有状态是“审批通过"、“审批未通过”数据可以修改批示！', 'info');
                    return;
                }
                //$.ajax({
                //    type: "post",
                //    url: "/Handler/MaketOperate.ashx",
                //    data: { keys: row.Id, flag: 'getshenpi', isps: '0', time: new Date().getTime() },
                //    dataType: "json",
                //    async: false,
                //    success: function (data) {
                //        if (data && data.Message) {
                //            parent.$.messager.alert('提示', data.Message, 'info');
                //            return;
                //        }
                //        var url = '/MarketInfo/PiShiXT.aspx?action=updpishi&keys=' + row.Id + '&a=' + data.StepId + '&b=' + data.InstanceId + '&time=' + new Date().getTime();
                //        parent.createAndOpenModalWindow('修改批示', 950, 500, url, function (data) {
                //            $('#tblist').datagrid('reload');  //重新加载列表
                //        });
                //    },
                //    error: function (msg) {
                //        parent.$.messager.alert('提示', '获取批示步骤及实例编号失败，请重新选择数据进行修改批示或确认是否有实例及批示步骤！', 'info');
                //        return;
                //    }
                //});
                $.post("/Handler/MaketOperate.ashx",
                    { keys: row.Id, flag: 'getshenpi', isps: '0', time: new Date().getTime() },
                    function (data) {
                        if (data && data.Message) {
                            parent.$.messager.alert('提示', data.Message, 'info');
                            return;
                        }
                        var url = '/MarketInfo/PiShiXT.aspx?action=updpishi&keys=' + row.Id + '&a=' + data.StepId + '&b=' + data.InstanceId + '&time=' + new Date().getTime();
                        parent.createAndOpenModalWindow('修改批示', 950, 500, url, function (data) {
                            $('#tblist').datagrid('reload');  //重新加载列表
                        });
                    }, "json");
            },

            //3.取消批示--已完成的，改为退回状态
            cancelPishi: function () {
                //1、选中行
                var row = $('#tblist').datagrid('getSelected');
                if (row == null || row.length == 0) {
                    parent.$.messager.alert('提示', '请选择要取消批示的数据！', 'info');
                    return;
                } else {
                    //2.判断审批状态
                    if (row.ApprovalStatus != '4' && row.ApprovalStatus != '5') {
                        parent.$.messager.alert('提示', '只有状态是“审批通过"、“审批未通过”数据可以取消批示！', 'info');
                        return;
                    }

                    //3.验证是否是是操作人的已办且是完成
                    $.ajax({
                        type: "post",
                        url: "/Handler/MaketOperate.ashx",
                        data: { keys: row.Id, flag: 'getshenpi', isps: '2' },
                        dataType: "json",
                        async: false,
                        success: function (data) {
                            if (data && data.Message) {
                                parent.$.messager.alert('提示', data.Message, 'info');
                                return;
                            }
                            $.messager.confirm('确认', '确定要取消批示？', function (result) {
                                if (!result) {
                                    return;
                                } else {
                                    //4、后台修改状态
                                    $.ajax({
                                        type: "post",
                                        url: "/Handler/MaketOperate.ashx",
                                        data: { keys: row.Id, flag: 'calpishi' },
                                        dataType: "json",
                                        async: false,
                                        success: function (data) {
                                            if (data && data.Message) {
                                                parent.$.messager.alert('提示', data.Message, 'info');
                                                return;
                                            }
                                        }
                                    });
                                    //5、重新加载列表
                                    //Action.Search();
                                    $('#tblist').datagrid('reload');  //重新加载列表
                                }
                            });
                        }
                    });
                }
            },

            //5.根据枚举value，获取pvalue,使得施工领域默认选中获取pvalue，然后灾根据pvalue，加载工程类型选项，默认选择id项
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

            //11.根据枚举value，获取pvalue,使得使用资质默认选中获取pvalue，然后再根据pvalue，加载上班公司选项，默认选择id项
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
<body>
    <div style="margin: 8px 0px 0px 0px;">
        <div class="toubu">
            <p>查询条件</p>
        </div>
        <div class="easyui-panel" style="width: 100%; background-color: #fff;">
            <input id="hdsel" name="hdsel" type="hidden" value="1" />
            <%--是否查询按钮还是帅选按钮--%>
            <form id="jwdfm" method="post" class="easyui-form" data-options="novalidate:false" action="../Handler/ExportExcel.aspx?action=seljwd">
                <input type="hidden" name="filename" value="冲突协调(经纬度查询)列表.xls" />
                <input type="hidden" name="sheetname" value="冲突协调(经纬度查询)列表" />
                <input id="hdcode" name="hdcode" type="hidden" />
                <input id="hdbj" name="hdbj" type="hidden" />
            </form>
            <form id="serMarkfm" method="post" action="../Handler/ExportExcel.aspx?action=ctxt">
                <input type="hidden" id="filename" name="filename" value="冲突协调列表.xls" />
                <input type="hidden" id="sheetname" name="sheetname" value="冲突协调列表" />
                <table cellpadding="3px" style="margin-left: 10px;">
                    <tr>
                        <td>公司简称:</td>
                        <td>
                            <input name="gsjc" id="gsjc" class="easyui-textbox" style="width: 180px;" />
                            <input name="gsjc1" id="gsjc1" type="hidden" />
                        </td>
                        <td style="width: 25px"></td>
                        <td>登记编号:</td>
                        <td>
                            <input id="djbh1" name="djbh1" class="easyui-textbox" data-options="validType:['intx','length[1,50]']" style="width: 180px;" /></td>
                        <td style="width: 25px"></td>
                        <td>建设单位:</td>
                        <td>
                            <input id="BuildCopeNameInput" name="BuildCopeNameInput" type="text" data-options="validType:'length[1,100]'" 
                                style="width: 173px;border:#d9d9d9 1px solid;border-radius:3px 3px 3px 3px;height:16px;font-size:12px; padding-left:4px; padding-top:3px;"  />
                        </td>
                        <td style="width: 25px"></td>
                        <td>工程名称:</td>
                        <td>
                            <input id="ProjectNameInput" name="ProjectNameInput" type="text" data-options="validType:'length[1,150]'" 
                                style="width: 173px;border:#d9d9d9 1px solid;border-radius:3px 3px 3px 3px;height:16px;font-size:12px; padding-left:4px; padding-top:3px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>所属省市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comsheng" id="comsheng" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>所属地市:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comshi" id="comshi" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>所属区县:</td>
                        <td>
                            <input class="easyui-textbox" type="text" name="comv" id="comv" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>工程地点:</td>
                        <td>
                            <input id="ProjectLocationInput" name="ProjectLocationInput" type="text" data-options="validType:'length[1,150]'" 
                                style="width: 173px;border:#d9d9d9 1px solid;border-radius:3px 3px 3px 3px;height:16px;font-size:12px; padding-left:4px; padding-top:3px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>登记起日:</td>
                        <td>
                            <input id="begintime1" name="begintime1" type="text" class="Wdate" onclick="WdatePicker();" maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>登记至日:</td>
                        <td>
                            <input id="endtime1" name="endtime1" type="text" class="Wdate" onclick="WdatePicker({ minDate: '#F{$dp.$D(\'begintime1\',{d:1})}' });"
                                maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>登记人员:</td>
                        <td>
                            <select name="CreateUser" id="CreateUser" style="width: 180px;">
                            </select>
                            <input id="CreateUserId" name="CreateUserId" type="hidden" />
                        </td>
                        <td></td>
                        <td>所属区域:</td>
                        <td>
                            <select id="RegionId1" name="RegionId1" class="easyui-combobox" style="width: 180px;"
                                data-options="
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200,
                                editable:false,
                                url: '/Handler/PublicArea.ashx?action=enum_auto_q',
                                valueField: 'Key',
                                textField: 'Value'">
                            </select>
                        </td>
                    </tr>
                    <tr>
                        <td>批示起日:</td>
                        <td>
                            <input id="psbegintime" name="psbegintime" type="text" class="Wdate" onclick="WdatePicker();" maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>批示至日:</td>
                        <td>
                            <input id="psendtime" name="psendtime" type="text" class="Wdate" onclick="WdatePicker({ minDate: '#F{$dp.$D(\'psbegintime\',{d:1})}' });"
                                maxlength="10" style="width: 173px;" />
                        </td>
                        <td></td>
                        <td>造价(万元):</td>
                        <td>
                            <input id="zj1" name="zj1" class="easyui-textbox" data-options="missingMessage:'请输入造价数值',validType:['number','length[0,30]']" style="width: 180px;" />
                        </td>
                        <td></td>
                        <td>至:</td>
                        <td>
                            <input id="zj2" name="zj2" class="easyui-textbox" data-options="missingMessage:'请输入造价数值',validType:['number','length[0,30]']" style="width: 180px;" />
                        </td>
                    </tr>
                    <tr>
                        <td>审批状态:</td>
                        <td>
                            <select id="spztdr" name="spztdr" class="easyui-combobox" style="width: 180px;"
                                data-options="
                                editable:false,
                                panelHeight: 'auto',
                                panelMinHeight: 20,
                                panelMaxHeight: 200">
                                <option value="">全部</option>
                                <option value="1">审批中</option>
                                <option value="4">审批通过</option>
                                <option value="5">审批未通过</option>
                            </select>
                        </td>
                        <td style="width: 20px"></td>
                        <td>协调结果:</td>
                        <td>
                            <select id="CoordinationResult" name="CoordinationResult" class="easyui-combobox" style="width: 180px;"
                                data-options="
                                    panelHeight: 'auto',
                                    panelMinHeight: 20,
                                    panelMaxHeight: 200,
                                    editable:false,
                                    url: '/Handler/PublicEnum.ashx?action=enum_auto_q&pvalue=F1A978E4-B0CC-41B2-B1FA-23BC917C03D1',
                                    valueField: 'Key',
                                    textField: 'Value'">
                            </select>
                        </td>
                        <td style="width: 20px"></td>
                        <td>是否批示:</td>
                        <td>
                            <select id="sfps" name="sfps" class="easyui-combobox" style="width: 180px;"
                                data-options="editable:false,
                                    panelHeight: 'auto',
                                    panelMinHeight: 20,
                                    panelMaxHeight: 200">
                                <option value="">全部</option>
                                <option value="1">是</option>
                                <option value="0">否</option>
                            </select>
                        </td>
                        <td style="width: 20px"></td>
                        <td></td>
                        <td style="float: right;">
                            <%-- <div class="a1" onclick="Action.Search()"></div>
                            <div class="a2" onclick="Action.ClearSer()"></div>--%>
                            <a class="easyui-linkbutton linkbutton" style="margin-left: 0px;" iconcls="icon-search" onclick="Action.Search()">查询</a>
                            <a class="easyui-linkbutton linkbutton" style="margin-left: 5px;" iconcls="icon-reset" onclick="Action.ClearSer()">重置</a>
                        </td>
                    </tr>
                </table>
            </form>
        </div>
    </div>
    <%--查询列表--%>
    <div style="margin: 8px 0px 0px 0px;">
        <div class="toubu">
            <p>冲突列表</p>
        </div>
        <table id="tblist"></table>
        <input type="hidden" name="shaixuan" id="shaixuan" value="0" />
    </div>

    <input type="hidden" id="hdused1" value="1" />
    <%--批示按钮是否启用--%>
    <input type="hidden" id="hdused2" value="1" />
    <%--取消批示是否启用--%>
    <input type="hidden" id="hdused3" value="1" />
    <%--修改批示钮是否启用--%>
</body>
</html>
